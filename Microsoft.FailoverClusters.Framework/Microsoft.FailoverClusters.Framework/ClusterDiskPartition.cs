using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterDiskPartition : IDataItem
{
	private ClusterCommand changeDriveLetterCommand;

	private ClusterCommand setRedirectedAccessCommand;

	public const uint FileVolumeIsCompressed = 32768u;

	public const uint BitLockerEnabled = 32u;

	private DirtyState isDirty;

	private readonly Guid id = Guid.NewGuid();

	private static readonly Icon2 DefaultVolumeIcon = new Icon2(InvariantResources.StorageVolume);

	public bool IsBitLockerEnabled => (Flags & 0x20) != 0;

	public long BitLockerFlags { get; set; }

	public ClusterDisk Owner { get; set; }

	public string Name { get; set; }

	public string Label { get; set; }

	public uint Flags { get; set; }

	public uint SerialNumber { get; set; }

	public ClusterSharedVolumeInfo SharedVolumeInfo { get; set; }

	public string DisplayName => CalculateDisplayName(Label, Name, HasDriveLetter, DriveLetter, MountPoints, IsCsvFs);

	public string FileSystem { get; set; }

	public Guid VolumeGuid { get; set; }

	public bool IsCompressed { get; set; }

	public DirtyState IsDirty
	{
		get
		{
			return isDirty;
		}
		set
		{
			isDirty = value;
		}
	}

	public uint PartitionNumber { get; set; }

	public ulong Size { get; set; }

	public ulong FreeSpace { get; set; }

	public List<string> MountPoints { get; private set; }

	public bool IncludeMountPoints { get; internal set; }

	public string Path { get; set; }

	public string VolumeGuidPath { get; set; }

	public bool? IsMaintenanceModeOn { get; set; }

	public uint DeviceNumber { get; set; }

	public Guid GptPartitionId { get; set; }

	public string PartitionName { get; set; }

	public bool IsFormatted
	{
		get
		{
			if (!IsFileSystem("RAW"))
			{
				return !IsFileSystem(string.Empty);
			}
			return false;
		}
	}

	public bool IsNtfs => IsFileSystem("NTFS");

	public bool IsReFs => IsFileSystem("ReFS");

	public bool IsCsvFs => IsFileSystem("CSVFS");

	public ClusterSharedVolumeFaultState CsvFaultState { get; set; }

	public static Icon2 VolumeIcon => DefaultVolumeIcon;

	public IEnumerable<ICommand> Commands => GetCommands();

	public Guid Id => id;

	public string DriveLetter
	{
		get
		{
			if (Name.Length == 2 && Name[1] == ':')
			{
				return Name.Substring(0, 1);
			}
			return null;
		}
	}

	public bool HasDriveLetter => DriveLetter != null;

	public ulong UsedSpace => Size - FreeSpace;

	public float PercentFree => (float)FreeSpace / (float)Size * 100f;

	public uint DriveLetterMask
	{
		get
		{
			char c = char.ToLowerInvariant('a');
			string driveLetter = DriveLetter;
			uint result = 0u;
			if (driveLetter != null)
			{
				int driveLetterBit = char.ToLowerInvariant(driveLetter[0]) - c;
				result = SetDriveLetterBit(driveLetterBit);
			}
			return result;
		}
	}

	public bool IsPartitionNumberValid => PartitionNumber != uint.MaxValue;

	public bool IsPartitionSizeValid => Size != 0;

	public CommandCollection GetCommands()
	{
		return InitializeDiskPartitionCommands(Owner.OwnerResource);
	}

	internal void UpdateCommandCanExecute()
	{
		changeDriveLetterCommand?.CanExecuteUpdate(this, new EventArgs());
		setRedirectedAccessCommand?.CanExecuteUpdate(this, new EventArgs());
	}

	private CommandCollection InitializeDiskPartitionCommands(StorageResource storageResource)
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.ClusterDiskPartition);
		CsvVolumeResource csvVolumeResource = storageResource as CsvVolumeResource;
		if (csvVolumeResource == null)
		{
			changeDriveLetterCommand = new ClusterCommand(storageResource, "ChangeDriveLetter", ClusterCommandId.StorageChangeDriveLetter, ClusterCommandCollectionId.ClusterDiskPartition)
			{
				Text = CommandResources.ChangeDriveLetterAction_Text,
				CanExecuteDelegate = delegate(object x)
				{
					Resource item = ((Tuple<Resource, uint>)x).Item1;
					StorageResource storageResource2 = item as StorageResource;
					if (storageResource2 != null && (storageResource2.OwnerGroup == null || storageResource2.OwnerGroup is ClusterSharedVolumeGroup) && storageResource2.ResourceType.ResourceKind == ResourceKind.PhysicalDisk)
					{
						return false;
					}
					return item.ResourceState == ResourceState.Online && storageResource2 != null && storageResource2.DiskInfo != null;
				},
				ExecuteDelegate = delegate
				{
					throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration.");
				},
				CommandParameter = new Tuple<Resource, uint>(storageResource, PartitionNumber)
			};
			commandCollection.Add(changeDriveLetterCommand);
		}
		else if (FileSystem == "CSVFS")
		{
			bool turnOnRedirectedAccess = CsvFaultState != ClusterSharedVolumeFaultState.NoDirectIO;
			setRedirectedAccessCommand = new ClusterCommand(storageResource, "ToggleRedirectedAccess", ClusterCommandId.ClusterShareVolumeTurnoffRedirectedAccess, ClusterCommandCollectionId.ClusterDiskPartition)
			{
				Text = (turnOnRedirectedAccess ? CommandResources.SharedVolumeRedirectedIO_Text : CommandResources.SharedVolumeDisableReDirectedIO_Text),
				CanExecuteDelegate = (object x) => csvVolumeResource.ResourceState == ResourceState.Online && csvVolumeResource.DiskInfo != null && csvVolumeResource.OwnerGroup is ClusterSharedVolumeGroup,
				ExecuteDelegate = delegate
				{
					if (csvVolumeResource.MaintenanceMode == false)
					{
						csvVolumeResource.SetRedirectedAccess(delegate(OperationResult r)
						{
							if (r.Error != null)
							{
								csvVolumeResource.SetError(r.Error);
							}
						}, VolumeGuid, turnOnRedirectedAccess, askConfirmation: true);
					}
					else
					{
						ConfirmationDialog confirmationDialog = new ConfirmationDialog(new TaskDialogButtonsStyle(TaskDialogButtonsSettings.Ok, TaskDialogStandardButtons.Ok));
						confirmationDialog.Icon = TaskDialogStandardIcon.Warning;
						confirmationDialog.Header = DialogResources.CsvInMMSettingRedirectedAccessHeader;
						confirmationDialog.Content = DialogResources.CsvInMMSettingRedirectedAccessContent;
						confirmationDialog.Caption = (turnOnRedirectedAccess ? DialogResources.TurnOnRedirectedAccessForCsvMessage_Title : DialogResources.TurnOffRedirectedAccessForCsvMessage_Title);
						confirmationDialog.ShowDialog();
					}
				}
			};
			commandCollection.Add(setRedirectedAccessCommand);
		}
		return commandCollection;
	}

	public ClusterDiskPartition(ClusterDisk owner)
	{
		Owner = owner;
		MountPoints = new List<string>();
	}

	internal ClusterDiskPartition(ClusterDisk owner, NativeMethods.CLUSPROP_PARTITION_INFO partitionInfo)
	{
		Owner = owner;
		Name = partitionInfo.partitionInfo.deviceName;
		FileSystem = partitionInfo.partitionInfo.fileSystem;
		IsCompressed = (partitionInfo.partitionInfo.fileSystemFlags & 0x8000) != 0;
		Flags = partitionInfo.partitionInfo.flags;
		SerialNumber = partitionInfo.partitionInfo.serialNumber;
		Label = partitionInfo.partitionInfo.volumeLabel;
		MountPoints = new List<string>();
		IncludeMountPoints = false;
		IsMaintenanceModeOn = null;
		CsvFaultState = ClusterSharedVolumeFaultState.NoFaults;
	}

	internal ClusterDiskPartition(ClusterDisk owner, NativeMethods.CLUSPROP_PARTITION_INFO_EX partitionInfoEx)
	{
		Owner = owner;
		Name = partitionInfoEx.partitionInfoEx.deviceName;
		DeviceNumber = partitionInfoEx.partitionInfoEx.deviceNumber;
		FileSystem = partitionInfoEx.partitionInfoEx.fileSystem;
		IsCompressed = (partitionInfoEx.partitionInfoEx.fileSystemFlags & 0x8000) != 0;
		Flags = partitionInfoEx.partitionInfoEx.flags;
		FreeSpace = partitionInfoEx.partitionInfoEx.freeSizeInBytes;
		PartitionNumber = partitionInfoEx.partitionInfoEx.partitionNumber;
		SerialNumber = partitionInfoEx.partitionInfoEx.serialNumber;
		Size = partitionInfoEx.partitionInfoEx.totalSizeInBytes;
		VolumeGuid = partitionInfoEx.partitionInfoEx.volumeGuid;
		Label = partitionInfoEx.partitionInfoEx.volumeLabel;
		MountPoints = new List<string>();
		IncludeMountPoints = false;
		IsMaintenanceModeOn = null;
		CsvFaultState = ClusterSharedVolumeFaultState.NoFaults;
	}

	internal ClusterDiskPartition(ClusterDisk owner, NativeMethods.CLUSPROP_PARTITION_INFO_EX2 partitionInfoEx2)
	{
		Owner = owner;
		Name = partitionInfoEx2.partitionInfoEx2.deviceName;
		DeviceNumber = partitionInfoEx2.partitionInfoEx2.deviceNumber;
		FileSystem = partitionInfoEx2.partitionInfoEx2.fileSystem;
		IsCompressed = (partitionInfoEx2.partitionInfoEx2.fileSystemFlags & 0x8000) != 0;
		Flags = partitionInfoEx2.partitionInfoEx2.flags;
		FreeSpace = partitionInfoEx2.partitionInfoEx2.freeSizeInBytes;
		PartitionNumber = partitionInfoEx2.partitionInfoEx2.partitionNumber;
		SerialNumber = partitionInfoEx2.partitionInfoEx2.serialNumber;
		Size = partitionInfoEx2.partitionInfoEx2.totalSizeInBytes;
		VolumeGuid = partitionInfoEx2.partitionInfoEx2.volumeGuid;
		Label = partitionInfoEx2.partitionInfoEx2.volumeLabel;
		GptPartitionId = partitionInfoEx2.partitionInfoEx2.gptPartitionId;
		PartitionName = partitionInfoEx2.partitionInfoEx2.partitionName;
		BitLockerFlags = partitionInfoEx2.partitionInfoEx2.bitLockerFlags;
		MountPoints = new List<string>();
		IncludeMountPoints = false;
		IsMaintenanceModeOn = null;
		CsvFaultState = ClusterSharedVolumeFaultState.NoFaults;
	}

	internal static string CalculateDisplayName(string label, string partitionName, bool hasDriveLetter, string driveLetter, IList<string> mountPoints, bool isCsvFs)
	{
		if (isCsvFs || mountPoints.Count == 0)
		{
			return CommonResources.DiskVolumeName_Text.FormatCurrentCulture(string.IsNullOrWhiteSpace(label) ? StorageResources.ClusteredDiskText : label, hasDriveLetter ? driveLetter : partitionName);
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string mountPoint in mountPoints)
		{
			stringBuilder.Append(mountPoint + " ");
		}
		return CommonResources.MountedVolumeFormat_Text.FormatCurrentCulture(stringBuilder.ToString().TrimEnd(' '));
	}

	private bool IsFileSystem(string fileSystem)
	{
		return string.Equals(fileSystem, FileSystem, StringComparison.OrdinalIgnoreCase);
	}

	private uint SetDriveLetterBit(int bitToSet)
	{
		return (uint)(1 << bitToSet);
	}

	internal void SetMountPoints(IEnumerable<string> mountPointList)
	{
		MountPoints.Clear();
		foreach (string mountPoint in mountPointList)
		{
			string text = mountPoint;
			while (text.EndsWith("\\", StringComparison.OrdinalIgnoreCase) || text.EndsWith(":", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring(0, text.Length - 1);
			}
			if (!string.Equals(text, DriveLetter, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(text))
			{
				MountPoints.Add(text);
			}
		}
	}
}
