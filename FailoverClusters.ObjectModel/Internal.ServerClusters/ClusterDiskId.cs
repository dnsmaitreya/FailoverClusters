using System;
using System.Runtime.CompilerServices;

namespace MS.Internal.ServerClusters;

public abstract class ClusterDiskId
{
	public abstract DiskIdType IdType { get; }

	private static ClusterDiskNumberId CreateDiskNumber(uint number)
	{
		return new ClusterDiskNumberId(number);
	}

	public static ClusterDiskSharedVolumeId CreateSharedVolume(string diskId)
	{
		return new ClusterDiskSharedVolumeId(diskId);
	}

	internal unsafe static ClusterDiskId CreateDiskId(_CLUSDSK_DISKID diskId)
	{
		if (*(int*)(&diskId) != 0)
		{
			if (*(int*)(&diskId) != 1)
			{
				if (*(int*)(&diskId) != 4000)
				{
					return null;
				}
				return new ClusterDiskNumberId(System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 4)));
			}
			Guid guid = new Guid(System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 4)), System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 8)), System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 10)), System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 12)), System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 13)), System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 14)), System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 15)), System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 16)), System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 17)), System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 18)), System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 19)));
			return new ClusterDiskGuidId(guid);
		}
		return new ClusterDiskSignatureId(System.Runtime.CompilerServices.Unsafe.As<_CLUSDSK_DISKID, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref diskId, 4)));
	}

	public static ClusterDiskId CreateDiskId(ClusterResource storageResource)
	{
		//Discarded unreachable code: IL_0030, IL_0032
		if (storageResource == null)
		{
			throw new ArgumentNullException("storageResource");
		}
		try
		{
			return storageResource.Storage_GetDiskInfo(includeMountPoints: false).DiskId;
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "There was an error getting the disk id from resource '{0}'.", storageResource.DisplayName);
			throw;
		}
	}

	internal static ClusterDiskSignatureId CreateDiskSignature(uint signature)
	{
		return new ClusterDiskSignatureId(signature);
	}

	internal static ClusterDiskGuidId CreateDiskGuid(Guid guid)
	{
		return new ClusterDiskGuidId(guid);
	}

	internal unsafe abstract _CLUSDSK_DISKID* GetClusDiskId(_CLUSDSK_DISKID* P_0);
}
