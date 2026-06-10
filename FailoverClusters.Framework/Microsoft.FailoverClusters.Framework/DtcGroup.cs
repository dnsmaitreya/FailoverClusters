using System;
using System.Diagnostics;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class DtcGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.Dtc;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.DtcGroup);
	}

	protected override ClusterCommand InitializeManageApplicationCommand()
	{
		return new ClusterCommand(this, "Manage", ClusterCommandId.DtcManage, ClusterCommandCollectionId.GroupGeneral)
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
					ProcessStartInfo processStartInfo = ProcessHelper.CreateSnapinStartInfo("comexp.msc", GroupType);
					string roleSpecificClientAccessPointServerName = Group.GetRoleSpecificClientAccessPointServerName(this);
					processStartInfo.EnvironmentVariables["MMC_SNAPIN_MACHINE_NAME"] = roleSpecificClientAccessPointServerName;
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

	internal DtcGroup(Cluster cluster)
		: base(cluster)
	{
	}
}

