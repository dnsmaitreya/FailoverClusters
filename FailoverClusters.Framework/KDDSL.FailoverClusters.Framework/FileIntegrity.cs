using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_FileIntegrity : MiInstanceBase
{
	public struct GetOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_FileIntegrity FileIntegrity { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetOutParameters lhs, GetOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetOutParameters lhs, GetOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RepairOutParameters
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

		public static bool operator ==(RepairOutParameters lhs, RepairOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RepairOutParameters lhs, RepairOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetOutParameters
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

		public static bool operator ==(SetOutParameters lhs, SetOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetOutParameters lhs, SetOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public bool? Enabled
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["Enabled"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["Enabled"].Value = value;
		}
	}

	public bool? Enforced
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["Enforced"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["Enforced"].Value = value;
		}
	}

	public string FileName
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["FileName"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["FileName"].Value = value;
		}
	}

	public MSFT_FileIntegrity()
	{
	}

	public MSFT_FileIntegrity(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_FileIntegrity GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_FileIntegrity", "root/windows/storage");
		CimInstance instance = session.GetInstance("root/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_FileIntegrity(session, instance);
	}

	public static MSFT_FileIntegrity CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_FileIntegrity", "root/windows/storage");
		return new MSFT_FileIntegrity(session, instance);
	}

	public static IEnumerable<MSFT_FileIntegrity> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_FileIntegrity")
			select new MSFT_FileIntegrity(session, i);
	}

	public static IEnumerable<MSFT_FileIntegrity> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_FileIntegrity";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_FileIntegrity(session, i);
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

	public static GetOutParameters Get(CimSession Session, string FileName = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FileName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileName", FileName, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/windows/storage", "MSFT_FileIntegrity", "Get", cimMethodParametersCollection);
		GetOutParameters result = default(GetOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["FileIntegrity"] != null)
		{
			result.FileIntegrity = new MSFT_FileIntegrity(Session, (CimInstance)cimMethodResult.OutParameters["FileIntegrity"].Value);
		}
		else
		{
			result.FileIntegrity = null;
		}
		if (cimMethodResult.OutParameters["ExtendedStatus"] != null)
		{
			result.ExtendedStatus = new MSFT_StorageExtendedStatus(Session, (CimInstance)cimMethodResult.OutParameters["ExtendedStatus"].Value);
		}
		else
		{
			result.ExtendedStatus = null;
		}
		return result;
	}

	public static RepairOutParameters Repair(CimSession Session, string FileName = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FileName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileName", FileName, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/windows/storage", "MSFT_FileIntegrity", "Repair", cimMethodParametersCollection);
		RepairOutParameters result = default(RepairOutParameters);
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
			result.ExtendedStatus = new MSFT_StorageExtendedStatus(Session, (CimInstance)cimMethodResult.OutParameters["ExtendedStatus"].Value);
		}
		else
		{
			result.ExtendedStatus = null;
		}
		return result;
	}

	public static SetOutParameters Set(CimSession Session, string FileName = null, bool? Enable = null, bool? Enforce = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FileName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileName", FileName, CimType.String, CimFlags.In));
		}
		if (Enable.HasValue)
		{
			bool? flag = Enable;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Enable", flag, CimType.Boolean, CimFlags.In));
		}
		if (Enforce.HasValue)
		{
			bool? flag2 = Enforce;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Enforce", flag2, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/windows/storage", "MSFT_FileIntegrity", "Set", cimMethodParametersCollection);
		SetOutParameters result = default(SetOutParameters);
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
			result.ExtendedStatus = new MSFT_StorageExtendedStatus(Session, (CimInstance)cimMethodResult.OutParameters["ExtendedStatus"].Value);
		}
		else
		{
			result.ExtendedStatus = null;
		}
		return result;
	}
}

