using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageQoSPolicy : MiInstanceBase
{
	public struct SetAttributesOutParameters
	{
		public int? ReturnValue { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(SetAttributesOutParameters lhs, SetAttributesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetAttributesOutParameters lhs, SetAttributesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct DeletePolicyOutParameters
	{
		public int? ReturnValue { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(DeletePolicyOutParameters lhs, DeletePolicyOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DeletePolicyOutParameters lhs, DeletePolicyOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_StorageQoSPolicyMSFT_StorageQoSPolicyToChildPolicy MSFT_StorageQoSPolicyToChildPolicy { get; private set; }

	public MSFT_StorageQoSPolicyMSFT_StorageQoSPolicyToFlow MSFT_StorageQoSPolicyToFlow { get; private set; }

	public ulong? BandwidthLimit
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["BandwidthLimit"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["BandwidthLimit"].Value = value;
		}
	}

	public string Name
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["Name"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["Name"].Value = value;
		}
	}

	public string ParentPolicy
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["ParentPolicy"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ParentPolicy"].Value = value;
		}
	}

	public string PolicyId => (string)base.Instance.CimInstanceProperties["PolicyId"].Value;

	public ushort? PolicyType => (ushort?)base.Instance.CimInstanceProperties["PolicyType"].Value;

	public ushort? Status => (ushort?)base.Instance.CimInstanceProperties["Status"].Value;

	public ulong? ThroughputLimit
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["ThroughputLimit"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ThroughputLimit"].Value = value;
		}
	}

	public ulong? ThroughputReservation
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["ThroughputReservation"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ThroughputReservation"].Value = value;
		}
	}

	public MSFT_StorageQoSPolicy()
	{
	}

	public MSFT_StorageQoSPolicy(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageQoSPolicyToChildPolicy = new MSFT_StorageQoSPolicyMSFT_StorageQoSPolicyToChildPolicy(session, instance);
		MSFT_StorageQoSPolicyToFlow = new MSFT_StorageQoSPolicyMSFT_StorageQoSPolicyToFlow(session, instance);
	}

	public static MSFT_StorageQoSPolicy GetInstance(CimSession session, string PolicyId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageQoSPolicy", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("PolicyId", PolicyId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageQoSPolicy(session, instance);
	}

	public static MSFT_StorageQoSPolicy CreateReference(CimSession session, string PolicyId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageQoSPolicy", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("PolicyId", PolicyId, CimFlags.Key));
		return new MSFT_StorageQoSPolicy(session, cimInstance);
	}

	public static IEnumerable<MSFT_StorageQoSPolicy> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageQoSPolicy")
			select new MSFT_StorageQoSPolicy(session, i);
	}

	public static IEnumerable<MSFT_StorageQoSPolicy> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageQoSPolicy";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageQoSPolicy(session, i);
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

	public SetAttributesOutParameters SetAttributes(string NewName = null, ulong? Limit = null, ulong? Reservation = null, ulong? BandwidthLimit = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (NewName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NewName", NewName, CimType.String, CimFlags.In));
		}
		if (Limit.HasValue)
		{
			ulong? num = Limit;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Limit", num, CimType.UInt64, CimFlags.In));
		}
		if (Reservation.HasValue)
		{
			ulong? num2 = Reservation;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Reservation", num2, CimType.UInt64, CimFlags.In));
		}
		if (BandwidthLimit.HasValue)
		{
			ulong? num3 = BandwidthLimit;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("BandwidthLimit", num3, CimType.UInt64, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetAttributes", cimMethodParametersCollection);
		SetAttributesOutParameters result = default(SetAttributesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (int?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		return result;
	}

	public DeletePolicyOutParameters DeletePolicy()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "DeletePolicy", methodParameters);
		DeletePolicyOutParameters result = default(DeletePolicyOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (int?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		return result;
	}
}

