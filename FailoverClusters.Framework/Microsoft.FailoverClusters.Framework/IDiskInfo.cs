namespace FailoverClusters.Framework;

public interface IDiskInfo : IKeyQueryable
{
	string ObjectId { get; }

	string DeviceId { get; }

	string FriendlyName { get; }

	UniqueIdFormat UniqueIdFormat { get; }

	ulong Size { get; }

	ulong AllocatedSize { get; }

	ulong PhysicalSectorSize { get; }

	ulong LogicalSectorSize { get; }
}

