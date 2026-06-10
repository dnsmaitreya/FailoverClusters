using System;
using System.Collections.Generic;

namespace MS.Internal.ServerClusters.Management;

internal abstract class SelectItemStrategy
{
	private string title;

	private string instructions;

	private string fetchItemsMessage;

	private string noItemsMessage;

	private string[] columnNames;

	public string Title => title;

	public string Instructions => instructions;

	public string[] ColumnNames => columnNames;

	public string FetchItemsMessage => fetchItemsMessage;

	public string NoItemsMessage => noItemsMessage;

	public event EventHandler<EnumerationResultsEventArgs> EnumerationResultsReady;

	public event EventHandler EnumerationCompleted;

	protected SelectItemStrategy(string title, string instructions, string fetchItemsMessage, string noItemsMessage, string[] columnNames)
	{
		this.title = title;
		this.instructions = instructions;
		this.fetchItemsMessage = fetchItemsMessage;
		this.noItemsMessage = noItemsMessage;
		this.columnNames = columnNames;
	}

	public abstract void StartItemEnumeration();

	protected void OnEnumerationResultsReady(ICollection<ClusterListItem> results)
	{
		EnumerationResultsEventArgs eventArgs = new EnumerationResultsEventArgs(results);
		OnEnumerationResultsReady(eventArgs);
	}

	protected void OnEnumerationResultsReady(Exception error)
	{
		EnumerationResultsEventArgs eventArgs = new EnumerationResultsEventArgs(error);
		OnEnumerationResultsReady(eventArgs);
	}

	private void OnEnumerationResultsReady(EnumerationResultsEventArgs eventArgs)
	{
		this.EnumerationResultsReady?.Invoke(this, eventArgs);
	}

	protected void OnEnumerationComplete()
	{
		this.EnumerationCompleted?.Invoke(this, EventArgs.Empty);
	}
}
