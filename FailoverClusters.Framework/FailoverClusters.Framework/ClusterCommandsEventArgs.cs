using System;

namespace FailoverClusters.Framework;

public class ClusterCommandsEventArgs : EventArgs
{
	public CommandCollection[] Commands { get; internal set; }

	public ClusterCommandsEventArgs(CommandCollection[] commands)
	{
		Commands = commands;
	}
}

