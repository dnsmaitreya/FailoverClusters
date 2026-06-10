using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_VolumeMSFT_DiskImageToVolume
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_DiskImage> DiskImage
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_DiskImageToVolume", "MSFT_DiskImage", "Volume", "DiskImage") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_DiskImageToVolume", "MSFT_DiskImage", "Volume", "DiskImage")
					select new MSFT_DiskImage(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_VolumeMSFT_DiskImageToVolume(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

