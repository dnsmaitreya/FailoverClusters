using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;

namespace MS.Internal.ServerClusters;

public class ClientAccessPoint
{
	private string m_netName;

	private string m_dnsName;

	private Collection<IPAddressInfo> m_addresses;

	private Cluster m_cluster;

	private ClusterResource m_netNameResource;

	private ClusterGroup m_group;

	private StringCollection m_nodeNames;

	private ReadOnlyNetworkInfoCollection m_networkInfos;

	private bool m_bringOnline;

	private EventHandler<OperationProgressEventArgs> _003Cbacking_store_003ESaveChangesProgress;

	public ClusterResource NetworkNameResource => m_netNameResource;

	public ClusterGroup Group
	{
		get
		{
			return m_group;
		}
		set
		{
			if (null == value)
			{
				throw new ArgumentNullException("value");
			}
			if (m_netNameResource != null)
			{
				throw new InvalidOperationException();
			}
			m_group = value;
		}
	}

	public ICollection<NetworkInfo> UnconfiguredNetworks => GetUnconfiguredNetworks();

	public ICollection<IPAddressInfo> Addresses => m_addresses;

	public bool WillSaveOfflineNetName
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return CheckOfflineNetworkResource();
		}
	}

	public string ClusterNameOU => GetClusterOU();

	public string DnsName
	{
		get
		{
			return m_dnsName;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				m_dnsName = value;
			}
		}
	}

	public string NetworkName
	{
		get
		{
			return m_netName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "value");
			}
			m_netName = value;
		}
	}

	[SpecialName]
	public event EventHandler<OperationProgressEventArgs> SaveChangesProgress
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003ESaveChangesProgress = (EventHandler<OperationProgressEventArgs>)Delegate.Combine(_003Cbacking_store_003ESaveChangesProgress, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003ESaveChangesProgress = (EventHandler<OperationProgressEventArgs>)Delegate.Remove(_003Cbacking_store_003ESaveChangesProgress, value);
		}
	}

	private ReadOnlyNetworkInfoCollection GetUnconfiguredNetworks()
	{
		return GetUnconfiguredPublicNetworks(m_networkInfos);
	}

	private unsafe ReadOnlyNetworkInfoCollection GetNodeNetworks()
	{
		//IL_000b: Expected I, but got I8
		//IL_002e: Expected I4, but got I8
		//IL_0032: Expected I, but got I8
		//IL_0036: Expected I, but got I8
		//IL_006f: Expected I8, but got I
		//IL_0264: Expected I, but got I8
		//IL_027c: Expected I, but got I8
		ReadOnlyNetworkInfoCollection readOnlyNetworkInfoCollection = new ReadOnlyNetworkInfoCollection();
		void* ptr = null;
		ulong num = (ulong)(int)((ulong)m_nodeNames.Count * 8uL);
		ushort** ptr2 = (ushort**)InteropHelp.AllocateArray(num);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlock(ptr2, 0, num);
		HINSTANCE__* ptr3 = null;
		delegate* unmanaged[Cdecl, Cdecl]<void*, void> delegate_002A = null;
		XmlReader xmlReader = null;
		TextReader textReader = null;
		try
		{
			int num2 = 0;
			StringEnumerator enumerator = m_nodeNames.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					*(long*)((long)num2 * 8L + (nint)ptr2) = (nint)InteropHelp.StringToWstr(current);
					num2++;
				}
			}
			finally
			{
				StringEnumerator stringEnumerator = enumerator;
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			ptr3 = LoadClNetCfg();
			delegate* unmanaged[Cdecl, Cdecl]<ushort**, uint, void**, uint*, uint> procAddress = (delegate* unmanaged[Cdecl, Cdecl]<ushort**, uint, void**, uint*, uint>)global::_003CModule_003E.GetProcAddress(ptr3, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0CF_0040FIBMGOOK_0040ClNetCfgGetNodesClusterSetupTop_0040));
			if (procAddress == (delegate* unmanaged[Cdecl, Cdecl]<ushort**, uint, void**, uint*, uint>)null)
			{
				throw ExceptionHelp.Build<ApplicationException>(Marshal.GetLastWin32Error(), new string[1] { Resources.GetProc_GetNodesTopo_Failed_Text });
			}
			delegate_002A = (delegate* unmanaged[Cdecl, Cdecl]<void*, void>)global::_003CModule_003E.GetProcAddress(ptr3, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0CB_0040OJGBFELH_0040ClNetCfgFreeClusterSetupTopolog_0040));
			if (delegate_002A == (delegate* unmanaged[Cdecl, Cdecl]<void*, void>)null)
			{
				throw ExceptionHelp.Build<ApplicationException>(Marshal.GetLastWin32Error(), new string[1] { Resources.GetProc_FreeTopo_Failed_Text });
			}
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
			uint num4 = procAddress(ptr2, (uint)m_nodeNames.Count, &ptr, &num3);
			if (num4 != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num4, new string[1] { Resources.NetworkTopology_Failed_Text });
			}
			textReader = new StringReader(InteropHelp.WstrToString((ushort*)ptr));
			xmlReader = new XmlTextReader(textReader);
			ReadOnlyNetworkInfoCollection readOnlyNetworkInfoCollection2 = new ReadOnlyNetworkInfoCollection();
			while (xmlReader.ReadToFollowing("Network"))
			{
				IEnumerator<NetworkInfo> enumerator2 = NetworkInfo.GetXmlNetworkInfo(xmlReader).GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						NetworkInfo current2 = enumerator2.Current;
						readOnlyNetworkInfoCollection2.InternalAdd(current2);
					}
				}
				finally
				{
					IEnumerator<NetworkInfo> enumerator3 = enumerator2;
					IDisposable disposable2 = enumerator2;
					enumerator2?.Dispose();
				}
			}
			bool flag = true;
			IEnumerator<NetworkInfo> enumerator4 = readOnlyNetworkInfoCollection2.GetEnumerator();
			try
			{
				while (enumerator4.MoveNext())
				{
					if (enumerator4.Current.IsPublic)
					{
						flag = false;
						break;
					}
				}
			}
			finally
			{
				IEnumerator<NetworkInfo> enumerator5 = enumerator4;
				IDisposable disposable3 = enumerator4;
				enumerator4?.Dispose();
			}
			if (flag)
			{
				IEnumerator<NetworkInfo> enumerator6 = readOnlyNetworkInfoCollection2.GetEnumerator();
				try
				{
					while (enumerator6.MoveNext())
					{
						enumerator6.Current.ForcePublic();
					}
				}
				finally
				{
					IEnumerator<NetworkInfo> enumerator7 = enumerator6;
					IDisposable disposable4 = enumerator6;
					enumerator6?.Dispose();
				}
			}
			IEnumerator<NetworkInfo> enumerator8 = readOnlyNetworkInfoCollection2.GetEnumerator();
			try
			{
				while (enumerator8.MoveNext())
				{
					NetworkInfo current3 = enumerator8.Current;
					if (current3.IsPublic)
					{
						readOnlyNetworkInfoCollection.InternalAdd(current3);
					}
				}
				return readOnlyNetworkInfoCollection;
			}
			finally
			{
				IEnumerator<NetworkInfo> enumerator9 = enumerator8;
				IDisposable disposable5 = enumerator8;
				enumerator8?.Dispose();
			}
		}
		finally
		{
			int num5 = 0;
			if (0 < m_nodeNames.Count)
			{
				ushort** ptr4 = ptr2;
				do
				{
					ushort* ptr5 = (ushort*)(*(ulong*)ptr4);
					if (ptr5 != null)
					{
						InteropHelp.FreeWstr(ptr5);
					}
					num5++;
					ptr4 = (ushort**)((ulong)(nint)ptr4 + 8uL);
				}
				while (num5 < m_nodeNames.Count);
			}
			InteropHelp.FreeArray(ptr2);
			if (ptr != null)
			{
				delegate_002A(ptr);
			}
			if (ptr3 != null && global::_003CModule_003E.FreeLibrary(ptr3) == 0)
			{
				DebugLog.LogWarning("FreeLibrary returned {0}", global::_003CModule_003E.GetLastError());
			}
			((IDisposable)xmlReader)?.Dispose();
			((IDisposable)textReader)?.Dispose();
		}
	}

	private ReadOnlyNetworkInfoCollection GetClusterNetworks()
	{
		ICollection<NetworkInfo> clusterNetworkInfo = NetworkInfo.GetClusterNetworkInfo(m_cluster);
		ReadOnlyNetworkInfoCollection readOnlyNetworkInfoCollection = new ReadOnlyNetworkInfoCollection();
		foreach (NetworkInfo item in clusterNetworkInfo)
		{
			bool flag = false;
			IEnumerator<IPAddressInfo> enumerator2 = m_addresses.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					IPAddressInfo current2 = enumerator2.Current;
					if (0 == string.Compare(current2.NetworkInfo.AssociatedNetwork.Name, item.AssociatedNetwork.Name, StringComparison.OrdinalIgnoreCase) && current2.AddressType == item.AddressType)
					{
						flag = true;
						break;
					}
				}
			}
			finally
			{
				IEnumerator<IPAddressInfo> enumerator3 = enumerator2;
				IDisposable disposable = enumerator2;
				enumerator2?.Dispose();
			}
			if (!flag)
			{
				readOnlyNetworkInfoCollection.InternalAdd(item);
			}
		}
		return readOnlyNetworkInfoCollection;
	}

	private ReadOnlyNetworkInfoCollection GetUnconfiguredPublicNetworks(ReadOnlyNetworkInfoCollection networks)
	{
		ReadOnlyNetworkInfoCollection readOnlyNetworkInfoCollection = new ReadOnlyNetworkInfoCollection();
		foreach (NetworkInfo network in networks)
		{
			if (!network.IsPublic)
			{
				continue;
			}
			bool flag = false;
			IEnumerator<IPAddressInfo> enumerator2 = m_addresses.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					IPAddressInfo current2 = enumerator2.Current;
					bool flag2 = false;
					if (current2.AddressType != network.AddressType || network.PrefixLength != current2.PrefixLength)
					{
						continue;
					}
					if (current2.NetworkInfo != null && current2.NetworkInfo.AssociatedNetwork != null)
					{
						Guid id = network.AssociatedNetwork.Id;
						if (!(current2.NetworkInfo.AssociatedNetwork.Id == id))
						{
							continue;
						}
						bool flag3 = true;
					}
					else if ((current2.UseDhcp || current2.AddressType == AddressType.IPv6) && network.Address.Equals(current2.Address))
					{
						bool flag3 = true;
					}
					else
					{
						if (current2.AddressType != AddressType.IPv4 || !NetworkHelp.IsAddressInNetwork(current2.Address, network.Address, network.PrefixLength))
						{
							continue;
						}
						bool flag3 = true;
					}
					flag = true;
					break;
				}
			}
			finally
			{
				IEnumerator<IPAddressInfo> enumerator3 = enumerator2;
				IDisposable disposable = enumerator2;
				enumerator2?.Dispose();
			}
			if (!flag)
			{
				readOnlyNetworkInfoCollection.InternalAdd(network);
			}
		}
		return readOnlyNetworkInfoCollection;
	}

	private void Construct(Cluster cluster, ClusterResource netNameResource, StringCollection nodeNames)
	{
		m_cluster = cluster;
		m_netName = string.Empty;
		m_addresses = new Collection<IPAddressInfo>();
		m_netNameResource = netNameResource;
		m_group = null;
		m_nodeNames = nodeNames;
		m_bringOnline = false;
		LoadNetworkInfos();
		if (m_netNameResource != null)
		{
			LoadCap();
			m_group = m_netNameResource.GetOwnerGroup();
		}
	}

	private void UpdateGroupName()
	{
		//Discarded unreachable code: IL_0053, IL_0055
		ClusterResource clusterResource = m_group.TryFindGroupNetworkName();
		if (clusterResource == null)
		{
			return;
		}
		try
		{
			if (0 == string.Compare(m_netNameResource.Name, clusterResource.Name, StringComparison.OrdinalIgnoreCase))
			{
				m_group.Rename(m_netName);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.ClientAccessPoint_UpdateGroupNameFailed_Text });
		}
		finally
		{
			((IDisposable)clusterResource)?.Dispose();
		}
	}

	private void SaveNetNameChanges(OfflineManager offlineManager)
	{
		//Discarded unreachable code: IL_00ff
		OnSaveChangesProgress(20, Resources.ClientAccessPoint_NetName_Text);
		try
		{
			PropertyCollection privateProperties = m_netNameResource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
			bool flag = false;
			bool flag2 = (byte)(m_netNameResource.Flags & ResourceFlags.Core) != 0;
			string strA = (string)privateProperties["DnsName"].Value;
			if (0 != string.Compare(strA, m_netName, StringComparison.OrdinalIgnoreCase))
			{
				if (flag2)
				{
					m_netNameResource.Cluster.Rename(m_netName);
				}
				else
				{
					if (string.IsNullOrEmpty(m_dnsName))
					{
						privateProperties["DnsName"].Value = m_netName;
					}
					else
					{
						privateProperties["DnsName"].Value = m_dnsName;
					}
					flag = true;
					offlineManager.TakeOffline();
					privateProperties.SaveChanges();
				}
			}
			if (0 != string.Compare(m_netNameResource.Name, m_netName, StringComparison.OrdinalIgnoreCase) && !flag2)
			{
				m_netNameResource.Rename(m_netName);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.ClientAccessPoint_UpdateNetworkNameFailed_Text });
		}
	}

	private void VerifyState()
	{
		OnSaveChangesProgress(0, Resources.ClientAccessPoint_VerifyState_Text);
		if (GetAddressesNeedingDependents().Count > 0)
		{
			throw new InvalidOperationException(Resources.ClientAccessPoint_MissingMatchingNonTunneledAddress_Text);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool CheckOfflineNetworkResource()
	{
		Collection<IPAddressInfo> collection = null;
		if (m_netNameResource.State == ResourceState.Online)
		{
			collection = new Collection<IPAddressInfo>();
			BuildOnlineIPAddr(m_netNameResource.GetDependencyRelationship(), collection);
			foreach (IPAddressInfo item in collection)
			{
				IEnumerator<IPAddressInfo> enumerator2 = ((IEnumerable<IPAddressInfo>)m_addresses).GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						IPAddressInfo current2 = enumerator2.Current;
						if (item.DoesMatch(current2))
						{
							return false;
						}
					}
				}
				finally
				{
					IEnumerator<IPAddressInfo> enumerator3 = enumerator2;
					IDisposable disposable = enumerator2;
					enumerator2?.Dispose();
				}
			}
		}
		if (collection != null && collection.Count > 0)
		{
			return true;
		}
		return false;
	}

	private void BuildOnlineIPAddr(ClusterResourceRelationship relationship, Collection<IPAddressInfo> onLineAddresses)
	{
		foreach (RelatedClusterResource childResource in relationship.ChildResources)
		{
			if (childResource.RelatedResource.IsNetwork && childResource.RelatedResource.State == ResourceState.Online)
			{
				IPAddressInfo item = new IPAddressInfo(m_networkInfos, childResource.RelatedResource);
				onLineAddresses.Add(item);
			}
		}
		foreach (ClusterResourceRelationship childRelationship in relationship.ChildRelationships)
		{
			BuildOnlineIPAddr(childRelationship, onLineAddresses);
		}
	}

	private void ConfigureResourceDependencies(Collection<IPAddressConfigureData> desiredConfiguration)
	{
		OnSaveChangesProgress(80, Resources.ClientAccessPoint_IPAddressDependencies_Text);
		foreach (IPAddressConfigureData item in desiredConfiguration)
		{
			if (item.IPAddressInfo.AddressType != AddressType.IPv6 || !item.IPAddressInfo.IsTunneled)
			{
				continue;
			}
			IPAddressConfigureData iPAddressConfigureData = null;
			IEnumerator<IPAddressConfigureData> enumerator2 = desiredConfiguration.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					IPAddressConfigureData current2 = enumerator2.Current;
					if (current2.IPAddressInfo.AddressType == AddressType.IPv4 && 0 == string.Compare(item.IPAddressInfo.NetworkInfo.AssociatedNetwork.Name, current2.IPAddressInfo.NetworkInfo.AssociatedNetwork.Name, StringComparison.OrdinalIgnoreCase))
					{
						iPAddressConfigureData = current2;
						break;
					}
				}
			}
			finally
			{
				IEnumerator<IPAddressConfigureData> enumerator3 = enumerator2;
				IDisposable disposable = enumerator2;
				enumerator2?.Dispose();
			}
			ClusterResourceCollection dependencies = item.Resource.GetDependencies();
			bool flag = false;
			if (dependencies.Count > 0)
			{
				item.Resource.TakeOffline();
			}
			IEnumerator<ClusterResource> enumerator4 = dependencies.GetEnumerator();
			try
			{
				while (enumerator4.MoveNext())
				{
					ClusterResource current3 = enumerator4.Current;
					Guid id = iPAddressConfigureData.Resource.Id;
					if (current3.Id == id)
					{
						flag = true;
					}
					else
					{
						item.Resource.RemoveDependency(current3);
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator5 = enumerator4;
				IDisposable disposable2 = enumerator4;
				enumerator4?.Dispose();
			}
			if (!flag)
			{
				item.Resource.AddDependency(iPAddressConfigureData.Resource);
			}
			if (iPAddressConfigureData.Resource.State == ResourceState.Online)
			{
				item.Resource.BringOnline();
			}
		}
	}

	private PropertySaveStatus ConfigureNetNameDependencies(Collection<IPAddressConfigureData> desiredConfiguration, OfflineManager offlineManager)
	{
		PropertySaveStatus result = PropertySaveStatus.ResourceRequiresRecycle;
		OnSaveChangesProgress(90, Resources.ClientAccessPoint_Dependencies_Text);
		ClusterResourceRelationship dependencyRelationship = m_netNameResource.GetDependencyRelationship();
		RemoveIpAddressDependencies(null, dependencyRelationship);
		ClusterResourceRelationship clusterResourceRelationship = dependencyRelationship;
		if ((dependencyRelationship.ChildRelationships.Count != 0 || dependencyRelationship.ChildRelationships.Count != 0) && dependencyRelationship.RelationshipType == ResourceRelationshipType.And)
		{
			clusterResourceRelationship = new ClusterResourceRelationship();
			dependencyRelationship.ChildRelationships.Add(clusterResourceRelationship);
		}
		clusterResourceRelationship.RelationshipType = ResourceRelationshipType.Or;
		foreach (IPAddressConfigureData item in desiredConfiguration)
		{
			clusterResourceRelationship.ChildResources.Add(new RelatedClusterResource(item.Resource, item.Resource.Name));
		}
		ClusterResourceRelationship dependencyRelationship2 = m_netNameResource.GetDependencyRelationship();
		if (!ClusterResourceRelationship.AreEqual(dependencyRelationship2, dependencyRelationship))
		{
			if (offlineManager != null)
			{
				result = PropertySaveStatus.Ok;
				offlineManager.TakeOffline();
			}
			m_netNameResource.SetDependencyRelationship(dependencyRelationship);
		}
		return result;
	}

	private void RemoveIpAddressDependencies(ClusterResourceRelationship parentRelationship, ClusterResourceRelationship relationship)
	{
		Collection<RelatedClusterResource> collection = new Collection<RelatedClusterResource>();
		foreach (RelatedClusterResource childResource in relationship.ChildResources)
		{
			if (childResource.RelatedResource.IsNetwork)
			{
				collection.Add(childResource);
			}
		}
		foreach (RelatedClusterResource item in collection)
		{
			relationship.ChildResources.Remove(item);
		}
		Collection<ClusterResourceRelationship> collection2 = new Collection<ClusterResourceRelationship>();
		foreach (ClusterResourceRelationship childRelationship in relationship.ChildRelationships)
		{
			collection2.Add(childRelationship);
		}
		foreach (ClusterResourceRelationship item2 in collection2)
		{
			RemoveIpAddressDependencies(relationship, item2);
		}
		if (parentRelationship == null || ((relationship.ChildResources.Count != 1 || relationship.ChildRelationships.Count != 0) && (relationship.ChildResources.Count != 0 || relationship.ChildRelationships.Count != 1) && (relationship.ChildResources.Count != 0 || relationship.ChildRelationships.Count != 0)))
		{
			return;
		}
		foreach (RelatedClusterResource childResource2 in relationship.ChildResources)
		{
			parentRelationship.ChildResources.Add(childResource2);
		}
		foreach (ClusterResourceRelationship childRelationship2 in relationship.ChildRelationships)
		{
			parentRelationship.ChildRelationships.Add(childRelationship2);
		}
		parentRelationship.ChildRelationships.Remove(relationship);
	}

	private void CreateNewIPAddresses(Collection<IPAddressConfigureData> configureDatas)
	{
		OnSaveChangesProgress(50, Resources.ClientAccessPoint_NewAddresses_Text);
		foreach (IPAddressConfigureData configureData in configureDatas)
		{
			if (configureData.Resource == null)
			{
				ClusterResource resource = CreateIPAddressResource(configureData.IPAddressInfo);
				configureData.Resource = resource;
			}
		}
	}

	private ClusterResource CreateIPAddressResource(IPAddressInfo ipAddressInfo)
	{
		ClusterResource result = null;
		if (ipAddressInfo.AddressType == AddressType.IPv4)
		{
			result = CreateIPv4AddressResource(ipAddressInfo);
		}
		else if (ipAddressInfo.AddressType == AddressType.IPv6 && !ipAddressInfo.IsTunneled)
		{
			result = CreateIPv6AddressResource(ipAddressInfo);
		}
		else if (ipAddressInfo.AddressType == AddressType.IPv6)
		{
			result = CreateIPv6TunnelAddressResource(ipAddressInfo);
		}
		return result;
	}

	private ClusterResource CreateIPv4AddressResource(IPAddressInfo ipAddressInfo)
	{
		string resourceName = ResourceHelp.GenerateIPAddressResourceName(m_cluster, m_netName, ipAddressInfo.Address.ToString());
		string resourceTypeName = "IP Address";
		ClusterResource clusterResource = m_group.CreateResource(resourceName, resourceTypeName);
		ConfigureIPv4AddressResource(clusterResource, ipAddressInfo);
		return clusterResource;
	}

	private PropertySaveStatus ConfigureIPv4AddressResource(ClusterResource ipAddressResource, IPAddressInfo ipAddressInfo)
	{
		PropertyCollection privateProperties = ipAddressResource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		string name = ipAddressInfo.NetworkInfo.AssociatedNetwork.Name;
		string name2 = "Network";
		privateProperties.GetProperty(name2).Value = name;
		string text = ipAddressInfo.Address.ToString();
		string name3 = "Address";
		privateProperties.GetProperty(name3).Value = text;
		string value = NetworkHelp.PrefixLengthToSubnetMask(ipAddressInfo.PrefixLength);
		string name4 = "SubnetMask";
		privateProperties.GetProperty(name4).Value = value;
		uint num = (ipAddressInfo.UseDhcp ? 1u : 0u);
		string name5 = "EnableDhcp";
		privateProperties.GetProperty(name5).Value = num;
		if (!ipAddressInfo.UseDhcp && text == ipAddressInfo.NetworkInfo.Address.ToString())
		{
			string name6 = "EnableDhcp";
			privateProperties.GetProperty(name6).Value = 1u;
		}
		return privateProperties.SaveChanges();
	}

	private ClusterResource CreateIPv6AddressResource(IPAddressInfo ipAddressInfo)
	{
		string resourceName = ResourceHelp.GenerateIPAddressResourceName(m_cluster, m_netName, ipAddressInfo.Address.ToString());
		string resourceTypeName = "IPv6 Address";
		ClusterResource clusterResource = m_group.CreateResource(resourceName, resourceTypeName);
		ConfigureIPv6AddressResource(clusterResource, ipAddressInfo);
		return clusterResource;
	}

	private PropertySaveStatus ConfigureIPv6AddressResource(ClusterResource ipAddressResource, IPAddressInfo ipAddressInfo)
	{
		PropertyCollection privateProperties = ipAddressResource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		string name = ipAddressInfo.NetworkInfo.AssociatedNetwork.Name;
		string name2 = "Network";
		privateProperties.GetProperty(name2).Value = name;
		string value = ipAddressInfo.Address.ToString();
		string name3 = "Address";
		privateProperties.GetProperty(name3).Value = value;
		string name4 = "PrefixLength";
		privateProperties.GetProperty(name4).Value = ipAddressInfo.PrefixLength;
		return privateProperties.SaveChanges();
	}

	private ClusterResource CreateIPv6TunnelAddressResource(IPAddressInfo ipAddressInfo)
	{
		string resourceName = ResourceHelp.GenerateIPAddressResourceName(m_cluster, m_netName, ipAddressInfo.Address.ToString());
		string resourceTypeName = "IPv6 Tunnel Address";
		return m_group.CreateResource(resourceName, resourceTypeName);
	}

	private Collection<IPAddressConfigureData> GetDesiredConfiguarion()
	{
		Collection<IPAddressConfigureData> collection = new Collection<IPAddressConfigureData>();
		foreach (IPAddressInfo address in m_addresses)
		{
			IPAddressConfigureData item = new IPAddressConfigureData(address);
			collection.Add(item);
		}
		return collection;
	}

	private Collection<IPAddressResourceData> GetExistingIPAddressResources()
	{
		ClusterResourceCollection dependencies = m_netNameResource.GetDependencies();
		Collection<IPAddressResourceData> collection = new Collection<IPAddressResourceData>();
		foreach (ClusterResource item2 in dependencies)
		{
			if (item2.IsNetwork)
			{
				IPAddressInfo iPAddressInfo = new IPAddressInfo(m_networkInfos, item2);
				if (iPAddressInfo.Address != null)
				{
					IPAddressResourceData item = new IPAddressResourceData(item2, iPAddressInfo);
					collection.Add(item);
				}
			}
		}
		return collection;
	}

	private PropertySaveStatus ConfigureExistingResources(Collection<IPAddressResourceData> existingResources, Collection<IPAddressConfigureData> configureDatas)
	{
		PropertySaveStatus result = PropertySaveStatus.Ok;
		if (existingResources.Count > 0)
		{
			OnSaveChangesProgress(40, Resources.ClientAccessPoint_ExistingIPAddresses_Text);
		}
		foreach (IPAddressResourceData existingResource in existingResources)
		{
			IEnumerator<IPAddressConfigureData> enumerator2 = configureDatas.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					IPAddressConfigureData current2 = enumerator2.Current;
					if (existingResource.IPAddressInfo.DoesMatch(current2.IPAddressInfo))
					{
						current2.Resource = existingResource.Resource;
						existingResource.IsUsed = true;
					}
				}
			}
			finally
			{
				IEnumerator<IPAddressConfigureData> enumerator3 = enumerator2;
				IDisposable disposable = enumerator2;
				enumerator2?.Dispose();
			}
		}
		foreach (IPAddressConfigureData configureData in configureDatas)
		{
			if (configureData.Resource != null)
			{
				continue;
			}
			IEnumerator<IPAddressResourceData> enumerator5 = existingResources.GetEnumerator();
			try
			{
				while (enumerator5.MoveNext())
				{
					IPAddressResourceData current4 = enumerator5.Current;
					if (!current4.IsUsed && current4.IPAddressInfo.DoesNetworkMatch(configureData.IPAddressInfo) && HijackExistingResource(current4, configureData) == PropertySaveStatus.ResourceRequiresRecycle)
					{
						result = PropertySaveStatus.ResourceRequiresRecycle;
						current4.Resource.TakeOffline();
					}
				}
			}
			finally
			{
				IEnumerator<IPAddressResourceData> enumerator6 = enumerator5;
				IDisposable disposable2 = enumerator5;
				enumerator5?.Dispose();
			}
		}
		return result;
	}

	private PropertySaveStatus HijackExistingResource(IPAddressResourceData existingResource, IPAddressConfigureData desiredConfiguration)
	{
		PropertySaveStatus result = PropertySaveStatus.Ok;
		string resourceType = "IP Address";
		if (existingResource.Resource.IsResourceOfType(resourceType))
		{
			result = ConfigureIPv4AddressResource(existingResource.Resource, desiredConfiguration.IPAddressInfo);
		}
		else
		{
			string resourceType2 = "IPv6 Address";
			if (existingResource.Resource.IsResourceOfType(resourceType2))
			{
				result = ConfigureIPv6AddressResource(existingResource.Resource, desiredConfiguration.IPAddressInfo);
			}
			else
			{
				string resourceType3 = "IPv6 Tunnel Address";
				if (!existingResource.Resource.IsResourceOfType(resourceType3))
				{
					goto IL_007b;
				}
			}
		}
		desiredConfiguration.Resource = existingResource.Resource;
		existingResource.IsUsed = true;
		goto IL_007b;
		IL_007b:
		return result;
	}

	private void DeleteExistingResources(Collection<IPAddressResourceData> existingResources)
	{
		if (existingResources.Count > 0)
		{
			OnSaveChangesProgress(40, Resources.ClientAccessPoint_DeleteOldIPAddresses_Text);
		}
		bool flag;
		do
		{
			flag = false;
			int num = existingResources.Count - 1;
			if (num < 0)
			{
				break;
			}
			do
			{
				if (!existingResources[num].Resource.HasDependents())
				{
					existingResources[num].Resource.Delete();
					existingResources.RemoveAt(num);
					flag = true;
				}
				num += -1;
			}
			while (num >= 0);
		}
		while (flag);
	}

	private void LoadNetworkInfos()
	{
		ReadOnlyNetworkInfoCollection networkInfos = ((m_cluster != null) ? GetClusterNetworks() : GetNodeNetworks());
		m_networkInfos = networkInfos;
	}

	private void LoadCap()
	{
		LoadNetName(m_netNameResource);
		ClusterResourceCollection clusterResourceCollection = m_netNameResource.GetDependencies();
		try
		{
			IEnumerator<ClusterResource> enumerator = clusterResourceCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResource current = enumerator.Current;
					if (current.IsNetwork)
					{
						AddIPAddressInfo(current);
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			IDisposable disposable2 = clusterResourceCollection as IDisposable;
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
		}
	}

	private void LoadNetName(ClusterResource netNameResource)
	{
		PropertyCollection privateProperties = netNameResource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		string name = "DnsName";
		string netName = (string)privateProperties.GetProperty(name).Value;
		m_netName = netName;
	}

	private void AddIPAddressInfo(ClusterResource ipAddressResource)
	{
		IPAddressInfo iPAddressInfo = new IPAddressInfo(m_networkInfos, ipAddressResource);
		if (iPAddressInfo.Address != null)
		{
			m_addresses.Add(iPAddressInfo);
		}
	}

	private void AddIPAddressInfo(IPAddressInfo ipAddressInfo)
	{
		m_addresses.Add(ipAddressInfo);
	}

	private unsafe HINSTANCE__* LoadClNetCfg()
	{
		HINSTANCE__* ptr = global::_003CModule_003E.LoadLibraryW((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BK_0040PGMIBBJE_0040_003F_0024AAc_003F_0024AAl_003F_0024AAn_003F_0024AAe_003F_0024AAt_003F_0024AAc_003F_0024AAf_003F_0024AAg_003F_0024AA_003F4_003F_0024AAd_003F_0024AAl_003F_0024AAl_0040));
		if (0L == (nint)ptr)
		{
			ushort* ptr2 = InteropHelp.StringToWstr(Environment.ExpandEnvironmentVariables("%SystemRoot%\\cluster\\clnetcfg.dll"));
			try
			{
				ptr = global::_003CModule_003E.LoadLibraryW(ptr2);
				if (0L == (nint)ptr)
				{
					throw ExceptionHelp.Build<ApplicationException>(Marshal.GetLastWin32Error(), new string[1] { Resources.LoadClNetCfg_Failed_Text });
				}
			}
			finally
			{
				InteropHelp.FreeWstr(ptr2);
			}
		}
		return ptr;
	}

	private void OnSaveChangesProgress(int percentage, string message)
	{
		OperationProgressEventArgs e = new OperationProgressEventArgs(OperationProgressWarningLevel.Information, message, percentage);
		_003Cbacking_store_003ESaveChangesProgress?.Invoke(this, e);
	}

	public PropertySaveStatus SaveAddressChanges()
	{
		return SaveAddressChanges(null);
	}

	private PropertySaveStatus SaveAddressChanges(OfflineManager offlineManager)
	{
		//Discarded unreachable code: IL_005a
		PropertySaveStatus propertySaveStatus = PropertySaveStatus.Ok;
		try
		{
			Collection<IPAddressConfigureData> desiredConfiguarion = GetDesiredConfiguarion();
			Collection<IPAddressResourceData> existingIPAddressResources = GetExistingIPAddressResources();
			propertySaveStatus = ConfigureExistingResources(existingIPAddressResources, desiredConfiguarion);
			CreateNewIPAddresses(desiredConfiguarion);
			ConfigureResourceDependencies(desiredConfiguarion);
			PropertySaveStatus propertySaveStatus2 = ConfigureNetNameDependencies(desiredConfiguarion, offlineManager);
			propertySaveStatus = ((propertySaveStatus == PropertySaveStatus.ResourceRequiresRecycle) ? propertySaveStatus2 : propertySaveStatus);
			DeleteExistingResources(existingIPAddressResources);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.ClientAccessPoint_UpdateIPAddressesFailed_Text });
		}
		OnSaveChangesProgress(100, Resources.ClientAccessPoint_SaveCompleted_Text);
		return propertySaveStatus;
	}

	private string GetClusterOU()
	{
		Cluster cluster = m_cluster;
		if (cluster != null)
		{
			return cluster.GetClusterNameOU();
		}
		return string.Empty;
	}

	public ClientAccessPoint(ClusterResource netNameResource)
	{
		Construct(netNameResource.Cluster, netNameResource, null);
	}

	public ClientAccessPoint(Cluster cluster)
	{
		Construct(cluster, null, null);
	}

	public ClientAccessPoint(StringCollection nodeNames)
	{
		Construct(null, null, nodeNames);
	}

	public void ConfigureUnconfiguredNetworks(NetworkConfigurationOptions options)
	{
		foreach (NetworkInfo item in (IEnumerable<NetworkInfo>)GetUnconfiguredPublicNetworks(m_networkInfos))
		{
			IPAddressInfo iPAddressInfo = item.CreateAutomaticIPAddress(options);
			if (iPAddressInfo != null)
			{
				AddIPAddressInfo(iPAddressInfo);
			}
		}
	}

	public ICollection<IPAddressInfo> GetAddressesNeedingDependents()
	{
		Collection<IPAddressInfo> collection = new Collection<IPAddressInfo>();
		foreach (IPAddressInfo address in m_addresses)
		{
			if (address.AddressType != AddressType.IPv6 || !address.IsTunneled)
			{
				continue;
			}
			bool flag = false;
			IEnumerator<IPAddressInfo> enumerator2 = m_addresses.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					IPAddressInfo current2 = enumerator2.Current;
					if (current2.AddressType == AddressType.IPv4 && current2.NetworkInfo.AssociatedNetwork == address.NetworkInfo.AssociatedNetwork)
					{
						flag = true;
						break;
					}
				}
			}
			finally
			{
				IEnumerator<IPAddressInfo> enumerator3 = enumerator2;
				IDisposable disposable = enumerator2;
				enumerator2?.Dispose();
			}
			if (!flag)
			{
				collection.Add(address);
			}
		}
		return collection;
	}

	public PropertySaveStatus SaveChanges()
	{
		//Discarded unreachable code: IL_00ab
		if (m_netName.Length == 0)
		{
			throw new InvalidOperationException(Resources.ClientAccessPoint_NetNameRequired_Text);
		}
		if (m_group == null)
		{
			throw new InvalidOperationException(Resources.ClientAccessPoint_GroupRequired_Text);
		}
		PropertySaveStatus propertySaveStatus = PropertySaveStatus.Ok;
		try
		{
			VerifyState();
			if (m_netNameResource == null)
			{
				string resourceName = ResourceHelp.GenerateNetNameResourceName(m_cluster, m_netName);
				string resourceTypeName = "Network Name";
				m_netNameResource = m_group.CreateResource(resourceName, resourceTypeName);
			}
			OfflineManager offlineManager = OfflineManager.Create(m_netNameResource);
			UpdateGroupName();
			SaveNetNameChanges(offlineManager);
			propertySaveStatus = SaveAddressChanges(offlineManager);
			if (propertySaveStatus != PropertySaveStatus.ResourceRequiresRecycle)
			{
				offlineManager.BeginBringOnline();
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.ClientAccessPoint_SaveFailed_Text });
		}
		return propertySaveStatus;
	}

	[SpecialName]
	protected void raise_SaveChangesProgress(object value0, OperationProgressEventArgs value1)
	{
		_003Cbacking_store_003ESaveChangesProgress?.Invoke(value0, value1);
	}
}
