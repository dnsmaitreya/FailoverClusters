using System.Collections.Generic;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualMachineStorageMoveParameters
{
	private List<VirtualHardDisk> virtualHardDisks;

	public VirtualMachineResource Resource { get; private set; }

	public List<VirtualHardDisk> VirtualHardDisks => virtualHardDisks;

	public string SnapshotFolder { get; set; }

	public string PageFileFolder { get; set; }

	public string ConfigurationFolder { get; set; }

	public VirtualMachineStorageMoveParameters(VirtualMachineResource virtualMachineResource)
	{
		Exceptions.ThrowIfNull(virtualMachineResource, "virtualMachineResource");
		virtualHardDisks = new List<VirtualHardDisk>();
		Resource = virtualMachineResource;
	}
}

