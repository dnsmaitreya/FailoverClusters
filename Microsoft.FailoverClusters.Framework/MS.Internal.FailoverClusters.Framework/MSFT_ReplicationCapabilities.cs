using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_ReplicationCapabilities : MSFT_StorageObject
{
	public struct GetSupportedOperationsOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ushort[] SupportedOperations { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSupportedOperationsOutParameters lhs, GetSupportedOperationsOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSupportedOperationsOutParameters lhs, GetSupportedOperationsOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSupportedGroupOperationsOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ushort[] SupportedGroupOperations { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSupportedGroupOperationsOutParameters lhs, GetSupportedGroupOperationsOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSupportedGroupOperationsOutParameters lhs, GetSupportedGroupOperationsOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSupportedFeaturesOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ushort[] Features { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSupportedFeaturesOutParameters lhs, GetSupportedFeaturesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSupportedFeaturesOutParameters lhs, GetSupportedFeaturesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSupportedGroupFeaturesOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ushort[] GroupFeatures { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSupportedGroupFeaturesOutParameters lhs, GetSupportedGroupFeaturesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSupportedGroupFeaturesOutParameters lhs, GetSupportedGroupFeaturesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSupportedCopyStatesOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ushort[] SupportedCopyStates { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSupportedCopyStatesOutParameters lhs, GetSupportedCopyStatesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSupportedCopyStatesOutParameters lhs, GetSupportedCopyStatesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSupportedGroupCopyStatesOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ushort[] SupportedCopyStates { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSupportedGroupCopyStatesOutParameters lhs, GetSupportedGroupCopyStatesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSupportedGroupCopyStatesOutParameters lhs, GetSupportedGroupCopyStatesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetRecoveryPointDataOutParameters
	{
		public uint? ReturnValue { get; set; }

		public uint? DefaultRecoveryPoint { get; set; }

		public uint[] RecoveryPointValues { get; set; }

		public ushort? RecoveryPointIndicator { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetRecoveryPointDataOutParameters lhs, GetRecoveryPointDataOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetRecoveryPointDataOutParameters lhs, GetRecoveryPointDataOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_ReplicationCapabilitiesMSFT_StorageSubSystemToReplicationCapabilities MSFT_StorageSubSystemToReplicationCapabilities { get; private set; }

	public uint? DefaultRecoveryPointObjective
	{
		get
		{
			return (uint?)base.Instance.CimInstanceProperties["DefaultRecoveryPointObjective"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["DefaultRecoveryPointObjective"].Value = value;
		}
	}

	public ushort[] SupportedAsynchronousActions
	{
		get
		{
			return (ushort[])base.Instance.CimInstanceProperties["SupportedAsynchronousActions"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportedAsynchronousActions"].Value = value;
		}
	}

	public ushort[] SupportedLogVolumeFeatures
	{
		get
		{
			return (ushort[])base.Instance.CimInstanceProperties["SupportedLogVolumeFeatures"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportedLogVolumeFeatures"].Value = value;
		}
	}

	public ulong? SupportedMaximumLogSize
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["SupportedMaximumLogSize"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportedMaximumLogSize"].Value = value;
		}
	}

	public ulong? SupportedMinimumLogSize
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["SupportedMinimumLogSize"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportedMinimumLogSize"].Value = value;
		}
	}

	public ushort[] SupportedObjectTypes
	{
		get
		{
			return (ushort[])base.Instance.CimInstanceProperties["SupportedObjectTypes"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportedObjectTypes"].Value = value;
		}
	}

	public ushort[] SupportedReplicatedPartitionFeatures
	{
		get
		{
			return (ushort[])base.Instance.CimInstanceProperties["SupportedReplicatedPartitionFeatures"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportedReplicatedPartitionFeatures"].Value = value;
		}
	}

	public ushort[] SupportedReplicationTypes
	{
		get
		{
			return (ushort[])base.Instance.CimInstanceProperties["SupportedReplicationTypes"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportedReplicationTypes"].Value = value;
		}
	}

	public ushort[] SupportedSynchronousActions
	{
		get
		{
			return (ushort[])base.Instance.CimInstanceProperties["SupportedSynchronousActions"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportedSynchronousActions"].Value = value;
		}
	}

	public bool? SupportsCreateReplicationRelationshipMethod
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["SupportsCreateReplicationRelationshipMethod"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportsCreateReplicationRelationshipMethod"].Value = value;
		}
	}

	public bool? SupportsEmptyReplicationGroup
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["SupportsEmptyReplicationGroup"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportsEmptyReplicationGroup"].Value = value;
		}
	}

	public bool? SupportsFullDiscovery
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["SupportsFullDiscovery"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportsFullDiscovery"].Value = value;
		}
	}

	public bool? SupportsReplicationGroup
	{
		get
		{
			return (bool?)base.Instance.CimInstanceProperties["SupportsReplicationGroup"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SupportsReplicationGroup"].Value = value;
		}
	}

	public MSFT_ReplicationCapabilities()
	{
	}

	public MSFT_ReplicationCapabilities(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageSubSystemToReplicationCapabilities = new MSFT_ReplicationCapabilitiesMSFT_StorageSubSystemToReplicationCapabilities(session, instance);
	}

	public static MSFT_ReplicationCapabilities GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_ReplicationCapabilities", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_ReplicationCapabilities(session, instance);
	}

	public static MSFT_ReplicationCapabilities CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_ReplicationCapabilities", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_ReplicationCapabilities(session, cimInstance);
	}

	public new static IEnumerable<MSFT_ReplicationCapabilities> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_ReplicationCapabilities")
			select new MSFT_ReplicationCapabilities(session, i);
	}

	public new static IEnumerable<MSFT_ReplicationCapabilities> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_ReplicationCapabilities";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_ReplicationCapabilities(session, i);
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

	public GetSupportedOperationsOutParameters GetSupportedOperations(ushort? ReplicationType = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ReplicationType.HasValue)
		{
			ushort? num = ReplicationType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationType", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSupportedOperations", cimMethodParametersCollection);
		GetSupportedOperationsOutParameters result = default(GetSupportedOperationsOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SupportedOperations"] != null)
		{
			result.SupportedOperations = (ushort[])cimMethodResult.OutParameters["SupportedOperations"].Value;
		}
		else
		{
			result.SupportedOperations = null;
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

	public GetSupportedGroupOperationsOutParameters GetSupportedGroupOperations(ushort? ReplicationType = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ReplicationType.HasValue)
		{
			ushort? num = ReplicationType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationType", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSupportedGroupOperations", cimMethodParametersCollection);
		GetSupportedGroupOperationsOutParameters result = default(GetSupportedGroupOperationsOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SupportedGroupOperations"] != null)
		{
			result.SupportedGroupOperations = (ushort[])cimMethodResult.OutParameters["SupportedGroupOperations"].Value;
		}
		else
		{
			result.SupportedGroupOperations = null;
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

	public GetSupportedFeaturesOutParameters GetSupportedFeatures(ushort? ReplicationType = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ReplicationType.HasValue)
		{
			ushort? num = ReplicationType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationType", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSupportedFeatures", cimMethodParametersCollection);
		GetSupportedFeaturesOutParameters result = default(GetSupportedFeaturesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["Features"] != null)
		{
			result.Features = (ushort[])cimMethodResult.OutParameters["Features"].Value;
		}
		else
		{
			result.Features = null;
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

	public GetSupportedGroupFeaturesOutParameters GetSupportedGroupFeatures(ushort? ReplicationType = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ReplicationType.HasValue)
		{
			ushort? num = ReplicationType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationType", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSupportedGroupFeatures", cimMethodParametersCollection);
		GetSupportedGroupFeaturesOutParameters result = default(GetSupportedGroupFeaturesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["GroupFeatures"] != null)
		{
			result.GroupFeatures = (ushort[])cimMethodResult.OutParameters["GroupFeatures"].Value;
		}
		else
		{
			result.GroupFeatures = null;
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

	public GetSupportedCopyStatesOutParameters GetSupportedCopyStates(ushort? ReplicationType = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ReplicationType.HasValue)
		{
			ushort? num = ReplicationType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationType", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSupportedCopyStates", cimMethodParametersCollection);
		GetSupportedCopyStatesOutParameters result = default(GetSupportedCopyStatesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SupportedCopyStates"] != null)
		{
			result.SupportedCopyStates = (ushort[])cimMethodResult.OutParameters["SupportedCopyStates"].Value;
		}
		else
		{
			result.SupportedCopyStates = null;
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

	public GetSupportedGroupCopyStatesOutParameters GetSupportedGroupCopyStates(ushort? ReplicationType = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ReplicationType.HasValue)
		{
			ushort? num = ReplicationType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationType", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSupportedGroupCopyStates", cimMethodParametersCollection);
		GetSupportedGroupCopyStatesOutParameters result = default(GetSupportedGroupCopyStatesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SupportedCopyStates"] != null)
		{
			result.SupportedCopyStates = (ushort[])cimMethodResult.OutParameters["SupportedCopyStates"].Value;
		}
		else
		{
			result.SupportedCopyStates = null;
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

	public GetRecoveryPointDataOutParameters GetRecoveryPointData(ushort? ReplicationType = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ReplicationType.HasValue)
		{
			ushort? num = ReplicationType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationType", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetRecoveryPointData", cimMethodParametersCollection);
		GetRecoveryPointDataOutParameters result = default(GetRecoveryPointDataOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["DefaultRecoveryPoint"] != null)
		{
			result.DefaultRecoveryPoint = (uint?)cimMethodResult.OutParameters["DefaultRecoveryPoint"].Value;
		}
		else
		{
			result.DefaultRecoveryPoint = null;
		}
		if (cimMethodResult.OutParameters["RecoveryPointValues"] != null)
		{
			result.RecoveryPointValues = (uint[])cimMethodResult.OutParameters["RecoveryPointValues"].Value;
		}
		else
		{
			result.RecoveryPointValues = null;
		}
		if (cimMethodResult.OutParameters["RecoveryPointIndicator"] != null)
		{
			result.RecoveryPointIndicator = (ushort?)cimMethodResult.OutParameters["RecoveryPointIndicator"].Value;
		}
		else
		{
			result.RecoveryPointIndicator = null;
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
