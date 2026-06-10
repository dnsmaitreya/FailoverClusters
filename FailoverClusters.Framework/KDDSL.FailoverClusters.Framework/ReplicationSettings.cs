using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_ReplicationSettings : MiInstanceBase
{
	public MSFT_Volume[] LogDevices
	{
		get
		{
			if (base.Instance.CimInstanceProperties["LogDevices"].Value != null)
			{
				return ((IEnumerable<CimInstance>)base.Instance.CimInstanceProperties["LogDevices"].Value).Select((CimInstance i) => new MSFT_Volume(base.Session, i)).ToArray();
			}
			return null;
		}
		set
		{
			base.Instance.CimInstanceProperties["LogDevices"].Value = value?.Select((MSFT_Volume i) => i.Instance).ToArray();
		}
	}

	public ulong? LogSizeInBytes
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["LogSizeInBytes"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["LogSizeInBytes"].Value = value;
		}
	}

	public ushort? ReplicationQuorum
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["ReplicationQuorum"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ReplicationQuorum"].Value = value;
		}
	}

	public ushort? SyncMode
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["SyncMode"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SyncMode"].Value = value;
		}
	}

	public ushort? TargetElementSupplier
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["TargetElementSupplier"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["TargetElementSupplier"].Value = value;
		}
	}

	public ushort? ThinProvisioningPolicy
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["ThinProvisioningPolicy"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ThinProvisioningPolicy"].Value = value;
		}
	}

	public MSFT_ReplicationSettings()
	{
	}

	public MSFT_ReplicationSettings(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_ReplicationSettings GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_ReplicationSettings", "root/windows/storage");
		CimInstance instance = session.GetInstance("root/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_ReplicationSettings(session, instance);
	}

	public static MSFT_ReplicationSettings CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_ReplicationSettings", "root/windows/storage");
		return new MSFT_ReplicationSettings(session, instance);
	}

	public static IEnumerable<MSFT_ReplicationSettings> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_ReplicationSettings")
			select new MSFT_ReplicationSettings(session, i);
	}

	public static IEnumerable<MSFT_ReplicationSettings> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_ReplicationSettings";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_ReplicationSettings(session, i);
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

