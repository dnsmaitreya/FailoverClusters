using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class PS_StorageCmdlets : MiInstanceBase
{
	public struct SetDiskOutParameters
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

		public static bool operator ==(SetDiskOutParameters lhs, SetDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetDiskOutParameters lhs, SetDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetVolumeOutParameters
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

		public static bool operator ==(SetVolumeOutParameters lhs, SetVolumeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetVolumeOutParameters lhs, SetVolumeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetPartitionOutParameters
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

		public static bool operator ==(SetPartitionOutParameters lhs, SetPartitionOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetPartitionOutParameters lhs, SetPartitionOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetPhysicalDiskOutParameters
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

		public static bool operator ==(SetPhysicalDiskOutParameters lhs, SetPhysicalDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetPhysicalDiskOutParameters lhs, SetPhysicalDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetStoragePoolOutParameters
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

		public static bool operator ==(SetStoragePoolOutParameters lhs, SetStoragePoolOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetStoragePoolOutParameters lhs, SetStoragePoolOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetVirtualDiskOutParameters
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

		public static bool operator ==(SetVirtualDiskOutParameters lhs, SetVirtualDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetVirtualDiskOutParameters lhs, SetVirtualDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetStorageTierOutParameters
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

		public static bool operator ==(SetStorageTierOutParameters lhs, SetStorageTierOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetStorageTierOutParameters lhs, SetStorageTierOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetStorageSubSystemOutParameters
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

		public static bool operator ==(SetStorageSubSystemOutParameters lhs, SetStorageSubSystemOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetStorageSubSystemOutParameters lhs, SetStorageSubSystemOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct AddPhysicalDiskOutParameters
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

	public struct LaunchProviderHostOutParameters
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

		public static bool operator ==(LaunchProviderHostOutParameters lhs, LaunchProviderHostOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(LaunchProviderHostOutParameters lhs, LaunchProviderHostOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetFileShareOutParameters
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

		public static bool operator ==(SetFileShareOutParameters lhs, SetFileShareOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetFileShareOutParameters lhs, SetFileShareOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CreateVolumeOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_Volume[] CreatedVolume { get; set; }

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

	public struct GetStorageReliabilityCounterOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageReliabilityCounter StorageReliabilityCounter { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetStorageReliabilityCounterOutParameters lhs, GetStorageReliabilityCounterOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetStorageReliabilityCounterOutParameters lhs, GetStorageReliabilityCounterOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public PS_StorageCmdlets()
	{
	}

	public PS_StorageCmdlets(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static PS_StorageCmdlets GetInstance(CimSession session)
	{
		CimInstance instanceId = new CimInstance("PS_StorageCmdlets", "root/microsoft/windows/storage");
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", instanceId);
		if (instance == null)
		{
			return null;
		}
		return new PS_StorageCmdlets(session, instance);
	}

	public static PS_StorageCmdlets CreateReference(CimSession session)
	{
		CimInstance instance = new CimInstance("PS_StorageCmdlets", "root/microsoft/windows/storage");
		return new PS_StorageCmdlets(session, instance);
	}

	public static IEnumerable<PS_StorageCmdlets> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "PS_StorageCmdlets")
			select new PS_StorageCmdlets(session, i);
	}

	public static IEnumerable<PS_StorageCmdlets> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM PS_StorageCmdlets";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new PS_StorageCmdlets(session, i);
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

	public static SetDiskOutParameters SetDisk(CimSession Session, MSFT_Disk[] InputObject = null, string UniqueId = null, string Path = null, uint? Number = null, ushort? PartitionStyle = null, bool? IsReadOnly = null, bool? IsOffline = null, uint? Signature = null, string Guid = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InputObject != null)
		{
			CimInstance[] value = InputObject?.Select((MSFT_Disk i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InputObject", value, CimType.InstanceArray, CimFlags.In));
		}
		if (UniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UniqueId", UniqueId, CimType.String, CimFlags.In));
		}
		if (Path != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Path", Path, CimType.String, CimFlags.In));
		}
		if (Number.HasValue)
		{
			uint? num = Number;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Number", num, CimType.UInt32, CimFlags.In));
		}
		if (PartitionStyle.HasValue)
		{
			ushort? num2 = PartitionStyle;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PartitionStyle", num2, CimType.UInt16, CimFlags.In));
		}
		if (IsReadOnly.HasValue)
		{
			bool? flag = IsReadOnly;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsReadOnly", flag, CimType.Boolean, CimFlags.In));
		}
		if (IsOffline.HasValue)
		{
			bool? flag2 = IsOffline;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsOffline", flag2, CimType.Boolean, CimFlags.In));
		}
		if (Signature.HasValue)
		{
			uint? num3 = Signature;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Signature", num3, CimType.UInt32, CimFlags.In));
		}
		if (Guid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Guid", Guid, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "SetDisk", cimMethodParametersCollection);
		SetDiskOutParameters result = default(SetDiskOutParameters);
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

	public static SetVolumeOutParameters SetVolume(CimSession Session, MSFT_Volume[] InputObject = null, string UniqueId = null, string Path = null, string FileSystemLabel = null, char? DriveLetter = null, string NewFileSystemLabel = null, uint? DedupMode = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InputObject != null)
		{
			CimInstance[] value = InputObject?.Select((MSFT_Volume i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InputObject", value, CimType.InstanceArray, CimFlags.In));
		}
		if (UniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UniqueId", UniqueId, CimType.String, CimFlags.In));
		}
		if (Path != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Path", Path, CimType.String, CimFlags.In));
		}
		if (FileSystemLabel != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileSystemLabel", FileSystemLabel, CimType.String, CimFlags.In));
		}
		if (DriveLetter.HasValue)
		{
			char? c = DriveLetter;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DriveLetter", c, CimType.Char16, CimFlags.In));
		}
		if (NewFileSystemLabel != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NewFileSystemLabel", NewFileSystemLabel, CimType.String, CimFlags.In));
		}
		if (DedupMode.HasValue)
		{
			uint? num = DedupMode;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DedupMode", num, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "SetVolume", cimMethodParametersCollection);
		SetVolumeOutParameters result = default(SetVolumeOutParameters);
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

	public static SetPartitionOutParameters SetPartition(CimSession Session, MSFT_Partition[] InputObject = null, string DiskId = null, ulong? Offset = null, uint? DiskNumber = null, uint? PartitionNumber = null, char? DriveLetter = null, char? NewDriveLetter = null, bool? IsOffline = null, bool? IsReadOnly = null, bool? NoDefaultDriveLetter = null, bool? IsActive = null, bool? IsHidden = null, bool? IsDAX = null, ushort? MbrType = null, string GptType = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InputObject != null)
		{
			CimInstance[] value = InputObject?.Select((MSFT_Partition i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InputObject", value, CimType.InstanceArray, CimFlags.In));
		}
		if (DiskId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskId", DiskId, CimType.String, CimFlags.In));
		}
		if (Offset.HasValue)
		{
			ulong? num = Offset;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Offset", num, CimType.UInt64, CimFlags.In));
		}
		if (DiskNumber.HasValue)
		{
			uint? num2 = DiskNumber;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskNumber", num2, CimType.UInt32, CimFlags.In));
		}
		if (PartitionNumber.HasValue)
		{
			uint? num3 = PartitionNumber;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PartitionNumber", num3, CimType.UInt32, CimFlags.In));
		}
		if (DriveLetter.HasValue)
		{
			char? c = DriveLetter;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DriveLetter", c, CimType.Char16, CimFlags.In));
		}
		if (NewDriveLetter.HasValue)
		{
			char? c2 = NewDriveLetter;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NewDriveLetter", c2, CimType.Char16, CimFlags.In));
		}
		if (IsOffline.HasValue)
		{
			bool? flag = IsOffline;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsOffline", flag, CimType.Boolean, CimFlags.In));
		}
		if (IsReadOnly.HasValue)
		{
			bool? flag2 = IsReadOnly;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsReadOnly", flag2, CimType.Boolean, CimFlags.In));
		}
		if (NoDefaultDriveLetter.HasValue)
		{
			bool? flag3 = NoDefaultDriveLetter;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NoDefaultDriveLetter", flag3, CimType.Boolean, CimFlags.In));
		}
		if (IsActive.HasValue)
		{
			bool? flag4 = IsActive;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsActive", flag4, CimType.Boolean, CimFlags.In));
		}
		if (IsHidden.HasValue)
		{
			bool? flag5 = IsHidden;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsHidden", flag5, CimType.Boolean, CimFlags.In));
		}
		if (IsDAX.HasValue)
		{
			bool? flag6 = IsDAX;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsDAX", flag6, CimType.Boolean, CimFlags.In));
		}
		if (MbrType.HasValue)
		{
			ushort? num4 = MbrType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("MbrType", num4, CimType.UInt16, CimFlags.In));
		}
		if (GptType != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("GptType", GptType, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "SetPartition", cimMethodParametersCollection);
		SetPartitionOutParameters result = default(SetPartitionOutParameters);
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

	public static SetPhysicalDiskOutParameters SetPhysicalDisk(CimSession Session, MSFT_PhysicalDisk[] InputObject = null, string UniqueId = null, string FriendlyName = null, string NewFriendlyName = null, string Description = null, ushort? Usage = null, ushort? MediaType = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InputObject != null)
		{
			CimInstance[] value = InputObject?.Select((MSFT_PhysicalDisk i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InputObject", value, CimType.InstanceArray, CimFlags.In));
		}
		if (UniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UniqueId", UniqueId, CimType.String, CimFlags.In));
		}
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (NewFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NewFriendlyName", NewFriendlyName, CimType.String, CimFlags.In));
		}
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		if (Usage.HasValue)
		{
			ushort? num = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num, CimType.UInt16, CimFlags.In));
		}
		if (MediaType.HasValue)
		{
			ushort? num2 = MediaType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("MediaType", num2, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "SetPhysicalDisk", cimMethodParametersCollection);
		SetPhysicalDiskOutParameters result = default(SetPhysicalDiskOutParameters);
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

	public static SetStoragePoolOutParameters SetStoragePool(CimSession Session, MSFT_StoragePool[] InputObject = null, string UniqueId = null, string Name = null, string FriendlyName = null, string NewFriendlyName = null, ushort? Usage = null, string OtherUsageDescription = null, ushort? ProvisioningTypeDefault = null, string ResiliencySettingNameDefault = null, bool? EnclosureAwareDefault = null, ushort? FaultDomainAwarenessDefault = null, ulong? WriteCacheSizeDefault = null, bool? AutoWriteCacheSize = null, bool? IsReadOnly = null, bool? ClearOnDeallocate = null, bool? IsPowerProtected = null, ushort? RepairPolicy = null, ushort? RetireMissingPhysicalDisks = null, ushort[] ThinProvisioningAlertThresholds = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InputObject != null)
		{
			CimInstance[] value = InputObject?.Select((MSFT_StoragePool i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InputObject", value, CimType.InstanceArray, CimFlags.In));
		}
		if (UniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UniqueId", UniqueId, CimType.String, CimFlags.In));
		}
		if (Name != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Name", Name, CimType.String, CimFlags.In));
		}
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (NewFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NewFriendlyName", NewFriendlyName, CimType.String, CimFlags.In));
		}
		if (Usage.HasValue)
		{
			ushort? num = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num, CimType.UInt16, CimFlags.In));
		}
		if (OtherUsageDescription != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("OtherUsageDescription", OtherUsageDescription, CimType.String, CimFlags.In));
		}
		if (ProvisioningTypeDefault.HasValue)
		{
			ushort? num2 = ProvisioningTypeDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ProvisioningTypeDefault", num2, CimType.UInt16, CimFlags.In));
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
			ushort? num3 = FaultDomainAwarenessDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FaultDomainAwarenessDefault", num3, CimType.UInt16, CimFlags.In));
		}
		if (WriteCacheSizeDefault.HasValue)
		{
			ulong? num4 = WriteCacheSizeDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("WriteCacheSizeDefault", num4, CimType.UInt64, CimFlags.In));
		}
		if (AutoWriteCacheSize.HasValue)
		{
			bool? flag2 = AutoWriteCacheSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AutoWriteCacheSize", flag2, CimType.Boolean, CimFlags.In));
		}
		if (IsReadOnly.HasValue)
		{
			bool? flag3 = IsReadOnly;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsReadOnly", flag3, CimType.Boolean, CimFlags.In));
		}
		if (ClearOnDeallocate.HasValue)
		{
			bool? flag4 = ClearOnDeallocate;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ClearOnDeallocate", flag4, CimType.Boolean, CimFlags.In));
		}
		if (IsPowerProtected.HasValue)
		{
			bool? flag5 = IsPowerProtected;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsPowerProtected", flag5, CimType.Boolean, CimFlags.In));
		}
		if (RepairPolicy.HasValue)
		{
			ushort? num5 = RepairPolicy;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RepairPolicy", num5, CimType.UInt16, CimFlags.In));
		}
		if (RetireMissingPhysicalDisks.HasValue)
		{
			ushort? num6 = RetireMissingPhysicalDisks;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RetireMissingPhysicalDisks", num6, CimType.UInt16, CimFlags.In));
		}
		if (ThinProvisioningAlertThresholds != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ThinProvisioningAlertThresholds", ThinProvisioningAlertThresholds, CimType.UInt16Array, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "SetStoragePool", cimMethodParametersCollection);
		SetStoragePoolOutParameters result = default(SetStoragePoolOutParameters);
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

	public static SetVirtualDiskOutParameters SetVirtualDisk(CimSession Session, MSFT_VirtualDisk[] InputObject = null, string UniqueId = null, string Name = null, string FriendlyName = null, string NewFriendlyName = null, ushort? Usage = null, string OtherUsageDescription = null, bool? IsManualAttach = null, string StorageNodeName = null, ushort? Access = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InputObject != null)
		{
			CimInstance[] value = InputObject?.Select((MSFT_VirtualDisk i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InputObject", value, CimType.InstanceArray, CimFlags.In));
		}
		if (UniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UniqueId", UniqueId, CimType.String, CimFlags.In));
		}
		if (Name != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Name", Name, CimType.String, CimFlags.In));
		}
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (NewFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NewFriendlyName", NewFriendlyName, CimType.String, CimFlags.In));
		}
		if (Usage.HasValue)
		{
			ushort? num = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num, CimType.UInt16, CimFlags.In));
		}
		if (OtherUsageDescription != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("OtherUsageDescription", OtherUsageDescription, CimType.String, CimFlags.In));
		}
		if (IsManualAttach.HasValue)
		{
			bool? flag = IsManualAttach;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsManualAttach", flag, CimType.Boolean, CimFlags.In));
		}
		if (StorageNodeName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageNodeName", StorageNodeName, CimType.String, CimFlags.In));
		}
		if (Access.HasValue)
		{
			ushort? num2 = Access;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Access", num2, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "SetVirtualDisk", cimMethodParametersCollection);
		SetVirtualDiskOutParameters result = default(SetVirtualDiskOutParameters);
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

	public static SetStorageTierOutParameters SetStorageTier(CimSession Session, MSFT_StorageTier[] InputObject = null, string UniqueId = null, string FriendlyName = null, string NewFriendlyName = null, ushort? MediaType = null, string Description = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InputObject != null)
		{
			CimInstance[] value = InputObject?.Select((MSFT_StorageTier i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InputObject", value, CimType.InstanceArray, CimFlags.In));
		}
		if (UniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UniqueId", UniqueId, CimType.String, CimFlags.In));
		}
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (NewFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NewFriendlyName", NewFriendlyName, CimType.String, CimFlags.In));
		}
		if (MediaType.HasValue)
		{
			ushort? num = MediaType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("MediaType", num, CimType.UInt16, CimFlags.In));
		}
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "SetStorageTier", cimMethodParametersCollection);
		SetStorageTierOutParameters result = default(SetStorageTierOutParameters);
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

	public static SetStorageSubSystemOutParameters SetStorageSubSystem(CimSession Session, MSFT_StorageSubSystem[] InputObject = null, string UniqueId = null, string Name = null, string FriendlyName = null, string Description = null, bool? AutomaticClusteringEnabled = null, ushort? FaultDomainAwarenessDefault = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InputObject != null)
		{
			CimInstance[] value = InputObject?.Select((MSFT_StorageSubSystem i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InputObject", value, CimType.InstanceArray, CimFlags.In));
		}
		if (UniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UniqueId", UniqueId, CimType.String, CimFlags.In));
		}
		if (Name != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Name", Name, CimType.String, CimFlags.In));
		}
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		if (AutomaticClusteringEnabled.HasValue)
		{
			bool? flag = AutomaticClusteringEnabled;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AutomaticClusteringEnabled", flag, CimType.Boolean, CimFlags.In));
		}
		if (FaultDomainAwarenessDefault.HasValue)
		{
			ushort? num = FaultDomainAwarenessDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FaultDomainAwarenessDefault", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "SetStorageSubSystem", cimMethodParametersCollection);
		SetStorageSubSystemOutParameters result = default(SetStorageSubSystemOutParameters);
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

	public static AddPhysicalDiskOutParameters AddPhysicalDisk(CimSession Session, MSFT_PhysicalDisk[] PhysicalDisks, MSFT_StoragePool StoragePool = null, string StoragePoolUniqueId = null, string StoragePoolName = null, string StoragePoolFriendlyName = null, MSFT_VirtualDisk VirtualDisk = null, string VirtualDiskUniqueId = null, string VirtualDiskName = null, string VirtualDiskFriendlyName = null, ushort? Usage = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (PhysicalDisks != null)
		{
			CimInstance[] value = PhysicalDisks?.Select((MSFT_PhysicalDisk i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDisks", value, CimType.InstanceArray, CimFlags.In));
		}
		if (StoragePool != null)
		{
			CimInstance value2 = StoragePool?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePool", value2, CimType.Instance, CimFlags.In));
		}
		if (StoragePoolUniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePoolUniqueId", StoragePoolUniqueId, CimType.String, CimFlags.In));
		}
		if (StoragePoolName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePoolName", StoragePoolName, CimType.String, CimFlags.In));
		}
		if (StoragePoolFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePoolFriendlyName", StoragePoolFriendlyName, CimType.String, CimFlags.In));
		}
		if (VirtualDisk != null)
		{
			CimInstance value3 = VirtualDisk?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDisk", value3, CimType.Instance, CimFlags.In));
		}
		if (VirtualDiskUniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDiskUniqueId", VirtualDiskUniqueId, CimType.String, CimFlags.In));
		}
		if (VirtualDiskName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDiskName", VirtualDiskName, CimType.String, CimFlags.In));
		}
		if (VirtualDiskFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDiskFriendlyName", VirtualDiskFriendlyName, CimType.String, CimFlags.In));
		}
		if (Usage.HasValue)
		{
			ushort? num = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "AddPhysicalDisk", cimMethodParametersCollection);
		AddPhysicalDiskOutParameters result = default(AddPhysicalDiskOutParameters);
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

	public static RemovePhysicalDiskOutParameters RemovePhysicalDisk(CimSession Session, MSFT_PhysicalDisk[] PhysicalDisks, MSFT_StoragePool StoragePool = null, string StoragePoolUniqueId = null, string StoragePoolName = null, string StoragePoolFriendlyName = null, MSFT_VirtualDisk VirtualDisk = null, string VirtualDiskUniqueId = null, string VirtualDiskName = null, string VirtualDiskFriendlyName = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (PhysicalDisks != null)
		{
			CimInstance[] value = PhysicalDisks?.Select((MSFT_PhysicalDisk i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDisks", value, CimType.InstanceArray, CimFlags.In));
		}
		if (StoragePool != null)
		{
			CimInstance value2 = StoragePool?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePool", value2, CimType.Instance, CimFlags.In));
		}
		if (StoragePoolUniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePoolUniqueId", StoragePoolUniqueId, CimType.String, CimFlags.In));
		}
		if (StoragePoolName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePoolName", StoragePoolName, CimType.String, CimFlags.In));
		}
		if (StoragePoolFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePoolFriendlyName", StoragePoolFriendlyName, CimType.String, CimFlags.In));
		}
		if (VirtualDisk != null)
		{
			CimInstance value3 = VirtualDisk?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDisk", value3, CimType.Instance, CimFlags.In));
		}
		if (VirtualDiskUniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDiskUniqueId", VirtualDiskUniqueId, CimType.String, CimFlags.In));
		}
		if (VirtualDiskName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDiskName", VirtualDiskName, CimType.String, CimFlags.In));
		}
		if (VirtualDiskFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDiskFriendlyName", VirtualDiskFriendlyName, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "RemovePhysicalDisk", cimMethodParametersCollection);
		RemovePhysicalDiskOutParameters result = default(RemovePhysicalDiskOutParameters);
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

	public static LaunchProviderHostOutParameters LaunchProviderHost(CimSession Session)
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "LaunchProviderHost", methodParameters);
		LaunchProviderHostOutParameters result = default(LaunchProviderHostOutParameters);
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

	public static SetFileShareOutParameters SetFileShare(CimSession Session, MSFT_FileShare[] InputObject = null, string UniqueId = null, string Name = null, string Description = null, bool? EncryptData = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InputObject != null)
		{
			CimInstance[] value = InputObject?.Select((MSFT_FileShare i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InputObject", value, CimType.InstanceArray, CimFlags.In));
		}
		if (UniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UniqueId", UniqueId, CimType.String, CimFlags.In));
		}
		if (Name != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Name", Name, CimType.String, CimFlags.In));
		}
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		if (EncryptData.HasValue)
		{
			bool? flag = EncryptData;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EncryptData", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "SetFileShare", cimMethodParametersCollection);
		SetFileShareOutParameters result = default(SetFileShareOutParameters);
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

	public static CreateVolumeOutParameters CreateVolume(CimSession Session, string FriendlyName, MSFT_StoragePool StoragePool = null, string StoragePoolUniqueId = null, string StoragePoolName = null, string StoragePoolFriendlyName = null, MSFT_Disk Disk = null, uint? DiskNumber = null, string DiskPath = null, string DiskUniqueId = null, ulong? Size = null, MSFT_StorageTier[] StorageTiers = null, ulong[] StorageTierSizes = null, ushort? ProvisioningType = null, string ResiliencySettingName = null, ushort? PhysicalDiskRedundancy = null, ushort? NumberOfColumns = null, ushort? FileSystem = null, string AccessPath = null, uint? AllocationUnitSize = null, ulong? ReadCacheSize = null, MSFT_FileServer FileServer = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (StoragePool != null)
		{
			CimInstance value = StoragePool?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePool", value, CimType.Instance, CimFlags.In));
		}
		if (StoragePoolUniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePoolUniqueId", StoragePoolUniqueId, CimType.String, CimFlags.In));
		}
		if (StoragePoolName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePoolName", StoragePoolName, CimType.String, CimFlags.In));
		}
		if (StoragePoolFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StoragePoolFriendlyName", StoragePoolFriendlyName, CimType.String, CimFlags.In));
		}
		if (Disk != null)
		{
			CimInstance value2 = Disk?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Disk", value2, CimType.Instance, CimFlags.In));
		}
		if (DiskNumber.HasValue)
		{
			uint? num = DiskNumber;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskNumber", num, CimType.UInt32, CimFlags.In));
		}
		if (DiskPath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskPath", DiskPath, CimType.String, CimFlags.In));
		}
		if (DiskUniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiskUniqueId", DiskUniqueId, CimType.String, CimFlags.In));
		}
		if (Size.HasValue)
		{
			ulong? num2 = Size;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Size", num2, CimType.UInt64, CimFlags.In));
		}
		if (StorageTiers != null)
		{
			CimInstance[] value3 = StorageTiers?.Select((MSFT_StorageTier i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageTiers", value3, CimType.InstanceArray, CimFlags.In));
		}
		if (StorageTierSizes != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageTierSizes", StorageTierSizes, CimType.UInt64Array, CimFlags.In));
		}
		if (ProvisioningType.HasValue)
		{
			ushort? num3 = ProvisioningType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ProvisioningType", num3, CimType.UInt16, CimFlags.In));
		}
		if (ResiliencySettingName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ResiliencySettingName", ResiliencySettingName, CimType.String, CimFlags.In));
		}
		if (PhysicalDiskRedundancy.HasValue)
		{
			ushort? num4 = PhysicalDiskRedundancy;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDiskRedundancy", num4, CimType.UInt16, CimFlags.In));
		}
		if (NumberOfColumns.HasValue)
		{
			ushort? num5 = NumberOfColumns;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfColumns", num5, CimType.UInt16, CimFlags.In));
		}
		if (FileSystem.HasValue)
		{
			ushort? num6 = FileSystem;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileSystem", num6, CimType.UInt16, CimFlags.In));
		}
		if (AccessPath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccessPath", AccessPath, CimType.String, CimFlags.In));
		}
		if (AllocationUnitSize.HasValue)
		{
			uint? num7 = AllocationUnitSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AllocationUnitSize", num7, CimType.UInt32, CimFlags.In));
		}
		if (ReadCacheSize.HasValue)
		{
			ulong? num8 = ReadCacheSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReadCacheSize", num8, CimType.UInt64, CimFlags.In));
		}
		if (FileServer != null)
		{
			CimInstance value4 = FileServer?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileServer", value4, CimType.Instance, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "CreateVolume", cimMethodParametersCollection);
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
			result.CreatedVolume = ((cimMethodResult.OutParameters["CreatedVolume"].Value == null) ? null : ((IEnumerable<CimInstance>)cimMethodResult.OutParameters["CreatedVolume"].Value).Select((CimInstance i) => new MSFT_Volume(Session, i)).ToArray());
		}
		else
		{
			result.CreatedVolume = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageJob"] != null)
		{
			result.CreatedStorageJob = new MSFT_StorageJob(Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageJob"].Value);
		}
		else
		{
			result.CreatedStorageJob = null;
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

	public static GetStorageReliabilityCounterOutParameters GetStorageReliabilityCounter(CimSession Session, MSFT_PhysicalDisk PhysicalDisk = null, MSFT_Disk Disk = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (PhysicalDisk != null)
		{
			CimInstance value = PhysicalDisk?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDisk", value, CimType.Instance, CimFlags.In));
		}
		if (Disk != null)
		{
			CimInstance value2 = Disk?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Disk", value2, CimType.Instance, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/microsoft/windows/storage", "PS_StorageCmdlets", "GetStorageReliabilityCounter", cimMethodParametersCollection);
		GetStorageReliabilityCounterOutParameters result = default(GetStorageReliabilityCounterOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["StorageReliabilityCounter"] != null)
		{
			result.StorageReliabilityCounter = new MSFT_StorageReliabilityCounter(Session, (CimInstance)cimMethodResult.OutParameters["StorageReliabilityCounter"].Value);
		}
		else
		{
			result.StorageReliabilityCounter = null;
		}
		return result;
	}
}
