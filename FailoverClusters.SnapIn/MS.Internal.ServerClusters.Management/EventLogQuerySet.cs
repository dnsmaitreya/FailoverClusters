using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters.Management;

internal class EventLogQuerySet : IDisposable
{
	private class QueryWorkerArgs
	{
		public string Node;

		public string Channel;

		public string Query;

		public bool Succeed;

		public QueryWorkerArgs(string node, string channel, string query)
		{
			Node = node;
			Channel = channel;
			Query = query;
			Succeed = true;
		}
	}

	public struct FindResult
	{
		public readonly bool Found;

		public readonly bool ErrorFound;

		public readonly int ItemIndex;

		public FindResult(bool found, int itemIndex)
		{
			Found = found;
			ErrorFound = false;
			ItemIndex = itemIndex;
		}

		public FindResult(int itemIndex)
		{
			ErrorFound = true;
			Found = false;
			ItemIndex = itemIndex;
		}
	}

	private delegate T AsyncCallDelegate<T>();

	private Dictionary<string, EventLogSession> sessions;

	private List<EventLogQuery> queries;

	private List<EventLogBookmark> bookmarks;

	private ManualResetEvent cancelEvent;

	private volatile bool cancel;

	private object sessionsLock;

	private object serializationLock;

	private object resultsLock;

	private int disposed;

	public event EventHandler<ResultsFetchedEventArgs> ResultsFetched;

	public EventLogQuerySet()
	{
		sessions = new Dictionary<string, EventLogSession>(StringComparer.OrdinalIgnoreCase);
		queries = new List<EventLogQuery>();
		bookmarks = new List<EventLogBookmark>();
		cancelEvent = new ManualResetEvent(initialState: false);
		cancel = false;
		sessionsLock = new object();
		serializationLock = new object();
		resultsLock = new object();
		disposed = 0;
	}

	~EventLogQuerySet()
	{
		Dispose(disposing: false);
	}

	public int GetResultsCount()
	{
		lock (resultsLock)
		{
			return bookmarks.Count;
		}
	}

	public List<EventLogEvent> GetResults(int startIndex, int endIndex)
	{
		lock (resultsLock)
		{
			return GetResults(bookmarks, startIndex, endIndex);
		}
	}

	private List<EventLogEvent> GetResults(List<EventLogBookmark> bookmarkList, int startIndex, int endIndex)
	{
		if (0 > startIndex || startIndex > endIndex)
		{
			throw new ArgumentException("Invalid index");
		}
		if (endIndex >= bookmarkList.Count)
		{
			throw new ArgumentOutOfRangeException("endIndex");
		}
		List<EventLogEvent> list = new List<EventLogEvent>();
		for (int i = 0; i < endIndex - startIndex + 1; i++)
		{
			if (cancel)
			{
				throw new OperationCanceledException();
			}
			EventLogEvent eventLogEvent = null;
			try
			{
				eventLogEvent = bookmarkList[startIndex + i].GetEvent();
				if (eventLogEvent == null)
				{
					continue;
				}
			}
			catch (Exception exception)
			{
				eventLogEvent = new EventLogEvent(exception);
			}
			list.Add(eventLogEvent);
		}
		return list;
	}

	public void Cancel()
	{
		cancel = true;
		cancelEvent.Set();
	}

	public void Query(EventLogFilter filter)
	{
		lock (serializationLock)
		{
			cancel = false;
			cancelEvent.Reset();
			filter.Tag = new List<string>();
			ClearQueries();
			Action<QueryWorkerArgs> onDoWork = delegate(QueryWorkerArgs queryCallback)
			{
				QueryWorker(queryCallback);
				if (!queryCallback.Succeed)
				{
					((List<string>)filter.Tag).Add(queryCallback.Node);
				}
			};
			if (filter.Nodes.Count == 1)
			{
				onDoWork(new QueryWorkerArgs(filter.Nodes[0], filter.Channels[0], filter.GetQuery()));
				return;
			}
			filter.Nodes.AsParallel().WithDegreeOfParallelism(4).Any(delegate(string nodeName)
			{
				if (cancel)
				{
					return true;
				}
				onDoWork(new QueryWorkerArgs(nodeName, filter.Channels[0], filter.GetQuery()));
				return false;
			});
		}
	}

