using System;
using System.Collections.Generic;

namespace MS.Internal.ServerClusters;

internal class ClusterResourceExEnumItem : ClusterEnumItem
{
	public Guid groupGuid;

	public string groupName;

	public Dictionary<string, Property> rwCommonProperties;

	public Dictionary<string, Property> roCommonProperties;
}
