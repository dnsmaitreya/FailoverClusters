using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_FileShareAccessControlEntry : MiInstanceBase
{
	public ushort? AccessControlType => (ushort?)base.Instance.CimInstanceProperties["AccessControlType"].Value;

	public ushort? AccessRight => (ushort?)base.Instance.CimInstanceProperties["AccessRight"].Value;

	public string AccountName => (string)base.Instance.CimInstanceProperties["AccountName"].Value;

	public MSFT_FileShareAccessControlEntry()
	{
	}

	public MSFT_FileShareAccessControlEntry(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_FileShareAccessControlEntry GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_FileShareAccessControlEntry", "root/microsoft/windows/storage");
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_FileShareAccessControlEntry(session, instance);
	}

	public static MSFT_FileShareAccessControlEntry CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_FileShareAccessControlEntry", "root/microsoft/windows/storage");
		return new MSFT_FileShareAccessControlEntry(session, instance);
	}

	public static IEnumerable<MSFT_FileShareAccessControlEntry> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_FileShareAccessControlEntry")
			select new MSFT_FileShareAccessControlEntry(session, i);
	}

	public static IEnumerable<MSFT_FileShareAccessControlEntry> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_FileShareAccessControlEntry";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_FileShareAccessControlEntry(session, i);
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

