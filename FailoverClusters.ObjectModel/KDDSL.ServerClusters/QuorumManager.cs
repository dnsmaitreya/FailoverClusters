using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class QuorumManager
{
	private Cluster m_cluster;

	private int m_currentPercent;

	private EventHandler<OperationProgressEventArgs> _003Cbacking_store_003EOperationProgress;

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

	private ClusterResource PickQuourmDisk(ICollection<PossibleQuorumStorage> possibleQuorumStorage)
	{
		ClusterResource result = null;
		ulong num = 0uL;
		foreach (PossibleQuorumStorage item in possibleQuorumStorage)
		{
			if (item.Status == PossibleQuorumStorageStatus.Valid && (item.Resource.GetOwnerGroup().GroupType == GroupType.AvailableStorage || item.Resource.IsQuorumResource))
			{
				ulong size = item.BestPartition.Size;
				if ((size >= 524288000 && num < 524288000) || (size > num && num < 524288000) || (size >= 524288000 && size < num))
				{
					result = item.Resource;
					num = size;
				}
			}
		}
		return result;
	}

	[SuppressMessage("Performance", "CA1811:AvoidUncalledPrivateCode")]
	private void OnOperationProgress(OperationProgressEventArgs e)
	{
		_003Cbacking_store_003EOperationProgress?.Invoke(this, e);
	}

	private void OnOperationProgress(object sender, OperationProgressEventArgs e)
	{
		int num = (int)((double)e.PercentDone * -0.3);
		int percentDone = m_currentPercent - num;
		OperationProgressEventArgs e2 = new OperationProgressEventArgs(e.WarningLevel, e.Message, percentDone);
		OnOperationProgress(e2);
	}

	private void ReportOperationProgress(int percentage, string message, OperationProgressWarningLevel level)
	{
		OperationProgressEventArgs e = new OperationProgressEventArgs(level, message, percentage);
		OnOperationProgress(e);
	}

	private void ReportOperationProgress(int percentage, string message)
	{
		OperationProgressEventArgs e = new OperationProgressEventArgs(OperationProgressWarningLevel.Information, message, percentage);
		OnOperationProgress(e);
	}

	internal QuorumManager(Cluster cluster)
	{
		m_cluster = cluster;
	}

	[SpecialName]
	protected void raise_OperationProgress(object value0, OperationProgressEventArgs value1)
	{
		_003Cbacking_store_003EOperationProgress?.Invoke(value0, value1);
	}

	public ICollection<PossibleQuorumStorage> DeterminePossibleQuorumStorage(IList<ClusterNode> nodes, [MarshalAs(UnmanagedType.U1)] bool filterOutAsymmetricDisks)
	{
		List<PossibleQuorumStorage> list = new List<PossibleQuorumStorage>();
		ClusterResourceCollection allStorage = m_cluster.GetAllStorage();
		if (m_cluster.CurrentVersion == ClusterVersion.Windows8)
		{
			foreach (KeyValuePair<ClusterResource, IList<ClusterNode>> item2 in m_cluster.GetDiskConnectivity(allStorage, nodes))
			{
				ValueType valueType = item2;
				PossibleQuorumStorage possibleQuorumStorage = new PossibleQuorumStorage(((KeyValuePair<ClusterResource, IList<ClusterNode>>)valueType).Key);
				if (((KeyValuePair<ClusterResource, IList<ClusterNode>>)valueType).Value.Count == nodes.Count)
				{
					possibleQuorumStorage.IsAsymmetric = false;
					list.Add(possibleQuorumStorage);
				}
				else if (!filterOutAsymmetricDisks)
				{
					possibleQuorumStorage.IsAsymmetric = true;
					list.Add(possibleQuorumStorage);
				}
			}
		}
		else
		{
			foreach (ClusterResource item3 in allStorage)
			{
				PossibleQuorumStorage item = new PossibleQuorumStorage(item3);
				list.Add(item);
			}
		}
		return list;
	}

	public ICollection<PossibleQuorumStorage> DeterminePossibleQuorumStorage([MarshalAs(UnmanagedType.U1)] bool filterOutAsymmetricDisks)
	{
		ClusterNodeCollection nodes = m_cluster.GetNodes();
		return DeterminePossibleQuorumStorage(nodes, filterOutAsymmetricDisks);
	}

	public QuorumSettings AutoSelectQuorumConfiguration()
	{
		ICollection<PossibleQuorumStorage> possibleQuorumStorage = DeterminePossibleQuorumStorage(filterOutAsymmetricDisks: true);
		ClusterResource clusterResource = PickQuourmDisk(possibleQuorumStorage);
		if (clusterResource == null)
		{
			return null;
		}
		return StorageQuorumSettings.CreateStorageWitnessQuorumSettings(clusterResource);
	}

	public void ChangeQuorum(QuorumSettings newQuorumSettings)
	{
		//Discarded unreachable code: IL_00d8, IL_00da
		QuorumSettings quorumSettings = m_cluster.GetQuorumSettings();
		try
		{
			quorumSettings.OperationProgress += OnOperationProgress;
			newQuorumSettings.OperationProgress += OnOperationProgress;
			if (!newQuorumSettings.AreQuorumSettingsEqual(quorumSettings))
			{
				m_currentPercent = 10;
				newQuorumSettings.VerifySettings();
				m_currentPercent = 40;
				newQuorumSettings.Configure();
				m_currentPercent = 70;
				if (quorumSettings.QuorumType != QuorumType.AzureWitness || newQuorumSettings.QuorumType != QuorumType.AzureWitness)
				{
					quorumSettings.Cleanup();
				}
				string quorumSettingsChanged_Text = Resources.QuorumSettingsChanged_Text;
				OperationProgressEventArgs e = new OperationProgressEventArgs(OperationProgressWarningLevel.Information, quorumSettingsChanged_Text, 100);
				OnOperationProgress(e);
			}
		}
		catch (Exception innerException)
		{
			if (newQuorumSettings is IHasQuorumResource hasQuorumResource)
			{
				ClusterResource quorumResource = hasQuorumResource.QuorumResource;
				if (quorumResource != null && !quorumResource.IsDeleted)
				{
					newQuorumSettings.Cleanup();
				}
			}
			ReportOperationProgress(100, Resources.QuorumSettingsFailed_Text, OperationProgressWarningLevel.Error);
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.QuorumSettingsFailed_Text });
		}
		finally
		{
			quorumSettings.OperationProgress -= OnOperationProgress;
			newQuorumSettings.OperationProgress -= OnOperationProgress;
		}
	}

	public MajorityQuorumSettings CreateMajorityOfNodesQuorumSettings()
	{
		return new MajorityQuorumSettings(m_cluster);
	}

	public FileShareQuorumSettings CreateFileShareQuorumSettings(string fileSharePath)
	{
		return new FileShareQuorumSettings(m_cluster, fileSharePath, null, null);
	}

	public AzureWitnessQuorumSettings CreateAzureWitnessQuorumSettings(string accountName, string endpoint, string primaryKey, string secondaryKey)
	{
		return new AzureWitnessQuorumSettings(m_cluster, accountName, endpoint, primaryKey, secondaryKey, "");
	}

	public StorageQuorumSettings CreateStorageWitnessQuorumSettings(ClusterResource resource)
	{
		return StorageQuorumSettings.CreateStorageWitnessQuorumSettings(resource);
	}

	public StorageQuorumSettings CreateLegacyDiskQuorumSettings(ClusterResource resource)
	{
		return StorageQuorumSettings.CreateLegacyDiskQuorumSettings(resource);
	}
}

