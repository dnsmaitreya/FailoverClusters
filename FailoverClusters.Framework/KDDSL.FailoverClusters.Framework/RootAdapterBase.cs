using System;
using System.Collections.Concurrent;

namespace KDDSL.FailoverClusters.Framework;

internal abstract class RootAdapterBase
{
	protected ConcurrentDictionary<Guid, string> MappingIdNameGroup { get; private set; }

	protected ConcurrentDictionary<string, Guid> MappingNameIdGroup { get; private set; }

	protected ConcurrentDictionary<Guid, string> MappingIdNameResource { get; private set; }

	protected ConcurrentDictionary<string, Guid> MappingNameIdResource { get; private set; }

	protected ConcurrentDictionary<Guid, string> MappingIdNameNode { get; private set; }

	protected ConcurrentDictionary<string, Guid> MappingNameIdNode { get; private set; }

	protected ConcurrentDictionary<Guid, string> MappingIdNameNetwork { get; private set; }

	protected ConcurrentDictionary<string, Guid> MappingNameIdNetwork { get; private set; }

	protected ConcurrentDictionary<Guid, string> MappingIdNameNetworkInterface { get; private set; }

	protected ConcurrentDictionary<string, Guid> MappingNameIdNetworkInterface { get; private set; }

	protected ConcurrentDictionary<int, string> MappingNumberNameNode { get; private set; }

	protected ConcurrentDictionary<string, int> MappingNameNumberNode { get; private set; }

	protected RootAdapterBase()
	{
		MappingIdNameGroup = new ConcurrentDictionary<Guid, string>();
		MappingNameIdGroup = new ConcurrentDictionary<string, Guid>();
		MappingIdNameResource = new ConcurrentDictionary<Guid, string>();
		MappingNameIdResource = new ConcurrentDictionary<string, Guid>();
		MappingIdNameNode = new ConcurrentDictionary<Guid, string>();
		MappingNameIdNode = new ConcurrentDictionary<string, Guid>();
		MappingIdNameNetwork = new ConcurrentDictionary<Guid, string>();
		MappingNameIdNetwork = new ConcurrentDictionary<string, Guid>();
		MappingIdNameNetworkInterface = new ConcurrentDictionary<Guid, string>();
		MappingNameIdNetworkInterface = new ConcurrentDictionary<string, Guid>();
		MappingNumberNameNode = new ConcurrentDictionary<int, string>();
		MappingNameNumberNode = new ConcurrentDictionary<string, int>();
	}
}
