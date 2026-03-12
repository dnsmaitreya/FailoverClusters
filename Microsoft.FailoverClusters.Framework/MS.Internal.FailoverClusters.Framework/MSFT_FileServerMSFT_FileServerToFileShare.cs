using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_FileServerMSFT_FileServerToFileShare
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_FileShare> FileShare
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_FileServerToFileShare", "MSFT_FileShare", "FileServer", "FileShare") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_FileServerToFileShare", "MSFT_FileShare", "FileServer", "FileShare")
					select new MSFT_FileShare(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_FileServerMSFT_FileServerToFileShare(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
