using System;

namespace Microsoft.FailoverClusters.Framework;

public class VirtualHardDisk
{
	public string Path { get; private set; }

	public ulong Size { get; set; }

	public string PoolId { get; private set; }

	public string DestinationPath { get; set; }

	public string DestinationPoolId { get; set; }

	public bool PersistentReservationsSupported { get; set; }

	public bool IsInPrimordialPool => PoolId == string.Empty;

	public VirtualHardDiskType VirtualHardDiskType
	{
		get
		{
			if (string.IsNullOrEmpty(Path))
			{
				return VirtualHardDiskType.Unknown;
			}
			if (Path.EndsWith(VirtualHardDiskType.Vhd.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				return VirtualHardDiskType.Vhd;
			}
			if (Path.EndsWith(VirtualHardDiskType.Vhdx.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				return VirtualHardDiskType.Vhdx;
			}
			if (Path.EndsWith(VirtualHardDiskType.Iso.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				return VirtualHardDiskType.Iso;
			}
			return VirtualHardDiskType.Unknown;
		}
	}

	public VirtualHardDisk(string path, string poolId, bool persistentReservationsSupported)
	{
		Path = path;
		Size = 0uL;
		PoolId = poolId ?? string.Empty;
		PersistentReservationsSupported = persistentReservationsSupported;
	}
}
