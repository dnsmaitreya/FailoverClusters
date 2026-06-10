using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_Disk : MSFT_StorageObject
{
	public struct CreatePartitionOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_Partition CreatedPartition { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CreatePartitionOutParameters lhs, CreatePartitionOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreatePartitionOutParameters lhs, CreatePartitionOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct InitializeOutParameters
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

		public static bool operator ==(InitializeOutParameters lhs, InitializeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(InitializeOutParameters lhs, InitializeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct ClearOutParameters
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

		public static bool operator ==(ClearOutParameters lhs, ClearOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ClearOutParameters lhs, ClearOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct ConvertStyleOutParameters
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

		public static bool operator ==(ConvertStyleOutParameters lhs, ConvertStyleOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ConvertStyleOutParameters lhs, ConvertStyleOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct OfflineOutParameters
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

		public static bool operator ==(OfflineOutParameters lhs, OfflineOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(OfflineOutParameters lhs, OfflineOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct OnlineOutParameters
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

		public static bool operator ==(OnlineOutParameters lhs, OnlineOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(OnlineOutParameters lhs, OnlineOutParameters rhs)
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

	public struct RefreshOutParameters
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

		public static bool operator ==(RefreshOutParameters lhs, RefreshOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RefreshOutParameters lhs, RefreshOutParameters rhs)
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

	public struct EnableHighAvailabilityOutParameters
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

		public static bool operator ==(EnableHighAvailabilityOutParameters lhs, EnableHighAvailabilityOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(EnableHighAvailabilityOutParameters lhs, EnableHighAvailabilityOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct DisableHighAvailabilityOutParameters
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

		public static bool operator ==(DisableHighAvailabilityOutParameters lhs, DisableHighAvailabilityOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DisableHighAvailabilityOutParameters lhs, DisableHighAvailabilityOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_DiskMSFT_DiskToPartition MSFT_DiskToPartition { get; private set; }

	public MSFT_DiskMSFT_DiskToStorageReliabilityCounter MSFT_DiskToStorageReliabilityCounter { get; private set; }

	public MSFT_DiskMSFT_StorageNodeToDisk MSFT_StorageNodeToDisk { get; private set; }

	public MSFT_DiskMSFT_StorageSubSystemToDisk MSFT_StorageSubSystemToDisk { get; private set; }

	public MSFT_DiskMSFT_VirtualDiskToDisk MSFT_VirtualDiskToDisk { get; private set; }

	public MSFT_DiskMSFT_iSCSIConnectionToDisk MSFT_iSCSIConnectionToDisk { get; private set; }

	public MSFT_DiskMSFT_iSCSISessionToDisk MSFT_iSCSISessionToDisk { get; private set; }

	public ulong? AllocatedSize => (ulong?)base.Instance.CimInstanceProperties["AllocatedSize"].Value;

	public bool? BootFromDisk => (bool?)base.Instance.CimInstanceProperties["BootFromDisk"].Value;

	public ushort? BusType => (ushort?)base.Instance.CimInstanceProperties["BusType"].Value;

	public string FirmwareVersion => (string)base.Instance.CimInstanceProperties["FirmwareVersion"].Value;

	public string FriendlyName => (string)base.Instance.CimInstanceProperties["FriendlyName"].Value;

	public string Guid => (string)base.Instance.CimInstanceProperties["Guid"].Value;

	public ushort? HealthStatus => (ushort?)base.Instance.CimInstanceProperties["HealthStatus"].Value;

	public bool? IsBoot => (bool?)base.Instance.CimInstanceProperties["IsBoot"].Value;

	public bool? IsClustered => (bool?)base.Instance.CimInstanceProperties["IsClustered"].Value;

	public bool? IsHighlyAvailable => (bool?)base.Instance.CimInstanceProperties["IsHighlyAvailable"].Value;

	public bool? IsOffline => (bool?)base.Instance.CimInstanceProperties["IsOffline"].Value;

	public bool? IsReadOnly => (bool?)base.Instance.CimInstanceProperties["IsReadOnly"].Value;

	public bool? IsScaleOut => (bool?)base.Instance.CimInstanceProperties["IsScaleOut"].Value;

	public bool? IsSystem => (bool?)base.Instance.CimInstanceProperties["IsSystem"].Value;

	public ulong? LargestFreeExtent => (ulong?)base.Instance.CimInstanceProperties["LargestFreeExtent"].Value;

	public string Location => (string)base.Instance.CimInstanceProperties["Location"].Value;

	public uint? LogicalSectorSize => (uint?)base.Instance.CimInstanceProperties["LogicalSectorSize"].Value;

	public string Manufacturer => (string)base.Instance.CimInstanceProperties["Manufacturer"].Value;

	public string Model => (string)base.Instance.CimInstanceProperties["Model"].Value;

	public uint? Number => (uint?)base.Instance.CimInstanceProperties["Number"].Value;

	public uint? NumberOfPartitions => (uint?)base.Instance.CimInstanceProperties["NumberOfPartitions"].Value;

	public ushort? OfflineReason => (ushort?)base.Instance.CimInstanceProperties["OfflineReason"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public ushort? PartitionStyle => (ushort?)base.Instance.CimInstanceProperties["PartitionStyle"].Value;

	public string Path => (string)base.Instance.CimInstanceProperties["Path"].Value;

	public uint? PhysicalSectorSize => (uint?)base.Instance.CimInstanceProperties["PhysicalSectorSize"].Value;

	public ushort? ProvisioningType => (ushort?)base.Instance.CimInstanceProperties["ProvisioningType"].Value;

	public string SerialNumber => (string)base.Instance.CimInstanceProperties["SerialNumber"].Value;

	public uint? Signature => (uint?)base.Instance.CimInstanceProperties["Signature"].Value;

	public ulong? Size => (ulong?)base.Instance.CimInstanceProperties["Size"].Value;

	public ushort? UniqueIdFormat => (ushort?)base.Instance.CimInstanceProperties["UniqueIdFormat"].Value;

	public MSFT_Disk()
	{
	}

	public MSFT_Disk(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_DiskToPartition = new MSFT_DiskMSFT_DiskToPartition(session, instance);
		MSFT_DiskToStorageReliabilityCounter = new MSFT_DiskMSFT_DiskToStorageReliabilityCounter(session, instance);
		MSFT_StorageNodeToDisk = new MSFT_DiskMSFT_StorageNodeToDisk(session, instance);
		MSFT_StorageSubSystemToDisk = new MSFT_DiskMSFT_StorageSubSystemToDisk(session, instance);
		MSFT_VirtualDiskToDisk = new MSFT_DiskMSFT_VirtualDiskToDisk(session, instance);
		MSFT_iSCSIConnectionToDisk = new MSFT_DiskMSFT_iSCSIConnectionToDisk(session, instance);
		MSFT_iSCSISessionToDisk = new MSFT_DiskMSFT_iSCSISessionToDisk(session, instance);
	}

	public static MSFT_Disk GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_Disk", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_Disk(session, instance);
	}

	public static MSFT_Disk CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_Disk", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_Disk(session, cimInstance);
	}

	public new static IEnumerable<MSFT_Disk> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_Disk")
			select new MSFT_Disk(session, i);
	}

	public new static IEnumerable<MSFT_Disk> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_Disk";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_Disk(session, i);
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

	public CreatePartitionOutParameters CreatePartition(ulong? Size = null, bool? UseMaximumSize = null, ulong? Offset = null, uint? Alignment = null, char? DriveLetter = null, bool? AssignDriveLetter = null, ushort? MbrType = null, string GptType = null, bool? IsHidden = null, bool? IsActive = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
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
		if (Offset.HasValue)
		{
			ulong? num2 = Offset;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Offset", num2, CimType.UInt64, CimFlags.In));
		}
		if (Alignment.HasValue)
		{
			uint? num3 = Alignment;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Alignment", num3, CimType.UInt32, CimFlags.In));
		}
		if (DriveLetter.HasValue)
		{
			char? c = DriveLetter;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DriveLetter", c, CimType.Char16, CimFlags.In));
		}
		if (AssignDriveLetter.HasValue)
		{
			bool? flag2 = AssignDriveLetter;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AssignDriveLetter", flag2, CimType.Boolean, CimFlags.In));
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
		if (IsHidden.HasValue)
		{
			bool? flag3 = IsHidden;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsHidden", flag3, CimType.Boolean, CimFlags.In));
		}
		if (IsActive.HasValue)
		{
			bool? flag4 = IsActive;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsActive", flag4, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreatePartition", cimMethodParametersCollection);
		CreatePartitionOutParameters result = default(CreatePartitionOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedPartition"] != null)
		{
			result.CreatedPartition = new MSFT_Partition(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedPartition"].Value);
		}
		else
		{
			result.CreatedPartition = null;
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

	public InitializeOutParameters Initialize(ushort? PartitionStyle = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (PartitionStyle.HasValue)
		{
			ushort? num = PartitionStyle;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PartitionStyle", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Initialize", cimMethodParametersCollection);
		InitializeOutParameters result = default(InitializeOutParameters);
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

	public ClearOutParameters Clear(bool? RemoveData = null, bool? RemoveOEM = null, bool? ZeroOutEntireDisk = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (RemoveData.HasValue)
		{
			bool? flag = RemoveData;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RemoveData", flag, CimType.Boolean, CimFlags.In));
		}
		if (RemoveOEM.HasValue)
		{
			bool? flag2 = RemoveOEM;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RemoveOEM", flag2, CimType.Boolean, CimFlags.In));
		}
		if (ZeroOutEntireDisk.HasValue)
		{
			bool? flag3 = ZeroOutEntireDisk;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ZeroOutEntireDisk", flag3, CimType.Boolean, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag4 = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag4, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Clear", cimMethodParametersCollection);
		ClearOutParameters result = default(ClearOutParameters);
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

	public ConvertStyleOutParameters ConvertStyle(ushort? PartitionStyle)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (PartitionStyle.HasValue)
		{
			ushort? num = PartitionStyle;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PartitionStyle", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "ConvertStyle", cimMethodParametersCollection);
		ConvertStyleOutParameters result = default(ConvertStyleOutParameters);
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

	public OfflineOutParameters Offline()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Offline", methodParameters);
		OfflineOutParameters result = default(OfflineOutParameters);
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

	public OnlineOutParameters Online()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Online", methodParameters);
		OnlineOutParameters result = default(OnlineOutParameters);
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

	public SetAttributesOutParameters SetAttributes(bool? IsReadOnly = null, uint? Signature = null, string Guid = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (IsReadOnly.HasValue)
		{
			bool? flag = IsReadOnly;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsReadOnly", flag, CimType.Boolean, CimFlags.In));
		}
		if (Signature.HasValue)
		{
			uint? num = Signature;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Signature", num, CimType.UInt32, CimFlags.In));
		}
		if (Guid != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Guid", Guid, CimType.String, CimFlags.In));
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

	public RefreshOutParameters _Refresh()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Refresh", methodParameters);
		RefreshOutParameters result = default(RefreshOutParameters);
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

	public CreateVolumeOutParameters CreateVolume(string FriendlyName, ushort? FileSystem = null, string AccessPath = null, uint? AllocationUnitSize = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (FileSystem.HasValue)
		{
			ushort? num = FileSystem;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileSystem", num, CimType.UInt16, CimFlags.In));
		}
		if (AccessPath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccessPath", AccessPath, CimType.String, CimFlags.In));
		}
		if (AllocationUnitSize.HasValue)
		{
			uint? num2 = AllocationUnitSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AllocationUnitSize", num2, CimType.UInt32, CimFlags.In));
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

	public EnableHighAvailabilityOutParameters EnableHighAvailability(bool? ScaleOut = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ScaleOut.HasValue)
		{
			bool? flag = ScaleOut;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ScaleOut", flag, CimType.Boolean, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag2 = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag2, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "EnableHighAvailability", cimMethodParametersCollection);
		EnableHighAvailabilityOutParameters result = default(EnableHighAvailabilityOutParameters);
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

	public DisableHighAvailabilityOutParameters DisableHighAvailability(bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "DisableHighAvailability", cimMethodParametersCollection);
		DisableHighAvailabilityOutParameters result = default(DisableHighAvailabilityOutParameters);
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
}

