using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace MS.Internal.ServerClusters;

internal class ObjectManager : IDisposable
{
	private Cluster cluster;

	private ObjectManagerHelper<ClusterGroup> groupSet;

	private ObjectManagerHelper<ClusterResource> resourceSet;

	private ObjectManagerHelper<ClusterResourceType> resourceTypeSet;

	private ObjectManagerHelper<ClusterNetwork> networkSet;

	private ObjectManagerHelper<ClusterNetworkInterface> networkConnectionSet;

	private ObjectManagerHelper<ClusterNode> nodeSet;

	private int disposed;

	private EventHandler<ObjectLifetimeEventArgs<ClusterGroup>> _003Cbacking_store_003EGroupLifetimeChanged;

	private EventHandler<ObjectLifetimeEventArgs<ClusterResource>> _003Cbacking_store_003EResourceLifetimeChanged;

	private EventHandler<ObjectLifetimeEventArgs<ClusterResourceType>> _003Cbacking_store_003EResourceTypeLifetimeChanged;

	private EventHandler<ObjectLifetimeEventArgs<ClusterNetwork>> _003Cbacking_store_003ENetworkLifetimeChanged;

	private EventHandler<ObjectLifetimeEventArgs<ClusterNetworkInterface>> _003Cbacking_store_003ENetworkInterfaceLifetimeChanged;

	private EventHandler<ObjectLifetimeEventArgs<ClusterNode>> _003Cbacking_store_003ENodeLifetimeChanged;

