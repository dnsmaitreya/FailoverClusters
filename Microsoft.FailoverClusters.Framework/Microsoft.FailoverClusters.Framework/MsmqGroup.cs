using System;
using System.Diagnostics;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class MsmqGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.Msmq;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.MsmqGroup);
	}

	protected override ClusterCommand InitializeManageApplicationCommand()
	{
		return new ClusterCommand(this, "Manage", ClusterCommandId.MsmqManage, ClusterCommandCollectionId.GroupGeneral)
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
					ProcessStartInfo processStartInfo = ProcessHelper.CreateSnapinStartInfo("compmgmt.msc", GroupType);
					string roleSpecificClientAccessPointServerName = Group.GetRoleSpecificClientAccessPointServerName(this);
					processStartInfo.EnvironmentVariables["COMPUTERNAME"] = roleSpecificClientAccessPointServerName;
					processStartInfo.EnvironmentVariables["_CLUSTER_NETWORK_NAME_"] = roleSpecificClientAccessPointServerName;
					processStartInfo.EnvironmentVariables["_CLUSTER_NETWORK_HOSTNAME_"] = roleSpecificClientAccessPointServerName;
					UIHelper.ApplicationActivate(Process.Start(processStartInfo));
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

	internal MsmqGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
