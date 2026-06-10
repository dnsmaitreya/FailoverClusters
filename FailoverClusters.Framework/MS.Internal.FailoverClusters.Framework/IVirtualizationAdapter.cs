using System;
using System.Drawing;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IVirtualizationAdapter
{
	void Collect();

	void DeleteSaveState(string virtualMachineId, string hostName);

	void Pause(string virtualMachineId, string hostName);

	void Resume(string virtualMachineId, string hostName);

	void Reset(string virtualMachineId, string hostName);

	void TakeCheckpoint(string virtualMachineId, string hostName);

	void RevertCheckpoint(string virtualMachineId, string hostName, PVirtualMachineResource virtualMachineResource);

	void ApplyCheckpoint(string virtualMachineId, string hostName, string checkpointId);

	void DeleteCheckpoint(string virtualMachineId, string hostName, string checkpointId);

	void DeleteCheckpointTree(string virtualMachineId, string hostName, string checkpointId);

	void RenameCheckpoint(string virtualMachineId, string hostName, string checkpointId, string newCheckpointName);

	Bitmap GetDesktopThumbnailImage(string virtualMachineId, string hostName);

	VirtualMachineStorageInformation GetStorageInformation(string virtualMachineId, string hostName);

	VirtualMachineCheckpointInformation GetCheckpointInformation(string virtualMachineId, string hostName, PVirtualMachineResource virtualMachineResource);

	VirtualMachineSummaryInformation GetSummaryInformation(string virtualMachineId, string hostName);

	VirtualMachineKeyValuePairs GetKeyValuePairs(string virtualMachineId, string hostName);

	VirtualMachineReplicationInformation GetReplicationInformation(string virtualMachineId, string hostName, ReplicationRelationshipType type);

	Guid GetTestFailoverVirtualMachineId(string virtualMachineId, string hostName);
}

