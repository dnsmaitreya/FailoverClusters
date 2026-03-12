using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_FileServerMSFT_FileServerToVolume
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_Volume> Volume
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_FileServerToVolume", "MSFT_Volume", "FileServer", "Volume") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_FileServerToVolume", "MSFT_Volume", "FileServer", "Volume")
					select new MSFT_Volume(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_FileServerMSFT_FileServerToVolume(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
