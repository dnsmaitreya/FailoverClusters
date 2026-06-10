using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageFaultDomain : MSFT_StorageObject
{
	public MSFT_StorageFaultDomainMSFT_StorageFaultDomainToStorageFaultDomain MSFT_StorageFaultDomainToStorageFaultDomain { get; private set; }

	public MSFT_StorageFaultDomainMSFT_StorageSubSystemToStorageFaultDomain MSFT_StorageSubSystemToStorageFaultDomain { get; private set; }

	public string Description => (string)base.Instance.CimInstanceProperties["Description"].Value;

	public string FriendlyName => (string)base.Instance.CimInstanceProperties["FriendlyName"].Value;

	public ushort? HealthStatus => (ushort?)base.Instance.CimInstanceProperties["HealthStatus"].Value;

	public string Manufacturer => (string)base.Instance.CimInstanceProperties["Manufacturer"].Value;

	public string Model => (string)base.Instance.CimInstanceProperties["Model"].Value;

	public string[] OperationalDetails => (string[])base.Instance.CimInstanceProperties["OperationalDetails"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public string PhysicalLocation => (string)base.Instance.CimInstanceProperties["PhysicalLocation"].Value;

	public string SerialNumber => (string)base.Instance.CimInstanceProperties["SerialNumber"].Value;

	public MSFT_StorageFaultDomain()
	{
	}

	public MSFT_StorageFaultDomain(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageFaultDomainToStorageFaultDomain = new MSFT_StorageFaultDomainMSFT_StorageFaultDomainToStorageFaultDomain(session, instance);
		MSFT_StorageSubSystemToStorageFaultDomain = new MSFT_StorageFaultDomainMSFT_StorageSubSystemToStorageFaultDomain(session, instance);
	}

	public new static IEnumerable<MSFT_StorageFaultDomain> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageFaultDomain")
			select new MSFT_StorageFaultDomain(session, i);
	}

	public new static IEnumerable<MSFT_StorageFaultDomain> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageFaultDomain";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageFaultDomain(session, i);
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

