using System.Collections.Generic;
using System.Management;

namespace Microsoft.FailoverClusters.Framework;

public class VirtualMachineStorageInformation
{
	public IEnumerable<VirtualHardDisk> VirtualHardDisks { get; private set; }

	public IEnumerable<VirtualMachineResourcePool> StoragePools { get; private set; }

	public string ConfigurationFolder { get; private set; }

	public string ConfigurationFile { get; private set; }

	public string PageFileFolder { get; private set; }

	public string SnapshotFolder { get; private set; }

	internal VirtualMachineStorageInformation(ManagementObject virtualSystemGlobalSettingData, IEnumerable<VirtualHardDisk> vhdPaths, IEnumerable<VirtualMachineResourcePool> storagePools)
	{
		VirtualHardDisks = vhdPaths;
		StoragePools = storagePools;
		InitializeInformation(virtualSystemGlobalSettingData);
	}

	private void InitializeInformation(ManagementBaseObject virtualSystemGlobalSettingData)
	{
		ConfigurationFolder = string.Empty;
		ConfigurationFile = string.Empty;
		PageFileFolder = string.Empty;
		SnapshotFolder = string.Empty;
		if (virtualSystemGlobalSettingData != null)
		{
			object obj = virtualSystemGlobalSettingData["ConfigurationDataRoot"];
			if (obj != null)
			{
				ConfigurationFolder = obj.ToString();
			}
			object obj2 = virtualSystemGlobalSettingData["ConfigurationFile"];
			if (obj2 != null)
			{
				ConfigurationFile = obj2.ToString();
			}
			object obj3 = virtualSystemGlobalSettingData["SwapFileDataRoot"];
			if (obj3 != null)
			{
				PageFileFolder = obj3.ToString();
			}
			object obj4 = virtualSystemGlobalSettingData["SnapshotDataRoot"];
			if (obj4 != null)
			{
				SnapshotFolder = obj4.ToString();
			}
		}
	}
}
