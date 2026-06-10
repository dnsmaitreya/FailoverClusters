using System;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class SelectScopeNodeEventArgs : EventArgs
{
	private readonly ScopeNode scopeNode;

	public ScopeNode ScopeNode => scopeNode;

	public SelectScopeNodeEventArgs(ScopeNode scopeNode)
	{
		this.scopeNode = scopeNode;
	}
}

