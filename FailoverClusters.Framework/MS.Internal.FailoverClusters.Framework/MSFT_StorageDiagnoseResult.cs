using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageDiagnoseResult : MiInstanceBase
{
	public DateTime? EventTime => (DateTime?)base.Instance.CimInstanceProperties["EventTime"].Value;

	public string FaultId => (string)base.Instance.CimInstanceProperties["FaultId"].Value;

	public MSFT_StorageObject FaultingObject
	{
		get
		{
			return new MSFT_StorageObject(base.Session, (CimInstance)base.Instance.CimInstanceProperties["FaultingObject"].Value);
		}
		set
		{
			base.Instance.CimInstanceProperties["FaultingObject"].Value = value?.Instance;
		}
	}

	public string FaultingObjectDescription => (string)base.Instance.CimInstanceProperties["FaultingObjectDescription"].Value;

	public string FaultingObjectLocation => (string)base.Instance.CimInstanceProperties["FaultingObjectLocation"].Value;

	public string FaultType => (string)base.Instance.CimInstanceProperties["FaultType"].Value;

	public ushort? PerceivedSeverity => (ushort?)base.Instance.CimInstanceProperties["PerceivedSeverity"].Value;

	public string Reason => (string)base.Instance.CimInstanceProperties["Reason"].Value;

	public string[] RecommendedActions => (string[])base.Instance.CimInstanceProperties["RecommendedActions"].Value;

	public MSFT_StorageDiagnoseResult()
	{
	}

	public MSFT_StorageDiagnoseResult(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_StorageDiagnoseResult GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_StorageDiagnoseResult", "root/microsoft/windows/storage");
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageDiagnoseResult(session, instance);
	}

	public static MSFT_StorageDiagnoseResult CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_StorageDiagnoseResult", "root/microsoft/windows/storage");
		return new MSFT_StorageDiagnoseResult(session, instance);
	}

	public static IEnumerable<MSFT_StorageDiagnoseResult> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageDiagnoseResult")
			select new MSFT_StorageDiagnoseResult(session, i);
	}

	public static IEnumerable<MSFT_StorageDiagnoseResult> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageDiagnoseResult";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageDiagnoseResult(session, i);
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

