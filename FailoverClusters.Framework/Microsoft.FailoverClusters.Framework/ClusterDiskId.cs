using System;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public abstract class ClusterDiskId
{
	public abstract DiskIdType IdType { get; }

	internal abstract NativeMethods.CLUSDSK_DISKID GetClusDiskId();

	internal static ClusterDiskSignatureId CreateDiskSignature(ulong signature)
	{
		return new ClusterDiskSignatureId(signature);
	}

	private static ClusterDiskNumberId CreateDiskNumber(ulong number)
	{
		return new ClusterDiskNumberId(number);
	}

	internal static ClusterDiskGuidId CreateDiskGuid(Guid guid)
	{
		return new ClusterDiskGuidId(guid);
	}

	public static ClusterDiskSharedVolumeId CreateSharedVolume(string sharedVolumeId)
	{
		return new ClusterDiskSharedVolumeId(sharedVolumeId);
	}

	internal static ClusterDiskId CreateDiskId(PResource storageResource)
	{
		Exceptions.ThrowIfNull(storageResource, "storageResource");
		try
		{
			storageResource.LoadObject(4);
			ClusterDiskId result = null;
			uint typedValue = ((ClusterPropertyUInt)storageResource.Properties["DiskSignature"]).TypedValue;
			string typedValue2 = ((ClusterPropertyString)storageResource.Properties["DiskIdGuid"]).TypedValue;
			if (typedValue2.Length != 0)
			{
				try
				{
					result = CreateDiskGuid(new Guid(typedValue2));
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
			}
			if (typedValue != 0)
			{
				try
				{
					result = CreateDiskSignature(typedValue);
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
			}
			return result;
		}
		catch (Exception exception)
		{
			ClusterLog.LogException(exception, "There was an error getting the disk id from resource '{0}'.", storageResource.Name);
			return null;
		}
	}

	internal static ClusterDiskId CreateDiskId(NativeMethods.CLUSDSK_DISKID diskId)
	{
		return diskId.DiskIdType switch
		{
			DiskIdType.Signature => CreateDiskSignature(diskId.DiskSignature), 
			DiskIdType.Guid => CreateDiskGuid(diskId.DiskGuid), 
			DiskIdType.Number => CreateDiskNumber(diskId.DeviceNumber), 
			_ => null, 
		};
	}
}

