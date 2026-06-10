using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class MsftPartitionInfo : ObservableCollectionItem<MsftPartitionInfo>
{
	private readonly CommandCollection commands = new CommandCollection(ClusterCommandCollectionId.DiskInfo);

	private const char KeySeparator = '|';

	public override string Key => BuildKey(DiskId, Offset);

	public override string DisplayName => DriveLetter.ToString(CultureInfo.InvariantCulture);

	public override IEnumerable<ICommand> Commands => commands;

	public IList<string> AccessPaths { get; internal set; }

	public string DiskId { get; internal set; }

	public uint DiskNumber { get; internal set; }

	public char DriveLetter { get; internal set; }

	public bool IsActive { get; internal set; }

	public bool IsBoot { get; internal set; }

	public bool IsHidden { get; internal set; }

	public bool IsOffline { get; internal set; }

	public bool IsReadOnly { get; internal set; }

	public bool IsShadowCopy { get; internal set; }

	public bool IsSystem { get; internal set; }

	public PartitionMbrType MbrType { get; internal set; }

	public bool NoDefaultDriveLetter { get; internal set; }

	public ulong Offset { get; internal set; }

	public PartitionOperationalStatus OperationalStatus { get; internal set; }

	public uint PartitionNumber { get; internal set; }

	public ulong Size { get; internal set; }

	public PartitionTransitionState TransitionState { get; internal set; }

	public IEnumerable<MsftVolumeInfo> Volumes => ObservableVolumeCollection.GetAssociation(this, base.Cluster, this);

	public MsftPartitionInfo(ClusterObject owner)
		: base(owner)
	{
	}

	public static void GetInstance(Cluster cluster, string key, Action<OperationResult<MsftPartitionInfo>> operationResult, string serverName = null)
	{
		ObservableCollectionItem<MsftPartitionInfo>.GetInstance(cluster, key, operationResult, serverName);
	}

	public override void CopyFrom(MsftPartitionInfo source)
	{
		Exceptions.ThrowIfNull(source, "source");
		if (AccessPaths != source.AccessPaths)
		{
			AccessPaths = source.AccessPaths;
			OnPropertyChanged("AccessPaths");
		}
		if (DiskId != source.DiskId)
		{
			DiskId = source.DiskId;
			OnPropertyChanged("DiskId");
		}
		if (DiskNumber != source.DiskNumber)
		{
			DiskNumber = source.DiskNumber;
			OnPropertyChanged("DiskNumber");
		}
		if (DriveLetter != source.DriveLetter)
		{
			DriveLetter = source.DriveLetter;
			OnPropertyChanged("DriveLetter");
		}
		if (IsActive != source.IsActive)
		{
			IsActive = source.IsActive;
			OnPropertyChanged("IsActive");
		}
		if (IsBoot != source.IsBoot)
		{
			IsBoot = source.IsBoot;
			OnPropertyChanged("IsBoot");
		}
		if (IsHidden != source.IsHidden)
		{
			IsHidden = source.IsHidden;
			OnPropertyChanged("IsHidden");
		}
		if (IsOffline != source.IsOffline)
		{
			IsOffline = source.IsOffline;
			OnPropertyChanged("IsOffline");
		}
		if (IsReadOnly != source.IsReadOnly)
		{
			IsReadOnly = source.IsReadOnly;
			OnPropertyChanged("IsReadOnly");
		}
		if (IsShadowCopy != source.IsShadowCopy)
		{
			IsShadowCopy = source.IsShadowCopy;
			OnPropertyChanged("IsShadowCopy");
		}
		if (IsSystem != source.IsSystem)
		{
			IsSystem = source.IsSystem;
			OnPropertyChanged("IsSystem");
		}
		if (MbrType != source.MbrType)
		{
			MbrType = source.MbrType;
			OnPropertyChanged("MbrType");
		}
		if (NoDefaultDriveLetter != source.NoDefaultDriveLetter)
		{
			NoDefaultDriveLetter = source.NoDefaultDriveLetter;
			OnPropertyChanged("NoDefaultDriveLetter");
		}
		if (Offset != source.Offset)
		{
			Offset = source.Offset;
			OnPropertyChanged("Offset");
		}
		if (OperationalStatus != source.OperationalStatus)
		{
			OperationalStatus = source.OperationalStatus;
			OnPropertyChanged("OperationalStatus");
		}
		if (PartitionNumber != source.PartitionNumber)
		{
			PartitionNumber = source.PartitionNumber;
			OnPropertyChanged("PartitionNumber");
		}
		if (Size != source.Size)
		{
			Size = source.Size;
			OnPropertyChanged("Size");
		}
		if (TransitionState != source.TransitionState)
		{
			TransitionState = source.TransitionState;
			OnPropertyChanged("TransitionState");
		}
	}

	public static string BuildKey(string diskId, ulong offset)
	{
		return diskId + "|" + offset;
	}

	public static void ParseKey(string key, out string diskId, out ulong offset)
	{
		string[] array = key.Split('|');
		if (array.Length != 2)
		{
			throw new ArgumentException("The partition key is not in the correct format.", "key");
		}
		diskId = array[0];
		offset = ulong.Parse(array[1], CultureInfo.InvariantCulture);
	}
}

