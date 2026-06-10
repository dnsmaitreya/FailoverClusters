using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_DiskMSFT_DiskToPartition
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_Partition> Partition
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_DiskToPartition", "MSFT_Partition", "Disk", "Partition") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_DiskToPartition", "MSFT_Partition", "Disk", "Partition")
					select new MSFT_Partition(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_DiskMSFT_DiskToPartition(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

