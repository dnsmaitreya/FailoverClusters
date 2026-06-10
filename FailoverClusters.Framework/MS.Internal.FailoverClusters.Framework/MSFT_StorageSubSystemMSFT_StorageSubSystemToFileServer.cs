using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageSubSystemMSFT_StorageSubSystemToFileServer
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_FileServer> FileServer
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToFileServer", "MSFT_FileServer", "StorageSubSystem", "FileServer") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToFileServer", "MSFT_FileServer", "StorageSubSystem", "FileServer")
					select new MSFT_FileServer(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToFileServer(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

