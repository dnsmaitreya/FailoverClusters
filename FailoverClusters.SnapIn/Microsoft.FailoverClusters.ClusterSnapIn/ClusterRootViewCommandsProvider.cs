using Microsoft.FailoverClusters.UIFramework;
using Microsoft.ManagementConsole;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.ClusterSnapIn;

internal class ClusterRootViewCommandsProvider : ViewCommandsProviderBase
{
	private RootContext RootContext { get; set; }

	public ClusterRootViewCommandsProvider(RootContext rootContext)
	{
		RootContext = rootContext;
	}

	protected override bool IsValidViewCommand(UICommandId commandId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)commandId != 18 && (int)commandId != 19)
		{
			return (int)commandId == 20;
		}
		return true;
	}

	protected override ActionBase GetActionFromUICommandId(UICommandId commandId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected I4, but got Unknown
		return (commandId - 18) switch
		{
			0 => RootContext.ValidateConfigurationAction, 
			1 => RootContext.CreateClusterAction, 
			2 => RootContext.ManageClusterAction, 
			_ => null, 
		};
	}
}
