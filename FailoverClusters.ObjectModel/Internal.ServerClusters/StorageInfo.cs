using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace MS.Internal.ServerClusters;

public class StorageInfo
{
	private int m_numDisks;

	private int m_numDisksOnline;

	private ulong m_totalCapacity;

	private ulong m_totalFreeSpace;

	public ulong FreeBytes => m_totalFreeSpace;

	public ulong TotalBytes => m_totalCapacity;

	public int OnlineDiskCount => m_numDisksOnline;

	public int DiskCount => m_numDisks;

	public StorageInfo()
	{
		m_numDisks = 0;
		m_numDisksOnline = 0;
		m_totalCapacity = 0uL;
		m_totalFreeSpace = 0uL;
	}

	public void AddDisk(ClusterResource storageResource)
	{
		//Discarded unreachable code: IL_011f
		if (storageResource == null)
		{
			throw new ArgumentNullException("storageResource");
		}
		if (!storageResource.IsStorage)
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StorageResourceIsNotStorageResource, storageResource.Name));
		}
		m_numDisks++;
		if (storageResource.State != ResourceState.Online)
		{
			return;
		}
		m_numDisksOnline++;
		try
		{
			IEnumerator<ClusterDiskPartition> enumerator = storageResource.Storage_GetDiskInfo(includeMountPoints: false).Partitions.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterDiskPartition current = enumerator.Current;
					bool flag = false;
					flag = current.IsCompressed;
					m_totalCapacity = current.Size + m_totalCapacity;
					m_totalFreeSpace = current.FreeSpace + m_totalFreeSpace;
				}
			}
			finally
			{
				IEnumerator<ClusterDiskPartition> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		catch (Exception ex)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
			if (firstException != null)
			{
				if (firstException.NativeErrorCode != -2147024895 && firstException.NativeErrorCode != -2147024891)
				{
					throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
					{
						Resources.DiskInfoFailed_Text,
						storageResource.DisplayName
					});
				}
				ExceptionHelp.LogException(ex, "Exception while querying the partition info for '{0}'", storageResource.DisplayName);
				return;
			}
			throw;
		}
	}

	public double GetPercentFreeCapacity()
	{
		double result = 0.0;
		ulong totalCapacity = m_totalCapacity;
		if (totalCapacity != 0L)
		{
			result = (double)m_totalFreeSpace / (double)totalCapacity * 100.0;
		}
		return result;
	}
}
