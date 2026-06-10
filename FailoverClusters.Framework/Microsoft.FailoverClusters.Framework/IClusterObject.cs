using System;
using System.ComponentModel;

namespace FailoverClusters.Framework;

public interface IClusterObject : IDataItem
{
	string Name { get; }

	string Information { get; }

	ClusterException Error { get; }

	bool IsProcessing { get; }

	int LoadSelection { get; }

	ClusterPropertyCollection Properties { get; }

	event EventHandler<ClusterInformationEventArgs> InformationChanged;

	event EventHandler<ClusterProcessingEventArgs> Processing;

	event PropertyChangedEventHandler PropertyChanged;

	event EventHandler<ClusterErrorEventArgs> ErrorOccurred;

	event EventHandler<ClusterRenamedEventArgs> Renamed;

	event EventHandler<ClusterOpenEventArgs> Opened;

	event EventHandler<ClusterRemovedEventArgs> Removed;

	event EventHandler<ClusterLoadedEventArgs> Loaded;

	event EventHandler<ClusterAddedEventArgs> Created;

	event EventHandler<ClusterPropertiesEventArgs> PropertiesChanged;

	void Refresh(bool targeted);

	void Refresh(bool targeted, Action<OperationResult> operationResult);

	void RedirectAsyncOutput(Action asyncActions, Action<OperationResult> asyncResult);

	void Delete(bool askConfirmation = false);

	void Delete(Action<OperationResult> operationResult, bool askConfirmation = false);
}

