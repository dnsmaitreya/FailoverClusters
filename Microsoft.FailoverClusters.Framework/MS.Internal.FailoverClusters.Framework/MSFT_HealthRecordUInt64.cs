using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_HealthRecordUInt64 : MSFT_HealthRecord
{
	public ulong? Value => (ulong?)base.Instance.CimInstanceProperties["Value"].Value;

	public MSFT_HealthRecordUInt64()
	{
	}

	public MSFT_HealthRecordUInt64(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_HealthRecordUInt64 GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_HealthRecordUInt64", "root/microsoft/windows/storage");
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_HealthRecordUInt64(session, instance);
	}

	public static MSFT_HealthRecordUInt64 CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_HealthRecordUInt64", "root/microsoft/windows/storage");
		return new MSFT_HealthRecordUInt64(session, instance);
	}

	public new static IEnumerable<MSFT_HealthRecordUInt64> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_HealthRecordUInt64")
			select new MSFT_HealthRecordUInt64(session, i);
	}

	public new static IEnumerable<MSFT_HealthRecordUInt64> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_HealthRecordUInt64";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_HealthRecordUInt64(session, i);
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
