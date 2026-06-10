namespace FailoverClusters.Framework;

public enum TemperatureSensorOperationalStatus : ushort
{
	Unknown = 0,
	Ok = 2,
	Degraded = 3,
	Error = 6,
	NonRecoverable = 7,
	NotInstalled = 53257,
	NotAvailable = 53258,
	NoAccessAllowed = 53259,
	NotReported = 53260
}

