namespace KDDSL.FailoverClusters.Framework;

public enum CimObservableErrorType
{
	None,
	ServerQuotaReached,
	ClassNotFound,
	ConnectionFailure,
	SubscriptionFailure,
	ExceptionObserved,
	Unknown
}
