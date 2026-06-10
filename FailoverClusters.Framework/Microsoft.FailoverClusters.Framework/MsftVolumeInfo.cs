using System;
using System.Collections.Generic;
using System.Windows.Input;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class MsftVolumeInfo : ObservableCollectionItem<MsftVolumeInfo>
{
	private const string NtFs = "NTFS";

	private const string ReFs = "ReFS";

	private const string Fat32 = "FAT32";

	private const string CsvFs = "CSVFS";

	private readonly CommandCollection commands = new CommandCollection(ClusterCommandCollectionId.DiskInfo);

	public override string Key => ObjectId;

	public override string DisplayName => Path;

	public override IEnumerable<ICommand> Commands => commands;

	public string ObjectId { get; internal set; }

	public char DriveLetter { get; internal set; }

	public string Path { get; internal set; }

	public VolumeHealthStatus HealthStatus { get; internal set; }

	public string FileSystem { get; internal set; }

	public string FileSystemLabel { get; internal set; }

	public ulong Size { get; internal set; }

	public ulong SizeRemaining { get; internal set; }

	public VolumeDriveType DriveType { get; internal set; }

	public bool IsCsvFS => string.Compare(FileSystem, "CSVFS", StringComparison.OrdinalIgnoreCase) == 0;

	public bool IsNtFS => string.Compare(FileSystem, "NTFS", StringComparison.OrdinalIgnoreCase) == 0;

	public bool IsReFS => string.Compare(FileSystem, "ReFS", StringComparison.OrdinalIgnoreCase) == 0;

	public bool IsFat32 => string.Compare(FileSystem, "FAT32", StringComparison.OrdinalIgnoreCase) == 0;

	public MsftVolumeInfo(ClusterObject owner)
		: base(owner)
	{
	}

	public static void GetInstance(Cluster cluster, string key, Action<OperationResult<MsftVolumeInfo>> operationResult, string serverName = null)
	{
		ObservableCollectionItem<MsftVolumeInfo>.GetInstance(cluster, key, operationResult, serverName);
	}

	public override void CopyFrom(MsftVolumeInfo source)
	{
		Exceptions.ThrowIfNull(source, "source");
		if (ObjectId != source.ObjectId)
		{
			ObjectId = source.ObjectId;
			OnPropertyChanged("ObjectId");
		}
		if (DriveLetter != source.DriveLetter)
		{
			DriveLetter = source.DriveLetter;
			OnPropertyChanged("DriveLetter");
		}
		if (Path != source.Path)
		{
			Path = source.Path;
			OnPropertyChanged("Path");
		}
		if (HealthStatus != source.HealthStatus)
		{
			HealthStatus = source.HealthStatus;
			OnPropertyChanged("HealthStatus");
		}
		if (FileSystem != source.FileSystem)
		{
			FileSystem = source.FileSystem;
			OnPropertyChanged("FileSystem");
		}
		if (FileSystemLabel != source.FileSystemLabel)
		{
			FileSystemLabel = source.FileSystemLabel;
			OnPropertyChanged("FileSystemLabel");
		}
		if (Size != source.Size)
		{
			Size = source.Size;
			OnPropertyChanged("Size");
		}
		if (SizeRemaining != source.SizeRemaining)
		{
			SizeRemaining = source.SizeRemaining;
			OnPropertyChanged("SizeRemaining");
		}
		if (DriveType != source.DriveType)
		{
			DriveType = source.DriveType;
			OnPropertyChanged("DriveType");
		}
	}
}

