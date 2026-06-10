using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_Volume : MSFT_StorageObject
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

	public struct FormatOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_Volume FormattedVolume { get; set; }

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

		public static bool operator ==(FormatOutParameters lhs, FormatOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(FormatOutParameters lhs, FormatOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RepairOutParameters
	{
		public uint? ReturnValue { get; set; }

		public uint? Output { get; set; }

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

		public static bool operator ==(RepairOutParameters lhs, RepairOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RepairOutParameters lhs, RepairOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct OptimizeOutParameters
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

		public static bool operator ==(OptimizeOutParameters lhs, OptimizeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(OptimizeOutParameters lhs, OptimizeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetFileSystemLabelOutParameters
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

		public static bool operator ==(SetFileSystemLabelOutParameters lhs, SetFileSystemLabelOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetFileSystemLabelOutParameters lhs, SetFileSystemLabelOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSupportedFileSystemsOutParameters
	{
		public uint? ReturnValue { get; set; }

		public string[] SupportedFileSystems { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSupportedFileSystemsOutParameters lhs, GetSupportedFileSystemsOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSupportedFileSystemsOutParameters lhs, GetSupportedFileSystemsOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSupportedClusterSizesOutParameters
	{
		public uint? ReturnValue { get; set; }

		public uint[] SupportedClusterSizes { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSupportedClusterSizesOutParameters lhs, GetSupportedClusterSizesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSupportedClusterSizesOutParameters lhs, GetSupportedClusterSizesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetCorruptionCountOutParameters
	{
		public uint? ReturnValue { get; set; }

		public uint? CorruptionCount { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetCorruptionCountOutParameters lhs, GetCorruptionCountOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetCorruptionCountOutParameters lhs, GetCorruptionCountOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetAttributesOutParameters
	{
		public uint? ReturnValue { get; set; }

		public bool? VolumeScrubEnabled { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetAttributesOutParameters lhs, GetAttributesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetAttributesOutParameters lhs, GetAttributesOutParameters rhs)
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

	public struct FlushOutParameters
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

		public static bool operator ==(FlushOutParameters lhs, FlushOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(FlushOutParameters lhs, FlushOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct ResizeOutParameters
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

		public static bool operator ==(ResizeOutParameters lhs, ResizeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ResizeOutParameters lhs, ResizeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct DiagnoseOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageDiagnoseResult[] DiagnoseResults { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(DiagnoseOutParameters lhs, DiagnoseOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DiagnoseOutParameters lhs, DiagnoseOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetDedupModeOutParameters
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

		public static bool operator ==(SetDedupModeOutParameters lhs, SetDedupModeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetDedupModeOutParameters lhs, SetDedupModeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetDedupPropertiesOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_DedupProperties DedupProperties { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetDedupPropertiesOutParameters lhs, GetDedupPropertiesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetDedupPropertiesOutParameters lhs, GetDedupPropertiesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetActionsOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_HealthAction[] ActionResults { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetActionsOutParameters lhs, GetActionsOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetActionsOutParameters lhs, GetActionsOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_VolumeMSFT_DiskImageToVolume MSFT_DiskImageToVolume { get; private set; }

	public MSFT_VolumeMSFT_FileServerToVolume MSFT_FileServerToVolume { get; private set; }

	public MSFT_VolumeMSFT_PartitionToVolume MSFT_PartitionToVolume { get; private set; }

	public MSFT_VolumeMSFT_StorageNodeToVolume MSFT_StorageNodeToVolume { get; private set; }

	public MSFT_VolumeMSFT_StoragePoolToVolume MSFT_StoragePoolToVolume { get; private set; }

	public MSFT_VolumeMSFT_StorageSubSystemToVolume MSFT_StorageSubSystemToVolume { get; private set; }

	public MSFT_VolumeMSFT_VolumeToFileShare MSFT_VolumeToFileShare { get; private set; }

	public uint? AllocationUnitSize => (uint?)base.Instance.CimInstanceProperties["AllocationUnitSize"].Value;

	public uint? DedupMode
	{
		get
		{
			return (uint?)base.Instance.CimInstanceProperties["DedupMode"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["DedupMode"].Value = value;
		}
	}

	public char? DriveLetter => (char?)base.Instance.CimInstanceProperties["DriveLetter"].Value;

	public uint? DriveType => (uint?)base.Instance.CimInstanceProperties["DriveType"].Value;

	public string FileSystem => (string)base.Instance.CimInstanceProperties["FileSystem"].Value;

	public string FileSystemLabel
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["FileSystemLabel"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["FileSystemLabel"].Value = value;
		}
	}

	public ushort? FileSystemType => (ushort?)base.Instance.CimInstanceProperties["FileSystemType"].Value;

	public ushort? HealthStatus => (ushort?)base.Instance.CimInstanceProperties["HealthStatus"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public string Path => (string)base.Instance.CimInstanceProperties["Path"].Value;

	public ulong? Size => (ulong?)base.Instance.CimInstanceProperties["Size"].Value;

	public ulong? SizeRemaining => (ulong?)base.Instance.CimInstanceProperties["SizeRemaining"].Value;

	public MSFT_Volume()
	{
	}

	public MSFT_Volume(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_DiskImageToVolume = new MSFT_VolumeMSFT_DiskImageToVolume(session, instance);
		MSFT_FileServerToVolume = new MSFT_VolumeMSFT_FileServerToVolume(session, instance);
		MSFT_PartitionToVolume = new MSFT_VolumeMSFT_PartitionToVolume(session, instance);
		MSFT_StorageNodeToVolume = new MSFT_VolumeMSFT_StorageNodeToVolume(session, instance);
		MSFT_StoragePoolToVolume = new MSFT_VolumeMSFT_StoragePoolToVolume(session, instance);
		MSFT_StorageSubSystemToVolume = new MSFT_VolumeMSFT_StorageSubSystemToVolume(session, instance);
		MSFT_VolumeToFileShare = new MSFT_VolumeMSFT_VolumeToFileShare(session, instance);
	}

	public static MSFT_Volume GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_Volume", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_Volume(session, instance);
	}

	public static MSFT_Volume CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_Volume", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_Volume(session, cimInstance);
	}

	public new static IEnumerable<MSFT_Volume> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_Volume")
			select new MSFT_Volume(session, i);
	}

	public new static IEnumerable<MSFT_Volume> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_Volume";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_Volume(session, i);
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

	public FormatOutParameters Format(string FileSystem = null, string FileSystemLabel = null, uint? AllocationUnitSize = null, bool? Full = null, bool? Force = null, bool? Compress = null, bool? ShortFileNameSupport = null, bool? SetIntegrityStreams = null, bool? UseLargeFRS = null, bool? DisableHeatGathering = null, bool? IsDAX = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FileSystem != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileSystem", FileSystem, CimType.String, CimFlags.In));
		}
		if (FileSystemLabel != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileSystemLabel", FileSystemLabel, CimType.String, CimFlags.In));
		}
		if (AllocationUnitSize.HasValue)
		{
			uint? num = AllocationUnitSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AllocationUnitSize", num, CimType.UInt32, CimFlags.In));
		}
		if (Full.HasValue)
		{
			bool? flag = Full;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Full", flag, CimType.Boolean, CimFlags.In));
		}
		if (Force.HasValue)
		{
			bool? flag2 = Force;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Force", flag2, CimType.Boolean, CimFlags.In));
		}
		if (Compress.HasValue)
		{
			bool? flag3 = Compress;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Compress", flag3, CimType.Boolean, CimFlags.In));
		}
		if (ShortFileNameSupport.HasValue)
		{
			bool? flag4 = ShortFileNameSupport;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ShortFileNameSupport", flag4, CimType.Boolean, CimFlags.In));
		}
		if (SetIntegrityStreams.HasValue)
		{
			bool? flag5 = SetIntegrityStreams;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SetIntegrityStreams", flag5, CimType.Boolean, CimFlags.In));
		}
		if (UseLargeFRS.HasValue)
		{
			bool? flag6 = UseLargeFRS;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UseLargeFRS", flag6, CimType.Boolean, CimFlags.In));
		}
		if (DisableHeatGathering.HasValue)
		{
			bool? flag7 = DisableHeatGathering;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DisableHeatGathering", flag7, CimType.Boolean, CimFlags.In));
		}
		if (IsDAX.HasValue)
		{
			bool? flag8 = IsDAX;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsDAX", flag8, CimType.Boolean, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag9 = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag9, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Format", cimMethodParametersCollection);
		FormatOutParameters result = default(FormatOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["FormattedVolume"] != null)
		{
			result.FormattedVolume = new MSFT_Volume(base.Session, (CimInstance)cimMethodResult.OutParameters["FormattedVolume"].Value);
		}
		else
		{
			result.FormattedVolume = null;
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

	public RepairOutParameters Repair(bool? OfflineScanAndFix = null, bool? Scan = null, bool? SpotFix = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (OfflineScanAndFix.HasValue)
		{
			bool? flag = OfflineScanAndFix;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("OfflineScanAndFix", flag, CimType.Boolean, CimFlags.In));
		}
		if (Scan.HasValue)
		{
			bool? flag2 = Scan;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Scan", flag2, CimType.Boolean, CimFlags.In));
		}
		if (SpotFix.HasValue)
		{
			bool? flag3 = SpotFix;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SpotFix", flag3, CimType.Boolean, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag4 = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag4, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Repair", cimMethodParametersCollection);
		RepairOutParameters result = default(RepairOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["Output"] != null)
		{
			result.Output = (uint?)cimMethodResult.OutParameters["Output"].Value;
		}
		else
		{
			result.Output = null;
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

	public OptimizeOutParameters Optimize(bool? ReTrim = null, bool? Analyze = null, bool? Defrag = null, bool? SlabConsolidate = null, bool? TierOptimize = null, bool? NormalPriority = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (ReTrim.HasValue)
		{
			bool? flag = ReTrim;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReTrim", flag, CimType.Boolean, CimFlags.In));
		}
		if (Analyze.HasValue)
		{
			bool? flag2 = Analyze;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Analyze", flag2, CimType.Boolean, CimFlags.In));
		}
		if (Defrag.HasValue)
		{
			bool? flag3 = Defrag;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Defrag", flag3, CimType.Boolean, CimFlags.In));
		}
		if (SlabConsolidate.HasValue)
		{
			bool? flag4 = SlabConsolidate;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SlabConsolidate", flag4, CimType.Boolean, CimFlags.In));
		}
		if (TierOptimize.HasValue)
		{
			bool? flag5 = TierOptimize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TierOptimize", flag5, CimType.Boolean, CimFlags.In));
		}
		if (NormalPriority.HasValue)
		{
			bool? flag6 = NormalPriority;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NormalPriority", flag6, CimType.Boolean, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag7 = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag7, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Optimize", cimMethodParametersCollection);
		OptimizeOutParameters result = default(OptimizeOutParameters);
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

	public SetFileSystemLabelOutParameters SetFileSystemLabel(string FileSystemLabel = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FileSystemLabel != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileSystemLabel", FileSystemLabel, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetFileSystemLabel", cimMethodParametersCollection);
		SetFileSystemLabelOutParameters result = default(SetFileSystemLabelOutParameters);
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

	public GetSupportedFileSystemsOutParameters GetSupportedFileSystems()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSupportedFileSystems", methodParameters);
		GetSupportedFileSystemsOutParameters result = default(GetSupportedFileSystemsOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SupportedFileSystems"] != null)
		{
			result.SupportedFileSystems = (string[])cimMethodResult.OutParameters["SupportedFileSystems"].Value;
		}
		else
		{
			result.SupportedFileSystems = null;
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

	public GetSupportedClusterSizesOutParameters GetSupportedClusterSizes(string FileSystem = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FileSystem != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileSystem", FileSystem, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSupportedClusterSizes", cimMethodParametersCollection);
		GetSupportedClusterSizesOutParameters result = default(GetSupportedClusterSizesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SupportedClusterSizes"] != null)
		{
			result.SupportedClusterSizes = (uint[])cimMethodResult.OutParameters["SupportedClusterSizes"].Value;
		}
		else
		{
			result.SupportedClusterSizes = null;
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

	public GetCorruptionCountOutParameters GetCorruptionCount()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetCorruptionCount", methodParameters);
		GetCorruptionCountOutParameters result = default(GetCorruptionCountOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CorruptionCount"] != null)
		{
			result.CorruptionCount = (uint?)cimMethodResult.OutParameters["CorruptionCount"].Value;
		}
		else
		{
			result.CorruptionCount = null;
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

	public GetAttributesOutParameters GetAttributes()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetAttributes", methodParameters);
		GetAttributesOutParameters result = default(GetAttributesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["VolumeScrubEnabled"] != null)
		{
			result.VolumeScrubEnabled = (bool?)cimMethodResult.OutParameters["VolumeScrubEnabled"].Value;
		}
		else
		{
			result.VolumeScrubEnabled = null;
		}
		return result;
	}

	public SetAttributesOutParameters SetAttributes(bool? EnableVolumeScrub = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (EnableVolumeScrub.HasValue)
		{
			bool? flag = EnableVolumeScrub;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EnableVolumeScrub", flag, CimType.Boolean, CimFlags.In));
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

	public FlushOutParameters Flush()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Flush", methodParameters);
		FlushOutParameters result = default(FlushOutParameters);
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

	public DiagnoseOutParameters Diagnose()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Diagnose", methodParameters);
		DiagnoseOutParameters result = default(DiagnoseOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["DiagnoseResults"] != null)
		{
			result.DiagnoseResults = ((cimMethodResult.OutParameters["DiagnoseResults"].Value == null) ? null : ((IEnumerable<CimInstance>)cimMethodResult.OutParameters["DiagnoseResults"].Value).Select((CimInstance i) => new MSFT_StorageDiagnoseResult(base.Session, i)).ToArray());
		}
		else
		{
			result.DiagnoseResults = null;
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

	public SetDedupModeOutParameters SetDedupMode(uint? DedupMode = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DedupMode.HasValue)
		{
			uint? num = DedupMode;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DedupMode", num, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetDedupMode", cimMethodParametersCollection);
		SetDedupModeOutParameters result = default(SetDedupModeOutParameters);
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

	public GetDedupPropertiesOutParameters GetDedupProperties()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetDedupProperties", methodParameters);
		GetDedupPropertiesOutParameters result = default(GetDedupPropertiesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["DedupProperties"] != null)
		{
			result.DedupProperties = new MSFT_DedupProperties(base.Session, (CimInstance)cimMethodResult.OutParameters["DedupProperties"].Value);
		}
		else
		{
			result.DedupProperties = null;
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

	public GetActionsOutParameters GetActions()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetActions", methodParameters);
		GetActionsOutParameters result = default(GetActionsOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["ActionResults"] != null)
		{
			result.ActionResults = ((cimMethodResult.OutParameters["ActionResults"].Value == null) ? null : ((IEnumerable<CimInstance>)cimMethodResult.OutParameters["ActionResults"].Value).Select((CimInstance i) => new MSFT_HealthAction(base.Session, i)).ToArray());
		}
		else
		{
			result.ActionResults = null;
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

