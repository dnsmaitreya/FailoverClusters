using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class VolumeShadowCopyServiceGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	private string applicationName;

	private string applicationParams;

	private string currentDirectory;

	private byte[] triggerArray;

	private bool propertiesDirty;

	private bool triggerArrayDirty;

	private IContainer components;

	private Label commandLabel;

	private TextBox commandTextBox;

	private TextBox parametersTextBox;

	private Label parametersLabel;

	private TextBox startInTextBox;

	private Label startInLabel;

	private Button scheduleButton;

	internal VolumeShadowCopyServiceGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		applicationName = (string)privateProperties["ApplicationName"].Value;
		applicationParams = (string)privateProperties["ApplicationParams"].Value;
		currentDirectory = (string)privateProperties["CurrentDirectory"].Value;
		triggerArray = (byte[])privateProperties["TriggerArray"].Value;
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		commandTextBox.Text = applicationName;
		parametersTextBox.Text = applicationParams;
		startInTextBox.Text = currentDirectory;
		propertiesDirty = false;
		triggerArrayDirty = false;
	}

	protected override bool ValidateProperties()
	{
		if (!base.ValidateProperties())
		{
			return false;
		}
		if (propertiesDirty)
		{
			applicationName = InputValidator.ValidateNonemptyString(commandTextBox.Text, Resources.CommandToRun_Text);
			applicationParams = parametersTextBox.Text.Trim();
			currentDirectory = InputValidator.ValidateNonemptyString(startInTextBox.Text, Resources.StartIn_Text);
		}
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			base.SaveProperties(waitDialog);
			PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
			if (propertiesDirty)
			{
				privateProperties["ApplicationName"].Value = applicationName;
				privateProperties["ApplicationParams"].Value = applicationParams;
				privateProperties["CurrentDirectory"].Value = currentDirectory;
			}
			if (triggerArrayDirty)
			{
				privateProperties["TriggerArray"].Value = triggerArray;
			}
			SaveProperties(privateProperties);
			propertiesDirty = false;
			triggerArrayDirty = false;
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving Vsc Service task properties");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[1] { Resources.VscServiceTaskSavedFailed_Text });
		}
	}

	private void PropertiesChanged(object sender, EventArgs e)
	{
		base.IsDirty = (propertiesDirty = true);
	}

	private void ScheduleClicked(object sender, EventArgs e)
	{
		IntPtr newTriggers = IntPtr.Zero;
		try
		{
			int newSize = 0;
			if (EditVssTaskSchedule(triggerArray, triggerArray.Length, ref newTriggers, ref newSize, currentDirectory, ((Control)(object)this).Handle) == 0)
			{
				byte[] array = new byte[newSize];
				if (newSize > 0)
				{
					Marshal.Copy(newTriggers, array, 0, newSize);
				}
				bool flag = array.Length == triggerArray.Length;
				int num = 0;
				while (flag && num < triggerArray.Length)
				{
					flag = array[num] == triggerArray[num];
					num++;
				}
				if (!flag)
				{
					triggerArray = array;
					base.IsDirty = (triggerArrayDirty = true);
				}
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

	[DllImport("failoverclusters.snapinsupport.dll")]
	private static extern uint EditVssTaskSchedule(byte[] triggers, int size, ref IntPtr newTriggers, ref int newSize, [MarshalAs(UnmanagedType.LPWStr)] string caption, IntPtr handleParent);

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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(VolumeShadowCopyServiceGeneralPropertiesPage));
		commandLabel = new Label();
		commandTextBox = new TextBox();
		parametersTextBox = new TextBox();
		parametersLabel = new Label();
		startInTextBox = new TextBox();
		startInLabel = new Label();
		scheduleButton = new Button();
		((Control)(object)this).SuspendLayout();
		commandLabel.AutoEllipsis = true;
		commandLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(commandLabel, "commandLabel");
		commandLabel.Name = "commandLabel";
		componentResourceManager.ApplyResources(commandTextBox, "commandTextBox");
		commandTextBox.BackColor = SystemColors.Window;
		commandTextBox.Name = "commandTextBox";
		commandTextBox.TextChanged += PropertiesChanged;
		componentResourceManager.ApplyResources(parametersTextBox, "parametersTextBox");
		parametersTextBox.BackColor = SystemColors.Window;
		parametersTextBox.Name = "parametersTextBox";
		parametersTextBox.TextChanged += PropertiesChanged;
		parametersLabel.AutoEllipsis = true;
		parametersLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(parametersLabel, "parametersLabel");
		parametersLabel.Name = "parametersLabel";
		componentResourceManager.ApplyResources(startInTextBox, "startInTextBox");
		startInTextBox.BackColor = SystemColors.Window;
		startInTextBox.Name = "startInTextBox";
		startInTextBox.TextChanged += PropertiesChanged;
		startInLabel.AutoEllipsis = true;
		startInLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(startInLabel, "startInLabel");
		startInLabel.Name = "startInLabel";
		componentResourceManager.ApplyResources(scheduleButton, "scheduleButton");
		scheduleButton.ForeColor = SystemColors.ControlText;
		scheduleButton.Name = "scheduleButton";
		scheduleButton.UseVisualStyleBackColor = true;
		scheduleButton.Click += ScheduleClicked;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(commandLabel);
		((Control)(object)this).Controls.Add(commandTextBox);
		((Control)(object)this).Controls.Add(parametersLabel);
		((Control)(object)this).Controls.Add(parametersTextBox);
		((Control)(object)this).Controls.Add(startInLabel);
		((Control)(object)this).Controls.Add(startInTextBox);
		((Control)(object)this).Controls.Add(scheduleButton);
		((Control)(object)this).Name = "VolumeShadowCopyServiceGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(scheduleButton, 0);
		((Control)(object)this).Controls.SetChildIndex(startInTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(startInLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(parametersTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(parametersLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(commandTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(commandLabel, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
