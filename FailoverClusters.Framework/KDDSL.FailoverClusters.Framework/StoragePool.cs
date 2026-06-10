using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StoragePool : MSFT_StorageObject
{
	public struct CreateVirtualDiskOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_VirtualDisk CreatedVirtualDisk { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CreateVirtualDiskOutParameters lhs, CreateVirtualDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateVirtualDiskOutParameters lhs, CreateVirtualDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CreateVolumeOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_Volume CreatedVolume { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CreateVolumeOutParameters lhs, CreateVolumeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateVolumeOutParameters lhs, CreateVolumeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CreateStorageTierOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageTier CreatedStorageTier { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CreateStorageTierOutParameters lhs, CreateStorageTierOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateStorageTierOutParameters lhs, CreateStorageTierOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct DeleteObjectOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(DeleteObjectOutParameters lhs, DeleteObjectOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DeleteObjectOutParameters lhs, DeleteObjectOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct UpgradeOutParameters
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

		public static bool operator ==(UpgradeOutParameters lhs, UpgradeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(UpgradeOutParameters lhs, UpgradeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct OptimizeOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(OptimizeOutParameters lhs, OptimizeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(OptimizeOutParameters lhs, OptimizeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct AddPhysicalDiskOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(AddPhysicalDiskOutParameters lhs, AddPhysicalDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(AddPhysicalDiskOutParameters lhs, AddPhysicalDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RemovePhysicalDiskOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(RemovePhysicalDiskOutParameters lhs, RemovePhysicalDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RemovePhysicalDiskOutParameters lhs, RemovePhysicalDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSupportedSizeOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ulong[] SupportedSizes { get; set; }

		public ulong? VirtualDiskSizeMin { get; set; }

		public ulong? VirtualDiskSizeMax { get; set; }

		public ulong? VirtualDiskSizeDivisor { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSupportedSizeOutParameters lhs, GetSupportedSizeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSupportedSizeOutParameters lhs, GetSupportedSizeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSecurityDescriptorOutParameters
	{
		public uint? ReturnValue { get; set; }

		public string SecurityDescriptor { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSecurityDescriptorOutParameters lhs, GetSecurityDescriptorOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSecurityDescriptorOutParameters lhs, GetSecurityDescriptorOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetSecurityDescriptorOutParameters
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

		public static bool operator ==(SetSecurityDescriptorOutParameters lhs, SetSecurityDescriptorOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetSecurityDescriptorOutParameters lhs, SetSecurityDescriptorOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetFriendlyNameOutParameters
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

		public static bool operator ==(SetFriendlyNameOutParameters lhs, SetFriendlyNameOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetFriendlyNameOutParameters lhs, SetFriendlyNameOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetUsageOutParameters
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

		public static bool operator ==(SetUsageOutParameters lhs, SetUsageOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetUsageOutParameters lhs, SetUsageOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetDefaultsOutParameters
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

		public static bool operator ==(SetDefaultsOutParameters lhs, SetDefaultsOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetDefaultsOutParameters lhs, SetDefaultsOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetAttributesOutParameters
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

		public static bool operator ==(SetAttributesOutParameters lhs, SetAttributesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetAttributesOutParameters lhs, SetAttributesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_StoragePoolMSFT_StorageNodeToStoragePool MSFT_StorageNodeToStoragePool { get; private set; }

	public MSFT_StoragePoolMSFT_StoragePoolToPhysicalDisk MSFT_StoragePoolToPhysicalDisk { get; private set; }

	public MSFT_StoragePoolMSFT_StoragePoolToResiliencySetting MSFT_StoragePoolToResiliencySetting { get; private set; }

	public MSFT_StoragePoolMSFT_StoragePoolToStorageTier MSFT_StoragePoolToStorageTier { get; private set; }

	public MSFT_StoragePoolMSFT_StoragePoolToVirtualDisk MSFT_StoragePoolToVirtualDisk { get; private set; }

	public MSFT_StoragePoolMSFT_StoragePoolToVolume MSFT_StoragePoolToVolume { get; private set; }

	public MSFT_StoragePoolMSFT_StorageSubSystemToStoragePool MSFT_StorageSubSystemToStoragePool { get; private set; }

	public ulong? AllocatedSize => (ulong?)base.Instance.CimInstanceProperties["AllocatedSize"].Value;

	public bool? ClearOnDeallocate => (bool?)base.Instance.CimInstanceProperties["ClearOnDeallocate"].Value;

	public bool? EnclosureAwareDefault => (bool?)base.Instance.CimInstanceProperties["EnclosureAwareDefault"].Value;

	public ushort? FaultDomainAwarenessDefault => (ushort?)base.Instance.CimInstanceProperties["FaultDomainAwarenessDefault"].Value;

	public string FriendlyName => (string)base.Instance.CimInstanceProperties["FriendlyName"].Value;

	public ushort? HealthStatus => (ushort?)base.Instance.CimInstanceProperties["HealthStatus"].Value;

	public bool? IsClustered => (bool?)base.Instance.CimInstanceProperties["IsClustered"].Value;

	public bool? IsPowerProtected => (bool?)base.Instance.CimInstanceProperties["IsPowerProtected"].Value;

	public bool? IsPrimordial => (bool?)base.Instance.CimInstanceProperties["IsPrimordial"].Value;

	public bool? IsReadOnly => (bool?)base.Instance.CimInstanceProperties["IsReadOnly"].Value;

	public ulong? LogicalSectorSize => (ulong?)base.Instance.CimInstanceProperties["LogicalSectorSize"].Value;

	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public string OtherOperationalStatusDescription => (string)base.Instance.CimInstanceProperties["OtherOperationalStatusDescription"].Value;

	public string OtherUsageDescription => (string)base.Instance.CimInstanceProperties["OtherUsageDescription"].Value;

	public ulong? PhysicalSectorSize => (ulong?)base.Instance.CimInstanceProperties["PhysicalSectorSize"].Value;

	public ushort? ProvisioningTypeDefault => (ushort?)base.Instance.CimInstanceProperties["ProvisioningTypeDefault"].Value;

	public ushort? ReadOnlyReason => (ushort?)base.Instance.CimInstanceProperties["ReadOnlyReason"].Value;

	public ushort? RepairPolicy => (ushort?)base.Instance.CimInstanceProperties["RepairPolicy"].Value;

	public string ResiliencySettingNameDefault => (string)base.Instance.CimInstanceProperties["ResiliencySettingNameDefault"].Value;

	public ushort? RetireMissingPhysicalDisks => (ushort?)base.Instance.CimInstanceProperties["RetireMissingPhysicalDisks"].Value;

	public ulong? Size => (ulong?)base.Instance.CimInstanceProperties["Size"].Value;

	public ushort[] SupportedProvisioningTypes => (ushort[])base.Instance.CimInstanceProperties["SupportedProvisioningTypes"].Value;

	public bool? SupportsDeduplication => (bool?)base.Instance.CimInstanceProperties["SupportsDeduplication"].Value;

	public ushort[] ThinProvisioningAlertThresholds => (ushort[])base.Instance.CimInstanceProperties["ThinProvisioningAlertThresholds"].Value;

	public ushort? Usage => (ushort?)base.Instance.CimInstanceProperties["Usage"].Value;

	public ushort? Version => (ushort?)base.Instance.CimInstanceProperties["Version"].Value;

	public ulong? WriteCacheSizeDefault
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["WriteCacheSizeDefault"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["WriteCacheSizeDefault"].Value = value;
		}
	}

	public ulong? WriteCacheSizeMax
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["WriteCacheSizeMax"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["WriteCacheSizeMax"].Value = value;
		}
	}

	public ulong? WriteCacheSizeMin
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["WriteCacheSizeMin"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["WriteCacheSizeMin"].Value = value;
		}
	}

	public MSFT_StoragePool()
	{
	}

	public MSFT_StoragePool(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageNodeToStoragePool = new MSFT_StoragePoolMSFT_StorageNodeToStoragePool(session, instance);
		MSFT_StoragePoolToPhysicalDisk = new MSFT_StoragePoolMSFT_StoragePoolToPhysicalDisk(session, instance);
		MSFT_StoragePoolToResiliencySetting = new MSFT_StoragePoolMSFT_StoragePoolToResiliencySetting(session, instance);
		MSFT_StoragePoolToStorageTier = new MSFT_StoragePoolMSFT_StoragePoolToStorageTier(session, instance);
		MSFT_StoragePoolToVirtualDisk = new MSFT_StoragePoolMSFT_StoragePoolToVirtualDisk(session, instance);
		MSFT_StoragePoolToVolume = new MSFT_StoragePoolMSFT_StoragePoolToVolume(session, instance);
		MSFT_StorageSubSystemToStoragePool = new MSFT_StoragePoolMSFT_StorageSubSystemToStoragePool(session, instance);
	}

	public static MSFT_StoragePool GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StoragePool", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StoragePool(session, instance);
	}

	public static MSFT_StoragePool CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StoragePool", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_StoragePool(session, cimInstance);
	}

	public new static IEnumerable<MSFT_StoragePool> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_StoragePool")
			select new MSFT_StoragePool(session, i);
	}

	public new static IEnumerable<MSFT_StoragePool> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StoragePool";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_StoragePool(session, i);
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

	public CreateVirtualDiskOutParameters CreateVirtualDisk(string FriendlyName, ulong? Size = null, bool? UseMaximumSize = null, ushort? ProvisioningType = null, ulong? AllocationUnitSize = null, string ResiliencySettingName = null, ushort? Usage = null, string OtherUsageDescription = null, ushort? NumberOfDataCopies = null, ushort? PhysicalDiskRedundancy = null, ushort? NumberOfColumns = null, bool? AutoNumberOfColumns = null, ulong? Interleave = null, ushort? NumberOfGroups = null, bool? IsEnclosureAware = null, ushort? FaultDomainAwareness = null, MSFT_PhysicalDisk[] PhysicalDisksToUse = null, MSFT_StorageTier[] StorageTiers = null, ulong[] StorageTierSizes = null, ulong? WriteCacheSize = null, bool? AutoWriteCacheSize = null, ulong? ReadCacheSize = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (Size.HasValue)
		{
			ulong? num = Size;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Size", num, CimType.UInt64, CimFlags.In));
		}
		if (UseMaximumSize.HasValue)
		{
			bool? flag = UseMaximumSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UseMaximumSize", flag, CimType.Boolean, CimFlags.In));
		}
		if (ProvisioningType.HasValue)
		{
			ushort? num2 = ProvisioningType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ProvisioningType", num2, CimType.UInt16, CimFlags.In));
		}
		if (AllocationUnitSize.HasValue)
		{
			ulong? num3 = AllocationUnitSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AllocationUnitSize", num3, CimType.UInt64, CimFlags.In));
		}
		if (ResiliencySettingName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ResiliencySettingName", ResiliencySettingName, CimType.String, CimFlags.In));
		}
		if (Usage.HasValue)
		{
			ushort? num4 = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num4, CimType.UInt16, CimFlags.In));
		}
		if (OtherUsageDescription != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("OtherUsageDescription", OtherUsageDescription, CimType.String, CimFlags.In));
		}
		if (NumberOfDataCopies.HasValue)
		{
			ushort? num5 = NumberOfDataCopies;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfDataCopies", num5, CimType.UInt16, CimFlags.In));
		}
		if (PhysicalDiskRedundancy.HasValue)
		{
			ushort? num6 = PhysicalDiskRedundancy;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDiskRedundancy", num6, CimType.UInt16, CimFlags.In));
		}
		if (NumberOfColumns.HasValue)
		{
			ushort? num7 = NumberOfColumns;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfColumns", num7, CimType.UInt16, CimFlags.In));
		}
		if (AutoNumberOfColumns.HasValue)
		{
			bool? flag2 = AutoNumberOfColumns;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AutoNumberOfColumns", flag2, CimType.Boolean, CimFlags.In));
		}
		if (Interleave.HasValue)
		{
			ulong? num8 = Interleave;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Interleave", num8, CimType.UInt64, CimFlags.In));
		}
		if (NumberOfGroups.HasValue)
		{
			ushort? num9 = NumberOfGroups;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfGroups", num9, CimType.UInt16, CimFlags.In));
		}
		if (IsEnclosureAware.HasValue)
		{
			bool? flag3 = IsEnclosureAware;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsEnclosureAware", flag3, CimType.Boolean, CimFlags.In));
		}
		if (FaultDomainAwareness.HasValue)
		{
			ushort? num10 = FaultDomainAwareness;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FaultDomainAwareness", num10, CimType.UInt16, CimFlags.In));
		}
		if (PhysicalDisksToUse != null)
		{
			CimInstance[] value = PhysicalDisksToUse?.Select((MSFT_PhysicalDisk i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDisksToUse", value, CimType.InstanceArray, CimFlags.In));
		}
		if (StorageTiers != null)
		{
			CimInstance[] value2 = StorageTiers?.Select((MSFT_StorageTier i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageTiers", value2, CimType.InstanceArray, CimFlags.In));
		}
		if (StorageTierSizes != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageTierSizes", StorageTierSizes, CimType.UInt64Array, CimFlags.In));
		}
		if (WriteCacheSize.HasValue)
		{
			ulong? num11 = WriteCacheSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("WriteCacheSize", num11, CimType.UInt64, CimFlags.In));
		}
		if (AutoWriteCacheSize.HasValue)
		{
			bool? flag4 = AutoWriteCacheSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AutoWriteCacheSize", flag4, CimType.Boolean, CimFlags.In));
		}
		if (ReadCacheSize.HasValue)
		{
			ulong? num12 = ReadCacheSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReadCacheSize", num12, CimType.UInt64, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag5 = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag5, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateVirtualDisk", cimMethodParametersCollection);
		CreateVirtualDiskOutParameters result = default(CreateVirtualDiskOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedVirtualDisk"] != null)
		{
			result.CreatedVirtualDisk = new MSFT_VirtualDisk(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedVirtualDisk"].Value);
		}
		else
		{
			result.CreatedVirtualDisk = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageJob"] != null)
		{
			result.CreatedStorageJob = new MSFT_StorageJob(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageJob"].Value);
		}
		else
		{
			result.CreatedStorageJob = null;
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

	public CreateVolumeOutParameters CreateVolume(string FriendlyName, ulong? Size = null, MSFT_StorageTier[] StorageTiers = null, ulong[] StorageTierSizes = null, ushort? ProvisioningType = null, string ResiliencySettingName = null, ushort? PhysicalDiskRedundancy = null, ushort? NumberOfColumns = null, ushort? FileSystem = null, string AccessPath = null, uint? AllocationUnitSize = null, ulong? ReadCacheSize = null, MSFT_FileServer FileServer = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (Size.HasValue)
		{
			ulong? num = Size;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Size", num, CimType.UInt64, CimFlags.In));
		}
		if (StorageTiers != null)
		{
			CimInstance[] value = StorageTiers?.Select((MSFT_StorageTier i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageTiers", value, CimType.InstanceArray, CimFlags.In));
		}
		if (StorageTierSizes != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageTierSizes", StorageTierSizes, CimType.UInt64Array, CimFlags.In));
		}
		if (ProvisioningType.HasValue)
		{
			ushort? num2 = ProvisioningType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ProvisioningType", num2, CimType.UInt16, CimFlags.In));
		}
		if (ResiliencySettingName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ResiliencySettingName", ResiliencySettingName, CimType.String, CimFlags.In));
		}
		if (PhysicalDiskRedundancy.HasValue)
		{
			ushort? num3 = PhysicalDiskRedundancy;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDiskRedundancy", num3, CimType.UInt16, CimFlags.In));
		}
		if (NumberOfColumns.HasValue)
		{
			ushort? num4 = NumberOfColumns;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfColumns", num4, CimType.UInt16, CimFlags.In));
		}
		if (FileSystem.HasValue)
		{
			ushort? num5 = FileSystem;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileSystem", num5, CimType.UInt16, CimFlags.In));
		}
		if (AccessPath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccessPath", AccessPath, CimType.String, CimFlags.In));
		}
		if (AllocationUnitSize.HasValue)
		{
			uint? num6 = AllocationUnitSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AllocationUnitSize", num6, CimType.UInt32, CimFlags.In));
		}
		if (ReadCacheSize.HasValue)
		{
			ulong? num7 = ReadCacheSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReadCacheSize", num7, CimType.UInt64, CimFlags.In));
		}
		if (FileServer != null)
		{
			CimInstance value2 = FileServer?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileServer", value2, CimType.Instance, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateVolume", cimMethodParametersCollection);
		CreateVolumeOutParameters result = default(CreateVolumeOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedVolume"] != null)
		{
			result.CreatedVolume = new MSFT_Volume(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedVolume"].Value);
		}
		else
		{
			result.CreatedVolume = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageJob"] != null)
		{
			result.CreatedStorageJob = new MSFT_StorageJob(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageJob"].Value);
		}
		else
		{
			result.CreatedStorageJob = null;
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

	public CreateStorageTierOutParameters CreateStorageTier(string FriendlyName = null, ushort? MediaType = null, string ResiliencySettingName = null, ulong? Interleave = null, ushort? NumberOfGroups = null, ushort? NumberOfColumns = null, ushort? PhysicalDiskRedundancy = null, string Description = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (MediaType.HasValue)
		{
			ushort? num = MediaType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("MediaType", num, CimType.UInt16, CimFlags.In));
		}
		if (ResiliencySettingName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ResiliencySettingName", ResiliencySettingName, CimType.String, CimFlags.In));
		}
		if (Interleave.HasValue)
		{
			ulong? num2 = Interleave;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Interleave", num2, CimType.UInt64, CimFlags.In));
		}
		if (NumberOfGroups.HasValue)
		{
			ushort? num3 = NumberOfGroups;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfGroups", num3, CimType.UInt16, CimFlags.In));
		}
		if (NumberOfColumns.HasValue)
		{
			ushort? num4 = NumberOfColumns;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfColumns", num4, CimType.UInt16, CimFlags.In));
		}
		if (PhysicalDiskRedundancy.HasValue)
		{
			ushort? num5 = PhysicalDiskRedundancy;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDiskRedundancy", num5, CimType.UInt16, CimFlags.In));
		}
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateStorageTier", cimMethodParametersCollection);
		CreateStorageTierOutParameters result = default(CreateStorageTierOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageTier"] != null)
		{
			result.CreatedStorageTier = new MSFT_StorageTier(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageTier"].Value);
		}
		else
		{
			result.CreatedStorageTier = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageJob"] != null)
		{
			result.CreatedStorageJob = new MSFT_StorageJob(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageJob"].Value);
		}
		else
		{
			result.CreatedStorageJob = null;
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

	public DeleteObjectOutParameters DeleteObject(bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "DeleteObject", cimMethodParametersCollection);
		DeleteObjectOutParameters result = default(DeleteObjectOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageJob"] != null)
		{
			result.CreatedStorageJob = new MSFT_StorageJob(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageJob"].Value);
		}
		else
		{
			result.CreatedStorageJob = null;
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

	public UpgradeOutParameters Upgrade()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Upgrade", methodParameters);
		UpgradeOutParameters result = default(UpgradeOutParameters);
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

	public OptimizeOutParameters Optimize(bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Optimize", cimMethodParametersCollection);
		OptimizeOutParameters result = default(OptimizeOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageJob"] != null)
		{
			result.CreatedStorageJob = new MSFT_StorageJob(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageJob"].Value);
		}
		else
		{
			result.CreatedStorageJob = null;
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

	public AddPhysicalDiskOutParameters AddPhysicalDisk(MSFT_PhysicalDisk[] PhysicalDisks, ushort? Usage = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (PhysicalDisks != null)
		{
			CimInstance[] value = PhysicalDisks?.Select((MSFT_PhysicalDisk i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDisks", value, CimType.InstanceArray, CimFlags.In));
		}
		if (Usage.HasValue)
		{
			ushort? num = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num, CimType.UInt16, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "AddPhysicalDisk", cimMethodParametersCollection);
		AddPhysicalDiskOutParameters result = default(AddPhysicalDiskOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageJob"] != null)
		{
			result.CreatedStorageJob = new MSFT_StorageJob(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageJob"].Value);
		}
		else
		{
			result.CreatedStorageJob = null;
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

	public RemovePhysicalDiskOutParameters RemovePhysicalDisk(MSFT_PhysicalDisk[] PhysicalDisks, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (PhysicalDisks != null)
		{
			CimInstance[] value = PhysicalDisks?.Select((MSFT_PhysicalDisk i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDisks", value, CimType.InstanceArray, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RemovePhysicalDisk", cimMethodParametersCollection);
		RemovePhysicalDiskOutParameters result = default(RemovePhysicalDiskOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageJob"] != null)
		{
			result.CreatedStorageJob = new MSFT_StorageJob(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageJob"].Value);
		}
		else
		{
			result.CreatedStorageJob = null;
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

	public GetSupportedSizeOutParameters GetSupportedSize(string ResiliencySettingName, ushort? FaultDomainAwareness = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ResiliencySettingName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ResiliencySettingName", ResiliencySettingName, CimType.String, CimFlags.In));
		}
		if (FaultDomainAwareness.HasValue)
		{
			ushort? num = FaultDomainAwareness;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FaultDomainAwareness", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSupportedSize", cimMethodParametersCollection);
		GetSupportedSizeOutParameters result = default(GetSupportedSizeOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SupportedSizes"] != null)
		{
			result.SupportedSizes = (ulong[])cimMethodResult.OutParameters["SupportedSizes"].Value;
		}
		else
		{
			result.SupportedSizes = null;
		}
		if (cimMethodResult.OutParameters["VirtualDiskSizeMin"] != null)
		{
			result.VirtualDiskSizeMin = (ulong?)cimMethodResult.OutParameters["VirtualDiskSizeMin"].Value;
		}
		else
		{
			result.VirtualDiskSizeMin = null;
		}
		if (cimMethodResult.OutParameters["VirtualDiskSizeMax"] != null)
		{
			result.VirtualDiskSizeMax = (ulong?)cimMethodResult.OutParameters["VirtualDiskSizeMax"].Value;
		}
		else
		{
			result.VirtualDiskSizeMax = null;
		}
		if (cimMethodResult.OutParameters["VirtualDiskSizeDivisor"] != null)
		{
			result.VirtualDiskSizeDivisor = (ulong?)cimMethodResult.OutParameters["VirtualDiskSizeDivisor"].Value;
		}
		else
		{
			result.VirtualDiskSizeDivisor = null;
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

	public GetSecurityDescriptorOutParameters GetSecurityDescriptor()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSecurityDescriptor", methodParameters);
		GetSecurityDescriptorOutParameters result = default(GetSecurityDescriptorOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SecurityDescriptor"] != null)
		{
			result.SecurityDescriptor = (string)cimMethodResult.OutParameters["SecurityDescriptor"].Value;
		}
		else
		{
			result.SecurityDescriptor = null;
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

	public SetSecurityDescriptorOutParameters SetSecurityDescriptor(string SecurityDescriptor)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (SecurityDescriptor != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SecurityDescriptor", SecurityDescriptor, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetSecurityDescriptor", cimMethodParametersCollection);
		SetSecurityDescriptorOutParameters result = default(SetSecurityDescriptorOutParameters);
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

	public SetFriendlyNameOutParameters SetFriendlyName(string FriendlyName)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetFriendlyName", cimMethodParametersCollection);
		SetFriendlyNameOutParameters result = default(SetFriendlyNameOutParameters);
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

	public SetUsageOutParameters SetUsage(ushort? Usage, string OtherUsageDescription = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Usage.HasValue)
		{
			ushort? num = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num, CimType.UInt16, CimFlags.In));
		}
		if (OtherUsageDescription != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("OtherUsageDescription", OtherUsageDescription, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetUsage", cimMethodParametersCollection);
		SetUsageOutParameters result = default(SetUsageOutParameters);
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

	public SetDefaultsOutParameters SetDefaults(ushort? ProvisioningTypeDefault = null, string ResiliencySettingNameDefault = null, bool? EnclosureAwareDefault = null, ushort? FaultDomainAwarenessDefault = null, ulong? WriteCacheSizeDefault = null, bool? AutoWriteCacheSize = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ProvisioningTypeDefault.HasValue)
		{
			ushort? num = ProvisioningTypeDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ProvisioningTypeDefault", num, CimType.UInt16, CimFlags.In));
		}
		if (ResiliencySettingNameDefault != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ResiliencySettingNameDefault", ResiliencySettingNameDefault, CimType.String, CimFlags.In));
		}
		if (EnclosureAwareDefault.HasValue)
		{
			bool? flag = EnclosureAwareDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EnclosureAwareDefault", flag, CimType.Boolean, CimFlags.In));
		}
		if (FaultDomainAwarenessDefault.HasValue)
		{
			ushort? num2 = FaultDomainAwarenessDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FaultDomainAwarenessDefault", num2, CimType.UInt16, CimFlags.In));
		}
		if (WriteCacheSizeDefault.HasValue)
		{
			ulong? num3 = WriteCacheSizeDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("WriteCacheSizeDefault", num3, CimType.UInt64, CimFlags.In));
		}
		if (AutoWriteCacheSize.HasValue)
		{
			bool? flag2 = AutoWriteCacheSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AutoWriteCacheSize", flag2, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetDefaults", cimMethodParametersCollection);
		SetDefaultsOutParameters result = default(SetDefaultsOutParameters);
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

	public SetAttributesOutParameters SetAttributes(bool? IsReadOnly = null, bool? ClearOnDeallocate = null, bool? IsPowerProtected = null, ushort? RepairPolicy = null, ushort? RetireMissingPhysicalDisks = null, ushort[] ThinProvisioningAlertThresholds = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (IsReadOnly.HasValue)
		{
			bool? flag = IsReadOnly;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsReadOnly", flag, CimType.Boolean, CimFlags.In));
		}
		if (ClearOnDeallocate.HasValue)
		{
			bool? flag2 = ClearOnDeallocate;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ClearOnDeallocate", flag2, CimType.Boolean, CimFlags.In));
		}
		if (IsPowerProtected.HasValue)
		{
			bool? flag3 = IsPowerProtected;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsPowerProtected", flag3, CimType.Boolean, CimFlags.In));
		}
		if (RepairPolicy.HasValue)
		{
			ushort? num = RepairPolicy;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RepairPolicy", num, CimType.UInt16, CimFlags.In));
		}
		if (RetireMissingPhysicalDisks.HasValue)
		{
			ushort? num2 = RetireMissingPhysicalDisks;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RetireMissingPhysicalDisks", num2, CimType.UInt16, CimFlags.In));
		}
		if (ThinProvisioningAlertThresholds != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ThinProvisioningAlertThresholds", ThinProvisioningAlertThresholds, CimType.UInt16Array, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetAttributes", cimMethodParametersCollection);
		SetAttributesOutParameters result = default(SetAttributesOutParameters);
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

