using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Win32;

namespace KDDSL.ServerClusters.Management;

internal class ClusterNodeDatabaseConnection
{
	private readonly RegistryKey clusterSeriviceParametersKey;

	public ClusterNodeDatabaseConnection(RegistryKey clusterServiceParametersSeriviceParametersKey)
	{
		if (clusterServiceParametersSeriviceParametersKey == null)
		{
			throw new ArgumentNullException("clusterServiceParametersSeriviceParametersKey");
		}
		clusterSeriviceParametersKey = clusterServiceParametersSeriviceParametersKey;
	}

	public string GetClusterName()
	{
		return (string)clusterSeriviceParametersKey.GetValue("ClusterName");
	}

	public ICollection<ClusterDatabaseNode> GetDatabaseNodes(string domain)
	{
		if (string.IsNullOrWhiteSpace(domain))
		{
			throw new ArgumentNullException("domain");
		}
		string[] array = (string[])clusterSeriviceParametersKey.GetValue("NodeNames");
		if (array == null)
		{
			return new List<ClusterDatabaseNode>();
		}
		return (from nodeName in array
			select nodeName.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries) into parts
			let nodeId = new Guid(0, 0, 0, new byte[8]
			{
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				byte.Parse(parts[1], CultureInfo.InvariantCulture)
			})
			select new ClusterDatabaseNode(nodeId, parts[0], domain)).ToList();
	}

	public Guid GetClusterInstanceId()
	{
		return new Guid((byte[])clusterSeriviceParametersKey.GetValue("InstanceGuid"));
	}
}

