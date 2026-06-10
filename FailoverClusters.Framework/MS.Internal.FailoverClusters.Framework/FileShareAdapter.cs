using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Common.Reactive.Concurrency;
using FailoverClusters.UI.Common.Reactive.Linq;

namespace MS.Internal.FailoverClusters.Framework;

public class FileShareAdapter : IFileShareAdapter
{
	private readonly List<FileShareDataSourceBase> shareDataSources = new List<FileShareDataSourceBase>();

	private const string reserved = "[^<>:\"\\\\/\\?\\*|]";

	public static readonly string ValidSharePathRegex = "^\\\\\\\\(?<server>R+?)\\\\*?\\\\(?<share>R+?)(?<dirs>(\\\\R*?)*)$".Replace("R", "[^<>:\"\\\\/\\?\\*|]");

	public FileShareAdapter()
	{
		shareDataSources.Add(new NfsAdapter());
		shareDataSources.Add(new SmbAdapter());
	}

	public void DeleteShare(FileShare fileShare)
	{
		try
		{
			shareDataSources.First((FileShareDataSourceBase s) => s.SupportedProtocol == fileShare.Protocol).DeleteShare(fileShare);
		}
		catch (ManagementException)
		{
			throw new ClusterFileShareDeletingException();
		}
		catch (COMException innerException)
		{
			throw new ClusterFileShareDeletingException(innerException);
		}
	}

	public void DeleteShares(IEnumerable<FileShare> fileShares)
	{
		try
		{
			shareDataSources.ForEach(delegate(FileShareDataSourceBase s)
			{
				s.DeleteShares(fileShares.Where((FileShare fs) => fs.Protocol == s.SupportedProtocol));
			});
		}
		catch (ManagementException)
		{
			throw new ClusterFileShareDeletingException();
		}
		catch (COMException innerException)
		{
			throw new ClusterFileShareDeletingException(innerException);
		}
	}

	public FileShare GetShare(string serverName, string netName)
	{
		FileShare fileShare = new FileShare();
		PopulateShare(fileShare, serverName, netName);
		return fileShare;
	}

	public void PopulateShare(FileShare fileShare, string serverName, string netName)
	{
		if (fileShare == null)
		{
			throw new ArgumentNullException("fileShare");
		}
		if (fileShare.Protocol == FileShareProtocol.Unknown)
		{
			using (List<FileShareDataSourceBase>.Enumerator enumerator = shareDataSources.GetEnumerator())
			{
				while (enumerator.MoveNext() && !enumerator.Current.PopulateShare(fileShare, serverName, netName))
				{
				}
				return;
			}
		}
		shareDataSources.First((FileShareDataSourceBase s) => s.SupportedProtocol == fileShare.Protocol).PopulateShare(fileShare, serverName, netName);
	}

	public void SetShare(FileShare fileShare)
	{
		shareDataSources.First((FileShareDataSourceBase s) => s.SupportedProtocol == fileShare.Protocol).SetShare(fileShare);
	}

	public IObservable<IFileShareDataItem> GetFileShareSubscriptionObservable(string nodeFqdn, string serverName)
	{
		return shareDataSources.Select((FileShareDataSourceBase s) => Observable.Defer(() => s.GetSubscriptionObservable(nodeFqdn, serverName)).SubscribeOn(Scheduler.ThreadPool)).Merge().Catch(delegate(OperationCanceledException ex)
		{
			ClusterLog.LogException(ex);
			return Observable.Empty<IFileShareDataItem>();
		});
	}

	public IObservable<IFileShareDataItem> GetFileShareObservable(string nodeFqdn, string scopeName)
	{
		return shareDataSources.Select((FileShareDataSourceBase s) => Observable.Defer(() => s.GetSharesObservable(nodeFqdn, scopeName)).SubscribeOn(Scheduler.ThreadPool)).Merge().Catch(delegate(OperationCanceledException ex)
		{
			ClusterLog.LogException(ex);
			return Observable.Empty<IFileShareDataItem>();
		});
	}
}

