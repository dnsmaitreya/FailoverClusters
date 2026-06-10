using System.Collections.Generic;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IConnectionAdapterResourceType
{
	void Load(PResourceType resourceType, ResourceTypeLoadSelection loadSelection);

	PResourceType Open(string resourceTypeName);

	IEnumerable<PResourceType> GetAll(IList<string> queryFields, bool nullElementOnError);

	IEnumerable<string> GetPossibleOwners(string name);

	IEnumerable<NativeMethods.SR_RESOURCE_TYPE_REPLICATED_DISK> GetReplicationResources();

	IEnumerable<ReplicationGroupInfo> GetReplicationGroups();

	PResourceType Create(string name, string displayName, string pathDll);

	void Delete(string resourceType);
}

