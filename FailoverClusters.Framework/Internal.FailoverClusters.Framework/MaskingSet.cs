using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_MaskingSet : MSFT_StorageObject
{
	public struct AddInitiatorIdOutParameters
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

		public static bool operator ==(AddInitiatorIdOutParameters lhs, AddInitiatorIdOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(AddInitiatorIdOutParameters lhs, AddInitiatorIdOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RemoveInitiatorIdOutParameters
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

		public static bool operator ==(RemoveInitiatorIdOutParameters lhs, RemoveInitiatorIdOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RemoveInitiatorIdOutParameters lhs, RemoveInitiatorIdOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct AddTargetPortOutParameters
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

		public static bool operator ==(AddTargetPortOutParameters lhs, AddTargetPortOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(AddTargetPortOutParameters lhs, AddTargetPortOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RemoveTargetPortOutParameters
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

		public static bool operator ==(RemoveTargetPortOutParameters lhs, RemoveTargetPortOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RemoveTargetPortOutParameters lhs, RemoveTargetPortOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct AddVirtualDiskOutParameters
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

		public static bool operator ==(AddVirtualDiskOutParameters lhs, AddVirtualDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(AddVirtualDiskOutParameters lhs, AddVirtualDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RemoveVirtualDiskOutParameters
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

		public static bool operator ==(RemoveVirtualDiskOutParameters lhs, RemoveVirtualDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RemoveVirtualDiskOutParameters lhs, RemoveVirtualDiskOutParameters rhs)
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

	public MSFT_MaskingSetMSFT_MaskingSetToInitiatorId MSFT_MaskingSetToInitiatorId { get; private set; }

	public MSFT_MaskingSetMSFT_MaskingSetToTargetPort MSFT_MaskingSetToTargetPort { get; private set; }

	public MSFT_MaskingSetMSFT_MaskingSetToVirtualDisk MSFT_MaskingSetToVirtualDisk { get; private set; }

	public MSFT_MaskingSetMSFT_StorageSubSystemToMaskingSet MSFT_StorageSubSystemToMaskingSet { get; private set; }

	public string FriendlyName => (string)base.Instance.CimInstanceProperties["FriendlyName"].Value;

	public ushort? HostType => (ushort?)base.Instance.CimInstanceProperties["HostType"].Value;

	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public MSFT_MaskingSet()
	{
	}

	public MSFT_MaskingSet(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_MaskingSetToInitiatorId = new MSFT_MaskingSetMSFT_MaskingSetToInitiatorId(session, instance);
		MSFT_MaskingSetToTargetPort = new MSFT_MaskingSetMSFT_MaskingSetToTargetPort(session, instance);
		MSFT_MaskingSetToVirtualDisk = new MSFT_MaskingSetMSFT_MaskingSetToVirtualDisk(session, instance);
		MSFT_StorageSubSystemToMaskingSet = new MSFT_MaskingSetMSFT_StorageSubSystemToMaskingSet(session, instance);
	}

	public static MSFT_MaskingSet GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_MaskingSet", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_MaskingSet(session, instance);
	}

	public static MSFT_MaskingSet CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_MaskingSet", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_MaskingSet(session, cimInstance);
	}

	public new static IEnumerable<MSFT_MaskingSet> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_MaskingSet")
			select new MSFT_MaskingSet(session, i);
	}

	public new static IEnumerable<MSFT_MaskingSet> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_MaskingSet";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_MaskingSet(session, i);
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

	public AddInitiatorIdOutParameters AddInitiatorId(string[] InitiatorIds, ushort? HostType = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InitiatorIds != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorIds", InitiatorIds, CimType.StringArray, CimFlags.In));
		}
		if (HostType.HasValue)
		{
			ushort? num = HostType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("HostType", num, CimType.UInt16, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "AddInitiatorId", cimMethodParametersCollection);
		AddInitiatorIdOutParameters result = default(AddInitiatorIdOutParameters);
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

	public RemoveInitiatorIdOutParameters RemoveInitiatorId(string[] InitiatorIds, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (InitiatorIds != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorIds", InitiatorIds, CimType.StringArray, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RemoveInitiatorId", cimMethodParametersCollection);
		RemoveInitiatorIdOutParameters result = default(RemoveInitiatorIdOutParameters);
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

	public AddTargetPortOutParameters AddTargetPort(string[] TargetPortAddresses, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (TargetPortAddresses != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortAddresses", TargetPortAddresses, CimType.StringArray, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "AddTargetPort", cimMethodParametersCollection);
		AddTargetPortOutParameters result = default(AddTargetPortOutParameters);
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

	public RemoveTargetPortOutParameters RemoveTargetPort(string[] TargetPortAddresses, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (TargetPortAddresses != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortAddresses", TargetPortAddresses, CimType.StringArray, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RemoveTargetPort", cimMethodParametersCollection);
		RemoveTargetPortOutParameters result = default(RemoveTargetPortOutParameters);
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

	public AddVirtualDiskOutParameters AddVirtualDisk(string[] VirtualDiskNames, string[] DeviceNumbers, ushort[] DeviceAccesses, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (VirtualDiskNames != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDiskNames", VirtualDiskNames, CimType.StringArray, CimFlags.In));
		}
		if (DeviceNumbers != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DeviceNumbers", DeviceNumbers, CimType.StringArray, CimFlags.In));
		}
		if (DeviceAccesses != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DeviceAccesses", DeviceAccesses, CimType.UInt16Array, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "AddVirtualDisk", cimMethodParametersCollection);
		AddVirtualDiskOutParameters result = default(AddVirtualDiskOutParameters);
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

	public RemoveVirtualDiskOutParameters RemoveVirtualDisk(string[] VirtualDiskNames, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (VirtualDiskNames != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDiskNames", VirtualDiskNames, CimType.StringArray, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RemoveVirtualDisk", cimMethodParametersCollection);
		RemoveVirtualDiskOutParameters result = default(RemoveVirtualDiskOutParameters);
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
}

