using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_VolumeMSFT_StorageNodeToVolume
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageNode> StorageNode
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageNodeToVolume", "MSFT_StorageNode", "Volume", "StorageNode") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageNodeToVolume", "MSFT_StorageNode", "Volume", "StorageNode")
					select new MSFT_StorageNode(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_VolumeMSFT_StorageNodeToVolume(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

