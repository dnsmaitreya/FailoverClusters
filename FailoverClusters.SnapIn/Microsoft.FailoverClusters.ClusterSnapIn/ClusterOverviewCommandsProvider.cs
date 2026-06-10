using FailoverClusters.UIFramework;
using ManagementConsole;
using MS.Internal.ServerClusters.Management;

namespace FailoverClusters.ClusterSnapIn;

internal class ClusterOverviewCommandsProvider : ViewCommandsProviderBase
{
	private ClusterContext ClusterContext { get; set; }

	public ClusterOverviewCommandsProvider(ClusterContext clusterContext)
	{
		ClusterContext = clusterContext;
	}

	protected override ActionBase GetActionFromUICommandId(UICommandId commandId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected I4, but got Unknown
		if (IsValidViewCommand(commandId))
		{
			switch (commandId - 21)
			{
			case 0:
				return ClusterContext.ConfigureRoleAction;
			case 1:
				return ClusterContext.ValidateClusterAction;
			case 2:
				return ClusterContext.AddNodeAction;
			case 3:
				return ClusterContext.CopyClusterRolesAction;
			case 4:
				return ClusterContext.ClusterAwareUpdatingAction;
			}
		}
		return null;
	}

	protected override bool IsValidViewCommand(UICommandId commandId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		if ((int)commandId != 21 && (int)commandId != 22 && (int)commandId != 23 && (int)commandId != 24)
		{
			return (int)commandId == 25;
		}
		return true;
	}
}

