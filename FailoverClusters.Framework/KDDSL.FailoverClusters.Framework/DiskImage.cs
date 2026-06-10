using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_DiskImage : MiInstanceBase
{
	public struct MountOutParameters
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

		public static bool operator ==(MountOutParameters lhs, MountOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MountOutParameters lhs, MountOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct DismountOutParameters
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

		public static bool operator ==(DismountOutParameters lhs, DismountOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DismountOutParameters lhs, DismountOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_DiskImageMSFT_DiskImageToVolume MSFT_DiskImageToVolume { get; private set; }

	public bool? Attached => (bool?)base.Instance.CimInstanceProperties["Attached"].Value;

	public ulong? BlockSize => (ulong?)base.Instance.CimInstanceProperties["BlockSize"].Value;

	public string DevicePath => (string)base.Instance.CimInstanceProperties["DevicePath"].Value;

	public ulong? FileSize => (ulong?)base.Instance.CimInstanceProperties["FileSize"].Value;

	public string ImagePath => (string)base.Instance.CimInstanceProperties["ImagePath"].Value;

	public ulong? LogicalSectorSize => (ulong?)base.Instance.CimInstanceProperties["LogicalSectorSize"].Value;

	public uint? Number => (uint?)base.Instance.CimInstanceProperties["Number"].Value;

	public ulong? Size => (ulong?)base.Instance.CimInstanceProperties["Size"].Value;

	public uint? StorageType => (uint?)base.Instance.CimInstanceProperties["StorageType"].Value;

	public MSFT_DiskImage()
	{
	}

	public MSFT_DiskImage(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_DiskImageToVolume = new MSFT_DiskImageMSFT_DiskImageToVolume(session, instance);
	}

	public static MSFT_DiskImage GetInstance(CimSession session, string ImagePath, uint? StorageType)
	{
		CimInstance cimInstance = new CimInstance("MSFT_DiskImage", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ImagePath", ImagePath, CimFlags.Key));
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("StorageType", StorageType, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_DiskImage(session, instance);
	}

	public static MSFT_DiskImage CreateReference(CimSession session, string ImagePath, uint? StorageType)
	{
		CimInstance cimInstance = new CimInstance("MSFT_DiskImage", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ImagePath", ImagePath, CimFlags.Key));
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("StorageType", StorageType, CimFlags.Key));
		return new MSFT_DiskImage(session, cimInstance);
	}

	public static IEnumerable<MSFT_DiskImage> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_DiskImage")
			select new MSFT_DiskImage(session, i);
	}

	public static IEnumerable<MSFT_DiskImage> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_DiskImage";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_DiskImage(session, i);
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

	public MountOutParameters Mount(ushort? Access = null, bool? NoDriveLetter = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Access.HasValue)
		{
			ushort? num = Access;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Access", num, CimType.UInt16, CimFlags.In));
		}
		if (NoDriveLetter.HasValue)
		{
			bool? flag = NoDriveLetter;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NoDriveLetter", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Mount", cimMethodParametersCollection);
		MountOutParameters result = default(MountOutParameters);
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

	public DismountOutParameters Dismount()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Dismount", methodParameters);
		DismountOutParameters result = default(DismountOutParameters);
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

