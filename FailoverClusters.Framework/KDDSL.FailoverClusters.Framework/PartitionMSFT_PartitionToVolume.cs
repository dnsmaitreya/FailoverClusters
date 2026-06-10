using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_PartitionMSFT_PartitionToVolume
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_Volume> Volume
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_PartitionToVolume", "MSFT_Volume", "Partition", "Volume") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_PartitionToVolume", "MSFT_Volume", "Partition", "Volume")
					select new MSFT_Volume(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_PartitionMSFT_PartitionToVolume(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

