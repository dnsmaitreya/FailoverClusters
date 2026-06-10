namespace FailoverClusters.Framework;

internal interface IDelayedWeakReference
{
	bool DelayGcCollection { get; }
}

