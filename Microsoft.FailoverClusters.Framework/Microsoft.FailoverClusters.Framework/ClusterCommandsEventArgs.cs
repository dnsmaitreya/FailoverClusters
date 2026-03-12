using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterCommandsEventArgs : EventArgs
{
	public CommandCollection[] Commands { get; internal set; }

	public ClusterCommandsEventArgs(CommandCollection[] commands)
	{
		Commands = commands;
	}
}
