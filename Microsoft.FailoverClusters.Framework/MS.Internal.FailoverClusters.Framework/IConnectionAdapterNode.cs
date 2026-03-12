using System;
using System.Collections.Generic;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IConnectionAdapterNode
{
	PNode Open(Guid id);

	PNode Open(string name);

	void Close(Guid id);

	void Close(string name);

	void Rename(Guid id, string newName);

	void Start(string name);

	void Stop(string name);

	void Pause(string name, NodePauseDrainType drainType, string targetNode);

	void Resume(string name, NodeResumeFailbackType failbackType);

	bool WillOfflineLoseQuorum(string name);

	bool WillEvictLoseQuorum(string name);

	PNode Add(string name);

	void Delete(Guid id);

	void Load(PNode node, NodeLoadSelection nodeSelection);

	IEnumerable<PNode> GetAll(bool nullElementOnError);

	NodeOperatingSystemInformation GetOperatingSystemInformation(string nodeName);

	ServerInformation GetServerInformation(string nodeName);

	string GetDomainName(string nodeName);

	ProcessorInformation GetProcessorInformation(string nodeName);
}
