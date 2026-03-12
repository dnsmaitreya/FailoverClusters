using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterResourceVirtualMachineGuestStatusEventArgs : ClusterEventArgs
{
	public ulong GuestAssignedMemory { get; internal set; }

	public ulong GuestAvailableMemory { get; internal set; }

	public ulong GuestMemoryDemand { get; internal set; }

	public ulong GuestMemoryStatus { get; internal set; }

	public ushort GuestCpuUsage { get; internal set; }

	public TimeSpan GuestUptime { get; internal set; }

	public DateTime GuestCreationTime { get; internal set; }

	public string GuestNotes { get; internal set; }

	public string GuestStatus { get; internal set; }

	public VirtualMachineHeartbeatStatus? HeartbeatStatus { get; internal set; }

	public ClusterResourceVirtualMachineGuestStatusEventArgs(Guid id, ulong? guestAssignedMemory, ulong? guestAvailableMemory, ulong? guestMemoryDemand, ushort? guestCpuUsage, TimeSpan? guestUptime, DateTime? guestCreationTime, string guestNotes, VirtualMachineHeartbeatStatus? heartbeatStatus, string guestStatus, ClusterException exception)
		: base(id, exception)
	{
		GuestAssignedMemory = (guestAssignedMemory.HasValue ? guestAssignedMemory.Value : 0);
		GuestAvailableMemory = (guestAvailableMemory.HasValue ? guestAvailableMemory.Value : 0);
		GuestMemoryDemand = (guestMemoryDemand.HasValue ? guestMemoryDemand.Value : 0);
		GuestCpuUsage = (ushort)(guestCpuUsage.HasValue ? guestCpuUsage.Value : 0);
		GuestUptime = (guestUptime.HasValue ? guestUptime.Value : new TimeSpan(0L));
		GuestCreationTime = (guestCreationTime.HasValue ? guestCreationTime.Value : new DateTime(0L));
		GuestNotes = guestNotes;
		GuestStatus = guestStatus;
		HeartbeatStatus = heartbeatStatus;
	}
}
