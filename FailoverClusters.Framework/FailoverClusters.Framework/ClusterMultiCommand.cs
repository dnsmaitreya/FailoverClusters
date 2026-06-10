using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FailoverClusters.Framework;

internal static class ClusterMultiCommand
{
	public static IList<CommandCollection> GetCommands(IList objects)
	{
		if (objects.OfType<Network>().Any())
		{
			return new List<CommandCollection>();
		}
		if (objects.OfType<NetworkInterface>().Any())
		{
			return new List<CommandCollection>();
		}
		if (objects.OfType<Cluster>().Any())
		{
			return new List<CommandCollection>();
		}
		IEnumerable<Node> enumerable = objects.OfType<Node>();
		int num = enumerable.Count();
		if (num > 0 && num == objects.Count)
		{
			return GetCommands(enumerable);
		}
		IEnumerable<Group> enumerable2 = objects.OfType<Group>();
		int num2 = enumerable2.Count();
		if (num2 > 0 && num2 == objects.Count)
		{
			return GetCommands(enumerable2);
		}
		IEnumerable<FileShare> enumerable3 = objects.OfType<FileShare>();
		int num3 = enumerable3.Count();
		if (num3 > 0 && num3 == objects.Count)
		{
			return GetCommands(enumerable3);
		}
		IEnumerable<Resource> enumerable4 = objects.OfType<Resource>();
		if (enumerable4.Count() == objects.Count)
		{
			return GetCommands(enumerable4);
		}
		if (objects.OfType<ClusterDiskPartition>().Count() > 0)
		{
			return new List<CommandCollection>();
		}
		IEnumerable<PoolPhysicalDiskInfo> source = objects.OfType<PoolPhysicalDiskInfo>();
		int num4 = source.Count();
		if (num4 == objects.Count)
		{
			if (num4 > 1)
			{
				CommandCollection item = (CommandCollection)source.First().Commands;
				return new List<CommandCollection> { item };
			}
			return new List<CommandCollection>();
		}
		IEnumerable<PhysicalDiskInfo> source2 = objects.OfType<PhysicalDiskInfo>();
		int num5 = source2.Count();
		if (num5 == objects.Count)
		{
			if (num5 > 1)
			{
				CommandCollection item2 = (CommandCollection)source2.First().Commands;
				return new List<CommandCollection> { item2 };
			}
			return new List<CommandCollection>();
		}
		IEnumerable<Enclosure> source3 = objects.OfType<Enclosure>();
		int num6 = source3.Count();
		if (num6 == objects.Count)
		{
			if (num6 > 1)
			{
				CommandCollection item3 = (CommandCollection)source3.First().Commands;
				return new List<CommandCollection> { item3 };
			}
			return new List<CommandCollection>();
		}
		IEnumerable<StorageNode> source4 = objects.OfType<StorageNode>();
		int num7 = source4.Count();
		if (num7 == objects.Count)
		{
			if (num7 > 1)
			{
				CommandCollection item4 = (CommandCollection)source4.First().Commands;
				return new List<CommandCollection> { item4 };
			}
			return new List<CommandCollection>();
		}
		throw new ArgumentException("objects is not of a known type");
	}

	public static IList<CommandCollection> GetCommands(IEnumerable<Node> nodes)
	{
		List<CommandCollection> list = new List<CommandCollection>();
		Node node = nodes.FirstOrDefault();
		if (node == null)
		{
			return list;
		}
		CommandCollection commandCollection = Node.InitializeApplicationCommands(node.Cluster, nodes);
		if (commandCollection != null)
		{
			list.Add(commandCollection);
		}
		return list;
	}

	public static IList<CommandCollection> GetCommands(IEnumerable<Group> groups)
	{
		List<CommandCollection> list = new List<CommandCollection>();
		Group group = groups.FirstOrDefault();
		if (group == null)
		{
			return list;
		}
		Cluster cluster = group.Cluster;
		GroupType lastKnownType = group.GroupType;
		bool num = groups.All((Group x) => x.GroupType == lastKnownType);
		if (!num || lastKnownType != GroupType.VirtualMachine)
		{
			list.Add(Group.InitializeStateCommands(groups));
		}
		if (num)
		{
			CommandCollection commandCollection = Group.InitializeApplicationCommands(cluster, lastKnownType, groups);
			if (commandCollection != null)
			{
				list.Add(commandCollection);
			}
		}
		if (!num)
		{
			list.Add(Group.InitializeOwnershipHeterogenousCommands(cluster, groups));
		}
		else
		{
			CommandCollection commandCollection2 = Group.InitializeOwnershipHomogenousCommands(cluster, lastKnownType, groups);
			if (commandCollection2 != null)
			{
				list.Add(commandCollection2);
			}
		}
		list.Add(Group.InitializeOtherCommands(cluster, groups));
		return list;
	}

	public static IList<CommandCollection> GetCommands(IEnumerable<Resource> resources)
	{
		List<CommandCollection> list = new List<CommandCollection>();
		Cluster cluster = null;
		list.Add(Resource.InitializeStateCommands(resources));
		ResourceKind resourceKind = ResourceKind.Fetching;
		bool flag = true;
		foreach (Resource resource in resources)
		{
			cluster = resource.Cluster;
			if (resourceKind == ResourceKind.Fetching)
			{
				resourceKind = resource.ResourceType.ResourceKind;
			}
			else if (resourceKind != resource.ResourceType.ResourceKind)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			CommandCollection commandCollection = Resource.InitializeApplicationCommands(cluster, resourceKind, resources);
			if (commandCollection != null)
			{
				list.Add(commandCollection);
			}
		}
		if (!flag)
		{
			CommandCollection commandCollection2 = Resource.InitializeHeterogenousCommands(cluster, resources);
			if (commandCollection2 != null)
			{
				list.Add(commandCollection2);
			}
		}
		else
		{
			CommandCollection commandCollection3 = Resource.InitializeHomogenousCommands(cluster, resourceKind, resources);
			if (commandCollection3 != null)
			{
				list.Add(commandCollection3);
			}
		}
		list.Add(Resource.InitializeOtherCommands(cluster, resources));
		return list;
	}

	public static IList<CommandCollection> GetCommands(IEnumerable<StorageResource> resources)
	{
		List<CommandCollection> list = new List<CommandCollection>();
		Cluster cluster = null;
		list.Add(Resource.InitializeStateCommands(resources));
		ResourceKind resourceKind = ResourceKind.Fetching;
		bool flag = true;
		foreach (StorageResource resource in resources)
		{
			cluster = resource.Cluster;
			if (resourceKind == ResourceKind.Fetching)
			{
				resourceKind = resource.ResourceType.ResourceKind;
			}
			else if (resourceKind != resource.ResourceType.ResourceKind)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			CommandCollection commandCollection = Resource.InitializeApplicationCommands(cluster, resourceKind, resources);
			if (commandCollection != null)
			{
				list.Add(commandCollection);
			}
		}
		if (!flag)
		{
			list.Add(Resource.InitializeHeterogenousCommands(cluster, resources));
		}
		else
		{
			CommandCollection commandCollection2 = Resource.InitializeHomogenousCommands(cluster, resourceKind, resources);
			if (commandCollection2 != null)
			{
				list.Add(commandCollection2);
			}
		}
		return list;
	}

	public static IList<CommandCollection> GetCommands(IEnumerable<FileShare> shares)
	{
		return new List<CommandCollection> { FileShare.InitializeApplicationCommands(shares) };
	}
}

