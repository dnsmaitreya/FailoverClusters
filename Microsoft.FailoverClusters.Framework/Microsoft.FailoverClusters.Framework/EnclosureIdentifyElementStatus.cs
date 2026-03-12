namespace Microsoft.FailoverClusters.Framework;

public enum EnclosureIdentifyElementStatus : uint
{
	Success = 0u,
	NotSupported = 1u,
	UnspecifiedError = 2u,
	Timeout = 3u,
	Failed = 4u,
	InvalidParameter = 5u,
	AccessDenied = 40001u,
	NotEnoughResources = 40002u,
	CannotConnectStorageProvider = 46000u,
	StorageProviderCannotConnectToStorageSubsystem = 46001u,
	SlotNumbersNotValid = 55000u,
	IdentificationNotSupported = 55001u
}
