using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_Volume_Repair_AsyncOutput : MSFT_StorageJobOutParams
{
	public uint? Output
	{
		get
		{
			return (uint?)base.Instance.CimInstanceProperties["Output"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["Output"].Value = value;
		}
	}

	public MSFT_Volume_Repair_AsyncOutput()
	{
	}

	public MSFT_Volume_Repair_AsyncOutput(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_Volume_Repair_AsyncOutput GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_Volume_Repair_AsyncOutput", "root/microsoft/windows/storage");
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_Volume_Repair_AsyncOutput(session, instance);
	}

	public static MSFT_Volume_Repair_AsyncOutput CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_Volume_Repair_AsyncOutput", "root/microsoft/windows/storage");
		return new MSFT_Volume_Repair_AsyncOutput(session, instance);
	}

	public new static IEnumerable<MSFT_Volume_Repair_AsyncOutput> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_Volume_Repair_AsyncOutput")
			select new MSFT_Volume_Repair_AsyncOutput(session, i);
	}

	public new static IEnumerable<MSFT_Volume_Repair_AsyncOutput> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_Volume_Repair_AsyncOutput";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_Volume_Repair_AsyncOutput(session, i);
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

