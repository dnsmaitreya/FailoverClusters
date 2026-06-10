using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_HealthAction : MSFT_StorageObject
{
	public string Description => (string)base.Instance.CimInstanceProperties["Description"].Value;

	public uint? ErrorCode => (uint?)base.Instance.CimInstanceProperties["ErrorCode"].Value;

	public string ErrorDescription => (string)base.Instance.CimInstanceProperties["ErrorDescription"].Value;

	public string[] MessageParameters => (string[])base.Instance.CimInstanceProperties["MessageParameters"].Value;

	public ushort? PercentComplete => (ushort?)base.Instance.CimInstanceProperties["PercentComplete"].Value;

	public string Reason => (string)base.Instance.CimInstanceProperties["Reason"].Value;

	public DateTime? StartTime => (DateTime?)base.Instance.CimInstanceProperties["StartTime"].Value;

	public ushort? State => (ushort?)base.Instance.CimInstanceProperties["State"].Value;

	public string Status => (string)base.Instance.CimInstanceProperties["Status"].Value;

	public string Type => (string)base.Instance.CimInstanceProperties["Type"].Value;

	public MSFT_HealthAction()
	{
	}

	public MSFT_HealthAction(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_HealthAction GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_HealthAction", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_HealthAction(session, instance);
	}

	public static MSFT_HealthAction CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_HealthAction", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_HealthAction(session, cimInstance);
	}

	public new static IEnumerable<MSFT_HealthAction> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_HealthAction")
			select new MSFT_HealthAction(session, i);
	}

	public new static IEnumerable<MSFT_HealthAction> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_HealthAction";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_HealthAction(session, i);
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

