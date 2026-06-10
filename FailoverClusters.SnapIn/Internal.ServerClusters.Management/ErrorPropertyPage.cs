using System;
using System.ComponentModel;
using System.Windows.Forms;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class ErrorPropertyPage : PropertyPageControlBase
{
	private Exception exception;

	private string name;

	public static void CreateErrorPropertySheet(PropertyPageCollection pages, Exception e, string name)
	{
		pages.Clear();
		ClusterPropertyPage clusterPropertyPage = new ClusterPropertyPage();
		clusterPropertyPage.SetControl(new ErrorPropertyPage(e, name));
		pages.Add(clusterPropertyPage);
	}

	public ErrorPropertyPage(Exception exception, string name)
		: base(Resources.Properties_Text, Guid.Empty)
	{
		this.exception = exception;
		this.name = name;
		InitializeComponent();
	}

	protected ErrorPropertyPage()
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		throw ExceptionHelp.Build<ApplicationException>(exception, new string[2]
		{
			Resources.CannotShowPropertyPages_Text,
			name
		});
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ErrorPropertyPage));
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Name = "ErrorPropertyPage";
		((Control)(object)this).ResumeLayout(performLayout: false);
	}
}

