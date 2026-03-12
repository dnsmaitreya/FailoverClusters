using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageHealth : MSFT_StorageObject
{
	public struct GetSettingOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageHealthSetting[] StorageHealthSetting { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSettingOutParameters lhs, GetSettingOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSettingOutParameters lhs, GetSettingOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetSettingOutParameters
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

		public static bool operator ==(SetSettingOutParameters lhs, SetSettingOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetSettingOutParameters lhs, SetSettingOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RemoveSettingOutParameters
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

		public static bool operator ==(RemoveSettingOutParameters lhs, RemoveSettingOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RemoveSettingOutParameters lhs, RemoveSettingOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetReportOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageHealthReport[] Reports { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetReportOutParameters lhs, GetReportOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetReportOutParameters lhs, GetReportOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct MaintenanceOutParameters
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

		public static bool operator ==(MaintenanceOutParameters lhs, MaintenanceOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MaintenanceOutParameters lhs, MaintenanceOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_StorageHealthMSFT_StorageSubSystemToStorageHealth MSFT_StorageSubSystemToStorageHealth { get; private set; }

	public MSFT_StorageHealth()
	{
	}

	public MSFT_StorageHealth(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageSubSystemToStorageHealth = new MSFT_StorageHealthMSFT_StorageSubSystemToStorageHealth(session, instance);
	}

	public static MSFT_StorageHealth GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageHealth", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageHealth(session, instance);
	}

	public static MSFT_StorageHealth CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageHealth", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_StorageHealth(session, cimInstance);
	}

	public new static IEnumerable<MSFT_StorageHealth> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageHealth")
			select new MSFT_StorageHealth(session, i);
	}

	public new static IEnumerable<MSFT_StorageHealth> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageHealth";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageHealth(session, i);
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

	public GetSettingOutParameters GetSetting(string Name = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Name != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Name", Name, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSetting", cimMethodParametersCollection);
		GetSettingOutParameters result = default(GetSettingOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["StorageHealthSetting"] != null)
		{
			result.StorageHealthSetting = ((cimMethodResult.OutParameters["StorageHealthSetting"].Value == null) ? null : ((IEnumerable<CimInstance>)cimMethodResult.OutParameters["StorageHealthSetting"].Value).Select((CimInstance i) => new MSFT_StorageHealthSetting(base.Session, i)).ToArray());
		}
		else
		{
			result.StorageHealthSetting = null;
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

	public SetSettingOutParameters SetSetting(string Name, string Value)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Name != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Name", Name, CimType.String, CimFlags.In));
		}
		if (Value != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Value", Value, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetSetting", cimMethodParametersCollection);
		SetSettingOutParameters result = default(SetSettingOutParameters);
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

	public RemoveSettingOutParameters RemoveSetting(string Name)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Name != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Name", Name, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RemoveSetting", cimMethodParametersCollection);
		RemoveSettingOutParameters result = default(RemoveSettingOutParameters);
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

	public GetReportOutParameters GetReport(MSFT_StorageObject TargetObject, uint? Count = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (TargetObject != null)
		{
			CimInstance value = TargetObject?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetObject", value, CimType.Instance, CimFlags.In));
		}
		if (Count.HasValue)
		{
			uint? num = Count;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Count", num, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetReport", cimMethodParametersCollection);
		GetReportOutParameters result = default(GetReportOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["Reports"] != null)
		{
			result.Reports = ((cimMethodResult.OutParameters["Reports"].Value == null) ? null : ((IEnumerable<CimInstance>)cimMethodResult.OutParameters["Reports"].Value).Select((CimInstance i) => new MSFT_StorageHealthReport(base.Session, i)).ToArray());
		}
		else
		{
			result.Reports = null;
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

	public MaintenanceOutParameters Maintenance(MSFT_StorageFaultDomain TargetObject, bool? EnableMaintenanceMode, bool? IgnoreDetachedVirtualDisks = null, uint? Timeout = null, string Model = null, string Manufacturer = null, ushort? ValidationFlags = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (TargetObject != null)
		{
			CimInstance value = TargetObject?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetObject", value, CimType.Instance, CimFlags.In));
		}
		if (EnableMaintenanceMode.HasValue)
		{
			bool? flag = EnableMaintenanceMode;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EnableMaintenanceMode", flag, CimType.Boolean, CimFlags.In));
		}
		if (IgnoreDetachedVirtualDisks.HasValue)
		{
			bool? flag2 = IgnoreDetachedVirtualDisks;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IgnoreDetachedVirtualDisks", flag2, CimType.Boolean, CimFlags.In));
		}
		if (Timeout.HasValue)
		{
			uint? num = Timeout;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Timeout", num, CimType.UInt32, CimFlags.In));
		}
		if (Model != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Model", Model, CimType.String, CimFlags.In));
		}
		if (Manufacturer != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Manufacturer", Manufacturer, CimType.String, CimFlags.In));
		}
		if (ValidationFlags.HasValue)
		{
			ushort? num2 = ValidationFlags;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ValidationFlags", num2, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Maintenance", cimMethodParametersCollection);
		MaintenanceOutParameters result = default(MaintenanceOutParameters);
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
