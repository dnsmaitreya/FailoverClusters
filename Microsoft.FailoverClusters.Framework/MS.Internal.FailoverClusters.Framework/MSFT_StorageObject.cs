using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageObject : MiInstanceBase
{
	public MSFT_StorageObjectMSFT_StorageJobToAffectedStorageObject MSFT_StorageJobToAffectedStorageObject { get; private set; }

	public string ObjectId => (string)base.Instance.CimInstanceProperties["ObjectId"].Value;

	public string PassThroughClass => (string)base.Instance.CimInstanceProperties["PassThroughClass"].Value;

	public string PassThroughIds => (string)base.Instance.CimInstanceProperties["PassThroughIds"].Value;

	public string PassThroughNamespace => (string)base.Instance.CimInstanceProperties["PassThroughNamespace"].Value;

	public string PassThroughServer => (string)base.Instance.CimInstanceProperties["PassThroughServer"].Value;

	public string UniqueId => (string)base.Instance.CimInstanceProperties["UniqueId"].Value;

	public MSFT_StorageObject()
	{
	}

	public MSFT_StorageObject(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageJobToAffectedStorageObject = new MSFT_StorageObjectMSFT_StorageJobToAffectedStorageObject(session, instance);
	}

	public static IEnumerable<MSFT_StorageObject> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageObject")
			select new MSFT_StorageObject(session, i);
	}

	public static IEnumerable<MSFT_StorageObject> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageObject";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageObject(session, i);
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
