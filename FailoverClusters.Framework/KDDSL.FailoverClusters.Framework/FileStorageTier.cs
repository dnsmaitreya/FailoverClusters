using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_FileStorageTier : MiInstanceBase
{
	public struct GetOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_FileStorageTier[] FileStorageTier { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetOutParameters lhs, GetOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetOutParameters lhs, GetOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetOutParameters
	{
		public uint? ReturnValue { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(SetOutParameters lhs, SetOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetOutParameters lhs, SetOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct ClearOutParameters
	{
		public uint? ReturnValue { get; set; }

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

	public string DesiredStorageTierName => (string)base.Instance.CimInstanceProperties["DesiredStorageTierName"].Value;

	public string FilePath => (string)base.Instance.CimInstanceProperties["FilePath"].Value;

	public ulong? FileSize => (ulong?)base.Instance.CimInstanceProperties["FileSize"].Value;

	public ulong? FileSizeOnDesiredStorageTier => (ulong?)base.Instance.CimInstanceProperties["FileSizeOnDesiredStorageTier"].Value;

	public ushort? PlacementStatus => (ushort?)base.Instance.CimInstanceProperties["PlacementStatus"].Value;

	public ushort? State => (ushort?)base.Instance.CimInstanceProperties["State"].Value;

	public MSFT_FileStorageTier()
	{
	}

	public MSFT_FileStorageTier(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static MSFT_FileStorageTier GetInstance(CimSession session, string FilePath)
	{
		CimInstance cimInstance = new CimInstance("MSFT_FileStorageTier", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("FilePath", FilePath, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_FileStorageTier(session, instance);
	}

	public static MSFT_FileStorageTier CreateReference(CimSession session, string FilePath)
	{
		CimInstance cimInstance = new CimInstance("MSFT_FileStorageTier", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("FilePath", FilePath, CimFlags.Key));
		return new MSFT_FileStorageTier(session, cimInstance);
	}

	public static IEnumerable<MSFT_FileStorageTier> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_FileStorageTier")
			select new MSFT_FileStorageTier(session, i);
	}

	public static IEnumerable<MSFT_FileStorageTier> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_FileStorageTier";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_FileStorageTier(session, i);
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

	public static GetOutParameters Get(CimSession Session, string FilePath = null, char? VolumeDriveLetter = null, string VolumePath = null, MSFT_Volume Volume = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FilePath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FilePath", FilePath, CimType.String, CimFlags.In));
		}
		if (VolumeDriveLetter.HasValue)
		{
			char? c = VolumeDriveLetter;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VolumeDriveLetter", c, CimType.Char16, CimFlags.In));
		}
		if (VolumePath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VolumePath", VolumePath, CimType.String, CimFlags.In));
		}
		if (Volume != null)
		{
			CimInstance value = Volume?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Volume", value, CimType.Instance, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/windows/storage", "MSFT_FileStorageTier", "Get", cimMethodParametersCollection);
		GetOutParameters result = default(GetOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["FileStorageTier"] != null)
		{
			result.FileStorageTier = ((cimMethodResult.OutParameters["FileStorageTier"].Value == null) ? null : ((IEnumerable<CimInstance>)cimMethodResult.OutParameters["FileStorageTier"].Value).Select((CimInstance i) => new MSFT_FileStorageTier(Session, i)).ToArray());
		}
		else
		{
			result.FileStorageTier = null;
		}
		return result;
	}

	public static SetOutParameters Set(CimSession Session, string FilePath = null, string DesiredStorageTierFriendlyName = null, string DesiredStorageTierUniqueId = null, MSFT_StorageTier DesiredStorageTier = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FilePath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FilePath", FilePath, CimType.String, CimFlags.In));
		}
		if (DesiredStorageTierFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DesiredStorageTierFriendlyName", DesiredStorageTierFriendlyName, CimType.String, CimFlags.In));
		}
		if (DesiredStorageTierUniqueId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DesiredStorageTierUniqueId", DesiredStorageTierUniqueId, CimType.String, CimFlags.In));
		}
		if (DesiredStorageTier != null)
		{
			CimInstance value = DesiredStorageTier?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DesiredStorageTier", value, CimType.Instance, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/windows/storage", "MSFT_FileStorageTier", "Set", cimMethodParametersCollection);
		SetOutParameters result = default(SetOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		return result;
	}

	public static ClearOutParameters Clear(CimSession Session, string FilePath = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FilePath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FilePath", FilePath, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = Session.InvokeMethod("root/windows/storage", "MSFT_FileStorageTier", "Clear", cimMethodParametersCollection);
		ClearOutParameters result = default(ClearOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		return result;
	}
}

