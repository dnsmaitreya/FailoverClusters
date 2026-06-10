using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_HealthRecord : MiInstanceBase
{
	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public ushort? Units => (ushort?)base.Instance.CimInstanceProperties["Units"].Value;

	public MSFT_HealthRecord()
	{
	}

	public MSFT_HealthRecord(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static IEnumerable<MSFT_HealthRecord> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_HealthRecord")
			select new MSFT_HealthRecord(session, i);
	}

	public static IEnumerable<MSFT_HealthRecord> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_HealthRecord";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_HealthRecord(session, i);
	}

	public override void Refresh()
	{
		base.Instance = base.Session.GetInstance("root/windows/storage", base.Instance);
	}

	public override void Delete()
	{
		base.Session.DeleteInstance("root/windows/storage", base.Instance);
	}

	public override void Save()
	{
		base.Session.ModifyInstance("root/windows/storage", base.Instance);
	}
}

