using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSITargetPortal : MiInstanceBase
{
	public struct NewOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_iSCSITargetPortal CreatedTargetPortal { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(NewOutParameters lhs, NewOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(NewOutParameters lhs, NewOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RemoveOutParameters
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

		public static bool operator ==(RemoveOutParameters lhs, RemoveOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RemoveOutParameters lhs, RemoveOutParameters rhs)
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

	public MSFT_iSCSITargetPortalMSFT_iSCSIConnectionToiSCSITargetPortal MSFT_iSCSIConnectionToiSCSITargetPortal { get; private set; }

	public MSFT_iSCSITargetPortalMSFT_iSCSISessionToiSCSITargetPortal MSFT_iSCSISessionToiSCSITargetPortal { get; private set; }

	public MSFT_iSCSITargetPortalMSFT_iSCSITargetToiSCSITargetPortal MSFT_iSCSITargetToiSCSITargetPortal { get; private set; }

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

	public string TargetPortalAddress
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["TargetPortalAddress"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["TargetPortalAddress"].Value = value;
		}
	}

	public ushort? TargetPortalPortNumber
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["TargetPortalPortNumber"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["TargetPortalPortNumber"].Value = value;
		}
	}

	public MSFT_iSCSITargetPortal()
	{
	}

	public MSFT_iSCSITargetPortal(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_iSCSIConnectionToiSCSITargetPortal = new MSFT_iSCSITargetPortalMSFT_iSCSIConnectionToiSCSITargetPortal(session, instance);
		MSFT_iSCSISessionToiSCSITargetPortal = new MSFT_iSCSITargetPortalMSFT_iSCSISessionToiSCSITargetPortal(session, instance);
		MSFT_iSCSITargetToiSCSITargetPortal = new MSFT_iSCSITargetPortalMSFT_iSCSITargetToiSCSITargetPortal(session, instance);
	}

	public static MSFT_iSCSITargetPortal GetInstance(CimSession session, string TargetPortalAddress)
	{
		CimInstance cimInstance = new CimInstance("MSFT_iSCSITargetPortal", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("TargetPortalAddress", TargetPortalAddress, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_iSCSITargetPortal(session, instance);
	}

	public static MSFT_iSCSITargetPortal CreateReference(CimSession session, string TargetPortalAddress)
	{
		CimInstance cimInstance = new CimInstance("MSFT_iSCSITargetPortal", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("TargetPortalAddress", TargetPortalAddress, CimFlags.Key));
		return new MSFT_iSCSITargetPortal(session, cimInstance);
	}

	public static IEnumerable<MSFT_iSCSITargetPortal> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_iSCSITargetPortal")
			select new MSFT_iSCSITargetPortal(session, i);
	}

	public static IEnumerable<MSFT_iSCSITargetPortal> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_iSCSITargetPortal";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_iSCSITargetPortal(session, i);
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

	public static NewOutParameters New(CimSession Session, string TargetPortalAddress = null, ushort? TargetPortalPortNumber = null, string InitiatorInstanceName = null, string InitiatorPortalAddress = null, string AuthenticationType = null, string ChapUsername = null, string ChapSecret = null, bool? IsHeaderDigest = null, bool? IsDataDigest = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (TargetPortalAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortalAddress", TargetPortalAddress, CimType.String, CimFlags.In));
		}
		if (TargetPortalPortNumber.HasValue)
		{
			ushort? num = TargetPortalPortNumber;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortalPortNumber", num, CimType.UInt16, CimFlags.In));
		}
		if (InitiatorInstanceName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorInstanceName", InitiatorInstanceName, CimType.String, CimFlags.In));
		}
		if (InitiatorPortalAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorPortalAddress", InitiatorPortalAddress, CimType.String, CimFlags.In));
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
		if (IsHeaderDigest.HasValue)
		{
			bool? flag = IsHeaderDigest;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsHeaderDigest", flag, CimType.Boolean, CimFlags.In));
		}
		if (IsDataDigest.HasValue)
		{
			bool? flag2 = IsDataDigest;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsDataDigest", flag2, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "MSFT_iSCSITargetPortal", "New", cimMethodParametersCollection);
		NewOutParameters result = default(NewOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedTargetPortal"] != null)
		{
			result.CreatedTargetPortal = new MSFT_iSCSITargetPortal(Session, (CimInstance)cimMethodResult.OutParameters["CreatedTargetPortal"].Value);
		}
		else
		{
			result.CreatedTargetPortal = null;
		}
		return result;
	}

	public RemoveOutParameters Remove(string InitiatorInstanceName = null, string InitiatorPortalAddress = null, ushort? TargetPortalPortNumber = null, string TargetPortalAddress = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InitiatorInstanceName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorInstanceName", InitiatorInstanceName, CimType.String, CimFlags.In));
		}
		if (InitiatorPortalAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorPortalAddress", InitiatorPortalAddress, CimType.String, CimFlags.In));
		}
		if (TargetPortalPortNumber.HasValue)
		{
			ushort? num = TargetPortalPortNumber;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortalPortNumber", num, CimType.UInt16, CimFlags.In));
		}
		if (TargetPortalAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortalAddress", TargetPortalAddress, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Remove", cimMethodParametersCollection);
		RemoveOutParameters result = default(RemoveOutParameters);
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

	public UpdateOutParameters Update(string InitiatorInstanceName = null, string InitiatorPortalAddress = null, string TargetPortalAddress = null, ushort? TargetPortalPortNumber = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InitiatorInstanceName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorInstanceName", InitiatorInstanceName, CimType.String, CimFlags.In));
		}
		if (InitiatorPortalAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorPortalAddress", InitiatorPortalAddress, CimType.String, CimFlags.In));
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
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Update", cimMethodParametersCollection);
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
}
