using System;
using System.Diagnostics;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterCommandCanExecuteEventArgs : EventArgs
{
	public object Parameter { get; set; }

	[DebuggerNonUserCode]
	public ClusterCommandCanExecuteEventArgs(object parameter)
	{
		Parameter = parameter;
	}
}
