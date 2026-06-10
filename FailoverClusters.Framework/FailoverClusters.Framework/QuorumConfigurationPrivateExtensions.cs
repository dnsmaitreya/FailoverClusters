namespace FailoverClusters.Framework;

internal static class QuorumConfigurationPrivateExtensions
{
	public static bool Equals(this QuorumConfigurationPrivate configA, QuorumConfigurationPrivate configB)
	{
		if (configA == null && configB == null)
		{
			return true;
		}
		if (configA != null && configB != null)
		{
			return configA.Equals(configB);
		}
		return false;
	}
}

