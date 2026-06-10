using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_InitiatorId : MSFT_StorageObject
{
	public struct DeleteObjectOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(DeleteObjectOutParameters lhs, DeleteObjectOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DeleteObjectOutParameters lhs, DeleteObjectOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_InitiatorIdMSFT_InitiatorIdToVirtualDisk MSFT_InitiatorIdToVirtualDisk { get; private set; }

	public MSFT_InitiatorIdMSFT_MaskingSetToInitiatorId MSFT_MaskingSetToInitiatorId { get; private set; }

	public MSFT_InitiatorIdMSFT_StorageSubSystemToInitiatorId MSFT_StorageSubSystemToInitiatorId { get; private set; }

	public ushort[] HostType => (ushort[])base.Instance.CimInstanceProperties["HostType"].Value;

	public string InitiatorAddress => (string)base.Instance.CimInstanceProperties["InitiatorAddress"].Value;

	public string[] OtherHostTypeDescription => (string[])base.Instance.CimInstanceProperties["OtherHostTypeDescription"].Value;

	public ushort? Type => (ushort?)base.Instance.CimInstanceProperties["Type"].Value;

	public MSFT_InitiatorId()
	{
	}

	public MSFT_InitiatorId(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_InitiatorIdToVirtualDisk = new MSFT_InitiatorIdMSFT_InitiatorIdToVirtualDisk(session, instance);
		MSFT_MaskingSetToInitiatorId = new MSFT_InitiatorIdMSFT_MaskingSetToInitiatorId(session, instance);
		MSFT_StorageSubSystemToInitiatorId = new MSFT_InitiatorIdMSFT_StorageSubSystemToInitiatorId(session, instance);
	}

	public static MSFT_InitiatorId GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_InitiatorId", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_InitiatorId(session, instance);
	}

	public static MSFT_InitiatorId CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_InitiatorId", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_InitiatorId(session, cimInstance);
	}

	public new static IEnumerable<MSFT_InitiatorId> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_InitiatorId")
			select new MSFT_InitiatorId(session, i);
	}

	public new static IEnumerable<MSFT_InitiatorId> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_InitiatorId";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_InitiatorId(session, i);
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

	public DeleteObjectOutParameters DeleteObject()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "DeleteObject", methodParameters);
		DeleteObjectOutParameters result = default(DeleteObjectOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["ExtendedStatus"] != null)
		{
			result.ExtendedStatus = new MSFT_StorageExtendedStatus(base.Session, (CimInstance)cimMethodResult.OutParameters["ExtendedStatus"].Value);
		}
		else
		{
			result.ExtendedStatus = null;
		}
		return result;
	}
}

