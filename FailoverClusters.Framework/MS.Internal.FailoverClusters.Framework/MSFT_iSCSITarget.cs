using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSITarget : MiInstanceBase
{
	public struct DisconnectOutParameters
	{
		public uint? ReturnValue { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(DisconnectOutParameters lhs, DisconnectOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DisconnectOutParameters lhs, DisconnectOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct UpdateOutParameters
	{
		public uint? ReturnValue { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(UpdateOutParameters lhs, UpdateOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(UpdateOutParameters lhs, UpdateOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct ConnectOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_iSCSISession CreatediSCSISession { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(ConnectOutParameters lhs, ConnectOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ConnectOutParameters lhs, ConnectOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_iSCSITargetMSFT_InitiatorPortToiSCSITarget MSFT_InitiatorPortToiSCSITarget { get; private set; }

	public MSFT_iSCSITargetMSFT_iSCSITargetToiSCSIConnection MSFT_iSCSITargetToiSCSIConnection { get; private set; }

	public MSFT_iSCSITargetMSFT_iSCSITargetToiSCSISession MSFT_iSCSITargetToiSCSISession { get; private set; }

	public MSFT_iSCSITargetMSFT_iSCSITargetToiSCSITargetPortal MSFT_iSCSITargetToiSCSITargetPortal { get; private set; }

	public bool? IsConnected
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["IsConnected"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["IsConnected"].Value = value;
		}
	}

	public string NodeAddress
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["NodeAddress"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["NodeAddress"].Value = value;
		}
	}

	public MSFT_iSCSITarget()
	{
	}

	public MSFT_iSCSITarget(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_InitiatorPortToiSCSITarget = new MSFT_iSCSITargetMSFT_InitiatorPortToiSCSITarget(session, instance);
		MSFT_iSCSITargetToiSCSIConnection = new MSFT_iSCSITargetMSFT_iSCSITargetToiSCSIConnection(session, instance);
		MSFT_iSCSITargetToiSCSISession = new MSFT_iSCSITargetMSFT_iSCSITargetToiSCSISession(session, instance);
		MSFT_iSCSITargetToiSCSITargetPortal = new MSFT_iSCSITargetMSFT_iSCSITargetToiSCSITargetPortal(session, instance);
	}

	public static MSFT_iSCSITarget GetInstance(CimSession session, string NodeAddress)
	{
		CimInstance cimInstance = new CimInstance("MSFT_iSCSITarget", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("NodeAddress", NodeAddress, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_iSCSITarget(session, instance);
	}

	public static MSFT_iSCSITarget CreateReference(CimSession session, string NodeAddress)
	{
		CimInstance cimInstance = new CimInstance("MSFT_iSCSITarget", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("NodeAddress", NodeAddress, CimFlags.Key));
		return new MSFT_iSCSITarget(session, cimInstance);
	}

	public static IEnumerable<MSFT_iSCSITarget> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_iSCSITarget")
			select new MSFT_iSCSITarget(session, i);
	}

	public static IEnumerable<MSFT_iSCSITarget> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_iSCSITarget";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_iSCSITarget(session, i);
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

	public DisconnectOutParameters Disconnect(string SessionIdentifier = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (SessionIdentifier != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SessionIdentifier", SessionIdentifier, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Disconnect", cimMethodParametersCollection);
		DisconnectOutParameters result = default(DisconnectOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		return result;
	}

	public UpdateOutParameters Update()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Update", methodParameters);
		UpdateOutParameters result = default(UpdateOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		return result;
	}

	public static ConnectOutParameters Connect(CimSession Session, string NodeAddress = null, string TargetPortalAddress = null, ushort? TargetPortalPortNumber = null, string InitiatorPortalAddress = null, bool? IsDataDigest = null, bool? IsHeaderDigest = null, bool? ReportToPnP = null, string AuthenticationType = null, string ChapUsername = null, string ChapSecret = null, bool? IsMultipathEnabled = null, bool? IsPersistent = null, string InitiatorInstanceName = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (NodeAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NodeAddress", NodeAddress, CimType.String, CimFlags.In));
		}
		if (TargetPortalAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortalAddress", TargetPortalAddress, CimType.String, CimFlags.In));
		}
		if (TargetPortalPortNumber.HasValue)
		{
			ushort? num = TargetPortalPortNumber;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortalPortNumber", num, CimType.UInt16, CimFlags.In));
		}
		if (InitiatorPortalAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorPortalAddress", InitiatorPortalAddress, CimType.String, CimFlags.In));
		}
		if (IsDataDigest.HasValue)
		{
			bool? flag = IsDataDigest;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsDataDigest", flag, CimType.Boolean, CimFlags.In));
		}
		if (IsHeaderDigest.HasValue)
		{
			bool? flag2 = IsHeaderDigest;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsHeaderDigest", flag2, CimType.Boolean, CimFlags.In));
		}
		if (ReportToPnP.HasValue)
		{
			bool? flag3 = ReportToPnP;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReportToPnP", flag3, CimType.Boolean, CimFlags.In));
		}
		if (AuthenticationType != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AuthenticationType", AuthenticationType, CimType.String, CimFlags.In));
		}
		if (ChapUsername != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ChapUsername", ChapUsername, CimType.String, CimFlags.In));
		}
		if (ChapSecret != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ChapSecret", ChapSecret, CimType.String, CimFlags.In));
		}
		if (IsMultipathEnabled.HasValue)
		{
			bool? flag4 = IsMultipathEnabled;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsMultipathEnabled", flag4, CimType.Boolean, CimFlags.In));
		}
		if (IsPersistent.HasValue)
		{
			bool? flag5 = IsPersistent;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsPersistent", flag5, CimType.Boolean, CimFlags.In));
		}
		if (InitiatorInstanceName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorInstanceName", InitiatorInstanceName, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "MSFT_iSCSITarget", "Connect", cimMethodParametersCollection);
		ConnectOutParameters result = default(ConnectOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatediSCSISession"] != null)
		{
			result.CreatediSCSISession = new MSFT_iSCSISession(Session, (CimInstance)cimMethodResult.OutParameters["CreatediSCSISession"].Value);
		}
		else
		{
			result.CreatediSCSISession = null;
		}
		return result;
	}
}

