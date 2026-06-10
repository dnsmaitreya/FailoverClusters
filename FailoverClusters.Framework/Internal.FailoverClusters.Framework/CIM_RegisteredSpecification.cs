using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_RegisteredSpecification : CIM_ManagedElement
{
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

	public string OtherSpecificationType
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["OtherSpecificationType"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["OtherSpecificationType"].Value = value;
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

	public ushort? SpecificationType
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["SpecificationType"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SpecificationType"].Value = value;
		}
	}

	public CIM_RegisteredSpecification()
	{
	}

	public CIM_RegisteredSpecification(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static CIM_RegisteredSpecification GetInstance(CimSession session, string InstanceID)
	{
		CimInstance cimInstance = new CimInstance("CIM_RegisteredSpecification", "root/interop");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("InstanceID", InstanceID, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/interop", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new CIM_RegisteredSpecification(session, instance);
	}

	public static CIM_RegisteredSpecification CreateReference(CimSession session, string InstanceID)
	{
		CimInstance cimInstance = new CimInstance("CIM_RegisteredSpecification", "root/interop");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("InstanceID", InstanceID, CimFlags.Key));
		return new CIM_RegisteredSpecification(session, cimInstance);
	}

	public new static IEnumerable<CIM_RegisteredSpecification> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/interop", "CIM_RegisteredSpecification")
			select new CIM_RegisteredSpecification(session, i);
	}

	public new static IEnumerable<CIM_RegisteredSpecification> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM CIM_RegisteredSpecification";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/interop", "WQL", text)
			select new CIM_RegisteredSpecification(session, i);
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

