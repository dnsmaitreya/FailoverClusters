using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FailoverClusters.UIFramework;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.SnapIn;

internal class DownClusterDataService : IDownClusterDataService, IDownClusterDataChangedService
{
	private RootContext RootContext { get; set; }

	public event EventHandler<DownClusterDataChangedEventArgs> ClustersDataChanged;

	public DownClusterDataService(RootContext rootContext)
	{
		RootContext = rootContext;
	}

	public void NotifyDownClusterChanged(string clusterName, DownClusterDataChangedType downClusterDataChangedType)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		if (this.ClustersDataChanged != null)
		{
			this.ClustersDataChanged(this, new DownClusterDataChangedEventArgs(clusterName, downClusterDataChangedType));
		}
	}

	public IEnumerable<DownClusterDataItem> GetDownClusters()
	{
		List<DownClusterDataItem> downClusterDataItems = new List<DownClusterDataItem>();
		if (RootContext != null)
		{
			RootContext.GetChildContexts().OfType<DownClusterContext>().ToList()
				.ForEach(delegate(DownClusterContext dcc)
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Expected O, but got Unknown
					downClusterDataItems.Add(new DownClusterDataItem(dcc.DisplayName));
				});
		}
		return downClusterDataItems;
	}
}
