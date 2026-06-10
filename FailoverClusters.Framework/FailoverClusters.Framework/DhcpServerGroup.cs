using System;
using System.Diagnostics;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class DhcpServerGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.DhcpServer;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.DhcpServerGroup);
	}

	protected override ClusterCommand InitializeManageApplicationCommand()
	{
		return new ClusterCommand(this, "Manage", ClusterCommandId.DhcpServerManage, ClusterCommandCollectionId.GroupGeneral)
		{
			Text = CommandResources.Application_Manage.FormatCurrentCulture(GroupType.Translate()),
			CanExecuteDelegate = (object x) => CanExecuteOnRoleSpecificResourcesClientAccessPoint(this),
			ExecuteDelegate = delegate
			{
				Manage();
			}
		};
	}

	public void Manage()
	{
		Manage(base.SetLastErrorIfNecessary);
	}

	public void Manage(Action<OperationResult> operationResult)
	{
		LoadAsync(delegate
		{
			this.ExecuteMethod(delegate
			{
				ClusterException ex = null;
				try
				{
					UIHelper.ApplicationActivate(Process.Start(GetExtensionStartInfoWithEnvironmentVariables()));
				}
				catch (ClusterDialogException ex2)
				{
					ex = ex2;
				}
				catch (Exception innerException)
				{
					ex = new ClusterManageApplicationException(GroupType.Translate(), innerException);
				}
				finally
				{
					operationResult.SafeCall(new OperationResult(this, ex));
				}
			}, LockAccess.Reader);
		});
	}

	internal DhcpServerGroup(Cluster cluster)
		: base(cluster)
	{
	}
}

