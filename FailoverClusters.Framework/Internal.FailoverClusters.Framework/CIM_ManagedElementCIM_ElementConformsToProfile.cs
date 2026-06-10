using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_ManagedElementCIM_ElementConformsToProfile
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<CIM_RegisteredProfile> ConformantStandard
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/interop", Instance, "CIM_ElementConformsToProfile", "CIM_RegisteredProfile", "ManagedElement", "ConformantStandard") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/interop", Instance, "CIM_ElementConformsToProfile", "CIM_RegisteredProfile", "ManagedElement", "ConformantStandard")
					select new CIM_RegisteredProfile(Session, i)).ToArray();
			}
			return null;
		}
	}

	public CIM_ManagedElementCIM_ElementConformsToProfile(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

