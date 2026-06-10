using System;
using System.Collections.Generic;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal interface IFileShareAdapter
{
	void DeleteShare(FileShare fileShare);

	void DeleteShares(IEnumerable<FileShare> shares);

	FileShare GetShare(string serverName, string netName);

	void PopulateShare(FileShare fileShare, string serverName, string netName);

	void SetShare(FileShare fileShare);

	IObservable<IFileShareDataItem> GetFileShareSubscriptionObservable(string nodeFqdn, string serverName);

	IObservable<IFileShareDataItem> GetFileShareObservable(string nodeFqdn, string scopeName);
}

