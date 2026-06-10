using System;
using System.Collections.Generic;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using Win32;

namespace KDDSL.ServerClusters.Management;

internal class ClusterDatabase
{
	private Guid instanceId;

	private string clusterName;

	private string fqdnClusterName;

	private readonly List<ClusterDatabaseNode> nodes;

	private readonly List<string> nodeNames;

	private string domain;

	private const string ClusterServiceParamatersRegKey = "System\\CurrentControlSet\\Services\\Clussvc\\Parameters";

	public string ClusterName => clusterName;

	public string FqdnClusterName => fqdnClusterName;

	public Guid ClusterInstanceId => instanceId;

	public ICollection<ClusterDatabaseNode> Nodes => nodes;

	public ICollection<string> NodeNames => nodeNames;

	public ClusterDatabase(string machineName)
	{
		nodes = new List<ClusterDatabaseNode>();
		nodeNames = new List<string>();
		LoadDatabase(machineName);
	}

	private void LoadDatabase(string machineName)
	{
		using RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName, RegistryView.Registry64);
		domain = DnsSupport.GetNodeDomain(machineName);
		using RegistryKey registryKey2 = registryKey.OpenSubKey("System\\CurrentControlSet\\Services\\Clussvc\\Parameters");
		if (registryKey2 != null)
		{
			ClusterNodeDatabaseConnection connection = new ClusterNodeDatabaseConnection(registryKey2);
			ExtractDbData(connection);
			return;
		}
		throw new ApplicationException(Resources.ClusterConnection_Failed_Text);
	}

	private void ExtractDbData(ClusterNodeDatabaseConnection connection)
	{
		ExtractClusterName(connection);
		ExtractNodeNames(connection);
		ExtractClusterInstanceId(connection);
	}

	private void ExtractClusterName(ClusterNodeDatabaseConnection connection)
	{
		clusterName = connection.GetClusterName();
		if (string.IsNullOrWhiteSpace(clusterName))
		{
			throw new ClusterRegistryException(ExceptionResources.ClusterRegistryClusterNameNotFound_Default);
		}
		fqdnClusterName = NetworkHelp.BuildFqdn(clusterName, domain);
	}

	private void ExtractClusterInstanceId(ClusterNodeDatabaseConnection connection)
	{
		instanceId = connection.GetClusterInstanceId();
	}

	private void ExtractNodeNames(ClusterNodeDatabaseConnection connection)
	{
		foreach (ClusterDatabaseNode databaseNode in connection.GetDatabaseNodes(domain))
		{
			nodes.Add(databaseNode);
			nodeNames.Add(databaseNode.FqdnName);
		}
	}
}

