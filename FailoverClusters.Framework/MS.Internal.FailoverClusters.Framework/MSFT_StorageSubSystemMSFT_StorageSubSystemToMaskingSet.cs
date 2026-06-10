using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageSubSystemMSFT_StorageSubSystemToMaskingSet
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_MaskingSet> MaskingSet
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToMaskingSet", "MSFT_MaskingSet", "StorageSubSystem", "MaskingSet") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToMaskingSet", "MSFT_MaskingSet", "StorageSubSystem", "MaskingSet")
					select new MSFT_MaskingSet(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToMaskingSet(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

