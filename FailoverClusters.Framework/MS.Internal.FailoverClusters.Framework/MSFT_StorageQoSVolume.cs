using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageQoSVolume : MiInstanceBase
{
	public ulong? Bandwidth => (ulong?)base.Instance.CimInstanceProperties["Bandwidth"].Value;

	public ulong? BandwidthLimit => (ulong?)base.Instance.CimInstanceProperties["BandwidthLimit"].Value;

	public ulong? Interval => (ulong?)base.Instance.CimInstanceProperties["Interval"].Value;

	public ulong? IOPS => (ulong?)base.Instance.CimInstanceProperties["IOPS"].Value;

	public ulong? Latency => (ulong?)base.Instance.CimInstanceProperties["Latency"].Value;

	public ulong? Limit => (ulong?)base.Instance.CimInstanceProperties["Limit"].Value;

	public string Mountpoint => (string)base.Instance.CimInstanceProperties["Mountpoint"].Value;

	public ulong? Reservation => (ulong?)base.Instance.CimInstanceProperties["Reservation"].Value;

	public ushort? Status => (ushort?)base.Instance.CimInstanceProperties["Status"].Value;

	public ulong? TimeStamp => (ulong?)base.Instance.CimInstanceProperties["TimeStamp"].Value;

	public string VolumeId => (string)base.Instance.CimInstanceProperties["VolumeId"].Value;

	public MSFT_StorageQoSVolume()
	{
	}

	public MSFT_StorageQoSVolume(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_StorageQoSVolume GetInstance(CimSession session, string VolumeId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageQoSVolume", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("VolumeId", VolumeId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageQoSVolume(session, instance);
	}

	public static MSFT_StorageQoSVolume CreateReference(CimSession session, string VolumeId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageQoSVolume", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("VolumeId", VolumeId, CimFlags.Key));
		return new MSFT_StorageQoSVolume(session, cimInstance);
	}

	public static IEnumerable<MSFT_StorageQoSVolume> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageQoSVolume")
			select new MSFT_StorageQoSVolume(session, i);
	}

	public static IEnumerable<MSFT_StorageQoSVolume> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageQoSVolume";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageQoSVolume(session, i);
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

