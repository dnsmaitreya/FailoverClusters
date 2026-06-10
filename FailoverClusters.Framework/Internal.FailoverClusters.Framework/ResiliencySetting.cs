using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_ResiliencySetting : MSFT_StorageObject
{
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

	public MSFT_ResiliencySettingMSFT_StoragePoolToResiliencySetting MSFT_StoragePoolToResiliencySetting { get; private set; }

	public string Description => (string)base.Instance.CimInstanceProperties["Description"].Value;

	public ulong? InterleaveDefault => (ulong?)base.Instance.CimInstanceProperties["InterleaveDefault"].Value;

	public ulong? InterleaveMax => (ulong?)base.Instance.CimInstanceProperties["InterleaveMax"].Value;

	public ulong? InterleaveMin => (ulong?)base.Instance.CimInstanceProperties["InterleaveMin"].Value;

	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public ushort? NumberOfColumnsDefault => (ushort?)base.Instance.CimInstanceProperties["NumberOfColumnsDefault"].Value;

	public ushort? NumberOfColumnsMax => (ushort?)base.Instance.CimInstanceProperties["NumberOfColumnsMax"].Value;

	public ushort? NumberOfColumnsMin => (ushort?)base.Instance.CimInstanceProperties["NumberOfColumnsMin"].Value;

	public ushort? NumberOfDataCopiesDefault => (ushort?)base.Instance.CimInstanceProperties["NumberOfDataCopiesDefault"].Value;

	public ushort? NumberOfDataCopiesMax => (ushort?)base.Instance.CimInstanceProperties["NumberOfDataCopiesMax"].Value;

	public ushort? NumberOfDataCopiesMin => (ushort?)base.Instance.CimInstanceProperties["NumberOfDataCopiesMin"].Value;

	public ushort? NumberOfGroupsDefault => (ushort?)base.Instance.CimInstanceProperties["NumberOfGroupsDefault"].Value;

	public ushort? NumberOfGroupsMax => (ushort?)base.Instance.CimInstanceProperties["NumberOfGroupsMax"].Value;

	public ushort? NumberOfGroupsMin => (ushort?)base.Instance.CimInstanceProperties["NumberOfGroupsMin"].Value;

	public ushort? ParityLayout => (ushort?)base.Instance.CimInstanceProperties["ParityLayout"].Value;

	public ushort? PhysicalDiskRedundancyDefault => (ushort?)base.Instance.CimInstanceProperties["PhysicalDiskRedundancyDefault"].Value;

	public ushort? PhysicalDiskRedundancyMax => (ushort?)base.Instance.CimInstanceProperties["PhysicalDiskRedundancyMax"].Value;

	public ushort? PhysicalDiskRedundancyMin => (ushort?)base.Instance.CimInstanceProperties["PhysicalDiskRedundancyMin"].Value;

	public bool? RequestNoSinglePointOfFailure => (bool?)base.Instance.CimInstanceProperties["RequestNoSinglePointOfFailure"].Value;

	public MSFT_ResiliencySetting()
	{
	}

	public MSFT_ResiliencySetting(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StoragePoolToResiliencySetting = new MSFT_ResiliencySettingMSFT_StoragePoolToResiliencySetting(session, instance);
	}

	public static MSFT_ResiliencySetting GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_ResiliencySetting", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_ResiliencySetting(session, instance);
	}

	public static MSFT_ResiliencySetting CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_ResiliencySetting", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_ResiliencySetting(session, cimInstance);
	}

	public new static IEnumerable<MSFT_ResiliencySetting> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_ResiliencySetting")
			select new MSFT_ResiliencySetting(session, i);
	}

	public new static IEnumerable<MSFT_ResiliencySetting> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_ResiliencySetting";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_ResiliencySetting(session, i);
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

	public SetDefaultsOutParameters SetDefaults(ushort? NumberOfDataCopiesDefault = null, ushort? PhysicalDiskRedundancyDefault = null, ushort? NumberOfColumnsDefault = null, bool? AutoNumberOfColumns = null, ulong? InterleaveDefault = null, ushort? NumberOfGroupsDefault = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (NumberOfDataCopiesDefault.HasValue)
		{
			ushort? num = NumberOfDataCopiesDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfDataCopiesDefault", num, CimType.UInt16, CimFlags.In));
		}
		if (PhysicalDiskRedundancyDefault.HasValue)
		{
			ushort? num2 = PhysicalDiskRedundancyDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDiskRedundancyDefault", num2, CimType.UInt16, CimFlags.In));
		}
		if (NumberOfColumnsDefault.HasValue)
		{
			ushort? num3 = NumberOfColumnsDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfColumnsDefault", num3, CimType.UInt16, CimFlags.In));
		}
		if (AutoNumberOfColumns.HasValue)
		{
			bool? flag = AutoNumberOfColumns;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AutoNumberOfColumns", flag, CimType.Boolean, CimFlags.In));
		}
		if (InterleaveDefault.HasValue)
		{
			ulong? num4 = InterleaveDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InterleaveDefault", num4, CimType.UInt64, CimFlags.In));
		}
		if (NumberOfGroupsDefault.HasValue)
		{
			ushort? num5 = NumberOfGroupsDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfGroupsDefault", num5, CimType.UInt16, CimFlags.In));
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
}

