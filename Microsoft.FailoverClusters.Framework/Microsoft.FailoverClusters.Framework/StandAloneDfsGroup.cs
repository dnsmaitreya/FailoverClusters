using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class StandAloneDfsGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.StandAloneDfs;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.StandAloneDfsGroup);
	}

	protected override ClusterCommand InitializeManageApplicationCommand()
	{
		return new ClusterCommand(this, "Manage", ClusterCommandId.StandAloneDfsManage, ClusterCommandCollectionId.GroupGeneral)
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
					UIHelper.ApplicationActivate(Process.Start(new ProcessStartInfo(UIHelper.FindSystem32Application("dfsmgmt.msc") ?? throw new ClusterManagerNotFoundException(GroupType), string.Format(arg0: Group.GetRoleSpecificClientAccessPointServerName(this), provider: CultureInfo.InvariantCulture, format: "/server:{0}"))));
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

	internal StandAloneDfsGroup(Cluster cluster)
		: base(cluster)
	{
	}
}
