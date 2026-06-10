using System;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public sealed class WellKnownResourceType
{
	public static string HyperVClusterWmi => "Virtual Machine Cluster WMI";

	public static string HealthService => "Health Service";

	public static string StorageQoS => "Storage QoS Policy Manager";

	public static string VirtualMachineReplicationCoordinator => "Virtual Machine Replication Coordinator";

	public static string StorageReplica => "Storage Replica";

	public static string AzureWitness => "Cloud Witness";

	public static string DistributedNetworkName => "Distributed Network Name";

	public static string ScaleoutFileServer => "Scale Out File Server";

	public static string IScsiTargetServer => "iSCSI Target Server";

	public static string DfsReplicatedFolder => "DFS Replicated Folder";

	public static string NfsShare => "NFS Share";

	public static string DfsNamespace => "Distributed File System";

	public static string VirtualIPv6Address => "Disjoint IPv6 Address";

	public static string VirtualIPv4Address => "Disjoint IPv4 Address";

	public static string NetworkAddressTranslator => "Nat";

	public static string HyperVNetworkVirtualizationPA => "Provider Address";

	public static string ClusterAwareUpdate => "ClusterAwareUpdatingResource";

	public static string TaskScheduler => "Task Scheduler";

	public static string StoragePool => "Storage Pool";

	public static string NetworkFileSystem => "Network File System";

	public static string VMReplicaBroker => "Virtual Machine Replication Broker";

	public static string VirtualMachineConfiguration => "Virtual Machine Configuration";

	public static string VirtualMachine => "Virtual Machine";

	public static string IScsiNameService => "iSNS";

	public static string VolumeShadowCopyServiceTask => "Volume Shadow Copy Service Task";

	public static string GenericScript => "Generic Script";

	public static string WinsService => "WINS Service";

	public static string DistributedTransactionCoordinator => "Distributed Transaction Coordinator";

	public static string MicrosoftMessageQueueTrigger => "MSMQTriggers";

	public static string MicrosoftMessageQueue => "MSMQ";

	public static string DhcpService => "DHCP Service";

	public static string PrintSpooler => "Print Spooler";

	public static string FileShareWitness => "File Share Witness";

	public static string FileServer => "File Server";

	public static string NetName => "Network Name";

	public static string IPv6TunnelAddress => "IPv6 Tunnel Address";

	public static string IPv6Address => "IPv6 Address";

	public static string IPAddress => "IP Address";

	public static string PhysicalDisk => "Physical Disk";

	public static string GenericService => "Generic Service";

	public static string GenericApplication => "Generic Application";

	private WellKnownResourceType()
	{
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsWellKnownResourceType(string resourceTypeName)
	{
		string[] array = new string[41];
		string text = "Generic Application";
		array[0] = text;
		string text2 = "Generic Service";
		array[1] = text2;
		string text3 = "Physical Disk";
		array[2] = text3;
		string text4 = "IP Address";
		array[3] = text4;
		string text5 = "IPv6 Address";
		array[4] = text5;
		string text6 = "IPv6 Tunnel Address";
		array[5] = text6;
		string text7 = "Network Name";
		array[6] = text7;
		string text8 = "File Server";
		array[7] = text8;
		string text9 = "File Share Witness";
		array[8] = text9;
		string text10 = "Print Spooler";
		array[9] = text10;
		string text11 = "DHCP Service";
		array[10] = text11;
		string text12 = "MSMQ";
		array[11] = text12;
		string text13 = "MSMQTriggers";
		array[12] = text13;
		string text14 = "Distributed Transaction Coordinator";
		array[13] = text14;
		string text15 = "WINS Service";
		array[14] = text15;
		string text16 = "Generic Script";
		array[15] = text16;
		string text17 = "Volume Shadow Copy Service Task";
		array[16] = text17;
		string text18 = "iSNS";
		array[17] = text18;
		string text19 = "NFS Share";
		array[18] = text19;
		string text20 = "Virtual Machine";
		array[19] = text20;
		string text21 = "Virtual Machine Configuration";
		array[20] = text21;
		string text22 = "Distributed File System";
		array[21] = text22;
		string text23 = "DFS Replicated Folder";
		array[22] = text23;
		string text24 = "iSCSI Target Server";
		array[23] = text24;
		string text25 = "Scale Out File Server";
		array[24] = text25;
		string text26 = "Distributed Network Name";
		array[25] = text26;
		string text27 = "Virtual Machine Replication Broker";
		array[26] = text27;
		string text28 = "Network File System";
		array[27] = text28;
		string text29 = "Task Scheduler";
		array[28] = text29;
		string text30 = "Storage Pool";
		array[29] = text30;
		string text31 = "ClusterAwareUpdatingResource";
		array[30] = text31;
		string text32 = "Provider Address";
		array[31] = text32;
		string text33 = "Nat";
		array[32] = text33;
		string text34 = "Disjoint IPv4 Address";
		array[33] = text34;
		string text35 = "Disjoint IPv6 Address";
		array[34] = text35;
		string text36 = "Cloud Witness";
		array[35] = text36;
		string text37 = "Storage Replica";
		array[36] = text37;
		string text38 = "Health Service";
		array[37] = text38;
		string text39 = "Virtual Machine Cluster WMI";
		array[38] = text39;
		string text40 = "Storage QoS Policy Manager";
		array[39] = text40;
		string text41 = "Virtual Machine Replication Coordinator";
		array[40] = text41;
		if (resourceTypeName == null)
		{
			throw new ArgumentNullException("resourceTypeName");
		}
		int num = 0;
		if (0 < (nint)array.LongLength)
		{
			do
			{
				string strA = array[num];
				if (0 != string.Compare(strA, resourceTypeName, StringComparison.OrdinalIgnoreCase))
				{
					num++;
					continue;
				}
				return true;
			}
			while (num < (nint)array.LongLength);
		}
		return false;
	}
}

