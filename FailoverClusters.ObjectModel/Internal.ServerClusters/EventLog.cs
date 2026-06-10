namespace MS.Internal.ServerClusters;

public sealed class EventLog
{
	public static string ReplicationChannelOperational => "Microsoft-Windows-StorageReplica/Operational";

	public static string ReplicationChannelAdmin => "Microsoft-Windows-StorageReplica/Admin";

	public static string ClusterAwareUpdatingChannelAdmin => "Microsoft-Windows-ClusterAwareUpdating/Admin";

	public static string ClusterAwareUpdatingManagementChannelAdmin => "Microsoft-Windows-ClusterAwareUpdating-Management/Admin";

	public static string ClusterChannelOperational => "Microsoft-Windows-FailoverClustering/Operational";

	public static string ClusterChannelRoot => "Microsoft-Windows-FailoverClustering";

	public static string SystemChannel => "System";

	public static string ClusterAwareUpdatingProvider => "Microsoft-Windows-ClusterAwareUpdating";

	public static string ClusterAwareUpdatingManagementProvider => "Microsoft-Windows-ClusterAwareUpdating-Management";

	public static string ClusterProvider => "Microsoft-Windows-FailoverClustering";

	private EventLog()
	{
	}
}
