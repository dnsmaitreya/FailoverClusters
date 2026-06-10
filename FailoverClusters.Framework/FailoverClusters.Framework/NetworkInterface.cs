using System;
using System.Collections.Generic;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class NetworkInterface : ClusterObject
{
	private NetworkInterfaceState? state;

	private string node;

	private string network;

	private string adapter;

	private IList<string> ipV4Addresses;

	private IList<string> ipV6Addresses;

	private Icon2 networkInterfaceIcon;

	public override ClusterIdentityType IdentityType => ClusterIdentityType.NetworkInterface;

	public NetworkInterfaceState State => LoadAsync<NetworkInterfaceState, NetworkInterfaceState>(state, 1);

	public string Node => LoadAsync(node, 3);

	public string Network => LoadAsync(network, 3);

	public string Adapter => LoadAsync(adapter, 3);

	public ICollection<string> IPv4Addresses => LoadAsync(ipV4Addresses, 3);

	public ICollection<string> IPv6Addresses => LoadAsync(ipV6Addresses, 3);

	public override Icon2 Icon => ReturnInstance(ref networkInterfaceIcon, () => new Icon2(InvariantResources.NetworkInterface));

	internal override Type OwnerType => typeof(PNetworkInterface);

	public event EventHandler<ClusterNetworkInterfaceStateEventArgs> StateChanged;

	protected override void InitializeCommands(CommandCollection collection)
	{
		base.InitializeCommands(collection);
		foreach (ClusterCommand miscellaneousCommand in MiscellaneousCommands)
		{
			collection.Add(miscellaneousCommand);
		}
	}

	public NetworkInterface(Cluster cluster)
		: base(cluster)
	{
	}

	public override void Delete(bool askConfirmation = false)
	{
		throw new NotImplementedException();
	}

	public override void Delete(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		throw new NotImplementedException();
	}

	public static void Get(Cluster cluster, Guid networkInterfaceId, Action<OperationResult<NetworkInterface>> operationResult, OperationType operationType)
	{
		NetworkInterface networkInterface = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(networkInterfaceId, ClusterIdentityType.NetworkInterface, LockAccess.Reader);
				if (clusterLock != null)
				{
					networkInterface = ((PNetworkInterface)clusterLock.Owner).GetProxy();
				}
				else
				{
					PNetworkInterface pNetworkInterface = (PNetworkInterface)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.NetworkInterface, networkInterfaceId);
					try
					{
						pNetworkInterface.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pNetworkInterface);
						throw;
					}
					networkInterface = pNetworkInterface.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<NetworkInterface> obj = new OperationResult<NetworkInterface>(cluster, networkInterface, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	public static void Get(Cluster cluster, string networkInterfaceName, Action<OperationResult<NetworkInterface>> operationResult, OperationType operationType)
	{
		NetworkInterface networkInterface = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(networkInterfaceName, ClusterIdentityType.Network, LockAccess.Reader);
				if (clusterLock != null)
				{
					networkInterface = ((PNetworkInterface)clusterLock.Owner).GetProxy();
				}
				else
				{
					PNetworkInterface pNetworkInterface = (PNetworkInterface)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.NetworkInterface, networkInterfaceName);
					try
					{
						pNetworkInterface.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pNetworkInterface);
						throw;
					}
					networkInterface = pNetworkInterface.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<NetworkInterface> obj = new OperationResult<NetworkInterface>(cluster, networkInterface, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	public NetworkInterface()
		: base(null)
	{
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents: false, ignorePossibleOwners);
		PNetworkInterface pNetworkInterface = (PNetworkInterface)privateObject;
		SetIdInternal(pNetworkInterface.Id);
		SetNameInternal(pNetworkInterface.Name);
		state = pNetworkInterface.State;
		base.LoadSelection = pNetworkInterface.LoadedSelection;
		Properties = pNetworkInterface.Properties;
		ParseProperties(pNetworkInterface.Properties, trackChanges: false);
		if (subscribeToEvents)
		{
			SubscribeToEvents(pNetworkInterface);
		}
	}

	private IEnumerable<string> ParseProperties(ClusterPropertyCollection properties, bool trackChanges)
	{
		List<string> list = (trackChanges ? new List<string>() : null);
		ParseProperty(properties, "Network", ref network, list);
		ParseProperty(properties, "Node", ref node, list);
		ParseProperty(properties, "Adapter", ref adapter, list);
		ParseProperty(properties, "IPv4Addresses", ref ipV4Addresses, list);
		ParseProperty(properties, "IPv6Addresses", ref ipV6Addresses, list);
		return list;
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.NetworkInterfaceStateChanged:
		{
			ClusterNetworkInterfaceStateEventArgs args = e.EventArgument as ClusterNetworkInterfaceStateEventArgs;
			if (state == args.State && args.Error == null)
			{
				return true;
			}
			state = args.State;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("State");
				this.StateChanged.SafeCall(this, args);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.PropertiesChanged:
		{
			ClusterPropertiesEventArgs clusterPropertiesEventArgs = e.EventArgument as ClusterPropertiesEventArgs;
			if (clusterPropertiesEventArgs.Error != null)
			{
				break;
			}
			IEnumerable<string> propertiesChanged = ParseProperties(clusterPropertiesEventArgs.Properties, trackChanges: true);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				if (propertiesChanged != null)
				{
					propertiesChanged.ForEach(base.OnPropertyChanged);
				}
			}, OperationType.Async, queueOnDispatcher);
			break;
		}
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	public static bool operator ==(NetworkInterface networkInterface1, NetworkInterface networkInterface2)
	{
		return networkInterface1?.Equals(networkInterface2) ?? ((object)networkInterface2 == null);
	}

	public static bool operator !=(NetworkInterface networkInterface1, NetworkInterface networkInterface2)
	{
		return !(networkInterface1 == networkInterface2);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		state = null;
		node = null;
		network = null;
		adapter = null;
		ipV4Addresses = null;
		ipV6Addresses = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("State");
			OnPropertyChanged("Node");
			OnPropertyChanged("Network");
			OnPropertyChanged("Adapter");
			OnPropertyChanged("IPv4Addresses");
			OnPropertyChanged("IPV6Addresses");
			this.StateChanged.SafeCall(this, null);
		}, OperationType.Async);
	}
}

