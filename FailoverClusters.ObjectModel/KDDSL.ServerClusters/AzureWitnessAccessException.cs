using System.ComponentModel;

namespace KDDSL.ServerClusters;

public class AzureWitnessAccessException : ClusterBaseException
{
	private string m_node;

	public string Node
	{
		get
		{
			return m_node;
		}
		set
		{
			m_node = value;
		}
	}

	public AzureWitnessAccessException(string node, Win32Exception innerException)
		: base(string.Format(Resources.AzureWitnessAccessException_Text, node), innerException)
	{
		Node = node;
	}
}
