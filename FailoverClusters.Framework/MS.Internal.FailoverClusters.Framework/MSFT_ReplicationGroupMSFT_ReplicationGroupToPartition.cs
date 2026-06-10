using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_ReplicationGroupMSFT_ReplicationGroupToPartition
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_Partition> Partition
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_ReplicationGroupToPartition", "MSFT_Partition", "ReplicationGroup", "Partition") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_ReplicationGroupToPartition", "MSFT_Partition", "ReplicationGroup", "Partition")
					select new MSFT_Partition(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_ReplicationGroupMSFT_ReplicationGroupToPartition(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

