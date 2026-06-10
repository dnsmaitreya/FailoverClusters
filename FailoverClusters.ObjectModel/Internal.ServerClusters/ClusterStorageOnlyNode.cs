using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class ClusterStorageOnlyNode : ClusterItem
{
	private Cluster m_cluster;

	private volatile string m_name;

	private string m_enclosureId;

	private string m_deviceId;

	private StorageEnclosureType m_type;

	private string m_connectionString;

	private string m_description;

	private string m_location;

	private string m_manufacturerId;

	private string m_serialNumber;

	private string m_productId;

	private string m_faultDomainParent;

	private string m_enclosures;

	public override Guid Id => Guid.Empty;

	public string Enclosures => m_enclosures;

	public string FaultDomainParent => m_faultDomainParent;

	public string ProductId => m_productId;

	public string SerialNumber => m_serialNumber;

	public string ManufacturerId => m_manufacturerId;

	public string Location => m_location;

	public string Description => m_description;

	public string ConnectionString => m_connectionString;

	public StorageEnclosureType Type => m_type;

	public string DeviceId => m_deviceId;

	public string EnclosureId => m_enclosureId;

	public override string Name => m_name;

	public override bool IsDeleted
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return false;
		}
	}

	public Cluster Cluster => m_cluster;

	private string GetName()
	{
		return m_name;
	}

	public unsafe ClusterStorageOnlyNode(Cluster cluster, _ClusStorageEnclosureInfo* enclosureInfo)
	{
		//IL_0015: Expected I, but got I8
		//IL_0027: Expected I, but got I8
		//IL_0038: Expected I, but got I8
		//IL_0055: Expected I, but got I8
		//IL_0066: Expected I, but got I8
		//IL_0077: Expected I, but got I8
		//IL_0088: Expected I, but got I8
		//IL_0099: Expected I, but got I8
		//IL_00aa: Expected I, but got I8
		//IL_00bb: Expected I, but got I8
		//IL_00cc: Expected I, but got I8
		try
		{
			m_cluster = cluster;
			m_name = InteropHelp.WstrToString((ushort*)(*(ulong*)enclosureInfo));
			m_enclosureId = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)enclosureInfo + 8uL)));
			m_deviceId = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)enclosureInfo + 16uL)));
			m_type = *(StorageEnclosureType*)((ulong)(nint)enclosureInfo + 24uL);
			m_connectionString = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)enclosureInfo + 32uL)));
			m_description = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)enclosureInfo + 40uL)));
			m_location = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)enclosureInfo + 48uL)));
			m_manufacturerId = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)enclosureInfo + 56uL)));
			m_serialNumber = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)enclosureInfo + 64uL)));
			m_productId = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)enclosureInfo + 72uL)));
			m_faultDomainParent = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)enclosureInfo + 80uL)));
			m_enclosures = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)enclosureInfo + 96uL)));
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(A_0: true);
			throw;
		}
	}

	public override ControlExecutor GetControlExecutor()
	{
		Cluster cluster = m_cluster;
		return new StorageOnlyNodeControlExecutor(this, cluster);
	}

	public override PropertyCollection GetCommonProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_0013, IL_0018, IL_001a
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return new PropertyCollection(this, ClusterPropertyScope.Common, propSet);
		}
		catch (Win32Exception)
		{
			throw;
		}
		catch (Exception)
		{
			throw;
		}
	}

	public override PropertyCollection GetPrivateProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_0013, IL_0018, IL_001a
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return new PropertyCollection(this, ClusterPropertyScope.Private, propSet);
		}
		catch (Win32Exception)
		{
			throw;
		}
		catch (Exception)
		{
			throw;
		}
	}

	public override void Close()
	{
	}

	public override void Refresh()
	{
	}

	public int Rename(string newName)
	{
		return 0;
	}
}
