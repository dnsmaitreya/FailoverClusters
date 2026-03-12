using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageSetting : MiInstanceBase
{
	public struct GetOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageSetting StorageSetting { get; set; }

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

	public struct SetOutParameters
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

		public static bool operator ==(SetOutParameters lhs, SetOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetOutParameters lhs, SetOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct UpdateHostStorageCacheOutParameters
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

		public static bool operator ==(UpdateHostStorageCacheOutParameters lhs, UpdateHostStorageCacheOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(UpdateHostStorageCacheOutParameters lhs, UpdateHostStorageCacheOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public ushort? NewDiskPolicy => (ushort?)base.Instance.CimInstanceProperties["NewDiskPolicy"].Value;

	public uint? ScrubPolicy => (uint?)base.Instance.CimInstanceProperties["ScrubPolicy"].Value;

	public MSFT_StorageSetting()
	{
	}

	public MSFT_StorageSetting(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_StorageSetting GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("MSFT_StorageSetting", "root/microsoft/windows/storage");
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageSetting(session, instance);
	}

	public static MSFT_StorageSetting CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("MSFT_StorageSetting", "root/microsoft/windows/storage");
		return new MSFT_StorageSetting(session, instance);
	}

	public static IEnumerable<MSFT_StorageSetting> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageSetting")
			select new MSFT_StorageSetting(session, i);
	}

	public static IEnumerable<MSFT_StorageSetting> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageSetting";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageSetting(session, i);
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

	public static GetOutParameters Get(CimSession Session)
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "MSFT_StorageSetting", "Get", methodParameters);
		GetOutParameters result = default(GetOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["StorageSetting"] != null)
		{
			result.StorageSetting = new MSFT_StorageSetting(Session, (CimInstance)cimMethodResult.OutParameters["StorageSetting"].Value);
		}
		else
		{
			result.StorageSetting = null;
		}
		return result;
	}

	public static SetOutParameters Set(CimSession Session, ushort? NewDiskPolicy = null, uint? ScrubPolicy = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (NewDiskPolicy.HasValue)
		{
			ushort? num = NewDiskPolicy;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NewDiskPolicy", num, CimType.UInt16, CimFlags.In));
		}
		if (ScrubPolicy.HasValue)
		{
			uint? num2 = ScrubPolicy;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ScrubPolicy", num2, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "MSFT_StorageSetting", "Set", cimMethodParametersCollection);
		SetOutParameters result = default(SetOutParameters);
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

	public static UpdateHostStorageCacheOutParameters UpdateHostStorageCache(CimSession Session)
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "MSFT_StorageSetting", "UpdateHostStorageCache", methodParameters);
		UpdateHostStorageCacheOutParameters result = default(UpdateHostStorageCacheOutParameters);
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
