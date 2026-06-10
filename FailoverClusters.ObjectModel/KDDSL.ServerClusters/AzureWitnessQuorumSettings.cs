using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Management.Infrastructure;

namespace KDDSL.ServerClusters;

public class AzureWitnessQuorumSettings : QuorumSettings, IHasQuorumResource
{
	public static string DefaultEndpoint = "core.windows.net";

	private CimSession m_wmiSession;

	private string m_wmiEndpoint;

	private string m_accountName;

	private string m_endpoint;

	private ClusterResource m_resource;

	private string m_pkey;

	private string m_skey;

	private string m_sasToken;

	public virtual ClusterResource QuorumResource => m_resource;

	public override QuorumType QuorumType => QuorumType.AzureWitness;

	public string Endpoint => m_endpoint;

	public string AccountName => m_accountName;

	public AzureWitnessQuorumSettings(ClusterResource resource)
		: base(resource.Cluster)
	{
		string resourceType = "Cloud Witness";
		if (!resource.IsResourceOfType(resourceType))
		{
			throw new ArgumentException("AzureWitnessQuorumSettings::ctor(): Resource must be of type AzureWitness.", "resource");
		}
		PropertyCollection privateProperties = resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		string name = "AccountName";
		Property property = privateProperties.GetProperty(name);
		m_accountName = (string)property.Value;
		string name2 = "EndpointInfo";
		Property property2 = privateProperties.GetProperty(name2);
		m_endpoint = (string)property2.Value;
		m_resource = resource;
		m_pkey = null;
		m_skey = null;
		EstablishWmiSession(resource.Cluster.FqdnName);
	}

	public AzureWitnessQuorumSettings(Cluster cluster, string AccountName, string Endpoint, string PrimaryKey, string SecondaryKey, string SASToken)
		: base(cluster)
	{
		if (PrimaryKey != "")
		{
			Convert.FromBase64String(PrimaryKey);
		}
		if (SecondaryKey != "")
		{
			Convert.FromBase64String(SecondaryKey);
		}
		m_accountName = AccountName;
		m_endpoint = Endpoint;
		m_pkey = PrimaryKey;
		m_skey = SecondaryKey;
		m_sasToken = SASToken;
		m_resource = null;
		EstablishWmiSession(cluster.FqdnName);
	}

	public override void Configure()
	{
		if (m_pkey == null)
		{
			throw new InvalidOperationException("AzureWitnessQuorumSettings::Configure(): Cannot call configure() on object constructed with pre-existing resource.");
		}
		ReportOperationProcess(10, Resources.AzureWitnessCreatingContainer_Text);
		ClusterResource clusterResource = (m_resource = base.Cluster.GetQuorumResource());
		if (clusterResource != null)
		{
			string resourceType = "Cloud Witness";
			if (clusterResource.IsResourceOfType(resourceType))
			{
				PropertyCollection privateProperties = m_resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
				string name = "AccountName";
				if (!((string)privateProperties.GetProperty(name).Value != m_accountName))
				{
					ReportOperationProcess(70, Resources.AzureWitnessConfiguringResource_Text);
					UpdateKey();
					return;
				}
				base.Cluster.SetMajorityQuorum(null, null);
				ReportOperationProcess(30, Resources.AzureWitnessDeletingResource_Text);
				m_resource.Delete();
			}
		}
		ReportOperationProcess(50, Resources.AzureWitnessCreatingResource_Text);
		CreateCloudWitness();
		ReportOperationProcess(80, Resources.AzureWitnessConfiguringQuorum_Text);
	}

	public override void Cleanup()
	{
	}

	public override void VerifySettings()
	{
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool AreQuorumSettingsEqual(QuorumSettings settings)
	{
		return false;
	}

	private void CreateCloudWitness()
	{
		CimInstance clusterService = GetClusterService();
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		cimMethodParametersCollection.Add(CimMethodParameter.Create("AccountName", m_accountName, CimFlags.In));
		cimMethodParametersCollection.Add(CimMethodParameter.Create("AccountKey", m_pkey, CimFlags.In));
		cimMethodParametersCollection.Add(CimMethodParameter.Create("SASToken", m_sasToken, CimFlags.In));
		cimMethodParametersCollection.Add(CimMethodParameter.Create("CloudWitnessName", ResourceHelp.GenerateResourceName(base.Cluster, Resources.AzureWitnessResourceName_Text), CimFlags.In));
		cimMethodParametersCollection.Add(CimMethodParameter.Create("EndpointInfo", m_endpoint, CimFlags.In));
		int num = (int)(uint)m_wmiSession.InvokeMethod(clusterService, "CreateCloudWitness", cimMethodParametersCollection).ReturnValue.Value;
		if (num != 0)
		{
			throw new AzureWitnessAccessException(m_wmiEndpoint, new Win32Exception(num, Resources.AzureWitnessAccessException_InternalError_Text));
		}
	}

	private void UpdateKey()
	{
		CimInstance clusterService = GetClusterService();
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		cimMethodParametersCollection.Add(CimMethodParameter.Create("AccountKey", m_pkey, CimFlags.In));
		cimMethodParametersCollection.Add(CimMethodParameter.Create("SASToken", m_sasToken, CimFlags.In));
		int num = (int)(uint)m_wmiSession.InvokeMethod(clusterService, "UpdateCloudWitnessKey", cimMethodParametersCollection).ReturnValue.Value;
		if (num != 0)
		{
			throw new AzureWitnessAccessException(m_wmiEndpoint, new Win32Exception(num, Resources.AzureWitnessAccessException_InternalError_Text));
		}
	}

	private void EstablishWmiSession(string endpoint)
	{
		m_wmiSession = CimSession.Create(endpoint);
		m_wmiEndpoint = endpoint;
	}

	private CimInstance GetClusterService()
	{
		CimInstance result = null;
		foreach (CimInstance item in m_wmiSession.EnumerateInstances("root/mscluster", "MSCluster_ClusterService"))
		{
			result = item;
		}
		return result;
	}
}

