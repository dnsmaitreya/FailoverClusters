using System;
using System.Collections.Generic;
using System.Threading;

namespace FailoverClusters.Framework;

public class ClusterGroupOwnerNodeEventArgs : ClusterEventArgs
{
	private List<ManualResetEventSlim> events;

	public Guid NodeId { get; internal set; }

	public ClusterGroupOwnerNodeEventArgs(Guid id, Guid nodeId, ClusterException exception)
		: base(id, exception)
	{
		NodeId = nodeId;
	}

	internal ManualResetEventSlim WaitEvent()
	{
		if (events == null)
		{
			events = new List<ManualResetEventSlim>();
		}
		ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim(initialState: false);
		events.Add(manualResetEventSlim);
		return manualResetEventSlim;
	}

	internal void WaitAndExecuteWhenFinished(Action action)
	{
		Worker.Start(delegate
		{
			try
			{
				if (events != null && events.Count > 0)
				{
					events.ForEach(delegate(ManualResetEventSlim waitHandle)
					{
						waitHandle.Wait();
					});
				}
				action();
			}
			finally
			{
				if (events != null)
				{
					events.ForEach(delegate(ManualResetEventSlim item)
					{
						item.Dispose();
					});
					events.Clear();
					events = null;
				}
			}
		});
	}
}

