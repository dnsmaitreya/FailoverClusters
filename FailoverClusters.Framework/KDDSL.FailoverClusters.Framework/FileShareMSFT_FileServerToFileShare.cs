using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_FileShareMSFT_FileServerToFileShare
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_FileServer> FileServer
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_FileServerToFileShare", "MSFT_FileServer", "FileShare", "FileServer") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_FileServerToFileShare", "MSFT_FileServer", "FileShare", "FileServer")
					select new MSFT_FileServer(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_FileShareMSFT_FileServerToFileShare(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

