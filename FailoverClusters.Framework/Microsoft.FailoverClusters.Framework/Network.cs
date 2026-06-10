using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class Network : ClusterObject
{
	private NetworkState? state;

	private NetworkRole? role;

	private Icon2 networkIcon;

	private IList<string> ipV4Addresses;

	private IList<string> ipV4PrefixLengths;

	private IList<string> ipV6Addresses;

	private IList<string> ipV6PrefixLengths;

	private WeakReferenceEx networkInterfacesWeak;

	public override ClusterIdentityType IdentityType => ClusterIdentityType.Network;

	public NetworkState State => LoadAsync<NetworkState, NetworkState>(state, 1);

	public NetworkRole Role => LoadAsync<NetworkRole, NetworkRole>(role, 3);

	public IList<string> IPv4Addresses => LoadAsync(ipV4Addresses, 3);

	public IList<string> IPv4PrefixLengths => LoadAsync(ipV4PrefixLengths, 3);

	public IList<string> IPv6Addresses => LoadAsync(ipV6Addresses, 3);

	public IList<string> IPv6PrefixLengths => LoadAsync(ipV6PrefixLengths, 3);

	public string Subnets => ComputeSubnetsString();

	public ClusterList<NetworkInterface> NetworkInterfaces => WeakReferenceEx.ReturnInstance(ref networkInterfacesWeak, delegate
	{
		string networkName = base.Name;
		return (ClusterList<NetworkInterface>)base.Cluster.NetworkInterfaces.Where((NetworkInterface ni) => ni.Network == networkName);
	});

	public override Icon2 Icon => ReturnInstance(ref networkIcon, () => new Icon2(InvariantResources.Network));

	internal override Type OwnerType => typeof(PNetwork);

	public event EventHandler<ClusterNetworkStateEventArgs> StateChanged;

	protected override void InitializeCommands(CommandCollection collection)
	{
		base.InitializeCommands(collection);
		foreach (ClusterCommand miscellaneousCommand in MiscellaneousCommands)
		{
			collection.Add(miscellaneousCommand);
		}
		ClusterCommand item2 = new ClusterCommand(this, "NetworkProperties", ClusterCommandId.NetworkProperties, ClusterCommandCollectionId.NetworkProperties)
		{
			Text = CommandResources.Properties,
			ExecuteDelegate = delegate
			{
				throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration for show properties.");
			},
			CommandParameter = this
		};
		collection.Add(item2);
	}

	public Network(Cluster cluster)
		: base(cluster)
	{
	}

	public override void Delete(bool askConfirmation = false)
	{
		throw new NotSupportedException();
	}

	public override void Delete(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		throw new NotSupportedException();
	}

	public static void Get(Cluster cluster, Guid networkId, Action<OperationResult<Network>> operationResult, OperationType operationType)
	{
		Network network = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(networkId, ClusterIdentityType.Network, LockAccess.Reader);
				if (clusterLock != null)
				{
					network = ((PNetwork)clusterLock.Owner).GetProxy();
				}
				else
				{
					PNetwork pNetwork = (PNetwork)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.Network, networkId);
					try
					{
						pNetwork.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pNetwork);
						throw;
					}
					network = pNetwork.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<Network> obj = new OperationResult<Network>(cluster, network, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	public static void Get(Cluster cluster, string networkName, Action<OperationResult<Network>> operationResult, OperationType operationType)
	{
		Network network = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(networkName, ClusterIdentityType.Network, LockAccess.Reader);
				if (clusterLock != null)
				{
					network = ((PNetwork)clusterLock.Owner).GetProxy();
				}
				else
				{
					PNetwork pNetwork = (PNetwork)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.Network, networkName);
					try
					{
						pNetwork.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pNetwork);
						throw;
					}
					network = pNetwork.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<Network> obj = new OperationResult<Network>(cluster, network, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	public Network()
		: base(null)
	{
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents: false, ignorePossibleOwners);
		PNetwork pNetwork = (PNetwork)privateObject;
		SetIdInternal(pNetwork.Id);
		SetNameInternal(pNetwork.Name);
		state = pNetwork.State;
		base.LoadSelection = pNetwork.LoadedSelection;
		Properties = pNetwork.Properties;
		ParseProperties(pNetwork.Properties, trackChanges: false);
		if (subscribeToEvents)
		{
			SubscribeToEvents(pNetwork);
		}
	}

	private IEnumerable<string> ParseProperties(ClusterPropertyCollection properties, bool trackChanges)
	{
		List<string> list = (trackChanges ? new List<string>() : null);
		ParseProperty(properties, "Role", ref role, list);
		if (false | ParseProperty(properties, "IPv4Addresses", ref ipV4Addresses, list) | ParseProperty(properties, "IPv4PrefixLengths", ref ipV4PrefixLengths, list) | ParseProperty(properties, "IPv6Addresses", ref ipV6Addresses, list) | ParseProperty(properties, "IPv6PrefixLengths", ref ipV6PrefixLengths, list))
		{
			list.TryAdd("Subnets");
		}
		return list;
	}

	private string ComputeSubnetsString()
	{
		IList<string> list = ipV4Addresses;
		IList<string> list2 = ipV4PrefixLengths;
		IList<string> list3 = ipV6Addresses;
		IList<string> list4 = ipV6PrefixLengths;
		if (list == null || list2 == null || list3 == null || list4 == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < list.Count; i++)
		{
			stringBuilder.Append(list[i] + "/" + list2[i] + " ");
		}
		for (int j = 0; j < ipV6Addresses.Count; j++)
		{
			stringBuilder.Append(list3[j] + "/" + list4[j] + " ");
		}
		return stringBuilder.ToString().Trim();
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.NetworkStateChanged:
		{
			ClusterNetworkStateEventArgs args = e.EventArgument as ClusterNetworkStateEventArgs;
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

	public static bool operator ==(Network network1, Network network2)
	{
		return network1?.Equals(network2) ?? ((object)network2 == null);
	}

	public static bool operator !=(Network network1, Network network2)
	{
		return !(network1 == network2);
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
		role = null;
		ipV4Addresses = null;
		ipV4PrefixLengths = null;
		ipV6Addresses = null;
		ipV6PrefixLengths = null;
		networkInterfacesWeak = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("State");
			OnPropertyChanged("Role");
			OnPropertyChanged("IPv4Addresses");
			OnPropertyChanged("IPv4PrefixLengths");
			OnPropertyChanged("IPv6Addresses");
			OnPropertyChanged("IPv6PrefixLengths");
			OnPropertyChanged("Subnets");
			OnPropertyChanged("NetworkInterfaces");
			this.StateChanged.SafeCall(this, null);
		}, OperationType.Async);
	}
}

