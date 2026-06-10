using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_DedupProperties : MiInstanceBase
{
	public ulong? InPolicyFilesCount => (ulong?)base.Instance.CimInstanceProperties["InPolicyFilesCount"].Value;

	public ulong? InPolicyFilesSize => (ulong?)base.Instance.CimInstanceProperties["InPolicyFilesSize"].Value;

	public ulong? OptimizedFilesCount => (ulong?)base.Instance.CimInstanceProperties["OptimizedFilesCount"].Value;

	public uint? OptimizedFilesSavingsRate => (uint?)base.Instance.CimInstanceProperties["OptimizedFilesSavingsRate"].Value;

	public ulong? OptimizedFilesSize => (ulong?)base.Instance.CimInstanceProperties["OptimizedFilesSize"].Value;

	public uint? SavingsRate => (uint?)base.Instance.CimInstanceProperties["SavingsRate"].Value;

	public ulong? SavingsSize => (ulong?)base.Instance.CimInstanceProperties["SavingsSize"].Value;

	public ulong? UnoptimizedSize => (ulong?)base.Instance.CimInstanceProperties["UnoptimizedSize"].Value;

	public MSFT_DedupProperties()
	{
	}

	public MSFT_DedupProperties(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_DedupProperties GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_DedupProperties", "root/microsoft/windows/storage");
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_DedupProperties(session, instance);
	}

	public static MSFT_DedupProperties CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_DedupProperties", "root/microsoft/windows/storage");
		return new MSFT_DedupProperties(session, instance);
	}

	public static IEnumerable<MSFT_DedupProperties> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_DedupProperties")
			select new MSFT_DedupProperties(session, i);
	}

	public static IEnumerable<MSFT_DedupProperties> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_DedupProperties";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_DedupProperties(session, i);
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

