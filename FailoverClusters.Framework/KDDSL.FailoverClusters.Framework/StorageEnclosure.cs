using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageEnclosure : MSFT_StorageFaultDomain
{
	public struct IdentifyElementOutParameters
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

		public static bool operator ==(IdentifyElementOutParameters lhs, IdentifyElementOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(IdentifyElementOutParameters lhs, IdentifyElementOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetVendorDataOutParameters
	{
		public uint? ReturnValue { get; set; }

		public string VendorData { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetVendorDataOutParameters lhs, GetVendorDataOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetVendorDataOutParameters lhs, GetVendorDataOutParameters rhs)
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

	public MSFT_StorageEnclosureMSFT_StorageEnclosureToPhysicalDisk MSFT_StorageEnclosureToPhysicalDisk { get; private set; }

	public MSFT_StorageEnclosureMSFT_StorageNodeToStorageEnclosure MSFT_StorageNodeToStorageEnclosure { get; private set; }

	public MSFT_StorageEnclosureMSFT_StorageSubSystemToStorageEnclosure MSFT_StorageSubSystemToStorageEnclosure { get; private set; }

	public ushort[] CurrentSensorOperationalStatus => (ushort[])base.Instance.CimInstanceProperties["CurrentSensorOperationalStatus"].Value;

	public string DeviceId => (string)base.Instance.CimInstanceProperties["DeviceId"].Value;

	public ushort[] FanOperationalStatus => (ushort[])base.Instance.CimInstanceProperties["FanOperationalStatus"].Value;

	public string FirmwareVersion => (string)base.Instance.CimInstanceProperties["FirmwareVersion"].Value;

	public ushort[] IOControllerOperationalStatus => (ushort[])base.Instance.CimInstanceProperties["IOControllerOperationalStatus"].Value;

	public uint? NumberOfSlots => (uint?)base.Instance.CimInstanceProperties["NumberOfSlots"].Value;

	public ushort[] PowerSupplyOperationalStatus => (ushort[])base.Instance.CimInstanceProperties["PowerSupplyOperationalStatus"].Value;

	public ushort[] TemperatureSensorOperationalStatus => (ushort[])base.Instance.CimInstanceProperties["TemperatureSensorOperationalStatus"].Value;

	public ushort[] VoltageSensorOperationalStatus => (ushort[])base.Instance.CimInstanceProperties["VoltageSensorOperationalStatus"].Value;

	public MSFT_StorageEnclosure()
	{
	}

	public MSFT_StorageEnclosure(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageEnclosureToPhysicalDisk = new MSFT_StorageEnclosureMSFT_StorageEnclosureToPhysicalDisk(session, instance);
		MSFT_StorageNodeToStorageEnclosure = new MSFT_StorageEnclosureMSFT_StorageNodeToStorageEnclosure(session, instance);
		MSFT_StorageSubSystemToStorageEnclosure = new MSFT_StorageEnclosureMSFT_StorageSubSystemToStorageEnclosure(session, instance);
	}

	public static MSFT_StorageEnclosure GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageEnclosure", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageEnclosure(session, instance);
	}

	public static MSFT_StorageEnclosure CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageEnclosure", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_StorageEnclosure(session, cimInstance);
	}

	public new static IEnumerable<MSFT_StorageEnclosure> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_StorageEnclosure")
			select new MSFT_StorageEnclosure(session, i);
	}

	public new static IEnumerable<MSFT_StorageEnclosure> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageEnclosure";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_StorageEnclosure(session, i);
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

	public IdentifyElementOutParameters IdentifyElement(bool? Enable = null, uint[] SlotNumbers = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Enable.HasValue)
		{
			bool? flag = Enable;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Enable", flag, CimType.Boolean, CimFlags.In));
		}
		if (SlotNumbers != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SlotNumbers", SlotNumbers, CimType.UInt32Array, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "IdentifyElement", cimMethodParametersCollection);
		IdentifyElementOutParameters result = default(IdentifyElementOutParameters);
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

	public GetVendorDataOutParameters GetVendorData(ushort? PageNumber)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (PageNumber.HasValue)
		{
			ushort? num = PageNumber;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PageNumber", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetVendorData", cimMethodParametersCollection);
		GetVendorDataOutParameters result = default(GetVendorDataOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["VendorData"] != null)
		{
			result.VendorData = (string)cimMethodResult.OutParameters["VendorData"].Value;
		}
		else
		{
			result.VendorData = null;
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

	public MaintenanceOutParameters Maintenance(bool? EnableMaintenanceMode = null, uint? Timeout = null, string Model = null, string Manufacturer = null, bool? IgnoreDetachedVirtualDisks = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (EnableMaintenanceMode.HasValue)
		{
			bool? flag = EnableMaintenanceMode;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EnableMaintenanceMode", flag, CimType.Boolean, CimFlags.In));
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
		if (IgnoreDetachedVirtualDisks.HasValue)
		{
			bool? flag2 = IgnoreDetachedVirtualDisks;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IgnoreDetachedVirtualDisks", flag2, CimType.Boolean, CimFlags.In));
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

