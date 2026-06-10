using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_PartitionMSFT_DiskToPartition
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_Disk> Disk
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_DiskToPartition", "MSFT_Disk", "Partition", "Disk") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_DiskToPartition", "MSFT_Disk", "Partition", "Disk")
					select new MSFT_Disk(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_PartitionMSFT_DiskToPartition(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

