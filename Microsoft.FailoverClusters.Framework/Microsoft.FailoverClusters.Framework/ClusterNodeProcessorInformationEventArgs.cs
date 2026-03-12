using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterNodeProcessorInformationEventArgs : ClusterEventArgs
{
	public ProcessorInformation ProcessorInformation { get; internal set; }

	public ClusterNodeProcessorInformationEventArgs(Guid id, ProcessorInformation processorInformation, ClusterException exception)
		: base(id, exception)
	{
		ProcessorInformation = processorInformation;
	}
}
