using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageSubSystemMSFT_StorageSubSystemToReplicationCapabilities
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_ReplicationCapabilities> ReplicationCapabilities
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageSubSystemToReplicationCapabilities", "MSFT_ReplicationCapabilities", "StorageSubSystem", "ReplicationCapabilities") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageSubSystemToReplicationCapabilities", "MSFT_ReplicationCapabilities", "StorageSubSystem", "ReplicationCapabilities")
					select new MSFT_ReplicationCapabilities(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToReplicationCapabilities(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