	[SpecialName]
	internal event EventHandler<ObjectLifetimeEventArgs<ClusterNode>> NodeLifetimeChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003ENodeLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterNode>>)Delegate.Combine(_003Cbacking_store_003ENodeLifetimeChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003ENodeLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterNode>>)Delegate.Remove(_003Cbacking_store_003ENodeLifetimeChanged, value);
		}
	}

	[SpecialName]
	internal event EventHandler<ObjectLifetimeEventArgs<ClusterNetworkInterface>> NetworkInterfaceLifetimeChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003ENetworkInterfaceLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterNetworkInterface>>)Delegate.Combine(_003Cbacking_store_003ENetworkInterfaceLifetimeChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003ENetworkInterfaceLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterNetworkInterface>>)Delegate.Remove(_003Cbacking_store_003ENetworkInterfaceLifetimeChanged, value);
		}
	}

	[SpecialName]
	internal event EventHandler<ObjectLifetimeEventArgs<ClusterNetwork>> NetworkLifetimeChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003ENetworkLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterNetwork>>)Delegate.Combine(_003Cbacking_store_003ENetworkLifetimeChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003ENetworkLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterNetwork>>)Delegate.Remove(_003Cbacking_store_003ENetworkLifetimeChanged, value);
		}
	}

	[SpecialName]
	internal event EventHandler<ObjectLifetimeEventArgs<ClusterResourceType>> ResourceTypeLifetimeChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EResourceTypeLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterResourceType>>)Delegate.Combine(_003Cbacking_store_003EResourceTypeLifetimeChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EResourceTypeLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterResourceType>>)Delegate.Remove(_003Cbacking_store_003EResourceTypeLifetimeChanged, value);
		}
	}

	[SpecialName]
	internal event EventHandler<ObjectLifetimeEventArgs<ClusterResource>> ResourceLifetimeChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EResourceLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterResource>>)Delegate.Combine(_003Cbacking_store_003EResourceLifetimeChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EResourceLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterResource>>)Delegate.Remove(_003Cbacking_store_003EResourceLifetimeChanged, value);
		}
	}

	[SpecialName]
	internal event EventHandler<ObjectLifetimeEventArgs<ClusterGroup>> GroupLifetimeChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EGroupLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterGroup>>)Delegate.Combine(_003Cbacking_store_003EGroupLifetimeChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EGroupLifetimeChanged = (EventHandler<ObjectLifetimeEventArgs<ClusterGroup>>)Delegate.Remove(_003Cbacking_store_003EGroupLifetimeChanged, value);
		}
	}

	private void OnGroupLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterGroup> e)
	{
		_003Cbacking_store_003EGroupLifetimeChanged?.Invoke(this, e);
	}

	private void OnResourceLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterResource> e)
	{
		_003Cbacking_store_003EResourceLifetimeChanged?.Invoke(this, e);
	}

	private void OnResourceTypeLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterResourceType> e)
	{
		_003Cbacking_store_003EResourceTypeLifetimeChanged?.Invoke(this, e);
	}

	private void OnNetworkLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterNetwork> e)
	{
		_003Cbacking_store_003ENetworkLifetimeChanged?.Invoke(this, e);
	}

	private void OnNetworkInterfaceLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterNetworkInterface> e)
	{
		_003Cbacking_store_003ENetworkInterfaceLifetimeChanged?.Invoke(this, e);
	}

	private void OnNodeLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterNode> e)
	{
		_003Cbacking_store_003ENodeLifetimeChanged?.Invoke(this, e);
	}

	internal ObjectManager(Cluster cluster)
	{
		this.cluster = cluster;
		groupSet = new ObjectManagerHelper<ClusterGroup>();
		resourceSet = new ObjectManagerHelper<ClusterResource>();
		resourceTypeSet = new ObjectManagerHelper<ClusterResourceType>();
		networkSet = new ObjectManagerHelper<ClusterNetwork>();
		networkConnectionSet = new ObjectManagerHelper<ClusterNetworkInterface>();
		nodeSet = new ObjectManagerHelper<ClusterNode>();
		groupSet.ObjectLifetimeChanged += OnGroupLifetimeChanged;
		resourceSet.ObjectLifetimeChanged += OnResourceLifetimeChanged;
		resourceTypeSet.ObjectLifetimeChanged += OnResourceTypeLifetimeChanged;
		networkSet.ObjectLifetimeChanged += OnNetworkLifetimeChanged;
		networkConnectionSet.ObjectLifetimeChanged += OnNetworkInterfaceLifetimeChanged;
		nodeSet.ObjectLifetimeChanged += OnNodeLifetimeChanged;
	}

	private void _007EObjectManager()
	{
		if (Interlocked.Increment(ref disposed) != 1)
		{
			return;
		}
		try
		{
			groupSet.ObjectLifetimeChanged -= OnGroupLifetimeChanged;
			ObjectManagerHelper<ClusterGroup> objectManagerHelper = groupSet;
			IDisposable disposable = objectManagerHelper;
			((IDisposable)objectManagerHelper)?.Dispose();
		}
		catch (Exception exception)
		{
			DebugLog.LogException(exception, "Error deleting the groupSet in the object manager");
		}
		finally
		{
			groupSet = null;
		}
		try
		{
			resourceSet.ObjectLifetimeChanged -= OnResourceLifetimeChanged;
			ObjectManagerHelper<ClusterResource> objectManagerHelper2 = resourceSet;
			IDisposable disposable2 = objectManagerHelper2;
			((IDisposable)objectManagerHelper2)?.Dispose();
		}
		catch (Exception exception2)
		{
			DebugLog.LogException(exception2, "Error deleting the resourceSet in the object manager");
		}
		finally
		{
			resourceSet = null;
		}
		try
		{
			resourceTypeSet.ObjectLifetimeChanged -= OnResourceTypeLifetimeChanged;
			ObjectManagerHelper<ClusterResourceType> objectManagerHelper3 = resourceTypeSet;
			IDisposable disposable3 = objectManagerHelper3;
			((IDisposable)objectManagerHelper3)?.Dispose();
		}
		catch (Exception exception3)
		{
			DebugLog.LogException(exception3, "Error deleting the resourceTypeSet in the object manager");
		}
		finally
		{
			resourceTypeSet = null;
		}
		try
		{
			networkSet.ObjectLifetimeChanged -= OnNetworkLifetimeChanged;
			ObjectManagerHelper<ClusterNetwork> objectManagerHelper4 = networkSet;
			IDisposable disposable4 = objectManagerHelper4;
			((IDisposable)objectManagerHelper4)?.Dispose();
		}
		catch (Exception exception4)
		{
			DebugLog.LogException(exception4, "Error deleting the networkSet in the object manager");
		}
		finally
		{
			networkSet = null;
		}
		try
		{
			networkConnectionSet.ObjectLifetimeChanged -= OnNetworkInterfaceLifetimeChanged;
			ObjectManagerHelper<ClusterNetworkInterface> objectManagerHelper5 = networkConnectionSet;
			IDisposable disposable5 = objectManagerHelper5;
			((IDisposable)objectManagerHelper5)?.Dispose();
		}
		catch (Exception exception5)
		{
			DebugLog.LogException(exception5, "Error deleting the networkConnectionSet in the object manager");
		}
		finally
		{
			networkConnectionSet = null;
		}
		try
		{
			nodeSet.ObjectLifetimeChanged -= OnNodeLifetimeChanged;
			ObjectManagerHelper<ClusterNode> objectManagerHelper6 = nodeSet;
			IDisposable disposable6 = objectManagerHelper6;
			((IDisposable)objectManagerHelper6)?.Dispose();
		}
		catch (Exception exception6)
		{
			DebugLog.LogException(exception6, "Error deleting the nodeSet in the object manager");
		}
		finally
		{
			nodeSet = null;
		}
	}

	internal void RefreshObjects()
	{
		if (disposed == 0)
		{
			groupSet.RefreshObjects();
			resourceSet.RefreshObjects();
			resourceTypeSet.RefreshObjects();
			networkSet.RefreshObjects();
			networkConnectionSet.RefreshObjects();
			nodeSet.RefreshObjects();
		}
	}

	internal void RegisterInstance(ClusterNode instance)
	{
		if (disposed == 0)
		{
			nodeSet.RegisterInstance(instance);
		}
	}

	internal void RegisterInstance(ClusterNetworkInterface instance)
	{
		if (disposed == 0)
		{
			networkConnectionSet.RegisterInstance(instance);
		}
	}

	internal void RegisterInstance(ClusterNetwork instance)
	{
		if (disposed == 0)
		{
			networkSet.RegisterInstance(instance);
		}
	}

	internal void RegisterInstance(ClusterResourceType instance)
	{
		if (disposed == 0)
		{
			resourceTypeSet.RegisterInstance(instance);
		}
	}

	internal void RegisterInstance(ClusterResource instance)
	{
		if (disposed == 0)
		{
			resourceSet.RegisterInstance(instance);
		}
	}

	internal void RegisterInstance(ClusterGroup instance)
	{
		if (disposed == 0)
		{
			groupSet.RegisterInstance(instance);
		}
	}

	internal void UnregisterInstance(ClusterNode instance)
	{
		if (disposed == 0)
		{
			nodeSet.UnregisterInstance(instance);
		}
	}

	internal void UnregisterInstance(ClusterNetworkInterface instance)
	{
		if (disposed == 0)
		{
			networkConnectionSet.UnregisterInstance(instance);
		}
	}

	internal void UnregisterInstance(ClusterNetwork instance)
	{
		if (disposed == 0)
		{
			networkSet.UnregisterInstance(instance);
		}
	}

	internal void UnregisterInstance(ClusterResourceType instance)
	{
		if (disposed == 0)
		{
			resourceTypeSet.UnregisterInstance(instance);
		}
	}

	internal void UnregisterInstance(ClusterResource instance)
	{
		if (disposed == 0)
		{
			resourceSet.UnregisterInstance(instance);
		}
	}

	internal void UnregisterInstance(ClusterGroup instance)
	{
		if (disposed == 0)
		{
			groupSet.UnregisterInstance(instance);
		}
	}

	internal void SyncGroupRename(ulong id, string name)
	{
		if (disposed == 0)
		{
			groupSet.SyncRename(id, name);
		}
	}

	internal void SyncResourceRename(ulong id, string name)
	{
		if (disposed == 0)
		{
			resourceSet.SyncRename(id, name);
		}
	}

	internal void SyncResourceTypeRename(ulong id, string name)
	{
		if (disposed == 0)
		{
			resourceTypeSet.SyncRename(id, name);
		}
	}

	internal void SyncNetworkRename(ulong id, string name)
	{
		if (disposed == 0)
		{
			networkSet.SyncRename(id, name);
		}
	}

	internal void SyncNetworkInterfaceRename(ulong id, string name)
	{
		if (disposed == 0)
		{
			networkConnectionSet.SyncRename(id, name);
		}
	}

	internal void SyncNodeRename(ulong id, string name)
	{
		if (disposed == 0)
		{
			nodeSet.SyncRename(id, name);
		}
	}

	internal ClusterGroup GetGroupInstance(ulong id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return groupSet.GetInstance(id);
	}

	internal ClusterGroup GetGroupInstance(string name)
	{
		if (disposed != 0)
		{
			return null;
		}
		return groupSet.GetInstance(name);
	}

	internal ClusterNetwork GetNetworkInstance(ulong id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return networkSet.GetInstance(id);
	}

	internal ClusterNetwork GetNetworkInstance(Guid id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return networkSet.GetInstance(id);
	}

	internal ClusterNetwork GetNetworkInstance(string name)
	{
		if (disposed != 0)
		{
			return null;
		}
		return networkSet.GetInstance(name);
	}

	internal ClusterNetworkInterface GetNetworkInterfaceInstance(ulong id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return networkConnectionSet.GetInstance(id);
	}

	internal ClusterNetworkInterface GetNetworkInterfaceInstance(Guid id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return networkConnectionSet.GetInstance(id);
	}

	internal ClusterNetworkInterface GetNetworkInterfaceInstance(string name)
	{
		if (disposed != 0)
		{
			return null;
		}
		return networkConnectionSet.GetInstance(name);
	}

	internal ClusterNode GetNodeInstance(ulong id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return nodeSet.GetInstance(id);
	}

	internal ClusterNode GetNodeInstance(Guid id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return nodeSet.GetInstance(id);
	}

	internal ClusterNode GetNodeInstance(string name)
	{
		if (disposed != 0)
		{
			return null;
		}
		return nodeSet.GetInstance(name);
	}

	internal ClusterResource GetResourceInstance(ulong id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return resourceSet.GetInstance(id);
	}

	internal ClusterResource GetResourceInstance(Guid id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return resourceSet.GetInstance(id);
	}

	internal ClusterResource GetResourceInstance(string name)
	{
		if (disposed != 0)
		{
			return null;
		}
		return resourceSet.GetInstance(name);
	}

	internal ClusterResourceType GetResourceTypeInstance(ulong id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return resourceTypeSet.GetInstance(id);
	}

	internal ClusterResourceType GetResourceTypeInstance(Guid id)
	{
		if (disposed != 0)
		{
			return null;
		}
		return resourceTypeSet.GetInstance(id);
	}

	internal Collection<ClusterNetwork> GetNetworks()
	{
		return networkSet.GetObjects();
	}

	[SpecialName]
	private protected void raise_GroupLifetimeChanged(object value0, ObjectLifetimeEventArgs<ClusterGroup> value1)
	{
		_003Cbacking_store_003EGroupLifetimeChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	private protected void raise_ResourceLifetimeChanged(object value0, ObjectLifetimeEventArgs<ClusterResource> value1)
	{
		_003Cbacking_store_003EResourceLifetimeChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	private protected void raise_ResourceTypeLifetimeChanged(object value0, ObjectLifetimeEventArgs<ClusterResourceType> value1)
	{
		_003Cbacking_store_003EResourceTypeLifetimeChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	private protected void raise_NetworkLifetimeChanged(object value0, ObjectLifetimeEventArgs<ClusterNetwork> value1)
	{
		_003Cbacking_store_003ENetworkLifetimeChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	private protected void raise_NetworkInterfaceLifetimeChanged(object value0, ObjectLifetimeEventArgs<ClusterNetworkInterface> value1)
	{
		_003Cbacking_store_003ENetworkInterfaceLifetimeChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	private protected void raise_NodeLifetimeChanged(object value0, ObjectLifetimeEventArgs<ClusterNode> value1)
	{
		_003Cbacking_store_003ENodeLifetimeChanged?.Invoke(value0, value1);
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EObjectManager();
		}
		else
		{
			base.Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(A_0: true);
		GC.SuppressFinalize(this);
	}
}
