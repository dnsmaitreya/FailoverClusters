using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.SnapIn;
using Microsoft.FailoverClusters.UIFramework;

namespace Microsoft.FailoverClusters.ClusterSnapIn;

internal class ClusterRolesViewAdapter : ClusterGridViewAdapterBase<ClusterRolesViewModel>
{
	protected override ClusterRolesViewModel CreateViewModel(Cluster cluster, ViewModelData viewModelData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		return new ClusterRolesViewModel(cluster);
	}

	public override IEnumerable<IDataItem> ApplyFilterOnSelectedItems(IEnumerable<IDataItem> originalSetOfSelectedItems)
	{
		IDataItem dataItem = originalSetOfSelectedItems.FirstOrDefault();
		IEnumerable<IDataItem> result = Enumerable.Empty<IDataItem>();
		if (dataItem != null)
		{
			Type firstItemType = dataItem.GetType();
			if (originalSetOfSelectedItems.All((IDataItem s) => s.GetType() == firstItemType) || originalSetOfSelectedItems.All((IDataItem s) => s is Resource) || originalSetOfSelectedItems.All((IDataItem s) => s is Group))
			{
				result = originalSetOfSelectedItems;
			}
		}
		return result;
	}
}
