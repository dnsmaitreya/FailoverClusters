using System;

namespace Microsoft.FailoverClusters.Framework;

public interface IObserverPlus<in T>
{
	void OnCompleted(SubscriptionOperation operation);

	void OnError(SubscriptionOperation operation, Exception ex);

	void OnNext(SubscriptionOperation operation, T value);
}
