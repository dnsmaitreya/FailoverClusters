using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class ClusterEventsStartPageControl : StartPageContainerControl
{
	private struct MultiplexorValue
	{
		public Exception Error;

		public EventLogSession Session;

		public List<string> Channels;
	}

	private TitleBarControl titleBarControl;

	private Button stopButton;

	private Label messageLabel;

	private ClusterEventsControl clusterEventsControl;

	private ClusterEventsContext context;

	private ActionBase queryAction;

	private ActionBase saveAction;

	private ActionBase openAction;

	private ActionBase findAction;

	private ActionBase saveEventsAsAction;

	private ActionBase chooseColumnsAction;

	private ActionBase resetRecentEventAction;

	private EventLogFilter eventLogFilter;

	private object eventLogFilterLock;

	private IContainer components;

	private EventLogFilter LogFilter
	{
		get
		{
			lock (eventLogFilterLock)
			{
				return eventLogFilter;
			}
		}
	}

	public ClusterEventsStartPageControl()
	{
		InitializeComponent();
		base.RefreshOnShow = false;
		eventLogFilter = null;
		eventLogFilterLock = new object();
	}

	protected override void InitializeInternal(FormView view)
	{
		base.InitializeInternal(view);
		context = (ClusterEventsContext)base.CluAdminScopeNode.Context;
		ClusterEventsContext clusterEventsContext = context;
		clusterEventsContext.NodeNamesChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Combine(clusterEventsContext.NodeNamesChanged, new EventHandler<ClusterObjectEventArgs>(OnClusterNodesChanged));
		context.ContextRefreshed += OnContextRefreshed;
		clusterEventsControl.Initialize(base.NotifyUser);
		clusterEventsControl.SortStart += OnSortStart;
		clusterEventsControl.SortCompleted += OnSortCompleted;
		clusterEventsControl.QueryCompleted += OnQueryCompleted;
		clusterEventsControl.QueryStarted += OnQueryStarted;
		((Control)(object)clusterEventsControl).KeyUp += ClusterEventsControl_KeyUp;
		titleBarControl.SubTitle = string.Empty;
		messageLabel.Visible = false;
		stopButton.Visible = false;
		queryAction = ActionFactory.CreateAction(CommandResources.QueryAction_Text, Resources.QueryActionDescription_Text, Icons.FilterIndex, QueryAction);
		saveAction = ActionFactory.CreateAction(Resources.SaveQueryAs_Text, Resources.SaveQueryActionDescription_Text, Icons.SaveFileIndex, SaveQueryAction);
		openAction = ActionFactory.CreateAction(CommandResources.OpenQueryAction_Text, Resources.OpenQueryActionDescription_Text, Icons.OpenFileIndex, OpenQueryAction);
		findAction = ActionFactory.CreateAction(CommandResources.FindNextAction_Text, Resources.FindNextActionDescription_Text, Icons.FindNextIndex, FindNextAction);
		saveEventsAsAction = ActionFactory.CreateAction(Resources.SaveEventsAs_Text, Resources.SaveEventsAsActionDescription_Text, Icons.SaveFileIndex, SaveEventsAsAction);
		chooseColumnsAction = ActionFactory.CreateAction(CommandResources.ChooseColumnsAction_Text, Resources.ChooseColumnsActionDescription_Text, Icons.ChooseColumnsIndex, ChooseColumnsAction);
		resetRecentEventAction = SharedActions.CreateResetRecentEventsAction(context.Cluster, OnResetRecentEventsCompleted);
		base.View.ActionsPaneItems.AddRange(new ActionsPaneItem[6] { queryAction, saveAction, openAction, findAction, saveEventsAsAction, chooseColumnsAction });
		if (context.Cluster != null)
		{
			base.View.ActionsPaneItems.AddRange(new ActionsPaneItem[2]
			{
				new ActionSeparator(),
				resetRecentEventAction
			});
		}
		saveAction.Enabled = false;
		saveEventsAsAction.Enabled = false;
		EnableFindAction(enabled: false);
	}

	private void OnResetRecentEventsCompleted()
	{
		EventLogFilter logFilter = LogFilter;
		if (logFilter != null)
		{
			logFilter.From = ClusterAdministrator.GetClusterEventsStartTime(context.Cluster, useWaitDialog: true);
			RefreshContent();
		}
	}

	private void ClusterEventsControl_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.F5)
		{
			RefreshContent();
		}
	}

	private void EnableFindAction(bool enabled)
	{
		findAction.Enabled = enabled;
		clusterEventsControl.EnableFindHotKey(enabled);
	}

	private CriticalEventsFilterDialog GetFilterDialog()
	{
		ICollection<string> nodes = null;
		ICollection<EventChannelName> channels = null;
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingEventLogInfo_Text, string.Empty))
		{
			cluadminWaitDialog.DisplayDelay = TimeSpan.FromMilliseconds(500.0);
			cluadminWaitDialog.ShowDialog(base.NotifyUser, delegate
			{
				nodes = context.NodeNames;
				channels = GetChannels(nodes);
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return null;
			}
		}
		return new CriticalEventsFilterDialog(context.Cluster, nodes, channels);
	}

	private static ICollection<EventChannelName> GetChannels(ICollection<string> nodes)
	{
		ICollection<MultiplexorValue> collection = new ActionMultiplexor<object, string, MultiplexorValue>(delegate(object dummy, string node)
		{
			MultiplexorValue result = default(MultiplexorValue);
			try
			{
				result.Error = null;
				result.Session = new EventLogSession(node);
				result.Channels = result.Session.GetChannels();
			}
			catch (Exception ex)
			{
				ExceptionHelp.LogException(ex, "Error retrieving event log channels from '{0}'", node);
				result.Error = ex;
			}
			return result;
		}).Execute(null, nodes);
		int num = 0;
		Exception innerException = null;
		Dictionary<string, EventChannelName> dictionary = new Dictionary<string, EventChannelName>(StringComparer.OrdinalIgnoreCase);
		foreach (MultiplexorValue item in collection)
		{
			try
			{
				if (item.Error != null)
				{
					num++;
					innerException = item.Error;
					continue;
				}
				foreach (string channel in item.Channels)
				{
					if (dictionary.ContainsKey(channel))
					{
						continue;
					}
					string text = null;
					try
					{
						using EventLogChannelInfo eventLogChannelInfo = new EventLogChannelInfo(item.Session, channel);
						EventLogChannelType channelType = eventLogChannelInfo.GetChannelType();
						if (channelType == EventLogChannelType.Analytic || channelType == EventLogChannelType.Debug)
						{
							continue;
						}
						if (eventLogChannelInfo.GetIsClassicChannel())
						{
							text = item.Session.GetClassicLogDisplayName(channel);
						}
						else
						{
							string channelPublisher = eventLogChannelInfo.GetChannelPublisher();
							using EventLogPublisher eventLogPublisher = item.Session.GetPublisher(channelPublisher);
							text = eventLogPublisher.GetChannelName(channel);
							if (!text.Contains("/") && channel.Contains("/"))
							{
								text = channelPublisher + "/" + text;
							}
						}
						goto IL_015a;
					}
					catch (Exception caughtException)
					{
						ExceptionHelp.LogException(caughtException, "Error retrieving the name for channel '{0}'", channel);
						text = channel;
						goto IL_015a;
					}
					IL_015a:
					dictionary.Add(channel, new EventChannelName(text, channel));
				}
			}
			finally
			{
				if (item.Session != null)
				{
					item.Session.Dispose();
				}
			}
		}
		if (num == collection.Count)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.RetrieveEventLogInfoFailed_Text });
		}
		return dictionary.Values;
	}

	private void StartQuery()
	{
		EventLogFilter logFilter = LogFilter;
		if (logFilter != null)
		{
			titleBarControl.SubTitle = string.Empty;
			PrepareForAsyncOperation(Resources.LoadingQueryResults_Text);
			clusterEventsControl.QueryAsync(logFilter);
		}
	}

	private void PrepareForAsyncOperation(string message)
	{
		messageLabel.Text = message;
		messageLabel.Visible = true;
		messageLabel.BringToFront();
		stopButton.Visible = true;
		stopButton.Enabled = true;
		stopButton.BringToFront();
		queryAction.Enabled = false;
		saveAction.Enabled = false;
		saveEventsAsAction.Enabled = false;
		openAction.Enabled = false;
		chooseColumnsAction.Enabled = false;
		resetRecentEventAction.Enabled = false;
		EnableFindAction(enabled: false);
	}

	private void AsyncOperationCompleted(AsyncCompletedEventArgs e)
	{
		messageLabel.Visible = false;
		stopButton.Visible = false;
		queryAction.Enabled = true;
		saveAction.Enabled = true;
		saveEventsAsAction.Enabled = clusterEventsControl.EventCount > 0;
		openAction.Enabled = true;
		chooseColumnsAction.Enabled = true;
		resetRecentEventAction.Enabled = true;
		EnableFindAction(clusterEventsControl.EventCount > 0);
	}

	private void StopButtonClick(object sender, EventArgs e)
	{
		try
		{
			messageLabel.Text = Resources.StoppingOperation_Text;
			stopButton.Enabled = false;
			clusterEventsControl.Cancel();
		}
		catch (Exception ex)
		{
			base.NotifyUser.ShowError(ex, Resources.CannotCancelQuery_Text);
		}
	}

	private void OnSortStart(object sender, EventArgs e)
	{
		try
		{
			PrepareForAsyncOperation(Resources.SortingQueryResults_Text);
		}
		catch (Exception ex)
		{
			base.NotifyUser.ShowError(ex, Resources.CannotSortQueryResults_Text);
		}
	}

	private void OnQueryStarted(object sender, EventArgs e)
	{
		try
		{
			titleBarControl.SubTitle = Resources.QueryingNodes_Text;
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error to set Title Label on Cluster Events Page");
		}
	}

	private void OnQueryCompleted(object sender, AsyncCompletedEventArgs e)
	{
		try
		{
			if (e.Cancelled)
			{
				titleBarControl.SubTitle = string.Format(CultureInfo.CurrentCulture, Resources.EventCountWithFailures_Text, clusterEventsControl.EventCount);
			}
			else if (e.Error != null)
			{
				base.NotifyUser.ShowError(e.Error, Resources.CannotExecuteQuery_Text);
			}
			else if (e.UserState is List<string> list && list.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item in list)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(item);
				}
				ClusterAdministrator.SetStatusBarProgressMessage(string.Format(CultureInfo.CurrentCulture, Resources.CannotQueryEventLogNode_Text, stringBuilder.ToString()), success: false);
				titleBarControl.SubTitle = string.Format(CultureInfo.CurrentCulture, Resources.EventCountWithFailures_Text, clusterEventsControl.EventCount);
			}
			else
			{
				titleBarControl.SubTitle = string.Format(CultureInfo.CurrentCulture, Resources.EventCount_Text, clusterEventsControl.EventCount);
			}
			AsyncOperationCompleted(e);
		}
		catch (Exception ex)
		{
			titleBarControl.SubTitle = string.Empty;
			base.NotifyUser.ShowError(ex, Resources.CannotExecuteQuery_Text);
		}
	}

	private void OnSortCompleted(object sender, AsyncCompletedEventArgs e)
	{
		try
		{
			if (!e.Cancelled && e.Error != null)
			{
				base.NotifyUser.ShowError(e.Error, Resources.CannotSortQueryResults_Text);
			}
			AsyncOperationCompleted(e);
		}
		catch (Exception ex)
		{
			base.NotifyUser.ShowError(ex, Resources.CannotSortQueryResults_Text);
		}
	}

	protected override void OnShowInternal()
	{
		base.OnShowInternal();
		if (ClusterEventsMonitor.CurrentEventsFilter != null)
		{
			clusterEventsControl.ShowMessage(Resources.ExecutingMonitorEventQuery_Text);
			SetLogFilter(ClusterEventsMonitor.CurrentEventsFilter);
			ClusterEventsMonitor.CurrentEventsFilter = null;
			StartQuery();
		}
		else if (LogFilter == null)
		{
			clusterEventsControl.ShowMessage(Resources.ExecutingDefaultEventQuery_Text);
			EventLogFilter eventLogFilter = CreateDefaultFilter();
			if (eventLogFilter != null)
			{
				SetLogFilter(eventLogFilter);
				StartQuery();
			}
			else
			{
				clusterEventsControl.ShowMessage(Resources.NoEventQuery_Text);
			}
		}
	}

	protected override void OnHideInternal()
	{
		base.OnHideInternal();
		context.ClearActions();
	}

	private void QueryAction(object sender, SnapinActionEventArgs e)
	{
		try
		{
			CriticalEventsFilterDialog filterDialog = GetFilterDialog();
			if (filterDialog == null)
			{
				return;
			}
			CriticalEventsFilterDialog criticalEventsFilterDialog = filterDialog;
			try
			{
				if (eventLogFilter != null)
				{
					filterDialog.SetEventFilter(eventLogFilter);
				}
				if (base.NotifyUser.ShowDialog((Form)(object)filterDialog) == DialogResult.OK)
				{
					SetLogFilter(filterDialog.GetEventFilter());
					StartQuery();
				}
			}
			finally
			{
				((IDisposable)criticalEventsFilterDialog)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			base.NotifyUser.ShowError(ex, Resources.CannotExecuteQuery_Text);
		}
	}

	private void SaveQueryAction(object sender, SnapinActionEventArgs e)
	{
		try
		{
			EventLogFilter logFilter = LogFilter;
			if (logFilter == null)
			{
				base.NotifyUser.ShowInformational(Resources.EventFilterNoSetSave_Text);
				return;
			}
			CriticalEventsFilterDialog filterDialog = GetFilterDialog();
			if (filterDialog == null)
			{
				return;
			}
			CriticalEventsFilterDialog criticalEventsFilterDialog = filterDialog;
			try
			{
				filterDialog.SetEventFilter(logFilter);
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Title = Resources.SaveQueryAsDialogTitle_Text;
				saveFileDialog.Filter = Resources.XMLFilter_Text;
				saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				saveFileDialog.RestoreDirectory = true;
				saveFileDialog.DefaultExt = "xml";
				saveFileDialog.AddExtension = true;
				saveFileDialog.OverwritePrompt = true;
				if (base.NotifyUser.ShowDialog((CommonDialog)saveFileDialog) == DialogResult.OK)
				{
					filterDialog.SaveQuery(saveFileDialog.FileName);
				}
			}
			finally
			{
				((IDisposable)criticalEventsFilterDialog)?.Dispose();
			}
		}
		catch (ClusterInputValidationException ex)
		{
			base.NotifyUser.ShowError((Exception)ex, Resources.SaveQueryError_Text);
		}
	}

	private void SaveEventsAsAction(object sender, SnapinActionEventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Title = Resources.SaveEventsAsDialogTitle_Text;
		saveFileDialog.Filter = Resources.EvtFilter_Text;
		saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		saveFileDialog.RestoreDirectory = true;
		saveFileDialog.DefaultExt = "evtx";
		saveFileDialog.AddExtension = true;
		saveFileDialog.OverwritePrompt = true;
		saveFileDialog.SupportMultiDottedExtensions = true;
		if (base.NotifyUser.ShowDialog((CommonDialog)saveFileDialog) == DialogResult.OK)
		{
			if (!saveFileDialog.FileName.EndsWith(".evtx", StringComparison.OrdinalIgnoreCase))
			{
				saveFileDialog.FileName = string.Format(CultureInfo.CurrentCulture, "{0}.{1}", saveFileDialog.FileName, saveFileDialog.DefaultExt);
			}
			SaveEventsAs(saveFileDialog.FileName);
		}
	}

	private void SaveEventsAs(string combinedFileName)
	{
		CluadminWaitDialog waitDialog = CluadminWaitDialog.Create(Resources.SaveEventsAsExportingLogs_Text, string.Empty);
		try
		{
			waitDialog.DisplayDelay = TimeSpan.FromMilliseconds(500.0);
			waitDialog.ShowDialog(base.NotifyUser, delegate
			{
				SaveEventsAs(combinedFileName, waitDialog);
			});
		}
		finally
		{
			if (waitDialog != null)
			{
				((IDisposable)waitDialog).Dispose();
			}
		}
	}

	private void SaveEventsAs(string combinedFileName, CluadminWaitDialog waitDialog)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		string text = null;
		try
		{
			using (IEnumerator<string> enumerator = context.NodeNames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string text2 = (waitDialog.StatusText = enumerator.Current);
					string text3 = ExportLogToNode(text2, LogFilter.GetQuery());
					list.Add(text3);
					if (text == null)
					{
						text = text2;
						list2.Add(string.Format(CultureInfo.InvariantCulture, "<Select Path=\"file://{0}\">*</Select>", text3));
					}
					else
					{
						string text4 = CopyExportedLogToNode(text2, text, text3);
						list2.Add(string.Format(CultureInfo.InvariantCulture, "<Select Path=\"file://{0}\">*</Select>", text4));
						list.Add(text4);
					}
				}
			}
			waitDialog.StatusText = Resources.SaveEventsAsSavingLogs_Text;
			string arg = string.Format(CultureInfo.InvariantCulture, "<QueryList><Query Id=\"0\" Path=\"file://{0}\">", list[0]);
			string filter = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", arg, string.Join(" ", list2.ToArray()), "</Query></QueryList>");
			string path = ExportLogToNode(text, filter);
			string text5 = string.Format(CultureInfo.InvariantCulture, "\\\\{0}\\admin$\\Cluster\\Reports\\{1}", text, Path.GetFileName(path));
			list.Add(text5);
			File.Copy(text5, combinedFileName, overwrite: true);
			text5 = string.Format(CultureInfo.InvariantCulture, "\\\\{0}\\admin$\\Cluster\\Reports\\LocaleMetaData\\{1}_{2}.MTA", text, Path.GetFileNameWithoutExtension(path), CultureInfo.CurrentCulture.LCID);
			string text6 = string.Format(CultureInfo.InvariantCulture, "{0}\\LocaleMetaData\\{1}_{2}.MTA", Path.GetDirectoryName(combinedFileName), Path.GetFileNameWithoutExtension(combinedFileName), CultureInfo.CurrentCulture.LCID);
			if (!Directory.Exists(Path.GetDirectoryName(text6)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(text6));
			}
			File.Copy(text5, text6, overwrite: true);
		}
		finally
		{
			foreach (string item in list)
			{
				Exception ex = null;
				try
				{
					File.Delete(item);
				}
				catch (IOException ex2)
				{
					ex = ex2;
				}
				if (ex != null)
				{
					ClusterLog.LogWarning(ex.Message);
					ex = null;
				}
				try
				{
					File.Delete(string.Format(CultureInfo.InvariantCulture, "{0}\\LocaleMetaData\\{1}_{2}.MTA", Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item), CultureInfo.CurrentCulture.LCID));
				}
				catch (IOException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					ClusterLog.LogWarning(ex.Message);
				}
			}
		}
	}

	private string CopyExportedLogToNode(string sourceNode, string targetNode, string fileName)
	{
		string sourceFileName = string.Format(CultureInfo.InvariantCulture, "\\\\{0}\\admin$\\Cluster\\Reports\\{1}", sourceNode, Path.GetFileName(fileName));
		string text = string.Format(CultureInfo.InvariantCulture, "\\\\{0}\\admin$\\Cluster\\Reports\\{1}", targetNode, Path.GetFileName(fileName));
		File.Copy(sourceFileName, text, overwrite: true);
		return text;
	}

	private string ExportLogToNode(string nodeName, string filter)
	{
		string text = string.Format(CultureInfo.InvariantCulture, "\\\\{0}\\admin$\\\\Cluster\\Reports\\EventLog.{0}.{1}.evtx", nodeName, new Random().Next(1000, 9999));
		new EventLogSession(nodeName).SaveEventsToFile(text, filter);
		return text;
	}

	private void OnClusterNodesChanged(object sender, ClusterObjectEventArgs e)
	{
		try
		{
			if (LogFilter == null)
			{
				return;
			}
			string item = NetworkHelp.BuildFqdn(e.ClusterObject, context.ClusterDomain);
			if (e.EventType == ClusterObjectEventType.Added)
			{
				lock (eventLogFilterLock)
				{
					eventLogFilter.Nodes.Add(item);
					return;
				}
			}
			if (e.EventType == ClusterObjectEventType.Deleted)
			{
				lock (eventLogFilterLock)
				{
					eventLogFilter.Nodes.Remove(item);
					return;
				}
			}
		}
		catch (Exception ex)
		{
			base.NotifyUser.ShowError(ex, Resources.CannotReadNodeChannelInfo_Text, new object[1] { e.ClusterObject });
		}
	}

	private void OnContextRefreshed(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		UIThreadHandlerV val = (UIThreadHandlerV)delegate
		{
			RefreshContent();
		};
		if (((Control)(object)this).InvokeRequired)
		{
			((Control)(object)this).BeginInvoke((Delegate)(object)val);
		}
		else
		{
			val.Invoke();
		}
	}

	private void OpenQueryAction(object sender, SnapinActionEventArgs e)
	{
		try
		{
			CriticalEventsFilterDialog filterDialog = GetFilterDialog();
			if (filterDialog == null)
			{
				return;
			}
			CriticalEventsFilterDialog criticalEventsFilterDialog = filterDialog;
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Title = Resources.SelectQueryFile_Text;
				openFileDialog.Filter = Resources.XMLFilter_Text;
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				openFileDialog.RestoreDirectory = true;
				openFileDialog.CheckFileExists = true;
				if (base.NotifyUser.ShowDialog((CommonDialog)openFileDialog) == DialogResult.OK)
				{
					try
					{
						filterDialog.OpenQuery(openFileDialog.FileName);
					}
					catch (ClusterInputValidationException)
					{
						base.NotifyUser.ShowError(Resources.InvalidEventQueryFile_Text, new object[1] { openFileDialog.FileName });
						return;
					}
					if (base.NotifyUser.ShowDialog((Form)(object)filterDialog) == DialogResult.OK)
					{
						SetLogFilter(filterDialog.GetEventFilter());
						StartQuery();
					}
				}
			}
			finally
			{
				((IDisposable)criticalEventsFilterDialog)?.Dispose();
			}
		}
		catch (Exception ex2)
		{
			base.NotifyUser.ShowError(ex2, Resources.OpenQueryError_Text);
		}
	}

	private void SetLogFilter(EventLogFilter filter)
	{
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		lock (eventLogFilterLock)
		{
			eventLogFilter = filter;
		}
	}

	private void RefreshContent()
	{
		try
		{
			ClusterAdministrator.SetStatusBarProgressMessage(CommandResources.RefreshAction_Text.Replace("&", ""), success: true);
			clusterEventsControl.ResetSort();
			StartQuery();
		}
		catch (Exception ex)
		{
			base.NotifyUser.ShowError(ex, Resources.CannotExecuteQuery_Text);
		}
	}

	private void FindNextAction(object sender, SnapinActionEventArgs e)
	{
		try
		{
			clusterEventsControl.ShowFindDialog();
		}
		catch (Exception ex)
		{
			base.NotifyUser.ShowError(ex);
		}
	}

	private void ChooseColumnsAction(object sender, SnapinActionEventArgs e)
	{
		try
		{
			clusterEventsControl.ChooseColumns();
		}
		catch (Exception ex)
		{
			base.NotifyUser.ShowError(ex);
		}
	}

	private EventLogFilter CreateDefaultFilter()
	{
		EventLogFilter filter = new EventLogFilter();
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingEventLogInfo_Text, string.Empty))
		{
			cluadminWaitDialog.DisplayDelay = TimeSpan.FromMilliseconds(500.0);
			cluadminWaitDialog.ShowDialog(base.NotifyUser, delegate
			{
				filter.Nodes.AddRange(context.NodeNames);
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return null;
			}
		}
		filter.Channels.Add(EventLog.SystemChannel);
		filter.Channels.Add(EventLog.ClusterChannelOperational);
		filter.Channels.Add(EventLog.ClusterAwareUpdatingChannelAdmin);
		filter.Channels.Add(EventLog.ClusterAwareUpdatingManagementChannelAdmin);
		filter.Providers = EventLogFilter.ProvidersForSystemChannel;
		filter.Level = EventLevel.Critical | EventLevel.Error | EventLevel.Warning;
		filter.From = ClusterAdministrator.GetClusterEventsStartTime(context.Cluster, useWaitDialog: true);
		return filter;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ClusterEventsContext clusterEventsContext = context;
			clusterEventsContext.NodeNamesChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Remove(clusterEventsContext.NodeNamesChanged, new EventHandler<ClusterObjectEventArgs>(OnClusterNodesChanged));
			context.ContextRefreshed -= OnContextRefreshed;
			clusterEventsControl.SortStart -= OnSortStart;
			clusterEventsControl.SortCompleted -= OnSortCompleted;
			clusterEventsControl.QueryCompleted -= OnQueryCompleted;
			clusterEventsControl.QueryStarted -= OnQueryStarted;
			((Control)(object)clusterEventsControl).KeyUp -= ClusterEventsControl_KeyUp;
			if (components != null)
			{
				components.Dispose();
			}
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ClusterEventsStartPageControl));
		titleBarControl = new TitleBarControl();
		stopButton = new Button();
		messageLabel = new Label();
		clusterEventsControl = new ClusterEventsControl();
		((Control)(object)titleBarControl).SuspendLayout();
		((Control)(object)this).SuspendLayout();
		((Control)(object)titleBarControl).Controls.Add(stopButton);
		((Control)(object)titleBarControl).Controls.Add(messageLabel);
		componentResourceManager.ApplyResources(titleBarControl, "titleBarControl");
		((Control)(object)titleBarControl).MinimumSize = new Size(20, 27);
		((Control)(object)titleBarControl).Name = "titleBarControl";
		componentResourceManager.ApplyResources(stopButton, "stopButton");
		stopButton.BackColor = Color.Transparent;
		stopButton.ForeColor = SystemColors.ControlText;
		stopButton.Name = "stopButton";
		stopButton.UseVisualStyleBackColor = false;
		stopButton.Click += StopButtonClick;
		componentResourceManager.ApplyResources(messageLabel, "messageLabel");
		messageLabel.BackColor = SystemColors.ControlDark;
		messageLabel.ForeColor = SystemColors.ControlText;
		messageLabel.Name = "messageLabel";
		componentResourceManager.ApplyResources(clusterEventsControl, "clusterEventsControl");
		((Control)(object)clusterEventsControl).Name = "clusterEventsControl";
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add((Control)(object)clusterEventsControl);
		((Control)(object)this).Controls.Add((Control)(object)titleBarControl);
		((Control)(object)this).MinimumSize = new Size(0, 0);
		((Control)(object)this).Name = "ClusterEventsStartPageControl";
		((Control)(object)titleBarControl).ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
	}
}

