using System;
using System.Collections.Generic;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IConnectionAdapterCluster
{
	IEnumerable<Guid> CoreGroups { get; }

	Guid Open(string clusterName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess);

	Guid Open(SafeClusterHandle handle);

	void Close();

	void Load(PCluster cluster, ClusterLoadSelection loadSelection);

	void Rename(string newName);

	string GetConnectedToNode();

	IEnumerable<ClusterDisk> GetAvailableDisks(Guid poolId);

	IEnumerable<ClusterDisk> GetAvailableDisks(Guid poolId, bool all);

	IEnumerable<PResource> CreateDiskResources(IEnumerable<ClusterDisk> clusterableDisks);

	void GetClusterableStoragePools(Action<ClusterableStoragePool> onNext, Action<Exception> onError, Action onCompleted);

	void AddStoragePoolToCluster(ClusterableStoragePool clusterableStoragePool, Action<Exception> onError, Action onCompleted);

	void AddVirtualMachine(Guid vmId, string ownerNodeName);

	bool WillVoterLossCauseQuorumLoss(QuorumVoterActionCheck voterActionCheck, string id);

	QuorumConfigurationPrivate GetQuorumConfiguration();

	void SetQuorumConfiguration(QuorumType quorumType, WitnessType witnessType, string quorumWitness, IEnumerable<string> nonVotingNodes);

	FileShareValidationStatus VerifyFileShare(string path);

	void UpdateFunctionalLevel(bool whatIf = false);

	void Shutdown();

	void Destroy(bool deleteComputerObjects);

	string GetFullyQualifiedDomainName();
}
