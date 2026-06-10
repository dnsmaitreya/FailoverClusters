using System;
using System.Management;
using System.Windows;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class VirtualMachineSummaryInformation
{
	internal enum RequestedInformation
	{
		Name = 0,
		ElementName = 1,
		CreationTime = 2,
		Notes = 3,
		EnabledState = 100,
		ProcessorLoad = 101,
		MemoryUsage = 103,
		Heartbeat = 104,
		Uptime = 105,
		GuestOs = 106,
		Snapshots = 107,
		Tasks = 108,
		HealthState = 109,
		OperationalStatus = 110,
		StatusDescriptions = 111,
		MemoryAvailable = 112,
		AvailableMemoryBuffer = 113,
		ReplicationMode = 114,
		ReplicationState = 115,
		ReplicationHealth = 116,
		TestReplicaSystem = 117,
		ReplicationStateEx = 118,
		ReplicationHealthEx = 119,
		ApplicationHealth = 120,
		SwapFilesInUse = 121,
		ReplicationProviderId = 122,
		ICVersionState = 123,
		MemorySpansPhysicalNumaNodes = 132,
		OtherEnabledState = 133,
		EnhancedSessionModeState = 134,
		VirtualSystemSubType = 135,
		HostComputerSystemName = 136
	}

	private static class WmiPropertyNames
	{
		public const string InstanceId = "Name";

		public const string ElementName = "ElementName";

		public const string Version = "Version";

		public const string EnabledState = "EnabledState";

		public const string Notes = "Notes";

		public const string CreationTime = "CreationTime";

		public const string ProcessorLoad = "ProcessorLoad";

		public const string MemoryUsage = "MemoryUsage";

		public const string MemoryAvailable = "MemoryAvailable";

		public const string Heartbeat = "Heartbeat";

		public const string Uptime = "UpTime";

		public const string Snapshots = "Snapshots";

		public const string Tasks = "AsynchronousTasks";

		public const string HealthState = "HealthState";

		public const string OperationalStatus = "OperationalStatus";

		public const string StatusDescriptions = "StatusDescriptions";

		public const string ReplicationState = "ReplicationState";

		public const string ReplicationTag = "ReplicationTag";

		public const string PrimaryServerName = "PrimaryServerName";

		public const string LastApplicationConsistentReplicationTime = "LastApplicationConsistentReplicationTime";

		public const string LastCrashConsistentReplicationTime = "LastCrashConsistentReplicationTime";

		public const string GuestOperatingSystem = "GuestOperatingSystem";

		public const string RelativePath = "__RELPATH";
	}

	private const int ThumbnailImageWidth = 128;

	private const int ThumbnailImageHeight = 96;

	private const ulong InvalidMemoryValue = ulong.MaxValue;

	private string elementName;

	private string version;

	private VirtualMachineState state;

	private string notes;

	private DateTime creationTime;

	private ushort processorLoad;

	private ulong assignedMemory;

	private ulong memoryDemand;

	private ulong availableMemory;

	private VirtualMachineHeartbeatStatus heartbeat;

	private TimeSpan uptime;

	private VirtualMachineComputerSystemHealthState healthState;

	private VirtualMachineComputerSystemOperationalStatus[] operationalStatus;

	private string[] statusDescription;

	private VirtualMachineReplicationState replicationState;

	private string replicationTag;

	private string primaryServerName;

	private DateTime lastApplicationConsistentReplicationTime;

	private DateTime lastCrashConsistentReplicationTime;

	private string guestOS;

	private string guestStatus;

	private DateTimeConverter dateTimeConverter = new DateTimeConverter();

	internal static readonly uint[] VMSummaryInformationBasic = new uint[16]
	{
		0u, 1u, 100u, 101u, 103u, 112u, 105u, 2u, 109u, 115u,
		114u, 136u, 104u, 3u, 111u, 110u
	};

	private static readonly uint[] VMSummaryInformationInit = new uint[14]
	{
		0u, 1u, 100u, 101u, 103u, 112u, 105u, 108u, 109u, 110u,
		111u, 115u, 114u, 136u
	};

	private static readonly uint[] VMSummaryInformationUpdate = new uint[14]
	{
		0u, 1u, 100u, 101u, 103u, 112u, 105u, 3u, 109u, 110u,
		111u, 115u, 114u, 136u
	};

	private static readonly uint[] VMSummaryInformationDetail = new uint[17]
	{
		0u, 1u, 100u, 2u, 3u, 101u, 103u, 112u, 104u, 105u,
		108u, 109u, 110u, 111u, 115u, 114u, 136u
	};

	private static readonly uint[] VMSummaryInformationFull = new uint[18]
	{
		0u, 1u, 100u, 2u, 3u, 101u, 103u, 112u, 104u, 105u,
		108u, 107u, 109u, 110u, 111u, 115u, 114u, 136u
	};

	private static readonly uint[] VMSummaryInformationSnapshots = new uint[2] { 0u, 107u };

	public string Version => version;

	public string ElementName => elementName;

	public string GuestOperatingSystem => guestOS;

	public string GuestStatus => guestStatus;

	public VirtualMachineState State => state;

	public string Notes => notes;

	public DateTime CreationTime => creationTime;

	public ushort ProcessorLoad => processorLoad;

	public ulong AssignedMemory => assignedMemory;

	public ulong MemoryDemand => memoryDemand;

	public ulong AvailableMemory => availableMemory;

	public VirtualMachineHeartbeatStatus Heartbeat => heartbeat;

	public TimeSpan Uptime => uptime;

	public VirtualMachineComputerSystemHealthState HealthState => healthState;

	public VirtualMachineReplicationState ReplicationState => replicationState;

	public string ReplicationMode => replicationTag;

	public string PrimaryServerName => primaryServerName;

	public static Size ThumbnailImageSize => new Size(128.0, 96.0);

	public static ulong InvalidMemory => ulong.MaxValue;

	internal VirtualMachineSummaryInformation(ManagementBaseObject summaryInformation)
	{
		InitializeInformation(summaryInformation, null);
	}

	internal VirtualMachineSummaryInformation(ManagementBaseObject summaryInformation, ManagementObject virtualSystemSettings)
	{
		InitializeInformation(summaryInformation, virtualSystemSettings);
	}

	public VirtualMachineComputerSystemOperationalStatus[] GetOperationalStatus()
	{
		return operationalStatus;
	}

	public string[] GetStatusDescriptions()
	{
		return statusDescription;
	}

	internal static uint[] GetRequestedInformationArray(VirtualMachineSummaryInformationRequest requestedInformation)
	{
		uint[] array = null;
		return requestedInformation switch
		{
			VirtualMachineSummaryInformationRequest.Full => VMSummaryInformationFull, 
			VirtualMachineSummaryInformationRequest.Detail => VMSummaryInformationDetail, 
			VirtualMachineSummaryInformationRequest.Basic => VMSummaryInformationBasic, 
			VirtualMachineSummaryInformationRequest.Initialization => VMSummaryInformationInit, 
			VirtualMachineSummaryInformationRequest.Update => VMSummaryInformationUpdate, 
			VirtualMachineSummaryInformationRequest.Snapshots => VMSummaryInformationSnapshots, 
			_ => throw new ArgumentOutOfRangeException("requestedInformation"), 
		};
	}

	private void InitializeInformation(ManagementBaseObject summaryInformation, ManagementObject virtualSystemSettings)
	{
		if (virtualSystemSettings != null)
		{
			version = (string)virtualSystemSettings.GetPropertyValue("Version");
		}
		else
		{
			version = string.Empty;
		}
		elementName = (string)summaryInformation.GetPropertyValue("ElementName");
		notes = (string)summaryInformation.GetPropertyValue("Notes");
		guestOS = (string)summaryInformation.GetPropertyValue("GuestOperatingSystem");
		object propertyValue = summaryInformation.GetPropertyValue("EnabledState");
		if (propertyValue != null)
		{
			ushort num = (ushort)propertyValue;
			state = (VirtualMachineState)num;
			WmiVMUtilities.IsVMComputerSystemStateValid(state);
		}
		propertyValue = summaryInformation.GetPropertyValue("Heartbeat");
		if (propertyValue != null)
		{
			heartbeat = (VirtualMachineHeartbeatStatus)(ushort)propertyValue;
		}
		propertyValue = summaryInformation.GetPropertyValue("CreationTime");
		if (propertyValue != null)
		{
			creationTime = dateTimeConverter.ConvertFromWmiType(propertyValue);
		}
		propertyValue = summaryInformation.GetPropertyValue("ProcessorLoad");
		if (propertyValue != null)
		{
			processorLoad = (ushort)propertyValue;
		}
		propertyValue = summaryInformation.GetPropertyValue("MemoryUsage");
		if (propertyValue != null)
		{
			assignedMemory = (ulong)propertyValue;
		}
		propertyValue = summaryInformation.GetPropertyValueOrDefault("MemoryAvailable");
		if (propertyValue != null)
		{
			int num2 = (int)propertyValue;
			if (num2 == int.MaxValue)
			{
				num2 = 0;
			}
			availableMemory = (ulong)num2;
		}
		else
		{
			availableMemory = InvalidMemory;
		}
		if (availableMemory != InvalidMemory)
		{
			memoryDemand = assignedMemory * (100 - availableMemory) / 100uL;
		}
		else
		{
			memoryDemand = InvalidMemory;
		}
		propertyValue = summaryInformation.GetPropertyValue("UpTime");
		if (propertyValue != null)
		{
			uptime = TimeSpan.FromMilliseconds((ulong)propertyValue);
		}
		propertyValue = summaryInformation.GetPropertyValue("HealthState");
		if (propertyValue != null)
		{
			ushort num3 = (ushort)propertyValue;
			healthState = (VirtualMachineComputerSystemHealthState)num3;
			WmiVMUtilities.IsVMComputerSystemHealthStateValid(healthState);
		}
		propertyValue = summaryInformation.GetPropertyValue("OperationalStatus");
		if (propertyValue != null)
		{
			operationalStatus = WmiVMUtilities.ConvertOperationalStatus((ushort[])propertyValue);
		}
		propertyValue = summaryInformation.GetPropertyValueOrDefault("StatusDescriptions");
		if (propertyValue != null)
		{
			statusDescription = (string[])propertyValue;
		}
		else
		{
			statusDescription = new string[1] { string.Empty };
		}
		guestStatus = WmiVMUtilities.GetFailureStatusDescription(healthState, operationalStatus, statusDescription);
		propertyValue = summaryInformation.GetPropertyValueOrDefault("ReplicationState");
		if (propertyValue != null)
		{
			ushort num4 = (ushort)propertyValue;
			replicationState = (VirtualMachineReplicationState)num4;
		}
		else
		{
			replicationState = VirtualMachineReplicationState.Disabled;
		}
		propertyValue = summaryInformation.GetPropertyValueOrDefault("ReplicationTag");
		replicationTag = ((propertyValue != null) ? ((string)propertyValue) : string.Empty);
		propertyValue = summaryInformation.GetPropertyValueOrDefault("PrimaryServerName");
		primaryServerName = ((propertyValue != null) ? ((string)propertyValue) : string.Empty);
		propertyValue = summaryInformation.GetPropertyValueOrDefault("LastApplicationConsistentReplicationTime");
		lastApplicationConsistentReplicationTime = ((propertyValue != null) ? dateTimeConverter.ConvertFromWmiType(propertyValue) : DateTime.MinValue);
		propertyValue = summaryInformation.GetPropertyValueOrDefault("LastApplicationConsistentReplicationTime");
		lastCrashConsistentReplicationTime = ((propertyValue != null) ? dateTimeConverter.ConvertFromWmiType(propertyValue) : DateTime.MinValue);
	}
}

