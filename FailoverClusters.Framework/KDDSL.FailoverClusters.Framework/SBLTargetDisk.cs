using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_SBLTargetDisk : MiInstanceBase
{
	public uint? CacheMode => (uint?)base.Instance.CimInstanceProperties["CacheMode"].Value;

	public uint? DeviceNumber => (uint?)base.Instance.CimInstanceProperties["DeviceNumber"].Value;

	public string Identifier => (string)base.Instance.CimInstanceProperties["Identifier"].Value;

	public bool? IsFlash => (bool?)base.Instance.CimInstanceProperties["IsFlash"].Value;

	public bool? IsSblCacheDevice => (bool?)base.Instance.CimInstanceProperties["IsSblCacheDevice"].Value;

	public DateTime? LastStateChangeTime => (DateTime?)base.Instance.CimInstanceProperties["LastStateChangeTime"].Value;

	public ulong? ReadMediaErrorCount => (ulong?)base.Instance.CimInstanceProperties["ReadMediaErrorCount"].Value;

	public ulong? ReadTotalErrorCount => (ulong?)base.Instance.CimInstanceProperties["ReadTotalErrorCount"].Value;

	public uint? State => (uint?)base.Instance.CimInstanceProperties["State"].Value;

	public ulong? WriteMediaErrorCount => (ulong?)base.Instance.CimInstanceProperties["WriteMediaErrorCount"].Value;

	public ulong? WriteTotalErrorCount => (ulong?)base.Instance.CimInstanceProperties["WriteTotalErrorCount"].Value;

	public MSFT_SBLTargetDisk()
	{
	}

	public MSFT_SBLTargetDisk(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_SBLTargetDisk GetInstance(CimSession session, string Identifier)
	{
		CimInstance cimInstance = new CimInstance("MSFT_SBLTargetDisk", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("Identifier", Identifier, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_SBLTargetDisk(session, instance);
	}

	public static MSFT_SBLTargetDisk CreateReference(CimSession session, string Identifier)
	{
		CimInstance cimInstance = new CimInstance("MSFT_SBLTargetDisk", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("Identifier", Identifier, CimFlags.Key));
		return new MSFT_SBLTargetDisk(session, cimInstance);
	}

	public static IEnumerable<MSFT_SBLTargetDisk> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_SBLTargetDisk")
			select new MSFT_SBLTargetDisk(session, i);
	}

	public static IEnumerable<MSFT_SBLTargetDisk> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_SBLTargetDisk";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_SBLTargetDisk(session, i);
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

