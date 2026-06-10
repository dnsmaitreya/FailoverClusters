using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_InitiatorPort : MiInstanceBase
{
	public struct SetNodeAddressOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(SetNodeAddressOutParameters lhs, SetNodeAddressOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetNodeAddressOutParameters lhs, SetNodeAddressOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_InitiatorPortMSFT_InitiatorPortToiSCSIConnection MSFT_InitiatorPortToiSCSIConnection { get; private set; }

	public MSFT_InitiatorPortMSFT_InitiatorPortToiSCSISession MSFT_InitiatorPortToiSCSISession { get; private set; }

	public MSFT_InitiatorPortMSFT_InitiatorPortToiSCSITarget MSFT_InitiatorPortToiSCSITarget { get; private set; }

	public string[] AlternateNodeAddress => (string[])base.Instance.CimInstanceProperties["AlternateNodeAddress"].Value;

	public string[] AlternatePortAddress => (string[])base.Instance.CimInstanceProperties["AlternatePortAddress"].Value;

	public ushort? ConnectionType => (ushort?)base.Instance.CimInstanceProperties["ConnectionType"].Value;

	public string InstanceName => (string)base.Instance.CimInstanceProperties["InstanceName"].Value;

	public string NodeAddress => (string)base.Instance.CimInstanceProperties["NodeAddress"].Value;

	public string ObjectId => (string)base.Instance.CimInstanceProperties["ObjectId"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public string OtherConnectionTypeDescription => (string)base.Instance.CimInstanceProperties["OtherConnectionTypeDescription"].Value;

	public string PortAddress => (string)base.Instance.CimInstanceProperties["PortAddress"].Value;

	public ushort? PortType => (ushort?)base.Instance.CimInstanceProperties["PortType"].Value;

	public MSFT_InitiatorPort()
	{
	}

	public MSFT_InitiatorPort(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_InitiatorPortToiSCSIConnection = new MSFT_InitiatorPortMSFT_InitiatorPortToiSCSIConnection(session, instance);
		MSFT_InitiatorPortToiSCSISession = new MSFT_InitiatorPortMSFT_InitiatorPortToiSCSISession(session, instance);
		MSFT_InitiatorPortToiSCSITarget = new MSFT_InitiatorPortMSFT_InitiatorPortToiSCSITarget(session, instance);
	}

	public static MSFT_InitiatorPort GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_InitiatorPort", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_InitiatorPort(session, instance);
	}

	public static MSFT_InitiatorPort CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_InitiatorPort", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_InitiatorPort(session, cimInstance);
	}

	public static IEnumerable<MSFT_InitiatorPort> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_InitiatorPort")
			select new MSFT_InitiatorPort(session, i);
	}

	public static IEnumerable<MSFT_InitiatorPort> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_InitiatorPort";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_InitiatorPort(session, i);
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

	public SetNodeAddressOutParameters SetNodeAddress(string NodeAddress)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (NodeAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NodeAddress", NodeAddress, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetNodeAddress", cimMethodParametersCollection);
		SetNodeAddressOutParameters result = default(SetNodeAddressOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["ExtendedStatus"] != null)
		{
			result.ExtendedStatus = new MSFT_StorageExtendedStatus(base.Session, (CimInstance)cimMethodResult.OutParameters["ExtendedStatus"].Value);
		}
		else
		{
			result.ExtendedStatus = null;
		}
		return result;
	}
}

