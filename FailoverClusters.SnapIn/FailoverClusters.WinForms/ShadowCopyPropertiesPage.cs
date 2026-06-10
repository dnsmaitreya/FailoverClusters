using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;
using KDDSL.ServerClusters;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.WinForms;

internal class ShadowCopyPropertiesPage : ResourcePropertyPage
{
	private byte[] triggerArray;

	private readonly Dictionary<FailoverClusters.Framework.ClusterDiskPartition, byte[]> triggerArrayDictionary = new Dictionary<FailoverClusters.Framework.ClusterDiskPartition, byte[]>();

	private FailoverClusters.Framework.ResourceState state;

	private string emptyMsg;

	private List<ListViewItem> partitions = new List<ListViewItem>();

	private string taskName;

	private readonly Dictionary<FailoverClusters.Framework.ClusterDiskPartition, string> tasksNameDictionary = new Dictionary<FailoverClusters.Framework.ClusterDiskPartition, string>();

	private int lastSelectedIndex = -1;

	private const int ClusterTaskTypeV1 = 4;

	private const string SystemDirectory = "%systemroot%\\system32";

	private const string VSSExeFullPath = "%systemroot%\\system32\\vssadmin.exe";

	private const string ApplicationParamter = "Create Shadow /AutoRetry=15 /For=\\\\?\\Volume{{{0}}}\\";

	private string volumeGuid;

	private readonly Dictionary<FailoverClusters.Framework.ClusterDiskPartition, string> volumeGuidDictionary = new Dictionary<FailoverClusters.Framework.ClusterDiskPartition, string>();

	private readonly int TriggerSize = 48;

	private Guid resourceId;

	private readonly FailoverClusters.Framework.Cluster cluster;

	private Resource resource;

	private IContainer components;

	private SnapinGroupBox scheduleGroupBox;

	private Label warningLabel;

	private Button scheduleButton;

	private Button enableButton;

	private Button disableButton;

	private Label selectVolumeLabel;

	private BaseListView volumesListView;

	private ColumnHeader Volume;

	private ColumnHeader Capacity;

	private ColumnHeader FreeSpace;

	internal ShadowCopyPropertiesPage()
		: base(Resources.ShadowCopy_Text)
	{
		InitializeComponent();
	}

