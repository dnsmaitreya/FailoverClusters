namespace KDDSL.ServerClusters;

public enum WindowsServiceState
{
	Stopped = 1,
	StartPending,
	StopPending,
	Running,
	ContinuePending,
	PausePending,
	Paused
}
