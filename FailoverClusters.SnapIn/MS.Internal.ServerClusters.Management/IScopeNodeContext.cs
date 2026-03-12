using System;
using System.Collections.Generic;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal interface IScopeNodeContext : IContext, IDisposable
{
	bool ChildrenPossible { get; }

	bool ClearActionsOnDeactivateScopeNode { get; }

	int SelectedImageIndex { get; }

	ViewDescriptionCollection ViewDescriptions { get; }

	bool Initialized { get; }

	bool Deleted { get; }

	event EventHandler<ChildDeletedEventArgs> ChildDeleted;

	event EventHandler<ChildAddedEventArgs> ChildAdded;

	event EventHandler<ChildInsertedEventArgs> ChildInserted;

	event EventHandler AsyncChildEnumerationComplete;

	event EventHandler ContextCleared;

	List<IContext> GetChildContexts();

	List<IContext> GetChildContexts(AsyncEnumOptions options);

	void Initialize();

	void EnumerateChildrenAsync();

	void ExecuteUnderActionsLock(Action<ActionsPaneItemCollection> action);
}
