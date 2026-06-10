using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageHealthReport : MiInstanceBase
{
	public MSFT_HealthRecord[] Records
	{
		get
		{
			if (base.Instance.CimInstanceProperties["Records"].Value != null)
			{
				return ((IEnumerable<CimInstance>)base.Instance.CimInstanceProperties["Records"].Value).Select((CimInstance i) => new MSFT_HealthRecord(base.Session, i)).ToArray();
			}
			return null;
		}
	}

	public string ReportedObjectId => (string)base.Instance.CimInstanceProperties["ReportedObjectId"].Value;

	public string StorageSubsystemObjectId => (string)base.Instance.CimInstanceProperties["StorageSubsystemObjectId"].Value;

	public MSFT_StorageHealthReport()
	{
	}

	public MSFT_StorageHealthReport(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_StorageHealthReport GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_StorageHealthReport", "root/microsoft/windows/storage");
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageHealthReport(session, instance);
	}

	public static MSFT_StorageHealthReport CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_StorageHealthReport", "root/microsoft/windows/storage");
		return new MSFT_StorageHealthReport(session, instance);
	}

	public static IEnumerable<MSFT_StorageHealthReport> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageHealthReport")
			select new MSFT_StorageHealthReport(session, i);
	}

	public static IEnumerable<MSFT_StorageHealthReport> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageHealthReport";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageHealthReport(session, i);
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

