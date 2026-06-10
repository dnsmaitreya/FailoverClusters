using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_RegisteredProfileCIM_ReferencedProfile
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<CIM_RegisteredProfile> Dependent
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/interop", Instance, "CIM_ReferencedProfile", "CIM_RegisteredProfile", "Antecedent", "Dependent") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/interop", Instance, "CIM_ReferencedProfile", "CIM_RegisteredProfile", "Antecedent", "Dependent")
					select new CIM_RegisteredProfile(Session, i)).ToArray();
			}
			return null;
		}
	}

	public IEnumerable<CIM_RegisteredProfile> Antecedent
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/interop", Instance, "CIM_ReferencedProfile", "CIM_RegisteredProfile", "Dependent", "Antecedent") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/interop", Instance, "CIM_ReferencedProfile", "CIM_RegisteredProfile", "Dependent", "Antecedent")
					select new CIM_RegisteredProfile(Session, i)).ToArray();
			}
			return null;
		}
	}

	public CIM_RegisteredProfileCIM_ReferencedProfile(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

