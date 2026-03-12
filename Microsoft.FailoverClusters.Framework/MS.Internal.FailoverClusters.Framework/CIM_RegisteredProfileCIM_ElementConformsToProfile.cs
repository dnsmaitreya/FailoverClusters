using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_RegisteredProfileCIM_ElementConformsToProfile
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<CIM_ManagedElement> ManagedElement
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/interop", Instance, "CIM_ElementConformsToProfile", "CIM_ManagedElement", "ConformantStandard", "ManagedElement") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/interop", Instance, "CIM_ElementConformsToProfile", "CIM_ManagedElement", "ConformantStandard", "ManagedElement")
					select new CIM_ManagedElement(Session, i)).ToArray();
			}
			return null;
		}
	}

	public CIM_RegisteredProfileCIM_ElementConformsToProfile(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
