using System;

namespace KDDSL.ServerClusters.Management;

internal interface ISnapInPropertyPage
{
	bool IsDirty { get; set; }

	string Title { get; }

	event EventHandler DirtyChanged;

	bool ApplyChanges();

	void Initialize(INotifyUser notifyUser);
}