	private void QueryWorker(QueryWorkerArgs args)
	{
		Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
		string debugMessage = string.Empty;
		try
		{
			debugMessage = string.Format(CultureInfo.CurrentCulture, Resources.CannotConnectToNode_Text, args.Node);
			EventLogSession session = null;
			lock (sessionsLock)
			{
				if (!sessions.TryGetValue(args.Node, out session))
				{
					session = null;
				}
			}
			if (session == null)
			{
				session = ExecuteAsyncCall(() => new EventLogSession(args.Node));
				lock (sessionsLock)
				{
					sessions[args.Node] = session;
				}
			}
			debugMessage = string.Format(CultureInfo.CurrentCulture, Resources.CannotQueryEventLog_Text, args.Channel, args.Node);
			EventLogQuery eventLogQuery = ExecuteAsyncCall(() => session.CreateQuery(args.Channel, args.Query));
			lock (resultsLock)
			{
				queries.Add(eventLogQuery);
			}
			debugMessage = string.Format(CultureInfo.CurrentCulture, Resources.CannotReadQueryResults_Text, args.Channel, args.Node);
			ICollection<EventLogBookmark> collection = null;
			while ((collection = eventLogQuery.GetResults(256u)) != null)
			{
				if (cancel)
				{
					throw new OperationCanceledException();
				}
				lock (resultsLock)
				{
					bookmarks.AddRange(collection);
				}
				OnResultsFetched(new ResultsFetchedEventArgs(collection.Count <= 256));
			}
		}
		catch (OperationCanceledException)
		{
			args.Succeed = false;
		}
		catch (Exception ex2)
		{
			args.Succeed = false;
			ExceptionHelp.LogException(ex2, debugMessage);
			ExceptionHelp.LogException(ex2, "Error reading critical events, probably connection lost with node");
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex2);
			if (firstException == null || firstException.NativeErrorCode != -2147024890)
			{
				ClusterLog.AdminEvents.WriteNodeUnreachableEvent(args.Node, ex2.ToString());
			}
		}
	}

	public void Sort(IComparer<EventLogEvent> comparer)
	{
		lock (serializationLock)
		{
			cancel = false;
			cancelEvent.Reset();
			List<EventLogEvent> list = null;
			try
			{
				List<EventLogBookmark> list2 = null;
				lock (resultsLock)
				{
					list2 = new List<EventLogBookmark>(bookmarks);
				}
				list = GetResults(list2, 0, list2.Count - 1);
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Tag = list2[i];
				}
				list.Sort(comparer);
				List<EventLogBookmark> list3 = new List<EventLogBookmark>();
				foreach (EventLogEvent item in list)
				{
					list3.Add((EventLogBookmark)item.Tag);
				}
				lock (resultsLock)
				{
					bookmarks = list3;
				}
			}
			catch (OperationCanceledException)
			{
			}
		}
	}

	public void Reverse()
	{
		lock (serializationLock)
		{
			cancel = false;
			cancelEvent.Reset();
			try
			{
				lock (resultsLock)
				{
					List<EventLogBookmark> list = new List<EventLogBookmark>();
					for (int num = bookmarks.Count - 1; num >= 0; num--)
					{
						list.Add(bookmarks[num]);
					}
					bookmarks = list;
				}
			}
			catch (OperationCanceledException)
			{
			}
		}
	}

	public FindResult Find(string text, int startIndex, FindDirection direction, StringComparison comparison)
	{
		lock (serializationLock)
		{
			cancel = false;
			cancelEvent.Reset();
			try
			{
				for (int i = startIndex; 0 <= i && i < bookmarks.Count; i += ((direction == FindDirection.Forward) ? 1 : (-1)))
				{
					if (cancel)
					{
						throw new OperationCanceledException();
					}
					try
					{
						if (bookmarks[i].GetEvent().Contains(text, comparison))
						{
							return new FindResult(found: true, i);
						}
					}
					catch (Exception)
					{
						return new FindResult(i);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			return new FindResult(found: false, -1);
		}
	}

	protected void OnResultsFetched(ResultsFetchedEventArgs e)
	{
		this.ResultsFetched?.Invoke(this, e);
	}

	private void ClearQueries()
	{
		lock (resultsLock)
		{
			foreach (EventLogBookmark bookmark in bookmarks)
			{
				bookmark.Dispose();
			}
			bookmarks.Clear();
			foreach (EventLogQuery q in queries)
			{
				Background.QueueWorker((WaitCallback)delegate
				{
					q.Dispose();
				});
			}
			queries.Clear();
		}
	}

	private void ClearSessions()
	{
		foreach (EventLogSession value in sessions.Values)
		{
			value.Dispose();
		}
		sessions.Clear();
	}

	private T ExecuteAsyncCall<T>(AsyncCallDelegate<T> asyncCall)
	{
		IAsyncResult asyncResult = asyncCall.BeginInvoke(null, null);
		if (WaitHandle.WaitAny(new WaitHandle[2] { cancelEvent, asyncResult.AsyncWaitHandle }) == 0)
		{
			throw new OperationCanceledException();
		}
		return asyncCall.EndInvoke(asyncResult);
	}

	private void Dispose(bool disposing)
	{
		if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
		{
			Cancel();
			cancelEvent.Close();
			ClearQueries();
			ClearSessions();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
