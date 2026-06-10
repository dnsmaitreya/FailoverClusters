using System;
using System.Collections.Generic;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IConnectionAdapterStorage
{
	void RemoveReplication(PStorageResource storageResource, bool fullCleanUp);

	IEnumerable<Guid> GetReplicationGroupPartnership(PNode ownerNode, Guid replicationGroupId, ReplicationGroupRole role);

	IEnumerable<string> GetReplicationGroupPartnership(PNode ownerNode, string replicationGroupName, ReplicationGroupRole role);

	void SetReplicationLogSize(PStorageResource storageResourcePrivate, long logSize);

	void LoadReplicationInfo(PStorageResource storageResource);

	uint? GetDiskNumber(PStorageResource storageResourcePrivate, string uniqueId, string nodeName);

	string GetUniqueId(uint diskNumber, string nodeName);

	IEnumerable<T1> Enumerate<T1>(ObservableKeyCollection<T1> collection, ObservableCollectionFilter<T1> filter) where T1 : IKeyQueryable<T1>;

	IEnumerable<T1> Association<T, T1>(ObservableKeyCollection<T1> collection, T association) where T1 : IKeyQueryable<T1>;

	void Subscribe<T1>(ObservableKeyCollection<T1> collection) where T1 : IKeyQueryable<T1>;

	void Unsubscribe<T1>(ObservableKeyCollection<T1> collection) where T1 : IKeyQueryable<T1>;

	T1 GetInstance<T1>(string key, string serverName = null) where T1 : IKeyQueryable;
}

