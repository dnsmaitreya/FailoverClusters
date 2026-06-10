using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_RegisteredProfile : CIM_ManagedElement
{
	public new CIM_RegisteredProfileCIM_ElementConformsToProfile CIM_ElementConformsToProfile { get; private set; }

	public CIM_RegisteredProfileCIM_ReferencedProfile CIM_ReferencedProfile { get; private set; }

	public string[] AdvertiseTypeDescriptions
	{
		get
		{
			return (string[])base.Instance.CimInstanceProperties["AdvertiseTypeDescriptions"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["AdvertiseTypeDescriptions"].Value = value;
		}
	}

	public ushort[] AdvertiseTypes
	{
		get
		{
			return (ushort[])base.Instance.CimInstanceProperties["AdvertiseTypes"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["AdvertiseTypes"].Value = value;
		}
	}

	public string OtherRegisteredOrganization
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["OtherRegisteredOrganization"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["OtherRegisteredOrganization"].Value = value;
		}
	}

	public string RegisteredName
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["RegisteredName"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["RegisteredName"].Value = value;
		}
	}

	public ushort? RegisteredOrganization
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["RegisteredOrganization"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["RegisteredOrganization"].Value = value;
		}
	}

	public string RegisteredVersion
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["RegisteredVersion"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["RegisteredVersion"].Value = value;
		}
	}

	public CIM_RegisteredProfile()
	{
	}

	public CIM_RegisteredProfile(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		CIM_ElementConformsToProfile = new CIM_RegisteredProfileCIM_ElementConformsToProfile(session, instance);
		CIM_ReferencedProfile = new CIM_RegisteredProfileCIM_ReferencedProfile(session, instance);
	}

	public static CIM_RegisteredProfile GetInstance(CimSession session, string InstanceID)
	{
		CimInstance cimInstance = new CimInstance("CIM_RegisteredProfile", "root/interop");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("InstanceID", InstanceID, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/interop", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new CIM_RegisteredProfile(session, instance);
	}

	public static CIM_RegisteredProfile CreateReference(CimSession session, string InstanceID)
	{
		CimInstance cimInstance = new CimInstance("CIM_RegisteredProfile", "root/interop");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("InstanceID", InstanceID, CimFlags.Key));
		return new CIM_RegisteredProfile(session, cimInstance);
	}

	public new static IEnumerable<CIM_RegisteredProfile> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/interop", "CIM_RegisteredProfile")
			select new CIM_RegisteredProfile(session, i);
	}

	public new static IEnumerable<CIM_RegisteredProfile> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM CIM_RegisteredProfile";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/interop", "WQL", text)
			select new CIM_RegisteredProfile(session, i);
	}

	public override void Refresh()
	{
		base.Instance = base.Session.GetInstance("root/interop", base.Instance);
	}

	public override void Delete()
	{
		base.Session.DeleteInstance("root/interop", base.Instance);
	}

	public override void Save()
	{
		base.Session.ModifyInstance("root/interop", base.Instance);
	}
}

