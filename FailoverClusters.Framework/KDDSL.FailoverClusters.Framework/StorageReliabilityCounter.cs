using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageReliabilityCounter : MSFT_StorageObject
{
	public struct ResetOutParameters
	{
		public uint? ReturnValue { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(ResetOutParameters lhs, ResetOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ResetOutParameters lhs, ResetOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_StorageReliabilityCounterMSFT_DiskToStorageReliabilityCounter MSFT_DiskToStorageReliabilityCounter { get; private set; }

	public MSFT_StorageReliabilityCounterMSFT_PhysicalDiskToStorageReliabilityCounter MSFT_PhysicalDiskToStorageReliabilityCounter { get; private set; }

	public string DeviceId => (string)base.Instance.CimInstanceProperties["DeviceId"].Value;

	public ulong? FlushLatencyMax => (ulong?)base.Instance.CimInstanceProperties["FlushLatencyMax"].Value;

	public uint? LoadUnloadCycleCount => (uint?)base.Instance.CimInstanceProperties["LoadUnloadCycleCount"].Value;

	public uint? LoadUnloadCycleCountMax => (uint?)base.Instance.CimInstanceProperties["LoadUnloadCycleCountMax"].Value;

	public string ManufactureDate => (string)base.Instance.CimInstanceProperties["ManufactureDate"].Value;

	public uint? PowerOnHours => (uint?)base.Instance.CimInstanceProperties["PowerOnHours"].Value;

	public ulong? ReadErrorsCorrected => (ulong?)base.Instance.CimInstanceProperties["ReadErrorsCorrected"].Value;

	public ulong? ReadErrorsTotal => (ulong?)base.Instance.CimInstanceProperties["ReadErrorsTotal"].Value;

	public ulong? ReadErrorsUncorrected => (ulong?)base.Instance.CimInstanceProperties["ReadErrorsUncorrected"].Value;

	public ulong? ReadLatencyMax => (ulong?)base.Instance.CimInstanceProperties["ReadLatencyMax"].Value;

	public uint? StartStopCycleCount => (uint?)base.Instance.CimInstanceProperties["StartStopCycleCount"].Value;

	public uint? StartStopCycleCountMax => (uint?)base.Instance.CimInstanceProperties["StartStopCycleCountMax"].Value;

	public byte? Temperature => (byte?)base.Instance.CimInstanceProperties["Temperature"].Value;

	public byte? TemperatureMax => (byte?)base.Instance.CimInstanceProperties["TemperatureMax"].Value;

	public byte? Wear => (byte?)base.Instance.CimInstanceProperties["Wear"].Value;

	public ulong? WriteErrorsCorrected => (ulong?)base.Instance.CimInstanceProperties["WriteErrorsCorrected"].Value;

	public ulong? WriteErrorsTotal => (ulong?)base.Instance.CimInstanceProperties["WriteErrorsTotal"].Value;

	public ulong? WriteErrorsUncorrected => (ulong?)base.Instance.CimInstanceProperties["WriteErrorsUncorrected"].Value;

	public ulong? WriteLatencyMax => (ulong?)base.Instance.CimInstanceProperties["WriteLatencyMax"].Value;

	public MSFT_StorageReliabilityCounter()
	{
	}

	public MSFT_StorageReliabilityCounter(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_DiskToStorageReliabilityCounter = new MSFT_StorageReliabilityCounterMSFT_DiskToStorageReliabilityCounter(session, instance);
		MSFT_PhysicalDiskToStorageReliabilityCounter = new MSFT_StorageReliabilityCounterMSFT_PhysicalDiskToStorageReliabilityCounter(session, instance);
	}

	public static MSFT_StorageReliabilityCounter GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageReliabilityCounter", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageReliabilityCounter(session, instance);
	}

	public static MSFT_StorageReliabilityCounter CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageReliabilityCounter", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_StorageReliabilityCounter(session, cimInstance);
	}

	public new static IEnumerable<MSFT_StorageReliabilityCounter> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_StorageReliabilityCounter")
			select new MSFT_StorageReliabilityCounter(session, i);
	}

	public new static IEnumerable<MSFT_StorageReliabilityCounter> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageReliabilityCounter";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_StorageReliabilityCounter(session, i);
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

	public ResetOutParameters Reset()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Reset", methodParameters);
		ResetOutParameters result = default(ResetOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		return result;
	}
}

