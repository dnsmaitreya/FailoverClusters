namespace MS.Internal.FailoverClusters.Framework;

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
