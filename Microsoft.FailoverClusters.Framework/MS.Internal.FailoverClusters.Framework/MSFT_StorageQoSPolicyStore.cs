using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageQoSPolicyStore : MiInstanceBase
{
	public struct CreatePolicyOutParameters
	{
		public int? ReturnValue { get; set; }

		public MSFT_StorageQoSPolicy Policy { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CreatePolicyOutParameters lhs, CreatePolicyOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreatePolicyOutParameters lhs, CreatePolicyOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

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

	public string Id => (string)base.Instance.CimInstanceProperties["Id"].Value;

	public uint? IOPSNormalizationSize
	{
		get
		{
			return (uint?)base.Instance.CimInstanceProperties["IOPSNormalizationSize"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["IOPSNormalizationSize"].Value = value;
		}
	}

	public MSFT_StorageQoSPolicyStore()
	{
	}

	public MSFT_StorageQoSPolicyStore(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_StorageQoSPolicyStore GetInstance(CimSession session, string Id)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageQoSPolicyStore", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("Id", Id, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageQoSPolicyStore(session, instance);
	}

	public static MSFT_StorageQoSPolicyStore CreateReference(CimSession session, string Id)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageQoSPolicyStore", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("Id", Id, CimFlags.Key));
		return new MSFT_StorageQoSPolicyStore(session, cimInstance);
	}

	public static IEnumerable<MSFT_StorageQoSPolicyStore> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageQoSPolicyStore")
			select new MSFT_StorageQoSPolicyStore(session, i);
	}

	public static IEnumerable<MSFT_StorageQoSPolicyStore> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageQoSPolicyStore";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageQoSPolicyStore(session, i);
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

	public CreatePolicyOutParameters CreatePolicy(MSFT_StorageQoSPolicy Policy = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Policy != null)
		{
			CimInstance value = Policy?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Policy", value, CimType.Instance, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreatePolicy", cimMethodParametersCollection);
		CreatePolicyOutParameters result = default(CreatePolicyOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (int?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["Policy"] != null)
		{
			result.Policy = new MSFT_StorageQoSPolicy(base.Session, (CimInstance)cimMethodResult.OutParameters["Policy"].Value);
		}
		else
		{
			result.Policy = null;
		}
		return result;
	}

	public SetAttributesOutParameters SetAttributes(uint? IOPSNormalizationSize)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (IOPSNormalizationSize.HasValue)
		{
			uint? num = IOPSNormalizationSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IOPSNormalizationSize", num, CimType.UInt32, CimFlags.In));
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
}
