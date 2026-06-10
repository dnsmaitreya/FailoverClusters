using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_OffloadDataTransferSetting : MSFT_StorageObject
{
	public MSFT_OffloadDataTransferSettingMSFT_StorageSubSystemToOffloadDataTransferSetting MSFT_StorageSubSystemToOffloadDataTransferSetting { get; private set; }

	public uint? NumberOfTokensInUse => (uint?)base.Instance.CimInstanceProperties["NumberOfTokensInUse"].Value;

	public uint? NumberOfTokensMax => (uint?)base.Instance.CimInstanceProperties["NumberOfTokensMax"].Value;

	public uint? OptimalDataTokenSize => (uint?)base.Instance.CimInstanceProperties["OptimalDataTokenSize"].Value;

	public bool? SupportInterSubsystem => (bool?)base.Instance.CimInstanceProperties["SupportInterSubsystem"].Value;

	public MSFT_OffloadDataTransferSetting()
	{
	}

	public MSFT_OffloadDataTransferSetting(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageSubSystemToOffloadDataTransferSetting = new MSFT_OffloadDataTransferSettingMSFT_StorageSubSystemToOffloadDataTransferSetting(session, instance);
	}

	public static MSFT_OffloadDataTransferSetting GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_OffloadDataTransferSetting", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_OffloadDataTransferSetting(session, instance);
	}

	public static MSFT_OffloadDataTransferSetting CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_OffloadDataTransferSetting", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_OffloadDataTransferSetting(session, cimInstance);
	}

	public new static IEnumerable<MSFT_OffloadDataTransferSetting> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_OffloadDataTransferSetting")
			select new MSFT_OffloadDataTransferSetting(session, i);
	}

	public new static IEnumerable<MSFT_OffloadDataTransferSetting> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_OffloadDataTransferSetting";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_OffloadDataTransferSetting(session, i);
	}

	public override void Refresh()
	{
		base.Instance = base.Session.GetInstance("root/windows/storage", base.Instance);
	}

	public override void Delete()
	{
		base.Session.DeleteInstance("root/windows/storage", base.Instance);
	}

	public override void Save()
	{
		base.Session.ModifyInstance("root/windows/storage", base.Instance);
	}
}

