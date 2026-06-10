using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class Win32_PowerSupplyProfile : CIM_RegisteredProfile
{
	public Win32_PowerSupplyProfile()
	{
	}

	public Win32_PowerSupplyProfile(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static Win32_PowerSupplyProfile GetInstance(CimSession session, string InstanceID)
	{
		CimInstance cimInstance = new CimInstance("Win32_PowerSupplyProfile", "root/interop");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("InstanceID", InstanceID, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/interop", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new Win32_PowerSupplyProfile(session, instance);
	}

	public new static Win32_PowerSupplyProfile CreateReference(CimSession session, string InstanceID)
	{
		CimInstance cimInstance = new CimInstance("Win32_PowerSupplyProfile", "root/interop");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("InstanceID", InstanceID, CimFlags.Key));
		return new Win32_PowerSupplyProfile(session, cimInstance);
	}

	public new static IEnumerable<Win32_PowerSupplyProfile> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/interop", "Win32_PowerSupplyProfile")
			select new Win32_PowerSupplyProfile(session, i);
	}

	public new static IEnumerable<Win32_PowerSupplyProfile> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM Win32_PowerSupplyProfile";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/interop", "WQL", text)
			select new Win32_PowerSupplyProfile(session, i);
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

