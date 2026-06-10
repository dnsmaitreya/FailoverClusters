using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageScaleUnit : MSFT_StorageFaultDomain
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

	public MSFT_StorageScaleUnit()
	{
	}

	public MSFT_StorageScaleUnit(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_StorageScaleUnit GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageScaleUnit", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageScaleUnit(session, instance);
	}

	public static MSFT_StorageScaleUnit CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageScaleUnit", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_StorageScaleUnit(session, cimInstance);
	}

	public new static IEnumerable<MSFT_StorageScaleUnit> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageScaleUnit")
			select new MSFT_StorageScaleUnit(session, i);
	}

	public new static IEnumerable<MSFT_StorageScaleUnit> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageScaleUnit";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageScaleUnit(session, i);
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

