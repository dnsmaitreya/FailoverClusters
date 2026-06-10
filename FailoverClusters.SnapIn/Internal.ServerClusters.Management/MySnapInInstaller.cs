using System.ComponentModel;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

[RunInstaller(true)]
public class MySnapInInstaller : SnapInInstaller
{
	private IContainer components;

	private void InitializeComponent()
	{
		components = new Container();
	}
}

