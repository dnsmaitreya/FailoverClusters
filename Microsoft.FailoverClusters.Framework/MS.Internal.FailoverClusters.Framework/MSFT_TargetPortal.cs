using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_TargetPortal : MSFT_StorageObject
{
	public MSFT_TargetPortalMSFT_StorageSubSystemToTargetPortal MSFT_StorageSubSystemToTargetPortal { get; private set; }

	public MSFT_TargetPortalMSFT_TargetPortToTargetPortal MSFT_TargetPortToTargetPortal { get; private set; }

	public string IPv4Address => (string)base.Instance.CimInstanceProperties["IPv4Address"].Value;

	public string IPv6Address => (string)base.Instance.CimInstanceProperties["IPv6Address"].Value;

	public uint? PortNumber => (uint?)base.Instance.CimInstanceProperties["PortNumber"].Value;

	public string SubnetMask => (string)base.Instance.CimInstanceProperties["SubnetMask"].Value;

	public MSFT_TargetPortal()
	{
	}

	public MSFT_TargetPortal(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageSubSystemToTargetPortal = new MSFT_TargetPortalMSFT_StorageSubSystemToTargetPortal(session, instance);
		MSFT_TargetPortToTargetPortal = new MSFT_TargetPortalMSFT_TargetPortToTargetPortal(session, instance);
	}

	public static MSFT_TargetPortal GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_TargetPortal", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_TargetPortal(session, instance);
	}

	public static MSFT_TargetPortal CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_TargetPortal", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_TargetPortal(session, cimInstance);
	}

	public new static IEnumerable<MSFT_TargetPortal> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_TargetPortal")
			select new MSFT_TargetPortal(session, i);
	}

	public new static IEnumerable<MSFT_TargetPortal> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_TargetPortal";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_TargetPortal(session, i);
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
