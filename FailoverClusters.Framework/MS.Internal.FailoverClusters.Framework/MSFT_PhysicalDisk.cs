using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_PhysicalDisk : MSFT_StorageFaultDomain
{
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

	public struct ResetOutParameters
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

		public static bool operator ==(ResetOutParameters lhs, ResetOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ResetOutParameters lhs, ResetOutParameters rhs)
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

	public struct SetDescriptionOutParameters
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

		public static bool operator ==(SetDescriptionOutParameters lhs, SetDescriptionOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetDescriptionOutParameters lhs, SetDescriptionOutParameters rhs)
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

	public struct IsDeviceCacheEnabledOutParameters
	{
		public uint? ReturnValue { get; set; }

		public bool? IsDeviceCacheEnabled { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(IsDeviceCacheEnabledOutParameters lhs, IsDeviceCacheEnabledOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(IsDeviceCacheEnabledOutParameters lhs, IsDeviceCacheEnabledOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct IsPowerProtectedOutParameters
	{
		public uint? ReturnValue { get; set; }

		public bool? IsPowerProtected { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(IsPowerProtectedOutParameters lhs, IsPowerProtectedOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(IsPowerProtectedOutParameters lhs, IsPowerProtectedOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetFirmwareInformationOutParameters
	{
		public uint? ReturnValue { get; set; }

		public bool? SupportsUpdate { get; set; }

		public ushort? NumberOfSlots { get; set; }

		public ushort? ActiveSlotNumber { get; set; }

		public ushort[] SlotNumber { get; set; }

		public bool[] IsSlotWritable { get; set; }

		public string[] FirmwareVersionInSlot { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetFirmwareInformationOutParameters lhs, GetFirmwareInformationOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetFirmwareInformationOutParameters lhs, GetFirmwareInformationOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct UpdateFirmwareOutParameters
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

		public static bool operator ==(UpdateFirmwareOutParameters lhs, UpdateFirmwareOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(UpdateFirmwareOutParameters lhs, UpdateFirmwareOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_PhysicalDiskMSFT_PhysicalDiskToStorageReliabilityCounter MSFT_PhysicalDiskToStorageReliabilityCounter { get; private set; }

	public MSFT_PhysicalDiskMSFT_StorageEnclosureToPhysicalDisk MSFT_StorageEnclosureToPhysicalDisk { get; private set; }

	public MSFT_PhysicalDiskMSFT_StorageNodeToPhysicalDisk MSFT_StorageNodeToPhysicalDisk { get; private set; }

	public MSFT_PhysicalDiskMSFT_StoragePoolToPhysicalDisk MSFT_StoragePoolToPhysicalDisk { get; private set; }

	public MSFT_PhysicalDiskMSFT_StorageSubSystemToPhysicalDisk MSFT_StorageSubSystemToPhysicalDisk { get; private set; }

	public MSFT_PhysicalDiskMSFT_VirtualDiskToPhysicalDisk MSFT_VirtualDiskToPhysicalDisk { get; private set; }

	public ulong? AllocatedSize => (ulong?)base.Instance.CimInstanceProperties["AllocatedSize"].Value;

	public ushort? BusType => (ushort?)base.Instance.CimInstanceProperties["BusType"].Value;

	public ushort[] CannotPoolReason => (ushort[])base.Instance.CimInstanceProperties["CannotPoolReason"].Value;

	public bool? CanPool => (bool?)base.Instance.CimInstanceProperties["CanPool"].Value;

	public string DeviceId => (string)base.Instance.CimInstanceProperties["DeviceId"].Value;

	public ushort? EnclosureNumber => (ushort?)base.Instance.CimInstanceProperties["EnclosureNumber"].Value;

	public string FirmwareVersion => (string)base.Instance.CimInstanceProperties["FirmwareVersion"].Value;

	public bool? IsIndicationEnabled => (bool?)base.Instance.CimInstanceProperties["IsIndicationEnabled"].Value;

	public bool? IsPartial => (bool?)base.Instance.CimInstanceProperties["IsPartial"].Value;

	public ulong? LogicalSectorSize => (ulong?)base.Instance.CimInstanceProperties["LogicalSectorSize"].Value;

	public ushort? MediaType
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["MediaType"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["MediaType"].Value = value;
		}
	}

	public string OtherCannotPoolReasonDescription => (string)base.Instance.CimInstanceProperties["OtherCannotPoolReasonDescription"].Value;

	public string PartNumber => (string)base.Instance.CimInstanceProperties["PartNumber"].Value;

	public ulong? PhysicalSectorSize => (ulong?)base.Instance.CimInstanceProperties["PhysicalSectorSize"].Value;

	public ulong? Size => (ulong?)base.Instance.CimInstanceProperties["Size"].Value;

	public ushort? SlotNumber => (ushort?)base.Instance.CimInstanceProperties["SlotNumber"].Value;

	public string SoftwareVersion => (string)base.Instance.CimInstanceProperties["SoftwareVersion"].Value;

	public uint? SpindleSpeed => (uint?)base.Instance.CimInstanceProperties["SpindleSpeed"].Value;

	public ushort[] SupportedUsages => (ushort[])base.Instance.CimInstanceProperties["SupportedUsages"].Value;

	public ushort? UniqueIdFormat => (ushort?)base.Instance.CimInstanceProperties["UniqueIdFormat"].Value;

	public ushort? Usage => (ushort?)base.Instance.CimInstanceProperties["Usage"].Value;

	public ulong? VirtualDiskFootprint => (ulong?)base.Instance.CimInstanceProperties["VirtualDiskFootprint"].Value;

	public MSFT_PhysicalDisk()
	{
	}

	public MSFT_PhysicalDisk(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_PhysicalDiskToStorageReliabilityCounter = new MSFT_PhysicalDiskMSFT_PhysicalDiskToStorageReliabilityCounter(session, instance);
		MSFT_StorageEnclosureToPhysicalDisk = new MSFT_PhysicalDiskMSFT_StorageEnclosureToPhysicalDisk(session, instance);
		MSFT_StorageNodeToPhysicalDisk = new MSFT_PhysicalDiskMSFT_StorageNodeToPhysicalDisk(session, instance);
		MSFT_StoragePoolToPhysicalDisk = new MSFT_PhysicalDiskMSFT_StoragePoolToPhysicalDisk(session, instance);
		MSFT_StorageSubSystemToPhysicalDisk = new MSFT_PhysicalDiskMSFT_StorageSubSystemToPhysicalDisk(session, instance);
		MSFT_VirtualDiskToPhysicalDisk = new MSFT_PhysicalDiskMSFT_VirtualDiskToPhysicalDisk(session, instance);
	}

	public static MSFT_PhysicalDisk GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_PhysicalDisk", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_PhysicalDisk(session, instance);
	}

	public static MSFT_PhysicalDisk CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_PhysicalDisk", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_PhysicalDisk(session, cimInstance);
	}

	public new static IEnumerable<MSFT_PhysicalDisk> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_PhysicalDisk")
			select new MSFT_PhysicalDisk(session, i);
	}

	public new static IEnumerable<MSFT_PhysicalDisk> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_PhysicalDisk";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_PhysicalDisk(session, i);
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

	public MaintenanceOutParameters Maintenance(bool? EnableIndication = null, bool? EnableMaintenanceMode = null, uint? Timeout = null, bool? IgnoreDetachedVirtualDisks = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (EnableIndication.HasValue)
		{
			bool? flag = EnableIndication;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EnableIndication", flag, CimType.Boolean, CimFlags.In));
		}
		if (EnableMaintenanceMode.HasValue)
		{
			bool? flag2 = EnableMaintenanceMode;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EnableMaintenanceMode", flag2, CimType.Boolean, CimFlags.In));
		}
		if (Timeout.HasValue)
		{
			uint? num = Timeout;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Timeout", num, CimType.UInt32, CimFlags.In));
		}
		if (IgnoreDetachedVirtualDisks.HasValue)
		{
			bool? flag3 = IgnoreDetachedVirtualDisks;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IgnoreDetachedVirtualDisks", flag3, CimType.Boolean, CimFlags.In));
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

	public ResetOutParameters Reset()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Reset", methodParameters);
		ResetOutParameters result = default(ResetOutParameters);
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

	public SetDescriptionOutParameters SetDescription(string Description)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetDescription", cimMethodParametersCollection);
		SetDescriptionOutParameters result = default(SetDescriptionOutParameters);
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

	public SetUsageOutParameters SetUsage(ushort? Usage)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Usage.HasValue)
		{
			ushort? num = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num, CimType.UInt16, CimFlags.In));
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

	public SetAttributesOutParameters SetAttributes(ushort? MediaType)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (MediaType.HasValue)
		{
			ushort? num = MediaType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("MediaType", num, CimType.UInt16, CimFlags.In));
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

	public IsDeviceCacheEnabledOutParameters IsDeviceCacheEnabled()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "IsDeviceCacheEnabled", methodParameters);
		IsDeviceCacheEnabledOutParameters result = default(IsDeviceCacheEnabledOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["IsDeviceCacheEnabled"] != null)
		{
			result.IsDeviceCacheEnabled = (bool?)cimMethodResult.OutParameters["IsDeviceCacheEnabled"].Value;
		}
		else
		{
			result.IsDeviceCacheEnabled = null;
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

	public IsPowerProtectedOutParameters IsPowerProtected()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "IsPowerProtected", methodParameters);
		IsPowerProtectedOutParameters result = default(IsPowerProtectedOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["IsPowerProtected"] != null)
		{
			result.IsPowerProtected = (bool?)cimMethodResult.OutParameters["IsPowerProtected"].Value;
		}
		else
		{
			result.IsPowerProtected = null;
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

	public GetFirmwareInformationOutParameters GetFirmwareInformation()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetFirmwareInformation", methodParameters);
		GetFirmwareInformationOutParameters result = default(GetFirmwareInformationOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SupportsUpdate"] != null)
		{
			result.SupportsUpdate = (bool?)cimMethodResult.OutParameters["SupportsUpdate"].Value;
		}
		else
		{
			result.SupportsUpdate = null;
		}
		if (cimMethodResult.OutParameters["NumberOfSlots"] != null)
		{
			result.NumberOfSlots = (ushort?)cimMethodResult.OutParameters["NumberOfSlots"].Value;
		}
		else
		{
			result.NumberOfSlots = null;
		}
		if (cimMethodResult.OutParameters["ActiveSlotNumber"] != null)
		{
			result.ActiveSlotNumber = (ushort?)cimMethodResult.OutParameters["ActiveSlotNumber"].Value;
		}
		else
		{
			result.ActiveSlotNumber = null;
		}
		if (cimMethodResult.OutParameters["SlotNumber"] != null)
		{
			result.SlotNumber = (ushort[])cimMethodResult.OutParameters["SlotNumber"].Value;
		}
		else
		{
			result.SlotNumber = null;
		}
		if (cimMethodResult.OutParameters["IsSlotWritable"] != null)
		{
			result.IsSlotWritable = (bool[])cimMethodResult.OutParameters["IsSlotWritable"].Value;
		}
		else
		{
			result.IsSlotWritable = null;
		}
		if (cimMethodResult.OutParameters["FirmwareVersionInSlot"] != null)
		{
			result.FirmwareVersionInSlot = (string[])cimMethodResult.OutParameters["FirmwareVersionInSlot"].Value;
		}
		else
		{
			result.FirmwareVersionInSlot = null;
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

	public UpdateFirmwareOutParameters UpdateFirmware(string ImagePath = null, ushort? SlotNumber = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ImagePath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ImagePath", ImagePath, CimType.String, CimFlags.In));
		}
		if (SlotNumber.HasValue)
		{
			ushort? num = SlotNumber;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SlotNumber", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "UpdateFirmware", cimMethodParametersCollection);
		UpdateFirmwareOutParameters result = default(UpdateFirmwareOutParameters);
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

