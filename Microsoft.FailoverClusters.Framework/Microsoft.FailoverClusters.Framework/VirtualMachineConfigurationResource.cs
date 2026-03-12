using System;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class VirtualMachineConfigurationResource : AverageResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.VirtualMachineConfigurationResource));

	public override bool? IsChild => LoadAsync(isChildResource, 16);

	public override ClusterList<Resource> Children => WeakReferenceEx.ReturnInstance(ref childrenWeak, () => new ClusterList<Resource>(base.Cluster));

	internal bool IgnoreDependantsWhenDelete { get; set; }

	public void RefreshSettings()
	{
		RefreshSettings(base.SetLastErrorIfNecessary);
	}

	public void RefreshSettings(Action<OperationResult> operationResult)
	{
		ExecuteSafe(7, operationResult, delegate(ILockable clusterObject)
		{
			clusterObject.Owner.Cluster.Server.Resource.VirtualMachineRefreshSettings(Id, base.OwnerGroup.OwnerNode.Name);
		}, delegate(Exception exception)
		{
			ClusterVirtualMachineConfigurationUpdateSettingsException result = new ClusterVirtualMachineConfigurationUpdateSettingsException(base.Name, exception);
			ClusterLog.LogException(exception, "Error updating VM settings.");
			return result;
		});
	}

	internal VirtualMachineConfigurationResource(Cluster cluster)
		: base(cluster)
	{
	}
}
