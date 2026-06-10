using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_ReplicaPeerMSFT_ReplicationGroupToReplicaPeer
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_ReplicationGroup> ReplicationGroup
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_ReplicationGroupToReplicaPeer", "MSFT_ReplicationGroup", "ReplicaPeer", "ReplicationGroup") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_ReplicationGroupToReplicaPeer", "MSFT_ReplicationGroup", "ReplicaPeer", "ReplicationGroup")
					select new MSFT_ReplicationGroup(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_ReplicaPeerMSFT_ReplicationGroupToReplicaPeer(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

