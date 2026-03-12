using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_VolumeMSFT_FileServerToVolume
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_FileServer> FileServer
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_FileServerToVolume", "MSFT_FileServer", "Volume", "FileServer") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_FileServerToVolume", "MSFT_FileServer", "Volume", "FileServer")
					select new MSFT_FileServer(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_VolumeMSFT_FileServerToVolume(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
