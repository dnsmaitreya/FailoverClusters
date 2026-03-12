using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageNode : MSFT_StorageObject
{
	public MSFT_StorageNodeMSFT_StorageNodeToDisk MSFT_StorageNodeToDisk { get; private set; }

	public MSFT_StorageNodeMSFT_StorageNodeToPhysicalDisk MSFT_StorageNodeToPhysicalDisk { get; private set; }

	public MSFT_StorageNodeMSFT_StorageNodeToStorageEnclosure MSFT_StorageNodeToStorageEnclosure { get; private set; }

	public MSFT_StorageNodeMSFT_StorageNodeToStoragePool MSFT_StorageNodeToStoragePool { get; private set; }

	public MSFT_StorageNodeMSFT_StorageNodeToVirtualDisk MSFT_StorageNodeToVirtualDisk { get; private set; }

	public MSFT_StorageNodeMSFT_StorageNodeToVolume MSFT_StorageNodeToVolume { get; private set; }

	public MSFT_StorageNodeMSFT_StorageSubSystemToStorageNode MSFT_StorageSubSystemToStorageNode { get; private set; }

	public string FirmwareVersion => (string)base.Instance.CimInstanceProperties["FirmwareVersion"].Value;

	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public ushort? NameFormat => (ushort?)base.Instance.CimInstanceProperties["NameFormat"].Value;

	public ushort? OperationalStatus => (ushort?)base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public string[] OtherIdentifyingInfo => (string[])base.Instance.CimInstanceProperties["OtherIdentifyingInfo"].Value;

	public string[] OtherIdentifyingInfoDescription => (string[])base.Instance.CimInstanceProperties["OtherIdentifyingInfoDescription"].Value;

	public MSFT_StorageNode()
	{
	}

	public MSFT_StorageNode(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageNodeToDisk = new MSFT_StorageNodeMSFT_StorageNodeToDisk(session, instance);
		MSFT_StorageNodeToPhysicalDisk = new MSFT_StorageNodeMSFT_StorageNodeToPhysicalDisk(session, instance);
		MSFT_StorageNodeToStorageEnclosure = new MSFT_StorageNodeMSFT_StorageNodeToStorageEnclosure(session, instance);
		MSFT_StorageNodeToStoragePool = new MSFT_StorageNodeMSFT_StorageNodeToStoragePool(session, instance);
		MSFT_StorageNodeToVirtualDisk = new MSFT_StorageNodeMSFT_StorageNodeToVirtualDisk(session, instance);
		MSFT_StorageNodeToVolume = new MSFT_StorageNodeMSFT_StorageNodeToVolume(session, instance);
		MSFT_StorageSubSystemToStorageNode = new MSFT_StorageNodeMSFT_StorageSubSystemToStorageNode(session, instance);
	}

	public static MSFT_StorageNode GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageNode", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageNode(session, instance);
	}

	public static MSFT_StorageNode CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageNode", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_StorageNode(session, cimInstance);
	}

	public new static IEnumerable<MSFT_StorageNode> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageNode")
			select new MSFT_StorageNode(session, i);
	}

	public new static IEnumerable<MSFT_StorageNode> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageNode";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageNode(session, i);
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
