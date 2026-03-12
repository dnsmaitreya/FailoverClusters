using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_ManagedElement : MiInstanceBase
{
	public CIM_ManagedElementCIM_ElementConformsToProfile CIM_ElementConformsToProfile { get; private set; }

	public string Caption
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["Caption"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["Caption"].Value = value;
		}
	}

	public string Description
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["Description"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["Description"].Value = value;
		}
	}

	public string ElementName
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["ElementName"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ElementName"].Value = value;
		}
	}

	public string InstanceID
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["InstanceID"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["InstanceID"].Value = value;
		}
	}

	public CIM_ManagedElement()
	{
	}

	public CIM_ManagedElement(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		CIM_ElementConformsToProfile = new CIM_ManagedElementCIM_ElementConformsToProfile(session, instance);
	}

	public static IEnumerable<CIM_ManagedElement> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/interop", "CIM_ManagedElement")
			select new CIM_ManagedElement(session, i);
	}

	public static IEnumerable<CIM_ManagedElement> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM CIM_ManagedElement";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/interop", "WQL", text)
			select new CIM_ManagedElement(session, i);
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
