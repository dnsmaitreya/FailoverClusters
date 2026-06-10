using System.Collections.Generic;
using System.Windows.Input;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public abstract class DiskInfo<T> : ObservableCollectionItem<T> where T : IDiskInfo
{
	private readonly CommandCollection commands = new CommandCollection(ClusterCommandCollectionId.DiskInfo);

	public override string Key => ObjectId;

	public override string DisplayName => FriendlyName;

	public override IEnumerable<ICommand> Commands => commands;

	public string ObjectId { get; internal set; }

	public string DeviceId { get; internal set; }

	public string FriendlyName { get; internal set; }

	public UniqueIdFormat UniqueIdFormat { get; internal set; }

	public ulong Size { get; internal set; }

	public ulong AllocatedSize { get; internal set; }

	public ulong PhysicalSectorSize { get; internal set; }

	public ulong LogicalSectorSize { get; internal set; }

	protected DiskInfo(ClusterObject owner)
		: base(owner)
	{
	}

	public override void CopyFrom(T source)
	{
		Exceptions.ThrowIfNull(source, "source");
		if (ObjectId != source.ObjectId)
		{
			ObjectId = source.ObjectId;
			OnPropertyChanged("ObjectId");
		}
		if (DeviceId != source.DeviceId)
		{
			DeviceId = source.DeviceId;
			OnPropertyChanged("DeviceId");
		}
		if (FriendlyName != source.FriendlyName)
		{
			FriendlyName = source.FriendlyName;
			OnPropertyChanged("FriendlyName");
			OnPropertyChanged("DisplayName");
		}
		if (UniqueIdFormat != source.UniqueIdFormat)
		{
			UniqueIdFormat = source.UniqueIdFormat;
			OnPropertyChanged("UniqueIdFormat");
		}
		if (Size != source.Size)
		{
			Size = source.Size;
			OnPropertyChanged("Size");
		}
		if (AllocatedSize != source.AllocatedSize)
		{
			AllocatedSize = source.AllocatedSize;
			OnPropertyChanged("AllocatedSize");
		}
		if (PhysicalSectorSize != source.PhysicalSectorSize)
		{
			PhysicalSectorSize = source.PhysicalSectorSize;
			OnPropertyChanged("PhysicalSectorSize");
		}
		if (LogicalSectorSize != source.LogicalSectorSize)
		{
			LogicalSectorSize = source.LogicalSectorSize;
			OnPropertyChanged("LogicalSectorSize");
		}
	}
}

