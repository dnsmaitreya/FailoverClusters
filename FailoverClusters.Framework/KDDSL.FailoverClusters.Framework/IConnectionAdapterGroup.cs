using System;
using System.Collections.Generic;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal interface IConnectionAdapterGroup
{
	PGroup Open(Guid id);

	PGroup Open(string groupName);

	PGroup Create(string name, GroupType groupType);

	void Close(Guid id);

	void Close(string name);

	void Delete(Guid id, bool force, bool cleanUp);

	void Rename(Guid id, string newName);

	void Online(Guid id, bool overrideLockState = false, bool chooseBestNode = false);

	void Offline(Guid id, bool overrideLockState = false);

	void Move(Guid id, string nodeName, bool overrideLockState = false);

	void CancelOperation(Guid id);

	void SetPriority(Guid id, Priority priority);

	IEnumerable<string> GetPreferredOwners(Guid id);

	void SetPreferredOwners(Guid id, IEnumerable<string> nodes);

	void Load(PGroup group, GroupLoadSelection loadSelection);

	IEnumerable<PGroup> GetAll(bool nullElementOnError);

	void MigrateVirtualMachine(PVirtualMachineGroup resource, PNode node, VirtualMachineMigrationType migrationType, bool overrideLockState = false);
}

