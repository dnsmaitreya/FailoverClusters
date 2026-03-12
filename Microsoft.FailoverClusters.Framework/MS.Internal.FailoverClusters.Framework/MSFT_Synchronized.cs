using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_Synchronized : MiInstanceBase
{
	public ushort? CopyMethodology => (ushort?)base.Instance.CimInstanceProperties["CopyMethodology"].Value;

	public ushort? CopyPriority => (ushort?)base.Instance.CimInstanceProperties["CopyPriority"].Value;

	public ushort? CopyState => (ushort?)base.Instance.CimInstanceProperties["CopyState"].Value;

	public ushort? CopyType => (ushort?)base.Instance.CimInstanceProperties["CopyType"].Value;

	public ushort? PercentSynced => (ushort?)base.Instance.CimInstanceProperties["PercentSynced"].Value;

	public ushort? ProgressStatus => (ushort?)base.Instance.CimInstanceProperties["ProgressStatus"].Value;

	public uint? RecoveryPointObjective => (uint?)base.Instance.CimInstanceProperties["RecoveryPointObjective"].Value;

	public ushort? ReplicaType => (ushort?)base.Instance.CimInstanceProperties["ReplicaType"].Value;

	public ushort? RequestedCopyState => (ushort?)base.Instance.CimInstanceProperties["RequestedCopyState"].Value;

	public bool? SyncMaintained => (bool?)base.Instance.CimInstanceProperties["SyncMaintained"].Value;

	public ushort? SyncMode => (ushort?)base.Instance.CimInstanceProperties["SyncMode"].Value;

	public ushort? SyncState => (ushort?)base.Instance.CimInstanceProperties["SyncState"].Value;

	public DateTime? SyncTime => (DateTime?)base.Instance.CimInstanceProperties["SyncTime"].Value;

	public ushort? SyncType => (ushort?)base.Instance.CimInstanceProperties["SyncType"].Value;

	public MSFT_Synchronized()
	{
	}

	public MSFT_Synchronized(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_Synchronized GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_Synchronized", "root/microsoft/windows/storage");
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_Synchronized(session, instance);
	}

	public static MSFT_Synchronized CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_Synchronized", "root/microsoft/windows/storage");
		return new MSFT_Synchronized(session, instance);
	}

	public static IEnumerable<MSFT_Synchronized> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_Synchronized")
			select new MSFT_Synchronized(session, i);
	}

	public static IEnumerable<MSFT_Synchronized> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_Synchronized";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_Synchronized(session, i);
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