	internal ShadowCopyPropertiesPage(FailoverClusters.Framework.Cluster cluster, Guid resourceId)
		: this()
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		Exceptions.ThrowIfEmpty(resourceId, "resourceId");
		this.cluster = cluster;
		this.resourceId = resourceId;
	}

	private void VolumesListViewSelectedIndexChanged(object sender, EventArgs e)
	{
		if (((ListView)(object)volumesListView).SelectedItems.Count > 0 && lastSelectedIndex == ((ListView)(object)volumesListView).SelectedIndices[0])
		{
			return;
		}
		if (((ListView)(object)volumesListView).SelectedItems.Count > 0)
		{
			lastSelectedIndex = ((ListView)(object)volumesListView).SelectedIndices[0];
			FailoverClusters.Framework.ClusterDiskPartition clusterDiskPartition = (FailoverClusters.Framework.ClusterDiskPartition)((ListView)(object)volumesListView).SelectedItems[0].Tag;
			volumeGuid = clusterDiskPartition.VolumeGuid.ToString();
			taskName = string.Format(CultureInfo.CurrentCulture, "{0}{{{1}}}", "ShadowCopyVolume", volumeGuid);
			if (!volumeGuidDictionary.ContainsKey(clusterDiskPartition))
			{
				volumeGuidDictionary.Add(clusterDiskPartition, volumeGuid);
			}
			if (!tasksNameDictionary.ContainsKey(clusterDiskPartition))
			{
				tasksNameDictionary.Add(clusterDiskPartition, taskName);
			}
			RefreshUIStates();
		}
		else
		{
			lastSelectedIndex = -1;
			taskName = string.Empty;
			volumeGuid = string.Empty;
			enableButton.Enabled = false;
			disableButton.Enabled = false;
			scheduleButton.Enabled = false;
		}
	}

	private void ScheduleButtonClick(object sender, EventArgs e)
	{
		IntPtr newTriggers = IntPtr.Zero;
		try
		{
			int newSize = 0;
			string text = ((ListView)(object)volumesListView).SelectedItems[0].Text;
			FailoverClusters.Framework.ClusterDiskPartition key = (FailoverClusters.Framework.ClusterDiskPartition)((ListView)(object)volumesListView).SelectedItems[0].Tag;
			if (triggerArrayDictionary.ContainsKey(key))
			{
				triggerArray = triggerArrayDictionary[key];
			}
			else
			{
				triggerArray = LoadTaskTriggers() ?? GenerateDefaultTaskTriggers();
				triggerArrayDictionary.Add(key, triggerArray);
			}
			uint num = NativeMethods.EditVssTaskSchedule(triggerArray, triggerArray.Length, ref newTriggers, ref newSize, text, ((Control)(object)this).Handle);
			if (num != 0)
			{
				Win32Exception ex = new Win32Exception((int)num);
				ClusterLog.LogException((Exception)ex, "An error occurred modifying the VSS task schedule");
				throw ex;
			}
			if (newSize == 0)
			{
				return;
			}
			byte[] array = new byte[newSize];
			Marshal.Copy(newTriggers, array, 0, newSize);
			bool flag = array.Length == triggerArray.Length;
			int num2 = 0;
			while (flag && num2 < triggerArray.Length)
			{
				flag = array[num2] == triggerArray[num2];
				if (!flag)
				{
					break;
				}
				num2++;
			}
			if (!flag)
			{
				triggerArray = array;
				triggerArrayDictionary[key] = triggerArray;
				base.IsDirty = true;
			}
		}
		finally
		{
			if (newTriggers != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(newTriggers);
			}
		}
	}

	private void EnableButtonClick(object sender, EventArgs e)
	{
		CreateDefaultTask();
		RefreshUIStates();
	}

	private void CreateDefaultTask()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		UIThreadHandlerV function = (UIThreadHandlerV)delegate
		{
			triggerArray = GenerateDefaultTaskTriggers();
			uint num = NativeMethods.ClusterTaskCreate_TS_V1(cluster.Name, taskName, 4, "%systemroot%\\system32", "%systemroot%\\system32\\vssadmin.exe", string.Format(CultureInfo.CurrentCulture, "Create Shadow /AutoRetry=15 /For=\\\\?\\Volume{{{0}}}\\", volumeGuid), triggerArray.Length / TriggerSize, triggerArray, triggerArray.Length, resource.Name);
			if (num != 0)
			{
				ClusterLog.LogWarning("CreateDefaultTask::NativeMethods.ClusterTaskCreate_TS_V1() returned error: {0}." + num);
			}
		};
		ExecuteOnBackground(function, Resources.CreatingShadowVolumeCopy_Text);
	}

	private void DisableButtonClick(object sender, EventArgs e)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		FailoverClusters.Framework.ClusterDiskPartition partition = null;
		if (((ListView)(object)volumesListView).SelectedItems.Count > 0)
		{
			partition = (FailoverClusters.Framework.ClusterDiskPartition)((ListView)(object)volumesListView).SelectedItems[0].Tag;
		}
		UIThreadHandlerV function = (UIThreadHandlerV)delegate
		{
			uint num = NativeMethods.ClusterTaskDelete_TS_V1(cluster.Name, taskName);
			if (num != 0)
			{
				ClusterLog.LogWarning("DisableButtonClick::NativeMethods.ClusterTaskDelete_TS_V1() returned error: {0}." + num);
			}
			if (partition != null && triggerArrayDictionary.ContainsKey(partition))
			{
				triggerArrayDictionary.Remove(partition);
			}
		};
		ExecuteOnBackground(function, Resources.DeletingShadowVolumeCopy_Text);
		RefreshUIStates();
	}

	private void RefreshUIStates()
	{
		if (TaskAlreadyExists())
		{
			enableButton.Enabled = false;
			disableButton.Enabled = true;
			scheduleButton.Enabled = true;
		}
		else
		{
			enableButton.Enabled = true;
			disableButton.Enabled = false;
			scheduleButton.Enabled = false;
		}
	}

	private void LoadDiskInfo(StorageResource diskResource)
	{
		partitions.Clear();
		if (state != FailoverClusters.Framework.ResourceState.Online)
		{
			emptyMsg = Resources.DiskNotOnlineMessage_Text;
		}
		try
		{
			FailoverClusters.Framework.ClusterDisk diskInfo = diskResource.DiskInfo;
			if (state != FailoverClusters.Framework.ResourceState.Online)
			{
				return;
			}
			if (diskInfo.Partitions == null || diskInfo.Partitions.Count == 0)
			{
				emptyMsg = Resources.DiskNoPartition_Text;
				return;
			}
			foreach (FailoverClusters.Framework.ClusterDiskPartition partition in diskInfo.Partitions)
			{
				ListViewItem listViewItem = new ListViewItem((!string.IsNullOrEmpty(partition.DriveLetter)) ? string.Format(CultureInfo.InvariantCulture, "{0}:\\", partition.DriveLetter) : partition.Name);
				listViewItem.ImageIndex = Icons.PhysicalDiskIndex;
				listViewItem.SubItems.Add(FormatHelp.GetStorageSizeStringFromULong(partition.Size));
				listViewItem.SubItems.Add(FormatHelp.GetStorageSizeStringFromULong(partition.FreeSpace));
				listViewItem.Tag = partition;
				partitions.Add(listViewItem);
			}
		}
		catch (Exception caughtException)
		{
			emptyMsg = string.Format(CultureInfo.CurrentCulture, Resources.DiskInfoFailed_Text, diskResource.DisplayName);
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
			if (firstException != null && (firstException.NativeErrorCode == -2147023728 || firstException.NativeErrorCode == -2147019873))
			{
				ExceptionHelp.LogException(caughtException, "Error getting disk info for property page");
				return;
			}
			throw;
		}
	}

	private void CallMethodInBackground(UIThreadHandlerV function)
	{
		ExecuteOnBackground(function, null);
	}

	private void ExecuteOnBackground(UIThreadHandlerV function, string caption)
	{
		if (caption == null)
		{
			caption = Resources.RetrievingItem_Text;
		}
		if (((Control)(object)this).InvokeRequired)
		{
			function.Invoke();
			return;
		}
		using CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(caption, string.Empty);
		cluadminWaitDialog.ShowDialog(base.NotifyUser, delegate
		{
			function.Invoke();
		});
	}

	protected override void InitializePage()
	{
		disableButton.Enabled = false;
		enableButton.Enabled = false;
		scheduleButton.Enabled = false;
		((Control)(object)this).Enabled = false;
	}

	protected override object LoadProperties(object context)
	{
		Resource.Get(cluster, resourceId, delegate(OperationResult<Resource> cacheResult)
		{
			if (cacheResult.Error != null)
			{
				throw cacheResult.Error;
			}
			resource = cacheResult.Result;
			resource.LoadAsync(delegate(ClusterLoadedEventArgs result)
			{
				if (result.Error != null)
				{
					throw result.Error;
				}
				StorageResource storageResource = resource as StorageResource;
				if (storageResource != null)
				{
					SnapinPropertyPageControlBase.UpdateControl((Control)(object)this, delegate
					{
						state = resource.ResourceState;
						LoadDiskInfo(storageResource);
						((ListView)(object)volumesListView).SmallImageList = IconsHelp.SmallImageList;
						volumesListView.EmptyText = emptyMsg;
						volumesListView.HideSelection = false;
						volumesListView.Items.AddRange(partitions.ToArray());
						((Control)(object)this).Enabled = true;
					});
				}
			}, ResourceLoadSelection.CommonProperties | ResourceLoadSelection.Storage);
		}, OperationType.Async);
		return null;
	}

	protected override bool SaveProperties()
	{
		foreach (KeyValuePair<FailoverClusters.Framework.ClusterDiskPartition, byte[]> item in triggerArrayDictionary)
		{
			FailoverClusters.Framework.ClusterDiskPartition key = item.Key;
			byte[] value = item.Value;
			string text = tasksNameDictionary[key];
			int num = (int)NativeMethods.ClusterTaskChange_TS_V1(cluster.Name, text, value.Length / TriggerSize, value, value.Length);
			if (num != 0)
			{
				ClusterShadowCopyException ex = new ClusterShadowCopyException(text, resource.DisplayName, new Win32Exception(num));
				ClusterLog.LogException((Exception)ex, "Error saving VSS schedule");
				throw ex;
			}
		}
		return true;
	}

	protected override bool ValidateProperties()
	{
		return true;
	}

	private bool TaskAlreadyExists()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		bool exist = false;
		UIThreadHandlerV function = (UIThreadHandlerV)delegate
		{
			uint num = NativeMethods.ClusterTaskExists_TS_V1(cluster.Name, taskName, resource.Name, ref exist);
			if (num != 0)
			{
				ClusterLog.LogWarning("TaskAlreadyExists::NativeMethods.ClusterTaskExists_TS_V1() returned error: {0}." + num);
			}
		};
		CallMethodInBackground(function);
		return exist;
	}

	private byte[] LoadTaskTriggers()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		byte[] triggers = null;
		UIThreadHandlerV function = (UIThreadHandlerV)delegate
		{
			IntPtr taskInfo;
			int num = NativeMethods.ClusterTaskQuery(cluster.Name, taskName, out taskInfo);
			if (num != 0)
			{
				Win32Exception ex = new Win32Exception(num);
				ClusterLog.LogException((Exception)ex, "ClusterTaskQuery returned {0} when trying to query cluster {1} for task {2}.", new object[3] { num, cluster.Name, taskName });
				throw ex;
			}
			try
			{
				NativeMethods.CLUSTER_TASK_INFO cLUSTER_TASK_INFO = (NativeMethods.CLUSTER_TASK_INFO)Marshal.PtrToStructure(taskInfo, typeof(NativeMethods.CLUSTER_TASK_INFO));
				triggers = new byte[cLUSTER_TASK_INFO.TriggerArraySize];
				Marshal.Copy(cLUSTER_TASK_INFO.TriggerArray, triggers, 0, (int)cLUSTER_TASK_INFO.TriggerArraySize);
			}
			finally
			{
				NativeMethods.ClusterFreeTaskInfo(taskInfo);
			}
		};
		CallMethodInBackground(function);
		return triggers;
	}

	private byte[] ConvertTriggersToBytes(List<NativeMethods.TASK_TRIGGER> triggers)
	{
		int num = Marshal.SizeOf(typeof(NativeMethods.TASK_TRIGGER));
		byte[] array = new byte[num * triggers.Count];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		try
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				Marshal.StructureToPtr(triggers[i], intPtr, fDeleteOld: false);
				Marshal.Copy(intPtr, array, i * num, num);
			}
			return array;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	private byte[] GenerateDefaultTaskTriggers()
	{
		List<NativeMethods.TASK_TRIGGER> triggers = new List<NativeMethods.TASK_TRIGGER>
		{
			new NativeMethods.TASK_TRIGGER
			{
				cbTriggerSize = 48,
				Reserved1 = 0,
				wBeginYear = 2011,
				wBeginMonth = 8,
				wBeginDay = 30,
				wEndYear = 0,
				wEndMonth = 0,
				wEndDay = 0,
				wStartHour = 7,
				wStartMinute = 0,
				MinutesDuration = 0u,
				MinutesInterval = 0u,
				rgFlags = 0u,
				TriggerType = NativeMethods.TASK_TRIGGER_TYPE.TASK_TIME_TRIGGER_WEEKLY,
				Type = new NativeMethods.TRIGGER_TYPE_UNION
				{
					Weekly = new NativeMethods.WEEKLY
					{
						WeeksInterval = 1,
						rgfDaysOfTheWeek = 62
					}
				},
				Reserved2 = 0,
				wRandomMinutesInterval = 0
			},
			new NativeMethods.TASK_TRIGGER
			{
				cbTriggerSize = 48,
				Reserved1 = 0,
				wBeginYear = 2011,
				wBeginMonth = 8,
				wBeginDay = 30,
				wEndYear = 0,
				wEndMonth = 0,
				wEndDay = 0,
				wStartHour = 12,
				wStartMinute = 0,
				MinutesDuration = 0u,
				MinutesInterval = 0u,
				rgFlags = 0u,
				TriggerType = NativeMethods.TASK_TRIGGER_TYPE.TASK_TIME_TRIGGER_WEEKLY,
				Type = new NativeMethods.TRIGGER_TYPE_UNION
				{
					Weekly = new NativeMethods.WEEKLY
					{
						WeeksInterval = 1,
						rgfDaysOfTheWeek = 62
					}
				},
				Reserved2 = 0,
				wRandomMinutesInterval = 0
			}
		};
		return ConvertTriggersToBytes(triggers);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ShadowCopyPropertiesPage));
		scheduleGroupBox = new SnapinGroupBox();
		scheduleButton = new Button();
		warningLabel = new Label();
		enableButton = new Button();
		disableButton = new Button();
		selectVolumeLabel = new Label();
		volumesListView = new BaseListView();
		Volume = new ColumnHeader();
		Capacity = new ColumnHeader();
		FreeSpace = new ColumnHeader();
		((Control)(object)scheduleGroupBox).SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(scheduleGroupBox, "scheduleGroupBox");
		((Control)(object)scheduleGroupBox).Controls.Add(scheduleButton);
		((Control)(object)scheduleGroupBox).Controls.Add(warningLabel);
		((GroupBox)(object)scheduleGroupBox).FlatStyle = FlatStyle.System;
		((Control)(object)scheduleGroupBox).ForeColor = SystemColors.ControlText;
		((Control)(object)scheduleGroupBox).Name = "scheduleGroupBox";
		((GroupBox)(object)scheduleGroupBox).TabStop = false;
		componentResourceManager.ApplyResources(scheduleButton, "scheduleButton");
		scheduleButton.Name = "scheduleButton";
		scheduleButton.UseVisualStyleBackColor = true;
		scheduleButton.Click += ScheduleButtonClick;
		componentResourceManager.ApplyResources(warningLabel, "warningLabel");
		warningLabel.ForeColor = SystemColors.ControlText;
		warningLabel.Name = "warningLabel";
		componentResourceManager.ApplyResources(enableButton, "enableButton");
		enableButton.Name = "enableButton";
		enableButton.UseVisualStyleBackColor = true;
		enableButton.Click += EnableButtonClick;
		componentResourceManager.ApplyResources(disableButton, "disableButton");
		disableButton.Name = "disableButton";
		disableButton.UseVisualStyleBackColor = true;
		disableButton.Click += DisableButtonClick;
		componentResourceManager.ApplyResources(selectVolumeLabel, "selectVolumeLabel");
		selectVolumeLabel.Name = "selectVolumeLabel";
		componentResourceManager.ApplyResources(volumesListView, "volumesListView");
		((ListView)(object)volumesListView).Columns.AddRange(new ColumnHeader[3] { Volume, Capacity, FreeSpace });
		volumesListView.EnableAutoResizeColumns = true;
		((ListView)(object)volumesListView).FullRowSelect = true;
		volumesListView.HideSelection = true;
		((ListView)(object)volumesListView).MultiSelect = false;
		((Control)(object)volumesListView).Name = "volumesListView";
		((ListView)(object)volumesListView).UseCompatibleStateImageBehavior = false;
		((ListView)(object)volumesListView).View = View.Details;
		((ListView)(object)volumesListView).SelectedIndexChanged += VolumesListViewSelectedIndexChanged;
		componentResourceManager.ApplyResources(Volume, "Volume");
		componentResourceManager.ApplyResources(Capacity, "Capacity");
		componentResourceManager.ApplyResources(FreeSpace, "FreeSpace");
		((Control)(object)this).AccessibleRole = AccessibleRole.PropertyPage;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(selectVolumeLabel);
		((Control)(object)this).Controls.Add((Control)(object)volumesListView);
		((Control)(object)this).Controls.Add((Control)(object)scheduleGroupBox);
		((Control)(object)this).Controls.Add(enableButton);
		((Control)(object)this).Controls.Add(disableButton);
		((Control)(object)this).Name = "ShadowCopyPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(disableButton, 0);
		((Control)(object)this).Controls.SetChildIndex(enableButton, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)scheduleGroupBox, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)volumesListView, 0);
		((Control)(object)this).Controls.SetChildIndex(selectVolumeLabel, 0);
		((Control)(object)scheduleGroupBox).ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

