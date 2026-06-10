using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_SBLTargetCacheConfiguration : MiInstanceBase
{
	public struct NotifyDiskOutParameters
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

		public static bool operator ==(NotifyDiskOutParameters lhs, NotifyDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(NotifyDiskOutParameters lhs, NotifyDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct NotifyEnclosureOutParameters
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

		public static bool operator ==(NotifyEnclosureOutParameters lhs, NotifyEnclosureOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(NotifyEnclosureOutParameters lhs, NotifyEnclosureOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct NotifyDiskStateChangeOutParameters
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

		public static bool operator ==(NotifyDiskStateChangeOutParameters lhs, NotifyDiskStateChangeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(NotifyDiskStateChangeOutParameters lhs, NotifyDiskStateChangeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetDiskUsageOutParameters
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

		public static bool operator ==(SetDiskUsageOutParameters lhs, SetDiskUsageOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetDiskUsageOutParameters lhs, SetDiskUsageOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct StartOptimizeOutParameters
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

		public static bool operator ==(StartOptimizeOutParameters lhs, StartOptimizeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(StartOptimizeOutParameters lhs, StartOptimizeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CheckSystemSupportsCacheStateOutParameters
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

		public static bool operator ==(CheckSystemSupportsCacheStateOutParameters lhs, CheckSystemSupportsCacheStateOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CheckSystemSupportsCacheStateOutParameters lhs, CheckSystemSupportsCacheStateOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CheckDiskSupportsCacheStateOutParameters
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

		public static bool operator ==(CheckDiskSupportsCacheStateOutParameters lhs, CheckDiskSupportsCacheStateOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CheckDiskSupportsCacheStateOutParameters lhs, CheckDiskSupportsCacheStateOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CheckAllDisksSupportCacheOutParameters
	{
		public uint? ReturnValue { get; set; }

		public string[] DiskGuids { get; set; }

		public uint[] DiskNumbers { get; set; }

		public uint[] SupportStatuses { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CheckAllDisksSupportCacheOutParameters lhs, CheckAllDisksSupportCacheOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CheckAllDisksSupportCacheOutParameters lhs, CheckAllDisksSupportCacheOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetDiskWriteCacheStateOutParameters
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

		public static bool operator ==(SetDiskWriteCacheStateOutParameters lhs, SetDiskWriteCacheStateOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetDiskWriteCacheStateOutParameters lhs, SetDiskWriteCacheStateOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct QueryBoundDevicesOutParameters
	{
		public uint? ReturnValue { get; set; }

		public string[] BoundDiskGuids { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(QueryBoundDevicesOutParameters lhs, QueryBoundDevicesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(QueryBoundDevicesOutParameters lhs, QueryBoundDevicesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetDiskCacheModeOutParameters
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

		public static bool operator ==(SetDiskCacheModeOutParameters lhs, SetDiskCacheModeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetDiskCacheModeOutParameters lhs, SetDiskCacheModeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetDiskCacheHintOutParameters
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

		public static bool operator ==(SetDiskCacheHintOutParameters lhs, SetDiskCacheHintOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetDiskCacheHintOutParameters lhs, SetDiskCacheHintOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public ulong? CacheBehavior => (ulong?)base.Instance.CimInstanceProperties["CacheBehavior"].Value;

	public uint? CachePageSizeinKB => (uint?)base.Instance.CimInstanceProperties["CachePageSizeinKB"].Value;

	public uint? CurrentState => (uint?)base.Instance.CimInstanceProperties["CurrentState"].Value;

	public ulong? CurrentStateProgress => (ulong?)base.Instance.CimInstanceProperties["CurrentStateProgress"].Value;

	public ulong? CurrentStateProgressMax => (ulong?)base.Instance.CimInstanceProperties["CurrentStateProgressMax"].Value;

	public uint? DesiredState => (uint?)base.Instance.CimInstanceProperties["DesiredState"].Value;

	public ulong? FlashMetadataReserveBytes => (ulong?)base.Instance.CimInstanceProperties["FlashMetadataReserveBytes"].Value;

	public uint? FlashReservePercent => (uint?)base.Instance.CimInstanceProperties["FlashReservePercent"].Value;

	public string Identifier => (string)base.Instance.CimInstanceProperties["Identifier"].Value;

	public ulong? ProvisioningStage => (ulong?)base.Instance.CimInstanceProperties["ProvisioningStage"].Value;

	public ulong? ProvisioningStageMax => (ulong?)base.Instance.CimInstanceProperties["ProvisioningStageMax"].Value;

	public bool? SpacesDirectEnabled => (bool?)base.Instance.CimInstanceProperties["SpacesDirectEnabled"].Value;

	public MSFT_SBLTargetCacheConfiguration()
	{
	}

	public MSFT_SBLTargetCacheConfiguration(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_SBLTargetCacheConfiguration GetInstance(CimSession session, string Identifier)
	{
		CimInstance cimInstance = new CimInstance("MSFT_SBLTargetCacheConfiguration", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("Identifier", Identifier, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_SBLTargetCacheConfiguration(session, instance);
	}

	public static MSFT_SBLTargetCacheConfiguration CreateReference(CimSession session, string Identifier)
	{
		CimInstance cimInstance = new CimInstance("MSFT_SBLTargetCacheConfiguration", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("Identifier", Identifier, CimFlags.Key));
		return new MSFT_SBLTargetCacheConfiguration(session, cimInstance);
	}

	public static IEnumerable<MSFT_SBLTargetCacheConfiguration> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_SBLTargetCacheConfiguration")
			select new MSFT_SBLTargetCacheConfiguration(session, i);
	}

	public static IEnumerable<MSFT_SBLTargetCacheConfiguration> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_SBLTargetCacheConfiguration";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_SBLTargetCacheConfiguration(session, i);
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

	public NotifyDiskOutParameters NotifyDisk(string DiskGuid = null, string PoolId = null, string Name = null, string Description = null, string Manufacturer = null, string ProductId = null, string Serial = null, uint? SlotNumber = null, string EnclosureId = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DiskGuid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskGuid", DiskGuid, CimType.String, CimFlags.In));
		}
		if (PoolId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PoolId", PoolId, CimType.String, CimFlags.In));
		}
		if (Name != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Name", Name, CimType.String, CimFlags.In));
		}
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		if (Manufacturer != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Manufacturer", Manufacturer, CimType.String, CimFlags.In));
		}
		if (ProductId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ProductId", ProductId, CimType.String, CimFlags.In));
		}
		if (Serial != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Serial", Serial, CimType.String, CimFlags.In));
		}
		if (SlotNumber.HasValue)
		{
			uint? num = SlotNumber;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SlotNumber", num, CimType.UInt32, CimFlags.In));
		}
		if (EnclosureId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EnclosureId", EnclosureId, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "NotifyDisk", cimMethodParametersCollection);
		NotifyDiskOutParameters result = default(NotifyDiskOutParameters);
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

	public NotifyEnclosureOutParameters NotifyEnclosure(string EnclosureGuid = null, string Name = null, string Description = null, string Manufacturer = null, string ProductId = null, string Serial = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (EnclosureGuid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EnclosureGuid", EnclosureGuid, CimType.String, CimFlags.In));
		}
		if (Name != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Name", Name, CimType.String, CimFlags.In));
		}
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		if (Manufacturer != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Manufacturer", Manufacturer, CimType.String, CimFlags.In));
		}
		if (ProductId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ProductId", ProductId, CimType.String, CimFlags.In));
		}
		if (Serial != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Serial", Serial, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "NotifyEnclosure", cimMethodParametersCollection);
		NotifyEnclosureOutParameters result = default(NotifyEnclosureOutParameters);
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

	public NotifyDiskStateChangeOutParameters NotifyDiskStateChange(string DiskGuid = null, uint? StateChange = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DiskGuid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskGuid", DiskGuid, CimType.String, CimFlags.In));
		}
		if (StateChange.HasValue)
		{
			uint? num = StateChange;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StateChange", num, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "NotifyDiskStateChange", cimMethodParametersCollection);
		NotifyDiskStateChangeOutParameters result = default(NotifyDiskStateChangeOutParameters);
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

	public SetDiskUsageOutParameters SetDiskUsage(string DiskGuid = null, uint? DiskNumber = null, uint? UseForStorageSpacesDirect = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DiskGuid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskGuid", DiskGuid, CimType.String, CimFlags.In));
		}
		if (DiskNumber.HasValue)
		{
			uint? num = DiskNumber;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskNumber", num, CimType.UInt32, CimFlags.In));
		}
		if (UseForStorageSpacesDirect.HasValue)
		{
			uint? num2 = UseForStorageSpacesDirect;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UseForStorageSpacesDirect", num2, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetDiskUsage", cimMethodParametersCollection);
		SetDiskUsageOutParameters result = default(SetDiskUsageOutParameters);
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

	public StartOptimizeOutParameters StartOptimize(uint? Flags = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Flags.HasValue)
		{
			uint? num = Flags;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Flags", num, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "StartOptimize", cimMethodParametersCollection);
		StartOptimizeOutParameters result = default(StartOptimizeOutParameters);
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

	public CheckSystemSupportsCacheStateOutParameters CheckSystemSupportsCacheState(uint? CacheState = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (CacheState.HasValue)
		{
			uint? num = CacheState;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("CacheState", num, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CheckSystemSupportsCacheState", cimMethodParametersCollection);
		CheckSystemSupportsCacheStateOutParameters result = default(CheckSystemSupportsCacheStateOutParameters);
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

	public CheckDiskSupportsCacheStateOutParameters CheckDiskSupportsCacheState(string DiskGuid = null, uint? CacheState = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DiskGuid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskGuid", DiskGuid, CimType.String, CimFlags.In));
		}
		if (CacheState.HasValue)
		{
			uint? num = CacheState;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("CacheState", num, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CheckDiskSupportsCacheState", cimMethodParametersCollection);
		CheckDiskSupportsCacheStateOutParameters result = default(CheckDiskSupportsCacheStateOutParameters);
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

	public CheckAllDisksSupportCacheOutParameters CheckAllDisksSupportCache(uint? CacheState)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (CacheState.HasValue)
		{
			uint? num = CacheState;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("CacheState", num, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CheckAllDisksSupportCache", cimMethodParametersCollection);
		CheckAllDisksSupportCacheOutParameters result = default(CheckAllDisksSupportCacheOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["DiskGuids"] != null)
		{
			result.DiskGuids = (string[])cimMethodResult.OutParameters["DiskGuids"].Value;
		}
		else
		{
			result.DiskGuids = null;
		}
		if (cimMethodResult.OutParameters["DiskNumbers"] != null)
		{
			result.DiskNumbers = (uint[])cimMethodResult.OutParameters["DiskNumbers"].Value;
		}
		else
		{
			result.DiskNumbers = null;
		}
		if (cimMethodResult.OutParameters["SupportStatuses"] != null)
		{
			result.SupportStatuses = (uint[])cimMethodResult.OutParameters["SupportStatuses"].Value;
		}
		else
		{
			result.SupportStatuses = null;
		}
		return result;
	}

	public SetDiskWriteCacheStateOutParameters SetDiskWriteCacheState(string DiskGuid, bool? EnableWriteCache)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DiskGuid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskGuid", DiskGuid, CimType.String, CimFlags.In));
		}
		if (EnableWriteCache.HasValue)
		{
			bool? flag = EnableWriteCache;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EnableWriteCache", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetDiskWriteCacheState", cimMethodParametersCollection);
		SetDiskWriteCacheStateOutParameters result = default(SetDiskWriteCacheStateOutParameters);
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

	public QueryBoundDevicesOutParameters QueryBoundDevices(string DiskGuid)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DiskGuid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskGuid", DiskGuid, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "QueryBoundDevices", cimMethodParametersCollection);
		QueryBoundDevicesOutParameters result = default(QueryBoundDevicesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["BoundDiskGuids"] != null)
		{
			result.BoundDiskGuids = (string[])cimMethodResult.OutParameters["BoundDiskGuids"].Value;
		}
		else
		{
			result.BoundDiskGuids = null;
		}
		return result;
	}

	public SetDiskCacheModeOutParameters SetDiskCacheMode(string DiskGuid, uint? CacheMode, uint? Flags = null, uint? Originator = null, bool? Force = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DiskGuid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskGuid", DiskGuid, CimType.String, CimFlags.In));
		}
		if (CacheMode.HasValue)
		{
			uint? num = CacheMode;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("CacheMode", num, CimType.UInt32, CimFlags.In));
		}
		if (Flags.HasValue)
		{
			uint? num2 = Flags;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Flags", num2, CimType.UInt32, CimFlags.In));
		}
		if (Originator.HasValue)
		{
			uint? num3 = Originator;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Originator", num3, CimType.UInt32, CimFlags.In));
		}
		if (Force.HasValue)
		{
			bool? flag = Force;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Force", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetDiskCacheMode", cimMethodParametersCollection);
		SetDiskCacheModeOutParameters result = default(SetDiskCacheModeOutParameters);
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

	public SetDiskCacheHintOutParameters SetDiskCacheHint(string DiskGuid, uint? CacheHint, uint? Flags = null, uint? Originator = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DiskGuid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskGuid", DiskGuid, CimType.String, CimFlags.In));
		}
		if (CacheHint.HasValue)
		{
			uint? num = CacheHint;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("CacheHint", num, CimType.UInt32, CimFlags.In));
		}
		if (Flags.HasValue)
		{
			uint? num2 = Flags;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Flags", num2, CimType.UInt32, CimFlags.In));
		}
		if (Originator.HasValue)
		{
			uint? num3 = Originator;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Originator", num3, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetDiskCacheHint", cimMethodParametersCollection);
		SetDiskCacheHintOutParameters result = default(SetDiskCacheHintOutParameters);
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

