using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageHealthSetting : MiInstanceBase
{
	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public string Value => (string)base.Instance.CimInstanceProperties["Value"].Value;

	public MSFT_StorageHealthSetting()
	{
	}

	public MSFT_StorageHealthSetting(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_StorageHealthSetting GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_StorageHealthSetting", "root/windows/storage");
		CimInstance instance = session.GetInstance("root/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageHealthSetting(session, instance);
	}

	public static MSFT_StorageHealthSetting CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_StorageHealthSetting", "root/windows/storage");
		return new MSFT_StorageHealthSetting(session, instance);
	}

	public static IEnumerable<MSFT_StorageHealthSetting> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_StorageHealthSetting")
			select new MSFT_StorageHealthSetting(session, i);
	}

	public static IEnumerable<MSFT_StorageHealthSetting> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageHealthSetting";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_StorageHealthSetting(session, i);
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

