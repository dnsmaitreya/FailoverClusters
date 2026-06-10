namespace KDDSL.ServerClusters;

public sealed class EventLog
{
	public static string ReplicationChannelOperational => "Windows-StorageReplica/Operational";

	public static string ReplicationChannelAdmin => "Windows-StorageReplica/Admin";

	public static string ClusterAwareUpdatingChannelAdmin => "Windows-ClusterAwareUpdating/Admin";

	public static string ClusterAwareUpdatingManagementChannelAdmin => "Windows-ClusterAwareUpdating-Management/Admin";

	public static string ClusterChannelOperational => "Windows-FailoverClustering/Operational";

	public static string ClusterChannelRoot => "Windows-FailoverClustering";

	public static string SystemChannel => "System";

	public static string ClusterAwareUpdatingProvider => "Windows-ClusterAwareUpdating";

	public static string ClusterAwareUpdatingManagementProvider => "Windows-ClusterAwareUpdating-Management";

	public static string ClusterProvider => "Windows-FailoverClustering";

	private EventLog()
	{
	}
}
