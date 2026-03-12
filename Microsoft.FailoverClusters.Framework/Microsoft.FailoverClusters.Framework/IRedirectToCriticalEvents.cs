namespace Microsoft.FailoverClusters.Framework;

public interface IRedirectToCriticalEvents
{
	ClusterObject CriticalEventsParameter { get; }
}
