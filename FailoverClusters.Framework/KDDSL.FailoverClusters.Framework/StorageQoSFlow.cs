using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageQoSFlow : MiInstanceBase
{
	public MSFT_StorageQoSFlowMSFT_StorageQoSPolicyToFlow MSFT_StorageQoSPolicyToFlow { get; private set; }

	public ulong? BandwidthLimit => (ulong?)base.Instance.CimInstanceProperties["BandwidthLimit"].Value;

	public string FilePath => (string)base.Instance.CimInstanceProperties["FilePath"].Value;

	public string FlowId => (string)base.Instance.CimInstanceProperties["FlowId"].Value;

	public ulong? InitiatorBandwidth => (ulong?)base.Instance.CimInstanceProperties["InitiatorBandwidth"].Value;

	public string InitiatorId => (string)base.Instance.CimInstanceProperties["InitiatorId"].Value;

	public ulong? InitiatorIOPS => (ulong?)base.Instance.CimInstanceProperties["InitiatorIOPS"].Value;

	public ulong? InitiatorLatency => (ulong?)base.Instance.CimInstanceProperties["InitiatorLatency"].Value;

	public string InitiatorName => (string)base.Instance.CimInstanceProperties["InitiatorName"].Value;

	public string InitiatorNodeName => (string)base.Instance.CimInstanceProperties["InitiatorNodeName"].Value;

	public ulong? Interval => (ulong?)base.Instance.CimInstanceProperties["Interval"].Value;

	public ulong? Limit => (ulong?)base.Instance.CimInstanceProperties["Limit"].Value;

	public string PolicyId => (string)base.Instance.CimInstanceProperties["PolicyId"].Value;

	public ulong? Reservation => (ulong?)base.Instance.CimInstanceProperties["Reservation"].Value;

	public ushort? Status => (ushort?)base.Instance.CimInstanceProperties["Status"].Value;

	public ulong? StorageNodeBandwidth => (ulong?)base.Instance.CimInstanceProperties["StorageNodeBandwidth"].Value;

	public ulong? StorageNodeIOPS => (ulong?)base.Instance.CimInstanceProperties["StorageNodeIOPS"].Value;

	public ulong? StorageNodeLatency => (ulong?)base.Instance.CimInstanceProperties["StorageNodeLatency"].Value;

	public string StorageNodeName => (string)base.Instance.CimInstanceProperties["StorageNodeName"].Value;

	public ulong? TimeStamp => (ulong?)base.Instance.CimInstanceProperties["TimeStamp"].Value;

	public string VolumeId => (string)base.Instance.CimInstanceProperties["VolumeId"].Value;

	public MSFT_StorageQoSFlow()
	{
	}

	public MSFT_StorageQoSFlow(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageQoSPolicyToFlow = new MSFT_StorageQoSFlowMSFT_StorageQoSPolicyToFlow(session, instance);
	}

	public static MSFT_StorageQoSFlow GetInstance(CimSession session, string FlowId, string StorageNodeName, string VolumeId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageQoSFlow", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("FlowId", FlowId, CimFlags.Key));
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("StorageNodeName", StorageNodeName, CimFlags.Key));
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("VolumeId", VolumeId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageQoSFlow(session, instance);
	}

	public static MSFT_StorageQoSFlow CreateReference(CimSession session, string FlowId, string StorageNodeName, string VolumeId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageQoSFlow", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("FlowId", FlowId, CimFlags.Key));
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("StorageNodeName", StorageNodeName, CimFlags.Key));
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("VolumeId", VolumeId, CimFlags.Key));
		return new MSFT_StorageQoSFlow(session, cimInstance);
	}

	public static IEnumerable<MSFT_StorageQoSFlow> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_StorageQoSFlow")
			select new MSFT_StorageQoSFlow(session, i);
	}

	public static IEnumerable<MSFT_StorageQoSFlow> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageQoSFlow";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_StorageQoSFlow(session, i);
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

