using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_ReplicaPeer : MSFT_StorageObject
{
	public MSFT_ReplicaPeerMSFT_PartitionToReplicaPeer MSFT_PartitionToReplicaPeer { get; private set; }

	public MSFT_ReplicaPeerMSFT_ReplicationGroupToReplicaPeer MSFT_ReplicationGroupToReplicaPeer { get; private set; }

	public MSFT_ReplicaPeerMSFT_StorageSubSystemToReplicaPeer MSFT_StorageSubSystemToReplicaPeer { get; private set; }

	public MSFT_ReplicaPeerMSFT_VirtualDiskToReplicaPeer MSFT_VirtualDiskToReplicaPeer { get; private set; }

	public bool? IsPrimary => (bool?)base.Instance.CimInstanceProperties["IsPrimary"].Value;

	public MSFT_StorageObject PeerObject
	{
		get
		{
			return new MSFT_StorageObject(base.Session, (CimInstance)base.Instance.CimInstanceProperties["PeerObject"].Value);
		}
		set
		{
			base.Instance.CimInstanceProperties["PeerObject"].Value = value?.Instance;
		}
	}

	public string PeerObjectId => (string)base.Instance.CimInstanceProperties["PeerObjectId"].Value;

	public string PeerObjectName => (string)base.Instance.CimInstanceProperties["PeerObjectName"].Value;

	public ushort? PeerObjectType => (ushort?)base.Instance.CimInstanceProperties["PeerObjectType"].Value;

	public string PeerProviderURI => (string)base.Instance.CimInstanceProperties["PeerProviderURI"].Value;

	public string PeerSubsystemName => (string)base.Instance.CimInstanceProperties["PeerSubsystemName"].Value;

	public string PeerUniqueId => (string)base.Instance.CimInstanceProperties["PeerUniqueId"].Value;

	public MSFT_ReplicaPeer()
	{
	}

	public MSFT_ReplicaPeer(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_PartitionToReplicaPeer = new MSFT_ReplicaPeerMSFT_PartitionToReplicaPeer(session, instance);
		MSFT_ReplicationGroupToReplicaPeer = new MSFT_ReplicaPeerMSFT_ReplicationGroupToReplicaPeer(session, instance);
		MSFT_StorageSubSystemToReplicaPeer = new MSFT_ReplicaPeerMSFT_StorageSubSystemToReplicaPeer(session, instance);
		MSFT_VirtualDiskToReplicaPeer = new MSFT_ReplicaPeerMSFT_VirtualDiskToReplicaPeer(session, instance);
	}

	public static MSFT_ReplicaPeer GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_ReplicaPeer", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_ReplicaPeer(session, instance);
	}

	public static MSFT_ReplicaPeer CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_ReplicaPeer", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_ReplicaPeer(session, cimInstance);
	}

	public new static IEnumerable<MSFT_ReplicaPeer> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_ReplicaPeer")
			select new MSFT_ReplicaPeer(session, i);
	}

	public new static IEnumerable<MSFT_ReplicaPeer> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_ReplicaPeer";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_ReplicaPeer(session, i);
	}

	public override void Refresh()
	{
		base.Instance = base.Session.GetInstance("root/microsoft/windows/storage", base.Instance);
	}

	public override void Delete()
	{
		base.Session.DeleteInstance("root/microsoft/windows/storage", base.Instance);
	}

	public override void Save()
	{
		base.Session.ModifyInstance("root/microsoft/windows/storage", base.Instance);
	}
}
