using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_VolumeMSFT_VolumeToFileShare
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_FileShare> FileShare
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_VolumeToFileShare", "MSFT_FileShare", "Volume", "FileShare") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_VolumeToFileShare", "MSFT_FileShare", "Volume", "FileShare")
					select new MSFT_FileShare(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_VolumeMSFT_VolumeToFileShare(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

