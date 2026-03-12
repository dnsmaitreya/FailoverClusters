namespace Microsoft.FailoverClusters.Framework;

internal interface IDelayedWeakReference
{
	bool DelayGcCollection { get; }
}
