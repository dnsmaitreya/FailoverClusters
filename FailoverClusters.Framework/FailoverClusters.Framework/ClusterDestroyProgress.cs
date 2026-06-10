using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterDestroyProgress
{
	public ClusterSetupPhrase SetupPhrase { get; internal set; }

	public ClusterSetupPhraseType SetupPhraseType { get; internal set; }

	public ClusterSetupPhraseSeverity SetupPhraseSeverity { get; internal set; }

	public int PercentComplete { get; internal set; }

	public string ObjectName { get; internal set; }

	public int Status { get; internal set; }

	public override string ToString()
	{
		return CommonResources.DestroyClusterProgressText.FormatCurrentCulture(PercentComplete);
	}
}

