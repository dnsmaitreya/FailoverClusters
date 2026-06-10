using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSIConnection : MiInstanceBase
{
	public MSFT_iSCSIConnectionMSFT_InitiatorPortToiSCSIConnection MSFT_InitiatorPortToiSCSIConnection { get; private set; }

	public MSFT_iSCSIConnectionMSFT_iSCSIConnectionToDisk MSFT_iSCSIConnectionToDisk { get; private set; }

	public MSFT_iSCSIConnectionMSFT_iSCSIConnectionToiSCSITargetPortal MSFT_iSCSIConnectionToiSCSITargetPortal { get; private set; }

	public MSFT_iSCSIConnectionMSFT_iSCSISessionToiSCSIConnection MSFT_iSCSISessionToiSCSIConnection { get; private set; }

	public MSFT_iSCSIConnectionMSFT_iSCSITargetToiSCSIConnection MSFT_iSCSITargetToiSCSIConnection { get; private set; }

	public string ConnectionIdentifier
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["ConnectionIdentifier"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ConnectionIdentifier"].Value = value;
		}
	}

	public string InitiatorAddress
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["InitiatorAddress"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["InitiatorAddress"].Value = value;
		}
	}

	public uint? InitiatorPortNumber
	{
		get
		{
			return (uint?)base.Instance.CimInstanceProperties["InitiatorPortNumber"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["InitiatorPortNumber"].Value = value;
		}
	}

	public string TargetAddress
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["TargetAddress"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["TargetAddress"].Value = value;
		}
	}

	public uint? TargetPortNumber
	{
		get
		{
			return (uint?)base.Instance.CimInstanceProperties["TargetPortNumber"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["TargetPortNumber"].Value = value;
		}
	}

	public MSFT_iSCSIConnection()
	{
	}

	public MSFT_iSCSIConnection(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_InitiatorPortToiSCSIConnection = new MSFT_iSCSIConnectionMSFT_InitiatorPortToiSCSIConnection(session, instance);
		MSFT_iSCSIConnectionToDisk = new MSFT_iSCSIConnectionMSFT_iSCSIConnectionToDisk(session, instance);
		MSFT_iSCSIConnectionToiSCSITargetPortal = new MSFT_iSCSIConnectionMSFT_iSCSIConnectionToiSCSITargetPortal(session, instance);
		MSFT_iSCSISessionToiSCSIConnection = new MSFT_iSCSIConnectionMSFT_iSCSISessionToiSCSIConnection(session, instance);
		MSFT_iSCSITargetToiSCSIConnection = new MSFT_iSCSIConnectionMSFT_iSCSITargetToiSCSIConnection(session, instance);
	}

	public static MSFT_iSCSIConnection GetInstance(CimSession session, string ConnectionIdentifier)
	{
		CimInstance cimInstance = new CimInstance("MSFT_iSCSIConnection", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ConnectionIdentifier", ConnectionIdentifier, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_iSCSIConnection(session, instance);
	}

	public static MSFT_iSCSIConnection CreateReference(CimSession session, string ConnectionIdentifier)
	{
		CimInstance cimInstance = new CimInstance("MSFT_iSCSIConnection", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ConnectionIdentifier", ConnectionIdentifier, CimFlags.Key));
		return new MSFT_iSCSIConnection(session, cimInstance);
	}

	public static IEnumerable<MSFT_iSCSIConnection> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_iSCSIConnection")
			select new MSFT_iSCSIConnection(session, i);
	}

	public static IEnumerable<MSFT_iSCSIConnection> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_iSCSIConnection";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_iSCSIConnection(session, i);
	}

	public override void Refresh()
	{
		base.Instance = base.Session.GetInstance("root/microsoft/windows/storage", base.Instance);
	}

	public override void Delete()
	{
		base.Session.DeleteInstance("root/microsoft/windows/storage", base.Instance);
	}

	public override void Save()
	{
		base.Session.ModifyInstance("root/microsoft/windows/storage", base.Instance);
	}
}

