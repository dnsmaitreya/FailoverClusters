using System;
using System.Collections.Generic;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IConnectionAdapterResource
{
	PResource Open(Guid id);

	PResource Open(string resourceName);

	PResource Create(PGroup privateGroup, string name, PResourceType resourceType, bool separateMonitor);

	void Delete(Guid id, bool cleanup);

	void Rename(Guid id, string newName);

	void Online(Guid id, bool overrideLockState = false, bool chooseBestNode = false);

	void Offline(Guid id, bool overrideLockState = false);

	void Fail(Guid id);

	void OfflineDependents(Guid id, bool overrideLockState = false);

	void SetDependencyRelationship(Guid id, string relationship);

	void AddDependency(Guid id, Guid dependencyId);

	void RemoveDependency(Guid id, Guid dependencyId);

	void MoveToGroup(Guid resourceId, Guid groupId);

	string GetType(Guid id, string name);

	void AddRegistryCheckpoint(Guid id, string checkpoint);

	void RemoveRegistryCheckpoint(Guid id, string checkpoint);

	IEnumerable<string> GetRegistryCheckpoints(Guid id);

	void AddCryptoCheckpoint(Guid id, string checkpoint);

	void RemoveCryptoCheckpoint(Guid id, string checkpoint);

	IEnumerable<string> GetCryptoCheckpoints(Guid id);

	IEnumerable<string> GetPossibleOwners(string name);

	void SetPossibleOwners(Guid id, IEnumerable<Guid> nodes);

	void AddPossibleOwner(Guid id, string node);

	void RemovePossibleOwner(Guid id, string node);

	void Load(PResource resource, ResourceLoadSelection loadSelection);

	IEnumerable<PResource> GetAll(bool nullElementOnError);

	void FetchVirtualPropertiesPayload(ClusterPropertiesEventArgs propertiesPayload);

	void VirtualMachineTurnOff(PVirtualMachineResource resource);

	void VirtualMachineSave(PVirtualMachineResource resource);

	void VirtualMachineShutdown(PVirtualMachineResource resource);

	string GetVirtualMachineOwnerGroup(Guid vmId);

	void VirtualMachineMoveStorage(Guid resourceId, string hostName, VirtualMachineStorageMoveParameters virtualMachineStorageMoveParameters);

	void VirtualMachineRefreshSettings(Guid resourceId, string hostName);

	void NetworkNameRepairActiveDirectoryObject(Guid id);

	void NetworkNameEnableAdObject(PNetNameResource netNameResourcePrivate);

	void NetworkNameResetCnoPassword(PNetNameResource netNameResourcePrivate);

	void NetworkNameRepairReAclDNSRecords(PNetNameResource privateCNOResource);

	void AddToClusterSharedVolumes(PStorageResource storageResourcePrivate);

	void RemoveFromClusterSharedVolumes(PCsvVolumeResource csvVolumeResourcePrivate);

	void SetCsvRedirectedAccess(PCsvVolumeResource csvVolumeResourcePrivate, Guid deviceId, bool csvRedirectedAccessMode);

	void SetMaintenanceMode(PStorageResource storageResourcePrivate, bool maintenanceMode);

	uint GetDiskNumber(PStorageResource storageResourcePrivate, string nodeName);

	CsvVolumeInformation GetCsvVolumeInformation(PCsvVolumeResource csvVolumeResourcePrivate);

	void Renew(PCommonIPAddressResource ipAddress);

	void Release(PCommonIPAddressResource ipAddress);
}
