using System.Collections.Generic;
using System.Linq;
using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UIFramework;

namespace Microsoft.FailoverClusters.SnapIn;

internal class ClusterEnclosuresViewAdapter : ClusterGridViewAdapterBase<ClusterEnclosuresViewModel>
{
	protected override ClusterEnclosuresViewModel CreateViewModel(Cluster cluster, ViewModelData viewModelData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		return new ClusterEnclosuresViewModel(cluster);
	}

	public override IEnumerable<IDataItem> ApplyFilterOnSelectedItems(IEnumerable<IDataItem> originalSetOfSelectedItems)
	{
		IDataItem firstItem = originalSetOfSelectedItems.FirstOrDefault();
		IEnumerable<IDataItem> result = Enumerable.Empty<IDataItem>();
		if (firstItem != null && originalSetOfSelectedItems.All((IDataItem s) => s.GetType() == firstItem.GetType()))
		{
			result = originalSetOfSelectedItems;
		}
		return result;
	}
}
