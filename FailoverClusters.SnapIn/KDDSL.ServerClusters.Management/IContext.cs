using System;
using System.Collections.Generic;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal interface IContext : IDisposable
{
	ActionsPaneItemCollection ActionsPaneItems { get; }

	StandardVerbs EnabledStandardVerbs { get; }

	string DisplayName { get; }

	bool IsDisposed { get; }

	Guid Id { get; }

	string HelpTopic { get; }

	int ImageIndex { get; }

	List<WritableSharedDataItem> SharedData { get; }

	Guid NodeType { get; }

	bool EnableRefresh { get; }

	bool IsResetActionsNeeded { get; }

	event EventHandler ActionsUpdated;

	event EventHandler DisplayNameChanged;

	event EventHandler ImageIndexChanged;

	event EventHandler<DeletingEventArgs> Deleting;

	void RefreshStateBasedActions();

	void ClearActions();
}

