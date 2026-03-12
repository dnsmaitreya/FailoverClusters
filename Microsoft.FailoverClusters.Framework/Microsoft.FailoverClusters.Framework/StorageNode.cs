using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class StorageNode : ObservableCollectionItem<StorageNode>
{
	private readonly CommandCollection commands = new CommandCollection(ClusterCommandCollectionId.DiskInfo);

	public override string Key => ObjectId;

	public override string DisplayName => Name;

	public override IEnumerable<ICommand> Commands => commands;

	public string ObjectId { get; internal set; }

	public string Name { get; internal set; }

	public StorageNodeNameFormat NameFormat { get; internal set; }

	public IList<string> OtherIdentifyingInfo { get; internal set; }

	public IList<string> OtherIdentifyingInfoDescription { get; internal set; }

	public StorageNodeOperationalStatus OperationalStatus { get; internal set; }

	public StorageNode(ClusterObject owner)
		: base(owner)
	{
	}

	public static void GetInstance(Cluster cluster, string key, Action<OperationResult<StorageNode>> operationResult, string serverName = null)
	{
		ObservableCollectionItem<StorageNode>.GetInstance(cluster, key, operationResult, serverName);
	}

	public override void CopyFrom(StorageNode source)
	{
		Exceptions.ThrowIfNull(source, "source");
		if (ObjectId != source.ObjectId)
		{
			ObjectId = source.ObjectId;
			OnPropertyChanged("ObjectId");
		}
		if (Name != source.Name)
		{
			Name = source.Name;
			OnPropertyChanged("Name");
			OnPropertyChanged("DisplayName");
		}
		if (NameFormat != source.NameFormat)
		{
			NameFormat = source.NameFormat;
			OnPropertyChanged("NameFormat");
		}
		if (OtherIdentifyingInfo != source.OtherIdentifyingInfo)
		{
			OtherIdentifyingInfo = source.OtherIdentifyingInfo;
			OnPropertyChanged("OtherIdentifyingInfo");
		}
		if (OtherIdentifyingInfoDescription != source.OtherIdentifyingInfoDescription)
		{
			OtherIdentifyingInfoDescription = source.OtherIdentifyingInfoDescription;
			OnPropertyChanged("OtherIdentifyingInfoDescription");
		}
		if (OperationalStatus != source.OperationalStatus)
		{
			OperationalStatus = source.OperationalStatus;
			OnPropertyChanged("OperationalStatus");
		}
	}
}
