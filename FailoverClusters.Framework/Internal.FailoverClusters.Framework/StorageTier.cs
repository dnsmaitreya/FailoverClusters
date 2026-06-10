using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageTier : MSFT_StorageObject
{
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

	public struct ResizeOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ulong? Size { get; set; }

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

		public static bool operator ==(ResizeOutParameters lhs, ResizeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ResizeOutParameters lhs, ResizeOutParameters rhs)
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

	public struct GetSupportedSizeOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ulong[] SupportedSizes { get; set; }

		public ulong? TierSizeMin { get; set; }

		public ulong? TierSizeMax { get; set; }

		public ulong? TierSizeDivisor { get; set; }

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

	public MSFT_StorageTierMSFT_StoragePoolToStorageTier MSFT_StoragePoolToStorageTier { get; private set; }

	public MSFT_StorageTierMSFT_VirtualDiskToStorageTier MSFT_VirtualDiskToStorageTier { get; private set; }

	public string Description
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["Description"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["Description"].Value = value;
		}
	}

	public string FriendlyName
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["FriendlyName"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["FriendlyName"].Value = value;
		}
	}

	public ulong? Interleave => (ulong?)base.Instance.CimInstanceProperties["Interleave"].Value;

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

	public ushort? NumberOfColumns => (ushort?)base.Instance.CimInstanceProperties["NumberOfColumns"].Value;

	public ushort? NumberOfGroups => (ushort?)base.Instance.CimInstanceProperties["NumberOfGroups"].Value;

	public ushort? PhysicalDiskRedundancy => (ushort?)base.Instance.CimInstanceProperties["PhysicalDiskRedundancy"].Value;

	public string ResiliencySettingName => (string)base.Instance.CimInstanceProperties["ResiliencySettingName"].Value;

	public ulong? Size
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["Size"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["Size"].Value = value;
		}
	}

	public MSFT_StorageTier()
	{
	}

	public MSFT_StorageTier(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StoragePoolToStorageTier = new MSFT_StorageTierMSFT_StoragePoolToStorageTier(session, instance);
		MSFT_VirtualDiskToStorageTier = new MSFT_StorageTierMSFT_VirtualDiskToStorageTier(session, instance);
	}

	public static MSFT_StorageTier GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageTier", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageTier(session, instance);
	}

	public static MSFT_StorageTier CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageTier", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_StorageTier(session, cimInstance);
	}

	public new static IEnumerable<MSFT_StorageTier> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageTier")
			select new MSFT_StorageTier(session, i);
	}

	public new static IEnumerable<MSFT_StorageTier> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageTier";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageTier(session, i);
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

	public ResizeOutParameters Resize(ulong? Size = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Size.HasValue)
		{
			ulong? num = Size;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Size", num, CimType.UInt64, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Resize", cimMethodParametersCollection);
		ResizeOutParameters result = default(ResizeOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["Size"] != null)
		{
			result.Size = (ulong?)cimMethodResult.OutParameters["Size"].Value;
		}
		else
		{
			result.Size = null;
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

	public GetSupportedSizeOutParameters GetSupportedSize(string ResiliencySettingName)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ResiliencySettingName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ResiliencySettingName", ResiliencySettingName, CimType.String, CimFlags.In));
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
		if (cimMethodResult.OutParameters["TierSizeMin"] != null)
		{
			result.TierSizeMin = (ulong?)cimMethodResult.OutParameters["TierSizeMin"].Value;
		}
		else
		{
			result.TierSizeMin = null;
		}
		if (cimMethodResult.OutParameters["TierSizeMax"] != null)
		{
			result.TierSizeMax = (ulong?)cimMethodResult.OutParameters["TierSizeMax"].Value;
		}
		else
		{
			result.TierSizeMax = null;
		}
		if (cimMethodResult.OutParameters["TierSizeDivisor"] != null)
		{
			result.TierSizeDivisor = (ulong?)cimMethodResult.OutParameters["TierSizeDivisor"].Value;
		}
		else
		{
			result.TierSizeDivisor = null;
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

