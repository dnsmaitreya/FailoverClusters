using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageProvider : MSFT_StorageObject
{
	public struct DiscoverOutParameters
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

		public static bool operator ==(DiscoverOutParameters lhs, DiscoverOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DiscoverOutParameters lhs, DiscoverOutParameters rhs)
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

	public struct RegisterSubsystemOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageSubSystem RegisteredSubsystem { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(RegisterSubsystemOutParameters lhs, RegisterSubsystemOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RegisterSubsystemOutParameters lhs, RegisterSubsystemOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct UnregisterSubsystemOutParameters
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

		public static bool operator ==(UnregisterSubsystemOutParameters lhs, UnregisterSubsystemOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(UnregisterSubsystemOutParameters lhs, UnregisterSubsystemOutParameters rhs)
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

	public MSFT_StorageProviderMSFT_StorageProviderToStorageSubSystem MSFT_StorageProviderToStorageSubSystem { get; private set; }

	public string CimServerName => (string)base.Instance.CimInstanceProperties["CimServerName"].Value;

	public string Manufacturer => (string)base.Instance.CimInstanceProperties["Manufacturer"].Value;

	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public ushort? RemoteSubsystemCacheMode => (ushort?)base.Instance.CimInstanceProperties["RemoteSubsystemCacheMode"].Value;

	public ushort[] SupportedRemoteSubsystemCacheModes => (ushort[])base.Instance.CimInstanceProperties["SupportedRemoteSubsystemCacheModes"].Value;

	public bool? SupportsSubsystemRegistration => (bool?)base.Instance.CimInstanceProperties["SupportsSubsystemRegistration"].Value;

	public ushort? Type => (ushort?)base.Instance.CimInstanceProperties["Type"].Value;

	public string URI => (string)base.Instance.CimInstanceProperties["URI"].Value;

	public string URI_IP => (string)base.Instance.CimInstanceProperties["URI_IP"].Value;

	public string Version => (string)base.Instance.CimInstanceProperties["Version"].Value;

	public MSFT_StorageProvider()
	{
	}

	public MSFT_StorageProvider(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageProviderToStorageSubSystem = new MSFT_StorageProviderMSFT_StorageProviderToStorageSubSystem(session, instance);
	}

	public static MSFT_StorageProvider GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageProvider", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageProvider(session, instance);
	}

	public static MSFT_StorageProvider CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageProvider", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_StorageProvider(session, cimInstance);
	}

	public new static IEnumerable<MSFT_StorageProvider> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_StorageProvider")
			select new MSFT_StorageProvider(session, i);
	}

	public new static IEnumerable<MSFT_StorageProvider> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageProvider";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_StorageProvider(session, i);
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

	public DiscoverOutParameters Discover(ushort? DiscoveryLevel, MSFT_StorageObject RootObject = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DiscoveryLevel.HasValue)
		{
			ushort? num = DiscoveryLevel;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DiscoveryLevel", num, CimType.UInt16, CimFlags.In));
		}
		if (RootObject != null)
		{
			CimInstance value = RootObject?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RootObject", value, CimType.Reference, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Discover", cimMethodParametersCollection);
		DiscoverOutParameters result = default(DiscoverOutParameters);
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

	public RegisterSubsystemOutParameters RegisterSubsystem(string ComputerName, string Credential = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ComputerName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ComputerName", ComputerName, CimType.String, CimFlags.In));
		}
		if (Credential != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Credential", Credential, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RegisterSubsystem", cimMethodParametersCollection);
		RegisterSubsystemOutParameters result = default(RegisterSubsystemOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["RegisteredSubsystem"] != null)
		{
			result.RegisteredSubsystem = new MSFT_StorageSubSystem(base.Session, (CimInstance)cimMethodResult.OutParameters["RegisteredSubsystem"].Value);
		}
		else
		{
			result.RegisteredSubsystem = null;
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

	public UnregisterSubsystemOutParameters UnregisterSubsystem(MSFT_StorageSubSystem Subsystem = null, string StorageSubSystemUniqueId = null, bool? Force = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Subsystem != null)
		{
			CimInstance value = Subsystem?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Subsystem", value, CimType.Instance, CimFlags.In));
		}
		if (StorageSubSystemUniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageSubSystemUniqueId", StorageSubSystemUniqueId, CimType.String, CimFlags.In));
		}
		if (Force.HasValue)
		{
			bool? flag = Force;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Force", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "UnregisterSubsystem", cimMethodParametersCollection);
		UnregisterSubsystemOutParameters result = default(UnregisterSubsystemOutParameters);
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

	public SetAttributesOutParameters SetAttributes(ushort? RemoteSubsystemCacheMode = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (RemoteSubsystemCacheMode.HasValue)
		{
			ushort? num = RemoteSubsystemCacheMode;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RemoteSubsystemCacheMode", num, CimType.UInt16, CimFlags.In));
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
}

