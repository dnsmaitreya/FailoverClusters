using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class ClusterDiskPartition
{
	private string m_name;

	private string m_label;

	private ulong m_size;

	private ulong m_freeSpace;

	private string m_fileSystem;

	private uint m_partitionNumber;

	private Guid m_volumeGuid;

	private Collection<string> m_mountPoints;

	private bool m_isCompressed;

	private DirtyState m_dirtyState;

	public bool IsPartitionSizeValid
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_size != 0;
		}
	}

	public bool IsPartitionNumberValid
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_partitionNumber != uint.MaxValue;
		}
	}

	public ICollection<string> MountPoints => new ReadOnlyCollection<string>(m_mountPoints);

	public uint DriveLetterMask
	{
		get
		{
			char c = char.ToLowerInvariant('a');
			string driveLetter = DriveLetter;
			int result = 0;
			if (driveLetter != null)
			{
				char c2 = driveLetter[0];
				int driveLetterBit = char.ToLowerInvariant(c2) - c;
				result = (int)SetDriveLetterBit(driveLetterBit);
			}
			return (uint)result;
		}
	}

	public float PercentFree => (float)((double)(float)m_freeSpace / (double)(float)m_size * 100.0);

	public ulong FreeSpace => m_freeSpace;

	public ulong UsedSpace => m_size - m_freeSpace;

	public ulong Size => m_size;

	public uint PartitionNumber => m_partitionNumber;

	public bool HasDriveLetter
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (!(null == DriveLetter)) ? true : false;
		}
	}

	public string DriveLetter
	{
		get
		{
			if (m_name.Length == 2 && m_name[1] == ':')
			{
				return m_name.Substring(0, 1);
			}
			return null;
		}
	}

	public bool IsReFs
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsFileSystem("ReFS");
		}
	}

	public bool IsNtfs
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsFileSystem("NTFS");
		}
	}

	public bool IsFormatted
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			if (!IsFileSystem("RAW") && !IsFileSystem(string.Empty))
			{
				return true;
			}
			return false;
		}
	}

	public DirtyState IsDirty => m_dirtyState;

	public bool IsCompressed
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_isCompressed;
		}
	}

	public ValueType VolumeGuid => m_volumeGuid;

	public string FileSystem => m_fileSystem;

	public string Label => m_label;

	public string Name => m_name;

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsFileSystem(string fileSystem)
	{
		return 0 == string.Compare(fileSystem, m_fileSystem, StringComparison.OrdinalIgnoreCase);
	}

	private uint SetDriveLetterBit(int bitToSet)
	{
		return (uint)(1 << bitToSet);
	}

	public string BuildPath(string relativePath)
	{
		if (relativePath[0] == '\\')
		{
			do
			{
				relativePath = relativePath.Substring(1, relativePath.Length - 1);
			}
			while (relativePath[0] == '\\');
		}
		return (null == DriveLetter) ? BuildVolumeGuidPath(relativePath) : string.Format(CultureInfo.InvariantCulture, "{0}:\\{1}", DriveLetter, relativePath);
	}

	public string BuildVolumeGuidPath(string relativePath)
	{
		return string.Format(CultureInfo.InvariantCulture, "\\\\?\\Volume{0:B}\\{1}", m_volumeGuid, relativePath);
	}

	internal ClusterDiskPartition(ManagementObject managementObject)
	{
		if (managementObject == null)
		{
			throw new ArgumentNullException("managementObject");
		}
		m_name = (string)managementObject["Path"];
		m_fileSystem = (string)managementObject["FileSystem"];
		m_size = (uint)managementObject["TotalSize"];
		m_freeSpace = (uint)managementObject["FreeSpace"];
		m_partitionNumber = (uint)managementObject["PartitionNumber"];
		Guid volumeGuid = new Guid((string)managementObject["VolumeGuid"]);
		m_volumeGuid = volumeGuid;
		m_mountPoints = new Collection<string>();
		bool isCompressed = (byte)(((uint)managementObject["FileSystemFlags"] >> 15) & (true ? 1u : 0u)) != 0;
		m_isCompressed = isCompressed;
		m_dirtyState = DirtyState.Unknown;
	}

	internal unsafe ClusterDiskPartition(CLUS_PARTITION_INFO_EX* pPartitionInfo)
	{
		//IL_001e: Expected I, but got I8
		//IL_0031: Expected I, but got I8
		//IL_0044: Expected I, but got I8
		if (pPartitionInfo == null)
		{
			throw new ArgumentNullException("pPartitionInfo");
		}
		m_name = InteropHelp.WstrToString((ushort*)((ulong)(nint)pPartitionInfo + 4uL));
		m_label = InteropHelp.WstrToString((ushort*)((ulong)(nint)pPartitionInfo + 524uL));
		m_fileSystem = InteropHelp.WstrToString((ushort*)((ulong)(nint)pPartitionInfo + 1056uL));
		m_size = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>((void*)((long)(nint)pPartitionInfo + 1120L));
		m_freeSpace = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>((void*)((long)(nint)pPartitionInfo + 1128L));
		m_partitionNumber = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>((void*)((long)(nint)pPartitionInfo + 1140L));
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _GUID gUID);
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref gUID, (long)(nint)pPartitionInfo + 1144L, 16);
		Guid volumeGuid = new Guid(*(uint*)(&gUID), System.Runtime.CompilerServices.Unsafe.As<_GUID, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref gUID, 4)), System.Runtime.CompilerServices.Unsafe.As<_GUID, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref gUID, 6)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref gUID, 8)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref gUID, 9)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref gUID, 10)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref gUID, 11)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref gUID, 12)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref gUID, 13)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref gUID, 14)), System.Runtime.CompilerServices.Unsafe.As<_GUID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref gUID, 15)));
		m_volumeGuid = volumeGuid;
		m_mountPoints = new Collection<string>();
		bool isCompressed = (byte)(((uint)System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>((void*)((long)(nint)pPartitionInfo + 1052L)) >> 15) & (true ? 1u : 0u)) != 0;
		m_isCompressed = isCompressed;
		m_dirtyState = DirtyState.Unknown;
	}

	internal unsafe ClusterDiskPartition(CLUS_PARTITION_INFO* pPartitionInfo)
	{
		//IL_001e: Expected I, but got I8
		//IL_0031: Expected I, but got I8
		//IL_0044: Expected I, but got I8
		if (pPartitionInfo == null)
		{
			throw new ArgumentNullException("pPartitionInfo");
		}
		m_name = InteropHelp.WstrToString((ushort*)((ulong)(nint)pPartitionInfo + 4uL));
		m_label = InteropHelp.WstrToString((ushort*)((ulong)(nint)pPartitionInfo + 524uL));
		m_fileSystem = InteropHelp.WstrToString((ushort*)((ulong)(nint)pPartitionInfo + 1056uL));
		m_size = 0uL;
		m_freeSpace = 0uL;
		m_partitionNumber = uint.MaxValue;
		m_volumeGuid = Guid.Empty;
		m_mountPoints = new Collection<string>();
		bool isCompressed = (byte)(((uint)(*(int*)((ulong)(nint)pPartitionInfo + 1052uL)) >> 15) & (true ? 1u : 0u)) != 0;
		m_isCompressed = isCompressed;
		m_dirtyState = DirtyState.Unknown;
	}

	internal void SetMountPoints(StringCollection mountPointList)
	{
		StringEnumerator enumerator = mountPointList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string text = enumerator.Current;
				while (text.EndsWith("\\", StringComparison.OrdinalIgnoreCase) || text.EndsWith(":", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(0, text.Length - 1);
				}
				if (0 != string.Compare(text, DriveLetter, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(text))
				{
					m_mountPoints.Add(text);
				}
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	internal void SetDirty(DirtyState state)
	{
		m_dirtyState = state;
	}
}
