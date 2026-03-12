using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class EventLogEntryListView : BaseListView
{
	private struct GetEventsArguments
	{
		public int QueryId;

		public List<EventListViewItem> Items;

		public int StartIndex;

		public int EndIndex;
	}

	private struct GetEventDetailsArguments
	{
		public int SelectedIndex;

		public EventLogEvent SelectedEvent;
	}

	private class FindArguments
	{
		public string Match;

		public int StartIndex;

		public FindDirection Direction;

		public StringComparison Comparison;
	}

	private INotifyUser notifyUser;

	private int sortColumn;

	private object columnLock;

	private StringFormat stringFormat;

	private EventLogQuerySet querySet;

	private int refreshInterval;

	private DateTime lastRefresh;

	private Dictionary<int, EventListViewItem> listViewItemCache;

	private object eventCacheLock;

	private AddRemoveColumnsDialog columnChooserDialog;

	private CursorManager cursorManager;

	private FindNextDialog findDialog;

	private bool findHotKeyEnabled;

	private ClusterEventsControl clusterEventsControl;

	private int queryId;

	private BackgroundOperation<EventLogFilter, object> queryBackgroundOperation;

	private BackgroundOperation<FindArguments, int> findBackgroundOperation;

	private BackgroundOperation<object, object> sortBackgroundOperation;

	private BackgroundOperation<object, object> reverseBackgroundOperation;

	private BackgroundOperation<GetEventsArguments, List<EventLogEvent>> getEventsBackgroundOperation;

	private BackgroundOperation<GetEventDetailsArguments, object> getEventDetailsBackgroundOperation;

	private BackgroundOperation<object, object> clearCacheBackgroundOperation;

	public event EventHandler QueryStarted;

	public event AsyncCompletedEventHandler QueryCompleted;

	public event AsyncCompletedEventHandler SortCompleted;

	public event EventHandler<AsyncFindCompletedEventArgs> FindCompleted;

	public EventLogEntryListView()
	{
		if (!UIHelper.DesignMode)
		{
			stringFormat = WinFormsHelp.CreateListViewStringFormat((ListView)(object)this);
			queryBackgroundOperation = new BackgroundOperation<EventLogFilter, object>((BackgroundOperationFunction<EventLogFilter, object>)QueryBackgroundOperation);
			queryBackgroundOperation.OperationCompleted += OnQueryBackgroundOperationCompleted;
			findBackgroundOperation = new BackgroundOperation<FindArguments, int>((BackgroundOperationFunction<FindArguments, int>)FindBackgroundOperation);
			findBackgroundOperation.OperationCompleted += OnFindBackgroundOperationCompleted;
			sortBackgroundOperation = new BackgroundOperation<object, object>((BackgroundOperationFunction<object, object>)SortBackgroundOperation);
			sortBackgroundOperation.OperationCompleted += OnSortBackgroundOperationCompleted;
			reverseBackgroundOperation = new BackgroundOperation<object, object>((BackgroundOperationFunction<object, object>)ReverseBackgroundOperation);
			reverseBackgroundOperation.OperationCompleted += OnSortBackgroundOperationCompleted;
			getEventsBackgroundOperation = new BackgroundOperation<GetEventsArguments, List<EventLogEvent>>((BackgroundOperationFunction<GetEventsArguments, List<EventLogEvent>>)GetEventsBackgroundOperation);
			getEventsBackgroundOperation.OperationCompleted += OnGetEventsBackgroundOperationCompleted;
			getEventDetailsBackgroundOperation = new BackgroundOperation<GetEventDetailsArguments, object>((BackgroundOperationFunction<GetEventDetailsArguments, object>)GetEventDetailsBackgroundOperation);
			getEventDetailsBackgroundOperation.OperationCompleted += OnGetEventDetailsBackgroundOperationCompleted;
			clearCacheBackgroundOperation = new BackgroundOperation<object, object>((BackgroundOperationFunction<object, object>)ClearCacheOperation);
			clearCacheBackgroundOperation.OperationCompleted += ClearCacheOperationCompleted;
		}
	}

	public void Initialize(INotifyUser notifyUserConsole, ClusterEventsControl eventsControl)
	{
		notifyUser = notifyUserConsole;
		clusterEventsControl = eventsControl;
		clusterEventsControl.Clear();
		columnLock = new object();
		((ListView)this).FullRowSelect = true;
		((BaseListView)this).HideSelection = false;
		((ListView)this).MultiSelect = false;
		((ListView)this).OwnerDraw = true;
		((ListView)this).VirtualMode = true;
		((ListView)this).View = View.Details;
		((ListView)this).SmallImageList = IconsHelp.SmallImageList;
		querySet = new EventLogQuerySet();
		querySet.ResultsFetched += OnResultsFetched;
		listViewItemCache = new Dictionary<int, EventListViewItem>();
		eventCacheLock = new object();
		ColumnHeader[] columns = new ColumnHeader[9]
		{
			CreateColumn(Resources.DateTime_Text, 128, new DateTimeComparer()),
			CreateColumn(Resources.Node_Text, 128, new ComputerComparer()),
			CreateColumn(Resources.Source_Text, 128, new SourceComparer()),
			CreateColumn(Resources.EventId_Text, 64, new EventIdComparer()),
			CreateColumn(Resources.LogName_Text, 256, new ChannelComparer()),
			CreateColumn(Resources.User_Text, 128, new UserComparer()),
			CreateColumn(Resources.Task_Text, 128, new TaskComparer()),
			CreateColumn(Resources.Keywords_Text, 128, new KeywordComparer()),
			CreateColumn(Resources.OpCode_Text, 128, new OpCodeComparer())
		};
		columnChooserDialog = new AddRemoveColumnsDialog(columns);
		columnChooserDialog.Select(Resources.EventId_Text);
		columnChooserDialog.Select(Resources.DateTime_Text);
		columnChooserDialog.Select(Resources.Node_Text);
		columnChooserDialog.Select(Resources.Task_Text);
		ResetColumns();
		cursorManager = new CursorManager((Control)(object)this);
		findDialog = new FindNextDialog(this);
		findHotKeyEnabled = false;
	}

	private static ColumnHeader CreateColumn(string text, int width, IComparer<EventLogEvent> comparer)
	{
		return new ColumnHeader
		{
			Text = text,
			Width = width,
			Tag = comparer
		};
	}

	private void ResetColumns()
	{
		lock (columnLock)
		{
			((BaseListView)this).VirtualListSize = 0;
			listViewItemCache.Clear();
			((ListView)this).Columns.Clear();
			int num = 0;
			((ListView)this).Columns.Add(CreateColumn(Resources.Level_Text, (num > 0) ? num : 100, new LevelComparer()));
			ColumnHeader[] selectedColumns = columnChooserDialog.GetSelectedColumns();
			foreach (ColumnHeader value in selectedColumns)
			{
				((ListView)this).Columns.Add(value);
			}
			((BaseListView)this).VirtualListSize = querySet.GetResultsCount();
		}
	}

	public void Reset()
	{
		queryId++;
		((BaseListView)this).HideMessage();
		((BaseListView)this).VirtualListSize = 0;
		ClearCache();
		((BaseListView)this).ResetSort();
	}

	private void BeginAsyncOperation()
	{
		cursorManager.BeginCursor(CursorType.DataLoad);
		EnableSort(enabled: false);
	}

	private void CompleteAsyncOperation()
	{
		CompleteAsyncOperation(-1);
	}

	private void CompleteAsyncOperation(int selectedIndex)
	{
		EnableSort(enabled: true);
		cursorManager.EndCursor();
		if (selectedIndex >= 0)
		{
			((ListView)this).SelectedIndices.Add(selectedIndex);
		}
	}

	public void QueryAsync(EventLogFilter filter)
	{
		BeginAsyncOperation();
		refreshInterval = 1;
		lastRefresh = DateTime.MaxValue;
		queryBackgroundOperation.QueueOperation(filter);
		if (this.QueryStarted != null)
		{
			this.QueryStarted(this, EventArgs.Empty);
		}
	}

	private object QueryBackgroundOperation(BackgroundOperationStatus dummy, EventLogFilter filter)
	{
		Reset();
		querySet.Query(filter);
		return null;
	}

	private void OnQueryBackgroundOperationCompleted(object sender, BackgroundOperationCompletedEventArgs<EventLogFilter, object> e)
	{
		try
		{
			if (e.Success)
			{
				((BaseListView)this).VirtualListSize = querySet.GetResultsCount();
				if (((BaseListView)this).VirtualListSize == 0)
				{
					((BaseListView)this).ShowMessage(Resources.NoEventsFound_Text);
				}
			}
			((BaseListView)this).ResetSort();
			OnQueryCompleted(new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.OperationParameter.Tag));
		}
		finally
		{
			CompleteAsyncOperation((((ListView)this).SelectedIndices.Count != 0 || ((BaseListView)this).VirtualListSize <= 0) ? (-1) : 0);
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		((Control)this).OnKeyDown(e);
		if (e.Control && e.KeyCode == Keys.F && findHotKeyEnabled)
		{
			ShowFindDialog();
		}
	}

	public DialogResult ShowFindDialog()
	{
		return notifyUser.ShowDialog((Form)(object)findDialog);
	}

	public void FindAsync(string match, int startIndex, FindDirection direction, StringComparison comparison)
	{
		BeginAsyncOperation();
		FindArguments findArguments = new FindArguments();
		findArguments.Match = match;
		findArguments.StartIndex = startIndex;
		findArguments.Direction = direction;
		findArguments.Comparison = comparison;
		findBackgroundOperation.QueueOperation(findArguments);
	}

	private int FindBackgroundOperation(BackgroundOperationStatus dummy, FindArguments arguments)
	{
		EventLogQuerySet.FindResult findResult = querySet.Find(arguments.Match, arguments.StartIndex, arguments.Direction, arguments.Comparison);
		if (findResult.ErrorFound)
		{
			lock (eventCacheLock)
			{
				EventListViewItem value = null;
				if (listViewItemCache.TryGetValue(findResult.ItemIndex, out value))
				{
					value = CreateDummyItem(Resources.RetrieveEventLogInfoFailed_Text, string.Empty, findResult.ItemIndex, ((ListView)this).Columns.Count);
				}
				else
				{
					listViewItemCache.Add(findResult.ItemIndex, CreateDummyItem(Resources.RetrieveEventLogInfoFailed_Text, string.Empty, findResult.ItemIndex, ((ListView)this).Columns.Count));
				}
			}
		}
		return findResult.ItemIndex;
	}

	private void OnFindBackgroundOperationCompleted(object sender, BackgroundOperationCompletedEventArgs<FindArguments, int> e)
	{
		try
		{
			int num = -1;
			if (e.Success)
			{
				num = e.OperationResult;
				if (num >= 0)
				{
					if (((ListView)this).SelectedIndices.Count > 0)
					{
						((ListView)this).SelectedIndices.Clear();
					}
					((ListView)this).SelectedIndices.Add(num);
					((ListView)this).EnsureVisible(num);
				}
			}
			OnFindCompleted(new AsyncFindCompletedEventArgs(e.Error, e.Cancelled, null, num));
		}
		finally
		{
			CompleteAsyncOperation();
		}
	}

	public void Cancel()
	{
		queryBackgroundOperation.CancelOperations();
		findBackgroundOperation.CancelOperations();
		sortBackgroundOperation.CancelOperations();
		reverseBackgroundOperation.CancelOperations();
		querySet.Cancel();
	}

	public void EnableSort(bool enabled)
	{
		if (!((Control)(object)findDialog).Visible)
		{
			int count = ((ListView)this).SelectedIndices.Count;
			((BaseListView)this).IsSortable = enabled;
			((BaseListView)this).HeaderStyle = ((!enabled) ? ColumnHeaderStyle.Nonclickable : ColumnHeaderStyle.Clickable);
			if (((ListView)this).SelectedIndices.Count != count)
			{
				((ListView)(object)this).OnSelectedIndexChanged(EventArgs.Empty);
			}
		}
	}

	public void EnableFindHotKey(bool enabled)
	{
		findHotKeyEnabled = enabled;
	}

	private void OnQueryCompleted(AsyncCompletedEventArgs e)
	{
		this.QueryCompleted?.Invoke(this, e);
	}

	private void OnSortStarted()
	{
		((BaseListView)this).SortStarted();
	}

	private void OnSortCompleted(AsyncCompletedEventArgs e)
	{
		this.SortCompleted?.Invoke(this, e);
	}

	private void OnFindCompleted(AsyncFindCompletedEventArgs e)
	{
		this.FindCompleted?.Invoke(this, e);
	}

	private void ClearCache()
	{
		listViewItemCache.Clear();
	}

	private void OnResultsFetched(object sender, ResultsFetchedEventArgs e)
	{
		if (((Control)this).InvokeRequired)
		{
			((Control)this).Invoke((Delegate)new EventHandler<ResultsFetchedEventArgs>(OnResultsFetched), new object[2] { sender, e });
		}
		else if (lastRefresh == DateTime.MaxValue || (DateTime.Now - lastRefresh).TotalSeconds >= (double)refreshInterval || e.EOF)
		{
			((BaseListView)this).VirtualListSize = querySet.GetResultsCount();
			refreshInterval = Math.Min(refreshInterval * 2, 8);
			lastRefresh = DateTime.Now;
		}
	}

	public void ChooseColumns()
	{
		if (notifyUser.ShowDialog((Form)(object)columnChooserDialog) == DialogResult.OK)
		{
			ResetColumns();
		}
	}

	protected override void OnCacheVirtualItems(CacheVirtualItemsEventArgs e)
	{
		try
		{
			CacheListViewItems(e.StartIndex, e.EndIndex);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error caching list view items {0}-{1}", e.StartIndex, e.EndIndex);
		}
		((ListView)this).OnCacheVirtualItems(e);
	}

	protected override void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
	{
		try
		{
			EventListViewItem value = null;
			if (!listViewItemCache.TryGetValue(e.ItemIndex, out value))
			{
				CacheListViewItems(e.ItemIndex, Math.Min(e.ItemIndex + 64, ((BaseListView)this).VirtualListSize - 1));
				value = listViewItemCache[e.ItemIndex];
			}
			e.Item = value;
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error retrieving virtual item {0}", e.ItemIndex);
			lock (columnLock)
			{
				EventListViewItem item = CreateDummyItem(string.Empty, string.Empty, e.ItemIndex, ((ListView)this).Columns.Count);
				SetEvent(item, new EventLogEvent(ex), GetFormatters());
				e.Item = item;
			}
		}
		((ListView)this).OnRetrieveVirtualItem(e);
	}

	private void SortCache(SortOrder direction)
	{
		lock (eventCacheLock)
		{
			List<EventListViewItem> list = new List<EventListViewItem>();
			foreach (EventListViewItem value in listViewItemCache.Values)
			{
				list.Add(value);
			}
			list.Sort(new ListViewItemComparer((IComparer<EventLogEvent>)((ListView)this).Columns[sortColumn].Tag, direction));
			listViewItemCache.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				listViewItemCache.Add(i, list[i]);
			}
			((Control)this).Invalidate();
		}
	}

	protected override void Sort(SortOrder direction, int columnIndex)
	{
		try
		{
			int num = sortColumn;
			sortColumn = columnIndex;
			BeginAsyncOperation();
			OnSortStarted();
			if (listViewItemCache.Count == ((BaseListView)this).VirtualListSize)
			{
				SortCache(direction);
				CompleteAsyncOperation();
				OnSortCompleted(new AsyncCompletedEventArgs(null, cancelled: false, null));
			}
			else if (columnIndex == num)
			{
				reverseBackgroundOperation.QueueOperation((object)null);
			}
			else
			{
				sortBackgroundOperation.QueueOperation((object)null);
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error sorting events by column {0}", columnIndex);
			notifyUser.ShowError(ex, Resources.CannotSortQueryResults_Text);
		}
	}

	private void CacheListViewItems(int startIndex, int endIndex)
	{
		if (listViewItemCache.ContainsKey(startIndex))
		{
			return;
		}
		lock (columnLock)
		{
			GetEventsArguments args = default(GetEventsArguments);
			args.QueryId = queryId;
			args.StartIndex = startIndex;
			args.EndIndex = endIndex;
			args.Items = new List<EventListViewItem>();
			for (int i = 0; i < endIndex - startIndex + 1; i++)
			{
				args.Items.Add(CreateDummyItem(CommonResources.LoadingText, CommonResources.LoadingText, startIndex + i, ((ListView)this).Columns.Count));
			}
			BeginGetEventsBackgroundOperation(args);
		}
	}

	private List<IFormatter> GetFormatters()
	{
		List<IFormatter> list = new List<IFormatter>();
		ColumnHeader[] selectedColumns = columnChooserDialog.GetSelectedColumns();
		foreach (ColumnHeader columnHeader in selectedColumns)
		{
			list.Add((IFormatter)columnHeader.Tag);
		}
		return list;
	}

	private EventListViewItem CreateDummyItem(string reason, string message, int index, int columns)
	{
		EventLogEvent eventLogEvent = new EventLogEvent(reason, message);
		EventListViewItem eventListViewItem = new EventListViewItem(eventLogEvent.DummyItemReason);
		eventListViewItem.Event = eventLogEvent;
		for (int i = 1; i < columns; i++)
		{
			eventListViewItem.SubItems.Add(string.Empty);
		}
		listViewItemCache[index] = eventListViewItem;
		return eventListViewItem;
	}

	private void SetEvent(EventListViewItem item, EventLogEvent evt, List<IFormatter> formatters)
	{
		if (evt.IsDummyItem)
		{
			item.Text = evt.DummyItemReason;
			for (int i = 0; i < formatters.Count; i++)
			{
				item.SubItems[i + 1].Text = string.Empty;
			}
		}
		else
		{
			item.ImageIndex = IconsHelp.GetEventLogEntryIconIndex(evt.Level);
			item.Text = evt.LevelName;
			for (int j = 0; j < formatters.Count; j++)
			{
				item.SubItems[j + 1].Text = formatters[j].Format(evt);
			}
		}
		item.Event = evt;
	}

	protected override void OnSortRefresh()
	{
		sortColumn = -1;
	}

	private object SortBackgroundOperation(BackgroundOperationStatus dummy1, object dummy2)
	{
		querySet.Sort((IComparer<EventLogEvent>)((ListView)this).Columns[sortColumn].Tag);
		return null;
	}

	private object ReverseBackgroundOperation(BackgroundOperationStatus dummy1, object dummy2)
	{
		querySet.Reverse();
		return null;
	}

	private object ClearCacheOperation(BackgroundOperationStatus status, object arg)
	{
		ClearCache();
		return null;
	}

	private void ClearCacheOperationCompleted(object sender, BackgroundOperationCompletedEventArgs<object, object> e)
	{
		if (e.Error != null)
		{
			ExceptionHelp.LogException(e.Error, "Could not clear the cached events");
		}
	}

	private void StartClearCache()
	{
		clearCacheBackgroundOperation.QueueOperation((object)null);
	}

	private void OnSortBackgroundOperationCompleted(object sender, BackgroundOperationCompletedEventArgs<object, object> e)
	{
		try
		{
			if (e.Success)
			{
				StartClearCache();
				((Control)this).Invalidate();
			}
			OnSortCompleted(new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
		}
		finally
		{
			CompleteAsyncOperation();
		}
	}

	protected override void OnDrawItem(DrawListViewItemEventArgs e)
	{
		if (e.ItemIndex % 2 == 0)
		{
			e.Item.BackColor = (SystemInformation.HighContrast ? e.Item.ListView.BackColor : Color.Lavender);
		}
		else
		{
			e.Item.BackColor = e.Item.ListView.BackColor;
		}
		e.DrawDefault = true;
		((ListView)this).OnDrawItem(e);
	}

	protected override void OnSelectedIndexChanged(EventArgs e)
	{
		((ListView)this).OnSelectedIndexChanged(e);
		if (((ListView)this).SelectedIndices.Count == 0)
		{
			clusterEventsControl.Clear();
			return;
		}
		GetEventDetailsArguments args = default(GetEventDetailsArguments);
		args.SelectedIndex = ((ListView)this).SelectedIndices[0];
		args.SelectedEvent = listViewItemCache[args.SelectedIndex].Event;
		BeginEventDetailsBackgroundOperation(args);
	}

	private void BeginGetEventsBackgroundOperation(GetEventsArguments args)
	{
		cursorManager.BeginCursor(CursorType.DataLoad);
		if (!getEventsBackgroundOperation.QueueOperation(args))
		{
			cursorManager.EndCursor();
		}
	}

	private List<EventLogEvent> GetEventsBackgroundOperation(BackgroundOperationStatus dummy, GetEventsArguments args)
	{
		if (args.QueryId != queryId)
		{
			return null;
		}
		return querySet.GetResults(args.StartIndex, args.EndIndex);
	}

	private void OnGetEventsBackgroundOperationCompleted(object sender, BackgroundOperationCompletedEventArgs<GetEventsArguments, List<EventLogEvent>> e)
	{
		cursorManager.EndCursor();
		if (e.Cancelled || e.OperationParameter.QueryId != queryId)
		{
			return;
		}
		lock (columnLock)
		{
			List<IFormatter> formatters = GetFormatters();
			List<EventListViewItem> items = e.OperationParameter.Items;
			List<EventLogEvent> list = null;
			if (e.Error == null)
			{
				list = e.OperationResult;
			}
			else
			{
				list = new List<EventLogEvent>();
				for (int i = 0; i < items.Count; i++)
				{
					list.Add(new EventLogEvent(e.Error));
				}
			}
			for (int j = 0; j < items.Count; j++)
			{
				SetEvent(items[j], list[j], formatters);
			}
			((Control)this).Invalidate();
			if (((ListView)this).SelectedIndices.Count > 0 && e.OperationParameter.StartIndex <= ((ListView)this).SelectedIndices[0] && ((ListView)this).SelectedIndices[0] <= e.OperationParameter.EndIndex)
			{
				GetEventDetailsArguments args = default(GetEventDetailsArguments);
				args.SelectedIndex = ((ListView)this).SelectedIndices[0];
				args.SelectedEvent = listViewItemCache[args.SelectedIndex].Event;
				BeginEventDetailsBackgroundOperation(args);
			}
		}
	}

	private void BeginEventDetailsBackgroundOperation(GetEventDetailsArguments args)
	{
		clusterEventsControl.Clear(CommonResources.LoadingText);
		cursorManager.BeginCursor(CursorType.DataLoad);
		if (!getEventDetailsBackgroundOperation.QueueOperation(args))
		{
			cursorManager.EndCursor();
		}
	}

	private object GetEventDetailsBackgroundOperation(BackgroundOperationStatus dummy, GetEventDetailsArguments args)
	{
		return args.SelectedEvent.Message;
	}

	private void OnGetEventDetailsBackgroundOperationCompleted(object sender, BackgroundOperationCompletedEventArgs<GetEventDetailsArguments, object> e)
	{
		cursorManager.EndCursor();
		if (!((Control)this).Visible)
		{
			return;
		}
		if (e.Cancelled)
		{
			clusterEventsControl.Clear();
		}
		else if (e.Error != null)
		{
			ClusterLog.AdminEvents.WriteFailedRetrieveEventDescriptionEvent(ExceptionHelp.GetExceptionMessage(e.Error));
			ExceptionHelp.LogException(e.Error, "Failed retrieving the event description");
		}
		else if (e.Success)
		{
			if (((ListView)this).SelectedIndices.Count > 0 && ((ListView)this).SelectedIndices[0] == e.OperationParameter.SelectedIndex)
			{
				clusterEventsControl.Show(e.OperationParameter.SelectedEvent);
			}
			else
			{
				clusterEventsControl.Clear();
			}
		}
	}
}
