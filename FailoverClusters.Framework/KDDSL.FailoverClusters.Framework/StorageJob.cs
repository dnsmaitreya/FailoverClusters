using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageJob : MSFT_StorageObject
{
	public struct RequestStateChangeOutParameters
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

		public static bool operator ==(RequestStateChangeOutParameters lhs, RequestStateChangeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RequestStateChangeOutParameters lhs, RequestStateChangeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetExtendedStatusOutParameters
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

		public static bool operator ==(GetExtendedStatusOutParameters lhs, GetExtendedStatusOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetExtendedStatusOutParameters lhs, GetExtendedStatusOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetMessagesOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ushort[] Channels { get; set; }

		public string[] Messages { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetMessagesOutParameters lhs, GetMessagesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetMessagesOutParameters lhs, GetMessagesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetOutParametersOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageJobOutParams OutParameters { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetOutParametersOutParameters lhs, GetOutParametersOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetOutParametersOutParameters lhs, GetOutParametersOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public new MSFT_StorageJobMSFT_StorageJobToAffectedStorageObject MSFT_StorageJobToAffectedStorageObject { get; private set; }

	public ulong? BytesProcessed => (ulong?)base.Instance.CimInstanceProperties["BytesProcessed"].Value;

	public ulong? BytesTotal => (ulong?)base.Instance.CimInstanceProperties["BytesTotal"].Value;

	public bool? DeleteOnCompletion => (bool?)base.Instance.CimInstanceProperties["DeleteOnCompletion"].Value;

	public string Description => (string)base.Instance.CimInstanceProperties["Description"].Value;

	public DateTime? ElapsedTime => (DateTime?)base.Instance.CimInstanceProperties["ElapsedTime"].Value;

	public ushort? ErrorCode => (ushort?)base.Instance.CimInstanceProperties["ErrorCode"].Value;

	public string ErrorDescription => (string)base.Instance.CimInstanceProperties["ErrorDescription"].Value;

	public bool? IsBackgroundTask => (bool?)base.Instance.CimInstanceProperties["IsBackgroundTask"].Value;

	public ushort? JobState => (ushort?)base.Instance.CimInstanceProperties["JobState"].Value;

	public string JobStatus => (string)base.Instance.CimInstanceProperties["JobStatus"].Value;

	public ushort? LocalOrUtcTime => (ushort?)base.Instance.CimInstanceProperties["LocalOrUtcTime"].Value;

	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public string OtherRecoveryAction => (string)base.Instance.CimInstanceProperties["OtherRecoveryAction"].Value;

	public ushort? PercentComplete => (ushort?)base.Instance.CimInstanceProperties["PercentComplete"].Value;

	public ushort? RecoveryAction => (ushort?)base.Instance.CimInstanceProperties["RecoveryAction"].Value;

	public DateTime? StartTime => (DateTime?)base.Instance.CimInstanceProperties["StartTime"].Value;

	public string[] StatusDescriptions => (string[])base.Instance.CimInstanceProperties["StatusDescriptions"].Value;

	public DateTime? TimeBeforeRemoval => (DateTime?)base.Instance.CimInstanceProperties["TimeBeforeRemoval"].Value;

	public DateTime? TimeOfLastStateChange => (DateTime?)base.Instance.CimInstanceProperties["TimeOfLastStateChange"].Value;

	public DateTime? TimeSubmitted => (DateTime?)base.Instance.CimInstanceProperties["TimeSubmitted"].Value;

	public MSFT_StorageJob()
	{
	}

	public MSFT_StorageJob(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageJobToAffectedStorageObject = new MSFT_StorageJobMSFT_StorageJobToAffectedStorageObject(session, instance);
	}

	public static MSFT_StorageJob GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageJob", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageJob(session, instance);
	}

	public static MSFT_StorageJob CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageJob", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_StorageJob(session, cimInstance);
	}

	public new static IEnumerable<MSFT_StorageJob> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_StorageJob")
			select new MSFT_StorageJob(session, i);
	}

	public new static IEnumerable<MSFT_StorageJob> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageJob";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_StorageJob(session, i);
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

	public RequestStateChangeOutParameters RequestStateChange(ushort? RequestedState = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (RequestedState.HasValue)
		{
			ushort? num = RequestedState;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RequestedState", num, CimType.UInt16, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RequestStateChange", cimMethodParametersCollection);
		RequestStateChangeOutParameters result = default(RequestStateChangeOutParameters);
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

	public GetExtendedStatusOutParameters GetExtendedStatus()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetExtendedStatus", methodParameters);
		GetExtendedStatusOutParameters result = default(GetExtendedStatusOutParameters);
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

	public GetMessagesOutParameters GetMessages()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetMessages", methodParameters);
		GetMessagesOutParameters result = default(GetMessagesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["Channels"] != null)
		{
			result.Channels = (ushort[])cimMethodResult.OutParameters["Channels"].Value;
		}
		else
		{
			result.Channels = null;
		}
		if (cimMethodResult.OutParameters["Messages"] != null)
		{
			result.Messages = (string[])cimMethodResult.OutParameters["Messages"].Value;
		}
		else
		{
			result.Messages = null;
		}
		return result;
	}

	public GetOutParametersOutParameters GetOutParameters()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetOutParameters", methodParameters);
		GetOutParametersOutParameters result = default(GetOutParametersOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["OutParameters"] != null)
		{
			result.OutParameters = new MSFT_StorageJobOutParams(base.Session, (CimInstance)cimMethodResult.OutParameters["OutParameters"].Value);
		}
		else
		{
			result.OutParameters = null;
		}
		return result;
	}
}

