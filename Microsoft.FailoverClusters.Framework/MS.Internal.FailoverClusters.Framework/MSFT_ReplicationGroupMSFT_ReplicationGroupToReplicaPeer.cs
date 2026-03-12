using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_ReplicationGroupMSFT_ReplicationGroupToReplicaPeer
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_ReplicaPeer> ReplicaPeer
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_ReplicationGroupToReplicaPeer", "MSFT_ReplicaPeer", "ReplicationGroup", "ReplicaPeer") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_ReplicationGroupToReplicaPeer", "MSFT_ReplicaPeer", "ReplicationGroup", "ReplicaPeer")
					select new MSFT_ReplicaPeer(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_ReplicationGroupMSFT_ReplicationGroupToReplicaPeer(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
