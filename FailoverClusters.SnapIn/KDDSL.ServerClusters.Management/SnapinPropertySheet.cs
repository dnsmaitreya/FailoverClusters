using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FailoverClusters.WinForms;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class SnapinPropertySheet : SnapinForm
{
	private delegate bool CanApplyChangesDelegate();

	private readonly INotifyUser notifyUser;

	private readonly string objectName;

	private IContainer components;

	private SnapinTabControl tabControl;

	private Button cancelButton;

	private Button applyButton;

	private Button okButton;

	private IEnumerable<SnapinUserControl> PageControls
	{
		get
		{
			foreach (TabPage tabPage in ((TabControl)(object)tabControl).TabPages)
			{
				yield return (SnapinUserControl)tabPage.Controls[0];
			}
		}
	}

	public SnapinPropertySheet()
	{
		InitializeComponent();
	}

	public SnapinPropertySheet(string objectName, IHasPropertyPages context)
		: this(objectName, context.PropertyPages)
	{
	}

	public SnapinPropertySheet(string objectName, IEnumerable<SnapinPropertyPageControlBase> pageControls)
	{
		InitializeComponent();
		this.objectName = objectName;
		notifyUser = (INotifyUser)(object)new PropertyPageNotifyUser(ClusterAdministrator.Console);
		foreach (SnapinPropertyPageControlBase pageControl in pageControls)
		{
			Add((SnapinUserControl)(object)pageControl);
		}
	}

	public SnapinPropertySheet(string objectName, PropertyPageCollection pages)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		InitializeComponent();
		this.objectName = objectName;
		notifyUser = (INotifyUser)new MessageBoxNotifyUser((IWin32Window)this);
		foreach (ClusterPropertyPage page in pages)
		{
			PropertyPageControlBase container = (PropertyPageControlBase)(object)page.Control;
			page.ClearControl();
			Add((SnapinUserControl)(object)container);
		}
	}

	protected override void OnCreateControl()
	{
		((Form)this).OnCreateControl();
		applyButton.Enabled = false;
		((Control)(object)this).Text = string.Format(CultureInfo.CurrentCulture, "{0} {1}", objectName, Resources.Properties_Text);
	}

	private void Add(SnapinUserControl container)
	{
		TabPage tabPage = new TabPage(((ISnapInPropertyPage)container).Title);
		tabPage.Controls.Add((Control)(object)container);
		((Control)(object)tabControl).Tag = null;
		((TabControl)(object)tabControl).TabPages.Add(tabPage);
		((ISnapInPropertyPage)container).DirtyChanged += EnableApplyButton;
	}

	private void EnableApplyButton(object sender, EventArgs args)
	{
		bool enabled = PageControls.Any((SnapinUserControl control) => ((ISnapInPropertyPage)control).IsDirty);
		applyButton.Enabled = enabled;
	}

	private void OkButtonClick(object sender, EventArgs e)
	{
		if (ApplyChanges())
		{
			((Form)this).DialogResult = DialogResult.OK;
		}
	}

	private void ApplyButtonClick(object sender, EventArgs e)
	{
		ApplyChanges();
	}

	private void CancelButtonClick(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.Cancel;
	}

	private bool ApplyChanges()
	{
		bool result = true;
		foreach (ISnapInPropertyPage pageControl in PageControls)
		{
			if (pageControl.IsDirty)
			{
				if ((bool)((Control)pageControl).Invoke(new CanApplyChangesDelegate(pageControl.ApplyChanges)))
				{
					pageControl.IsDirty = false;
				}
				else
				{
					result = false;
				}
			}
		}
		return result;
	}

	private void TabControlSelectedIndexChanged(object sender, EventArgs e)
	{
		if (((TabControl)(object)tabControl).SelectedIndex != -1)
		{
			InitializePageControl(((TabControl)(object)tabControl).SelectedIndex);
		}
	}

	private void InitializePageControl(int index)
	{
		try
		{
			TabPage tabPage = ((TabControl)(object)tabControl).TabPages[index];
			if (tabPage.Tag == null)
			{
				((ISnapInPropertyPage)tabPage.Controls[0]).Initialize(notifyUser);
				tabPage.Tag = tabPage;
			}
		}
		catch (Exception innerException)
		{
			((Form)this).Close();
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.InitializePropertyPageFailed_Text });
		}
	}

	private void SnapinPropertySheetLoad(object sender, EventArgs e)
	{
		if (((TabControl)(object)tabControl).TabPages.Count > 0)
		{
			InitializePageControl(0);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SnapinPropertySheet));
		tabControl = new SnapinTabControl();
		cancelButton = new Button();
		applyButton = new Button();
		okButton = new Button();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tabControl, "tabControl");
		((Control)(object)tabControl).Name = "tabControl";
		((TabControl)(object)tabControl).SelectedIndex = 0;
		((TabControl)(object)tabControl).SelectedIndexChanged += TabControlSelectedIndexChanged;
		cancelButton.DialogResult = DialogResult.Cancel;
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.Name = "cancelButton";
		cancelButton.Click += CancelButtonClick;
		componentResourceManager.ApplyResources(applyButton, "applyButton");
		applyButton.Name = "applyButton";
		applyButton.Click += ApplyButtonClick;
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.Name = "okButton";
		okButton.Click += OkButtonClick;
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add(okButton);
		((Control)this).Controls.Add(applyButton);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Controls.Add((Control)(object)tabControl);
		((Form)this).FormBorderStyle = FormBorderStyle.FixedDialog;
		((Control)this).Name = "SnapinPropertySheet";
		((Form)this).Load += SnapinPropertySheetLoad;
		((Control)this).ResumeLayout(performLayout: false);
	}
}

