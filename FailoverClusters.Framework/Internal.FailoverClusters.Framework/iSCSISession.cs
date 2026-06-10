using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSISession : MiInstanceBase
{
	public struct RegisterOutParameters
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

		public static bool operator ==(RegisterOutParameters lhs, RegisterOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RegisterOutParameters lhs, RegisterOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct UnregisterOutParameters
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

		public static bool operator ==(UnregisterOutParameters lhs, UnregisterOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(UnregisterOutParameters lhs, UnregisterOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetCHAPSecretOutParameters
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

		public static bool operator ==(SetCHAPSecretOutParameters lhs, SetCHAPSecretOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetCHAPSecretOutParameters lhs, SetCHAPSecretOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_iSCSISessionMSFT_InitiatorPortToiSCSISession MSFT_InitiatorPortToiSCSISession { get; private set; }

	public MSFT_iSCSISessionMSFT_iSCSISessionToDisk MSFT_iSCSISessionToDisk { get; private set; }

	public MSFT_iSCSISessionMSFT_iSCSISessionToiSCSIConnection MSFT_iSCSISessionToiSCSIConnection { get; private set; }

	public MSFT_iSCSISessionMSFT_iSCSISessionToiSCSITargetPortal MSFT_iSCSISessionToiSCSITargetPortal { get; private set; }

	public MSFT_iSCSISessionMSFT_iSCSITargetToiSCSISession MSFT_iSCSITargetToiSCSISession { get; private set; }

	public string AuthenticationType
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["AuthenticationType"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["AuthenticationType"].Value = value;
		}
	}

	public string InitiatorInstanceName
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["InitiatorInstanceName"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["InitiatorInstanceName"].Value = value;
		}
	}

	public string InitiatorNodeAddress
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["InitiatorNodeAddress"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["InitiatorNodeAddress"].Value = value;
		}
	}

	public string InitiatorPortalAddress
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["InitiatorPortalAddress"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["InitiatorPortalAddress"].Value = value;
		}
	}

	public string InitiatorSideIdentifier
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["InitiatorSideIdentifier"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["InitiatorSideIdentifier"].Value = value;
		}
	}

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

	public bool? IsDataDigest
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["IsDataDigest"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["IsDataDigest"].Value = value;
		}
	}

	public bool? IsDiscovered
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["IsDiscovered"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["IsDiscovered"].Value = value;
		}
	}

	public bool? IsHeaderDigest
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["IsHeaderDigest"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["IsHeaderDigest"].Value = value;
		}
	}

	public bool? IsPersistent
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["IsPersistent"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["IsPersistent"].Value = value;
		}
	}

	public uint? NumberOfConnections
	{
		get
		{
			return (uint?)base.Instance.CimInstanceProperties["NumberOfConnections"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["NumberOfConnections"].Value = value;
		}
	}

	public string SessionIdentifier
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["SessionIdentifier"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SessionIdentifier"].Value = value;
		}
	}

	public string TargetNodeAddress
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["TargetNodeAddress"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["TargetNodeAddress"].Value = value;
		}
	}

	public string TargetSideIdentifier
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["TargetSideIdentifier"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["TargetSideIdentifier"].Value = value;
		}
	}

	public MSFT_iSCSISession()
	{
	}

	public MSFT_iSCSISession(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_InitiatorPortToiSCSISession = new MSFT_iSCSISessionMSFT_InitiatorPortToiSCSISession(session, instance);
		MSFT_iSCSISessionToDisk = new MSFT_iSCSISessionMSFT_iSCSISessionToDisk(session, instance);
		MSFT_iSCSISessionToiSCSIConnection = new MSFT_iSCSISessionMSFT_iSCSISessionToiSCSIConnection(session, instance);
		MSFT_iSCSISessionToiSCSITargetPortal = new MSFT_iSCSISessionMSFT_iSCSISessionToiSCSITargetPortal(session, instance);
		MSFT_iSCSITargetToiSCSISession = new MSFT_iSCSISessionMSFT_iSCSITargetToiSCSISession(session, instance);
	}

	public static MSFT_iSCSISession GetInstance(CimSession session, string SessionIdentifier)
	{
		CimInstance cimInstance = new CimInstance("MSFT_iSCSISession", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("SessionIdentifier", SessionIdentifier, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_iSCSISession(session, instance);
	}

	public static MSFT_iSCSISession CreateReference(CimSession session, string SessionIdentifier)
	{
		CimInstance cimInstance = new CimInstance("MSFT_iSCSISession", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("SessionIdentifier", SessionIdentifier, CimFlags.Key));
		return new MSFT_iSCSISession(session, cimInstance);
	}

	public static IEnumerable<MSFT_iSCSISession> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_iSCSISession")
			select new MSFT_iSCSISession(session, i);
	}

	public static IEnumerable<MSFT_iSCSISession> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_iSCSISession";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_iSCSISession(session, i);
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

	public RegisterOutParameters Register(bool? IsMultipathEnabled = null, string ChapUsername = null, string ChapSecret = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (IsMultipathEnabled.HasValue)
		{
			bool? flag = IsMultipathEnabled;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsMultipathEnabled", flag, CimType.Boolean, CimFlags.In));
		}
		if (ChapUsername != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ChapUsername", ChapUsername, CimType.String, CimFlags.In));
		}
		if (ChapSecret != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ChapSecret", ChapSecret, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Register", cimMethodParametersCollection);
		RegisterOutParameters result = default(RegisterOutParameters);
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

	public UnregisterOutParameters Unregister()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Unregister", methodParameters);
		UnregisterOutParameters result = default(UnregisterOutParameters);
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

	public static SetCHAPSecretOutParameters SetCHAPSecret(CimSession Session, string ChapSecret = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ChapSecret != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ChapSecret", ChapSecret, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "MSFT_iSCSISession", "SetCHAPSecret", cimMethodParametersCollection);
		SetCHAPSecretOutParameters result = default(SetCHAPSecretOutParameters);
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
}

