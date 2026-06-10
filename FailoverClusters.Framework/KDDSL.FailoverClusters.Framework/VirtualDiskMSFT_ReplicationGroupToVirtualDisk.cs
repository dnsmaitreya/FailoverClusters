using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_VirtualDiskMSFT_ReplicationGroupToVirtualDisk
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_ReplicationGroup> ReplicationGroup
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_ReplicationGroupToVirtualDisk", "MSFT_ReplicationGroup", "VirtualDisk", "ReplicationGroup") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_ReplicationGroupToVirtualDisk", "MSFT_ReplicationGroup", "VirtualDisk", "ReplicationGroup")
					select new MSFT_ReplicationGroup(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_VirtualDiskMSFT_ReplicationGroupToVirtualDisk(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

