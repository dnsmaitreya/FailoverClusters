using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PVirtualMachineGroup : PGroup
{
	private readonly Dictionary<Guid, VirtualMachineState> memberVirtualMachineResources = new Dictionary<Guid, VirtualMachineState>();

	private readonly ConcurrentDictionary<Guid, ClusterForwardedEventArgs> forwardedArgs = new ConcurrentDictionary<Guid, ClusterForwardedEventArgs>();

	private VirtualMachineReplicationHealth replicationHealth;

	public VirtualMachineReplicationHealth ReplicationHealth => replicationHealth;

	public ConcurrentDictionary<Guid, ClusterForwardedEventArgs> ForwardedArgs => forwardedArgs;

	public PVirtualMachineGroup(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, GroupType.VirtualMachine)
	{
	}

	public Dictionary<Guid, VirtualMachineState> GetVirtualMachineChildResources()
	{
		lock (memberVirtualMachineResources)
		{
			Dictionary<Guid, VirtualMachineState> resources = new Dictionary<Guid, VirtualMachineState>();
			memberVirtualMachineResources.ForEach(delegate(KeyValuePair<Guid, VirtualMachineState> keyValue)
			{
				resources.Add(keyValue.Key, keyValue.Value);
			});
			return resources;
		}
	}

	public void AddVirtualMachineChildResource(PVirtualMachineResource privateResource, VirtualMachineState virtualMachineState)
	{
		lock (memberVirtualMachineResources)
		{
			if (!memberVirtualMachineResources.ContainsKey(privateResource.Id))
			{
				memberVirtualMachineResources.Add(privateResource.Id, virtualMachineState);
			}
		}
	}

	public void RemoveVirtualMachineChildResource(Guid resource)
	{
		lock (memberVirtualMachineResources)
		{
			if (memberVirtualMachineResources.ContainsKey(resource))
			{
				memberVirtualMachineResources.Remove(resource);
			}
		}
	}

	public void Migrate(PNode node, VirtualMachineMigrationType migrationType, bool overrideLockState = false)
	{
		if (migrationType != VirtualMachineMigrationType.Live && migrationType != VirtualMachineMigrationType.Quick)
		{
			throw new ArgumentException("Migrate only supports migration types of Quick and Live.");
		}
		ProtectedScope(delegate
		{
			ClusterEventArgs eventArgs2 = new ClusterResourceVirtualMachineLiveMigrateEventArgs(base.Id, VirtualMachineMigrateState.Start, null);
			ClusterWrapperEventArgs e = new ClusterWrapperEventArgs((migrationType == VirtualMachineMigrationType.Live) ? EventType.GroupVirtualMachineLiveMigrateState : EventType.GroupVirtualMachineQuickMigrateState, eventArgs2);
			RouteEvent(e);
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Group.MigrateVirtualMachine(this, node, migrationType, overrideLockState);
			});
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineMigrateException(base.Name, migrationType, ex);
				SetError(ex);
			}
			else
			{
				ClusterEventArgs eventArgs = new ClusterResourceVirtualMachineLiveMigrateEventArgs(base.Id, VirtualMachineMigrateState.End, ex);
				EventType eventType = ((migrationType == VirtualMachineMigrationType.Live) ? EventType.GroupVirtualMachineLiveMigrateState : EventType.GroupVirtualMachineQuickMigrateState);
				RouteEvent(new ClusterWrapperEventArgs(eventType, eventArgs));
			}
			return ex;
		});
	}

	public override List<Action> ProcessNotification(Notification notification)
	{
		List<Action> result = base.ProcessNotification(notification);
		ClusterForwardedEventArgs clusterForwardedEventArgs = notification.Payload as ClusterForwardedEventArgs;
		if (clusterForwardedEventArgs != null && clusterForwardedEventArgs.ForwardedPayload is ClusterPropertiesEventArgs clusterPropertiesEventArgs)
		{
			if (clusterPropertiesEventArgs.Properties.PrivatePropertiesLoaded)
			{
				lock (memberVirtualMachineResources)
				{
					Guid id = clusterPropertiesEventArgs.Id;
					if (memberVirtualMachineResources.ContainsKey(id))
					{
						ClusterProperty clusterProperty = clusterPropertiesEventArgs.Properties["VmState"];
						if (clusterProperty == null)
						{
							memberVirtualMachineResources[id] = VirtualMachineState.Unknown;
						}
						else
						{
							memberVirtualMachineResources[id] = (VirtualMachineState)(uint)clusterProperty.Value;
						}
					}
				}
			}
			clusterPropertiesEventArgs.Properties.Get("ResourceSpecificData2", delegate(ClusterPropertyULong resourceSpecificData2)
			{
				ulong num = resourceSpecificData2.TypedValue >> 58;
				ulong num2 = resourceSpecificData2.TypedValue >> 50;
				VirtualMachineReplicationHealth virtualMachineReplicationHealth = (VirtualMachineReplicationHealth)(num & 3);
				VirtualMachineReplicationHealth virtualMachineReplicationHealth2 = (VirtualMachineReplicationHealth)(num2 & 3);
				VirtualMachineReplicationHealth virtualMachineReplicationHealth3 = VirtualMachineReplicationHealth.NotApplicable;
				if (virtualMachineReplicationHealth == VirtualMachineReplicationHealth.Error || virtualMachineReplicationHealth2 == VirtualMachineReplicationHealth.Error)
				{
					virtualMachineReplicationHealth3 = VirtualMachineReplicationHealth.Error;
				}
				else if (virtualMachineReplicationHealth == VirtualMachineReplicationHealth.Warning || virtualMachineReplicationHealth2 == VirtualMachineReplicationHealth.Warning)
				{
					virtualMachineReplicationHealth3 = VirtualMachineReplicationHealth.Warning;
				}
				else if (virtualMachineReplicationHealth == VirtualMachineReplicationHealth.Normal || virtualMachineReplicationHealth2 == VirtualMachineReplicationHealth.Normal)
				{
					virtualMachineReplicationHealth3 = VirtualMachineReplicationHealth.Normal;
				}
				if (replicationHealth != virtualMachineReplicationHealth3)
				{
					replicationHealth = virtualMachineReplicationHealth3;
					base.ExecuteOnReaderCallbacks.Add(delegate
					{
						RouteEvent(new ClusterWrapperEventArgs(EventType.VirtualMachineReplicationHealthChanged, new ClusterResourceVirtualMachineReplicationHealthEventArgs(base.Id, replicationHealth)));
					});
				}
			});
			forwardedArgs.AddOrUpdate(clusterForwardedEventArgs.Id, clusterForwardedEventArgs, (Guid key, ClusterForwardedEventArgs value) => clusterForwardedEventArgs);
		}
		return result;
	}

	protected override void OnPropertiesChanged(object sender, ClusterPropertiesEventArgs e)
	{
		e.Properties.Get("ResourceSpecificStatus", delegate(ClusterPropertyString resourceSpecificStatusProperty)
		{
			base.ExecuteOnReaderCallbacks.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Information, new ClusterInformationEventArgs(base.Id, resourceSpecificStatusProperty.TypedValue)));
			});
		});
		base.OnPropertiesChanged(sender, e);
	}
}

