using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public abstract class QuorumSettings
{
	private Cluster m_cluster;

	public const uint QuorumArbitrationTimeoutDefault = 20u;

	public const uint QuorumArbitrationTimeoutWitnessDefault = 90u;

	private EventHandler<OperationProgressEventArgs> _003Cbacking_store_003EOperationProgress;

	public bool IsDynamicQuorum
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_cluster.DynamicQuorumEnabled;
		}
	}

	public abstract QuorumType QuorumType { get; }

	protected Cluster Cluster => m_cluster;

	[SpecialName]
	public event EventHandler<OperationProgressEventArgs> OperationProgress
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EOperationProgress = (EventHandler<OperationProgressEventArgs>)Delegate.Combine(_003Cbacking_store_003EOperationProgress, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EOperationProgress = (EventHandler<OperationProgressEventArgs>)Delegate.Remove(_003Cbacking_store_003EOperationProgress, value);
		}
	}

	internal abstract void Configure();

	internal abstract void Cleanup();

	protected QuorumSettings(Cluster cluster)
	{
		m_cluster = cluster;
	}

	protected void ReportOperationProcess(int percentage, string message, OperationProgressWarningLevel level)
	{
		OperationProgressEventArgs e = new OperationProgressEventArgs(level, message, percentage);
		OnOperationProgress(e);
	}

	protected void ReportOperationProcess(int percentage, string format, object arg0)
	{
		string message = string.Format(CultureInfo.CurrentCulture, format, arg0);
		OperationProgressEventArgs e = new OperationProgressEventArgs(OperationProgressWarningLevel.Information, message, percentage);
		OnOperationProgress(e);
	}

	protected void ReportOperationProcess(int percentage, string message)
	{
		OperationProgressEventArgs e = new OperationProgressEventArgs(OperationProgressWarningLevel.Information, message, percentage);
		OnOperationProgress(e);
	}

	[SuppressMessage("Performance", "CA1811:AvoidUncalledPrivateCode")]
	private void OnOperationProgress(OperationProgressEventArgs e)
	{
		_003Cbacking_store_003EOperationProgress?.Invoke(this, e);
	}

	private static long CalculateVoterFailures(long voters)
	{
		return voters - voters / 2 - 1;
	}

	public QuorumType GetRecommendedType()
	{
		int num = 0;
		foreach (ClusterNode node in m_cluster.GetNodes())
		{
			if (node.NodeWeight != 0)
			{
				num++;
			}
		}
		return (num % 2 == 0) ? QuorumType.StorageWitness : QuorumType.MajorityOfNodes;
	}

	public long GetFailuresSustained()
	{
		ClusterResource quorumResource = m_cluster.GetQuorumResource();
		switch (QuorumType)
		{
		default:
			return 0L;
		case QuorumType.MajorityOfNodes:
		{
			long nodeCount = m_cluster.GetNodeCount();
			return nodeCount - nodeCount / 2 - 1;
		}
		case QuorumType.LegacyDisk:
		case QuorumType.StorageWitness:
			if (quorumResource == null && quorumResource.State == ResourceState.Online)
			{
				long num = m_cluster.GetNodeCount() + 1;
				return num - num / 2 - 1;
			}
			return CalculateWitnessOfflineFailures(m_cluster.GetNodeCount());
		}
	}

	public static long CalculateNodeMajorityFailures(long nodeCount)
	{
		return nodeCount - nodeCount / 2 - 1;
	}

	public static long CalculateWitnessOnlineFailures(long nodeCount)
	{
		long num = nodeCount + 1;
		return num - num / 2 - 1;
	}

	public static long CalculateWitnessOfflineFailures(long nodeCount)
	{
		long num = nodeCount + 1;
		return Math.Max(0L, num - num / 2 - 2);
	}

	public abstract void VerifySettings();

	[return: MarshalAs(UnmanagedType.U1)]
	public abstract bool AreQuorumSettingsEqual(QuorumSettings settings);

	[SpecialName]
	protected void raise_OperationProgress(object value0, OperationProgressEventArgs value1)
	{
		_003Cbacking_store_003EOperationProgress?.Invoke(value0, value1);
	}
}

