using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_VirtualDiskMSFT_VirtualDiskToVirtualDisk
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_VirtualDisk> TargetVirtualDisk
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_VirtualDiskToVirtualDisk", "MSFT_VirtualDisk", "SourceVirtualDisk", "TargetVirtualDisk") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_VirtualDiskToVirtualDisk", "MSFT_VirtualDisk", "SourceVirtualDisk", "TargetVirtualDisk")
					select new MSFT_VirtualDisk(Session, i)).ToArray();
			}
			return null;
		}
	}

	public IEnumerable<MSFT_VirtualDisk> SourceVirtualDisk
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_VirtualDiskToVirtualDisk", "MSFT_VirtualDisk", "TargetVirtualDisk", "SourceVirtualDisk") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_VirtualDiskToVirtualDisk", "MSFT_VirtualDisk", "TargetVirtualDisk", "SourceVirtualDisk")
					select new MSFT_VirtualDisk(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_VirtualDiskMSFT_VirtualDiskToVirtualDisk(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

