using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_TargetPort : MSFT_StorageObject
{
	public MSFT_TargetPortMSFT_MaskingSetToTargetPort MSFT_MaskingSetToTargetPort { get; private set; }

	public MSFT_TargetPortMSFT_StorageSubSystemToTargetPort MSFT_StorageSubSystemToTargetPort { get; private set; }

	public MSFT_TargetPortMSFT_TargetPortToTargetPortal MSFT_TargetPortToTargetPortal { get; private set; }

	public MSFT_TargetPortMSFT_TargetPortToVirtualDisk MSFT_TargetPortToVirtualDisk { get; private set; }

	public ushort? ConnectionType => (ushort?)base.Instance.CimInstanceProperties["ConnectionType"].Value;

	public string FriendlyName => (string)base.Instance.CimInstanceProperties["FriendlyName"].Value;

	public ushort? HealthStatus => (ushort?)base.Instance.CimInstanceProperties["HealthStatus"].Value;

	public ushort? LinkTechnology => (ushort?)base.Instance.CimInstanceProperties["LinkTechnology"].Value;

	public ulong? MaxSpeed => (ulong?)base.Instance.CimInstanceProperties["MaxSpeed"].Value;

	public string[] NetworkAddresses => (string[])base.Instance.CimInstanceProperties["NetworkAddresses"].Value;

	public string NodeAddress => (string)base.Instance.CimInstanceProperties["NodeAddress"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public string OtherConnectionTypeDescription => (string)base.Instance.CimInstanceProperties["OtherConnectionTypeDescription"].Value;

	public string OtherLinkTechnology => (string)base.Instance.CimInstanceProperties["OtherLinkTechnology"].Value;

	public string OtherOperationalStatusDescription => (string)base.Instance.CimInstanceProperties["OtherOperationalStatusDescription"].Value;

	public string PortAddress => (string)base.Instance.CimInstanceProperties["PortAddress"].Value;

	public ushort[] PortNumbers => (ushort[])base.Instance.CimInstanceProperties["PortNumbers"].Value;

	public ushort? PortType => (ushort?)base.Instance.CimInstanceProperties["PortType"].Value;

	public ushort? Role => (ushort?)base.Instance.CimInstanceProperties["Role"].Value;

	public ulong? Speed => (ulong?)base.Instance.CimInstanceProperties["Speed"].Value;

	public string StorageControllerId => (string)base.Instance.CimInstanceProperties["StorageControllerId"].Value;

	public ushort? UsageRestriction => (ushort?)base.Instance.CimInstanceProperties["UsageRestriction"].Value;

	public MSFT_TargetPort()
	{
	}

	public MSFT_TargetPort(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_MaskingSetToTargetPort = new MSFT_TargetPortMSFT_MaskingSetToTargetPort(session, instance);
		MSFT_StorageSubSystemToTargetPort = new MSFT_TargetPortMSFT_StorageSubSystemToTargetPort(session, instance);
		MSFT_TargetPortToTargetPortal = new MSFT_TargetPortMSFT_TargetPortToTargetPortal(session, instance);
		MSFT_TargetPortToVirtualDisk = new MSFT_TargetPortMSFT_TargetPortToVirtualDisk(session, instance);
	}

	public static MSFT_TargetPort GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_TargetPort", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_TargetPort(session, instance);
	}

	public static MSFT_TargetPort CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_TargetPort", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_TargetPort(session, cimInstance);
	}

	public new static IEnumerable<MSFT_TargetPort> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_TargetPort")
			select new MSFT_TargetPort(session, i);
	}

	public new static IEnumerable<MSFT_TargetPort> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_TargetPort";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_TargetPort(session, i);
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

