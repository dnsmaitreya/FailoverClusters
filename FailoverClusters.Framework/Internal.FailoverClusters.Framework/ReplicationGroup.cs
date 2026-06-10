using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_ReplicationGroup : MSFT_StorageObject
{
	public struct CreateReplicaOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_ReplicaPeer CreatedReplicaPeer { get; set; }

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

		public static bool operator ==(CreateReplicaOutParameters lhs, CreateReplicaOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateReplicaOutParameters lhs, CreateReplicaOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetReplicationRelationshipOutParameters
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

		public static bool operator ==(SetReplicationRelationshipOutParameters lhs, SetReplicationRelationshipOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetReplicationRelationshipOutParameters lhs, SetReplicationRelationshipOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetReplicationSettingsOutParameters
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

		public static bool operator ==(SetReplicationSettingsOutParameters lhs, SetReplicationSettingsOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetReplicationSettingsOutParameters lhs, SetReplicationSettingsOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetReplicationSettingsOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_ReplicationSettings ReplicationSettings { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetReplicationSettingsOutParameters lhs, GetReplicationSettingsOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetReplicationSettingsOutParameters lhs, GetReplicationSettingsOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct AddMemberOutParameters
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

		public static bool operator ==(AddMemberOutParameters lhs, AddMemberOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(AddMemberOutParameters lhs, AddMemberOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RemoveMemberOutParameters
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

		public static bool operator ==(RemoveMemberOutParameters lhs, RemoveMemberOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RemoveMemberOutParameters lhs, RemoveMemberOutParameters rhs)
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

	public struct DeleteObjectOutParameters
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

		public static bool operator ==(DeleteObjectOutParameters lhs, DeleteObjectOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DeleteObjectOutParameters lhs, DeleteObjectOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_ReplicationGroupMSFT_ReplicationGroupToPartition MSFT_ReplicationGroupToPartition { get; private set; }

	public MSFT_ReplicationGroupMSFT_ReplicationGroupToReplicaPeer MSFT_ReplicationGroupToReplicaPeer { get; private set; }

	public MSFT_ReplicationGroupMSFT_ReplicationGroupToVirtualDisk MSFT_ReplicationGroupToVirtualDisk { get; private set; }

	public MSFT_ReplicationGroupMSFT_StorageSubSystemToReplicationGroup MSFT_StorageSubSystemToReplicationGroup { get; private set; }

	public string Description => (string)base.Instance.CimInstanceProperties["Description"].Value;

	public string FriendlyName => (string)base.Instance.CimInstanceProperties["FriendlyName"].Value;

	public ushort? HealthStatus => (ushort?)base.Instance.CimInstanceProperties["HealthStatus"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public MSFT_ReplicationGroup()
	{
	}

	public MSFT_ReplicationGroup(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_ReplicationGroupToPartition = new MSFT_ReplicationGroupMSFT_ReplicationGroupToPartition(session, instance);
		MSFT_ReplicationGroupToReplicaPeer = new MSFT_ReplicationGroupMSFT_ReplicationGroupToReplicaPeer(session, instance);
		MSFT_ReplicationGroupToVirtualDisk = new MSFT_ReplicationGroupMSFT_ReplicationGroupToVirtualDisk(session, instance);
		MSFT_StorageSubSystemToReplicationGroup = new MSFT_ReplicationGroupMSFT_StorageSubSystemToReplicationGroup(session, instance);
	}

	public static MSFT_ReplicationGroup GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_ReplicationGroup", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_ReplicationGroup(session, instance);
	}

	public static MSFT_ReplicationGroup CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_ReplicationGroup", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_ReplicationGroup(session, cimInstance);
	}

	public new static IEnumerable<MSFT_ReplicationGroup> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_ReplicationGroup")
			select new MSFT_ReplicationGroup(session, i);
	}

	public new static IEnumerable<MSFT_ReplicationGroup> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_ReplicationGroup";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_ReplicationGroup(session, i);
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

	public CreateReplicaOutParameters CreateReplica(MSFT_ReplicaPeer TargetStorageSubsystem, string TargetGroupObjectId, MSFT_ReplicationSettings ReplicationSettings, ushort? SyncType, string FriendlyName = null, string TargetStoragePoolObjectId = null, uint? RecoveryPointObjective = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (TargetStorageSubsystem != null)
		{
			CimInstance value = TargetStorageSubsystem?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStorageSubsystem", value, CimType.Instance, CimFlags.In));
		}
		if (TargetGroupObjectId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetGroupObjectId", TargetGroupObjectId, CimType.String, CimFlags.In));
		}
		if (ReplicationSettings != null)
		{
			CimInstance value2 = ReplicationSettings?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationSettings", value2, CimType.Instance, CimFlags.In));
		}
		if (SyncType.HasValue)
		{
			ushort? num = SyncType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SyncType", num, CimType.UInt16, CimFlags.In));
		}
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (TargetStoragePoolObjectId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStoragePoolObjectId", TargetStoragePoolObjectId, CimType.String, CimFlags.In));
		}
		if (RecoveryPointObjective.HasValue)
		{
			uint? num2 = RecoveryPointObjective;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RecoveryPointObjective", num2, CimType.UInt32, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateReplica", cimMethodParametersCollection);
		CreateReplicaOutParameters result = default(CreateReplicaOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedReplicaPeer"] != null)
		{
			result.CreatedReplicaPeer = new MSFT_ReplicaPeer(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedReplicaPeer"].Value);
		}
		else
		{
			result.CreatedReplicaPeer = null;
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

	public SetReplicationRelationshipOutParameters SetReplicationRelationship(ushort? Operation, MSFT_ReplicaPeer TargetGroup, MSFT_StorageObject[] SourceStorageObjects, MSFT_StorageObject[] TargetStorageObjects, MSFT_Synchronized[] SyncPairs, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Operation.HasValue)
		{
			ushort? num = Operation;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Operation", num, CimType.UInt16, CimFlags.In));
		}
		if (TargetGroup != null)
		{
			CimInstance value = TargetGroup?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetGroup", value, CimType.Instance, CimFlags.In));
		}
		if (SourceStorageObjects != null)
		{
			CimInstance[] value2 = SourceStorageObjects?.Select((MSFT_StorageObject i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SourceStorageObjects", value2, CimType.InstanceArray, CimFlags.In));
		}
		if (TargetStorageObjects != null)
		{
			CimInstance[] value3 = TargetStorageObjects?.Select((MSFT_StorageObject i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStorageObjects", value3, CimType.InstanceArray, CimFlags.In));
		}
		if (SyncPairs != null)
		{
			CimInstance[] value4 = SyncPairs?.Select((MSFT_Synchronized i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SyncPairs", value4, CimType.InstanceArray, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetReplicationRelationship", cimMethodParametersCollection);
		SetReplicationRelationshipOutParameters result = default(SetReplicationRelationshipOutParameters);
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

	public SetReplicationSettingsOutParameters SetReplicationSettings(MSFT_ReplicationSettings ReplicationSettings)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ReplicationSettings != null)
		{
			CimInstance value = ReplicationSettings?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationSettings", value, CimType.Instance, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetReplicationSettings", cimMethodParametersCollection);
		SetReplicationSettingsOutParameters result = default(SetReplicationSettingsOutParameters);
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

	public GetReplicationSettingsOutParameters GetReplicationSettings()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetReplicationSettings", methodParameters);
		GetReplicationSettingsOutParameters result = default(GetReplicationSettingsOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["ReplicationSettings"] != null)
		{
			result.ReplicationSettings = new MSFT_ReplicationSettings(base.Session, (CimInstance)cimMethodResult.OutParameters["ReplicationSettings"].Value);
		}
		else
		{
			result.ReplicationSettings = null;
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

	public AddMemberOutParameters AddMember(MSFT_StorageObject[] StorageObjects)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (StorageObjects != null)
		{
			CimInstance[] value = StorageObjects?.Select((MSFT_StorageObject i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageObjects", value, CimType.InstanceArray, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "AddMember", cimMethodParametersCollection);
		AddMemberOutParameters result = default(AddMemberOutParameters);
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

	public RemoveMemberOutParameters RemoveMember(MSFT_StorageObject[] StorageObjects)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (StorageObjects != null)
		{
			CimInstance[] value = StorageObjects?.Select((MSFT_StorageObject i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageObjects", value, CimType.InstanceArray, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RemoveMember", cimMethodParametersCollection);
		RemoveMemberOutParameters result = default(RemoveMemberOutParameters);
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

	public DeleteObjectOutParameters DeleteObject()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "DeleteObject", methodParameters);
		DeleteObjectOutParameters result = default(DeleteObjectOutParameters);
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

