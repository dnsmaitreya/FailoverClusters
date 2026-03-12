using System;
using System.Collections.Generic;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class PStoragePoolResource : PResource
{
	public IEnumerable<PoolPhysicalDiskInfoInternal> PhysicalDisksInfo { get; internal set; }

	public PStoragePoolResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.StoragePool))
	{
	}

	protected PStoragePoolResource(PCluster cluster, Guid id, string name, PResourceType resourceType)
		: base(cluster, id, name, resourceType)
	{
	}

	protected override void OnPropertiesChanged(object sender, ClusterPropertiesEventArgs e)
	{
		base.OnPropertiesChanged(sender, e);
		if (e.PropertyKind == ClusterPropertyKind.Private)
		{
			PhysicalDisksInfo = null;
			base.LoadedSelection &= -1025;
			RouteEvent(new ClusterWrapperEventArgs(EventType.PoolPhysicalDisksInfoChanged, new ClusterStoragePoolPhysicalDisksInfoChangedEventArgs(base.Id, PhysicalDisksInfo)));
		}
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		int num = loadSelectionNeutral;
		int num2 = 0;
		if ((loadSelectionNeutral & 4) == 4)
		{
			num &= -5;
		}
		if ((loadSelectionNeutral & 0x400) == 1024)
		{
			num &= -1025;
		}
		base.LoadObject(num);
		num2 |= num;
		if ((loadSelectionNeutral & 4) == 4)
		{
			try
			{
				int num3 = 4;
				if ((loadSelectionNeutral & 0x20000000) == 536870912)
				{
					num3 |= 0x20000000;
				}
				base.LoadObject(num3);
			}
			catch (ClusterObjectLoadFailedException)
			{
				base.Properties.Add(new ClusterPropertyUInt("Health", null, ClusterPropertyKind.Private, readOnly: true, 0u));
				base.Properties.Add(new ClusterPropertyUInt("State", null, ClusterPropertyKind.Private, readOnly: true, 0u));
				base.LoadedSelection |= 4;
				RouteEvent(new ClusterWrapperEventArgs(EventType.PropertiesChanged, new ClusterPropertiesEventArgs(base.Id, base.Name, null, null)
				{
					Properties = base.Properties
				}));
			}
			num2 |= 4;
		}
		if ((loadSelectionNeutral & 0x400) == 1024)
		{
			try
			{
				int num4 = 1024;
				if ((loadSelectionNeutral & 0x20000000) == 536870912)
				{
					num4 |= 0x20000000;
				}
				base.LoadObject(num4);
				RouteEvent(new ClusterWrapperEventArgs(EventType.PoolPhysicalDisksInfoChanged, new ClusterStoragePoolPhysicalDisksInfoChangedEventArgs(base.Id, PhysicalDisksInfo)));
			}
			catch (ClusterObjectLoadFailedException exception)
			{
				base.LoadedSelection |= 1024;
				RouteEvent(new ClusterWrapperEventArgs(EventType.PoolPhysicalDisksInfoChanged, new ClusterStoragePoolPhysicalDisksInfoChangedEventArgs(base.Id, exception)));
			}
			num2 |= 0x400;
		}
		return new ClusterLoadedEventArgs(base.Id, loaded: true, num2, null);
	}
}
