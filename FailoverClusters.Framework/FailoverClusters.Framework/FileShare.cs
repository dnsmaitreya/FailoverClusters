using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using FailoverClusters.UI.Common;
using FileServer.Management.ServerManagerProxy;
using WindowsAPICodePack.Dialogs;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class FileShare : INotifyPropertyChanged, IFileShareDataItem, IDataItem
{
	internal struct SecurityDescriptor
	{
		public byte Revision;

		public byte Size;

		public short Control;

		public IntPtr Owner;

		public IntPtr Group;

		public IntPtr Sacl;

		public IntPtr Dacl;
	}

	private WeakReferenceEx applicationCommandsWeak;

	private WeakReferenceEx memberStopSharingCommandWeak;

	private WeakReferenceEx memberRefreshShareCommandWeak;

	private int version;

	private readonly Guid id = Guid.NewGuid();

	private string name;

	private ShareInfoType shareInfoType;

	private string path;

	private string remark;

	private int maxUses;

	private int currentUses;

	private string serverName;

	private FileShareProtocol protocol = FileShareProtocol.Unknown;

	private FileShareCollection memberOwner;

	private bool continuousAvailability;

	private ClusterException error;

	internal int Version
	{
		get
		{
			return version;
		}
		set
		{
			version = value;
		}
	}

	public virtual IEnumerable<ICommand> Commands => ApplicationCommands;

	public CommandCollection ApplicationCommands => WeakReferenceEx.ReturnInstance(ref applicationCommandsWeak, delegate
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.FileShare);
		InitializeApplicationCommands(commandCollection);
		return commandCollection;
	});

	internal string Key => serverName + "\\" + name;

	public ShareEventType EventType { get; internal set; }

	public ClusterException Error
	{
		get
		{
			return error;
		}
		set
		{
			if (error != value)
			{
				error = value;
				OnPropertyChanged("Error");
			}
		}
	}

	public FileShareCollection Owner
	{
		get
		{
			return memberOwner;
		}
		internal set
		{
			if (memberOwner != value)
			{
				memberOwner = value;
				OnPropertyChanged("Owner");
			}
		}
	}

	public bool IsProcessing => false;

	public Cluster Cluster { get; internal set; }

	public string Name
	{
		get
		{
			return name;
		}
		internal set
		{
			if (!(name == value))
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}
	}

	public FileShareProtocol Protocol
	{
		get
		{
			return protocol;
		}
		internal set
		{
			protocol = value;
			OnPropertyChanged("Protocol");
		}
	}

	public bool Deleted { get; internal set; }

	public ShareInfoType ShareInfoType
	{
		get
		{
			return shareInfoType;
		}
		internal set
		{
			if (shareInfoType != value)
			{
				shareInfoType = value;
				OnPropertyChanged("ShareInfoType");
			}
		}
	}

	public string Remark
	{
		get
		{
			return remark;
		}
		set
		{
			if (!(remark == value))
			{
				remark = value;
				OnPropertyChanged("Remark");
			}
		}
	}

	public int MaxUses
	{
		get
		{
			return maxUses;
		}
		set
		{
			if (maxUses != value)
			{
				maxUses = value;
				OnPropertyChanged("MaxUses");
			}
		}
	}

	public int CurrentUses
	{
		get
		{
			return currentUses;
		}
		internal set
		{
			if (currentUses != value)
			{
				currentUses = value;
				OnPropertyChanged("CurrentUses");
			}
		}
	}

	public string Path
	{
		get
		{
			return path;
		}
		internal set
		{
			if (!(path == value))
			{
				path = value;
				OnPropertyChanged("Path");
			}
		}
	}

	public string ServerName
	{
		get
		{
			return serverName;
		}
		internal set
		{
			if (!(serverName == value))
			{
				serverName = value;
				OnPropertyChanged("ServerName");
			}
		}
	}

	public bool ContinuousAvailability
	{
		get
		{
			return continuousAvailability;
		}
		internal set
		{
			if (continuousAvailability != value)
			{
				continuousAvailability = value;
				OnPropertyChanged("ContinuousAvailability");
			}
		}
	}

	public string DisplayName => Name;

	public Guid Id => id;

	internal string ConnectionFqdn { get; set; }

	public string VcoFqdn { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void InitializeApplicationCommands(CommandCollection commandsCollection)
	{
		ClusterCommand clusterCommand = WeakReferenceEx.ReturnInstance(ref memberStopSharingCommandWeak, () => new ClusterCommand(null, "StopSharing", ClusterCommandId.FileShareStopSharing, commandsCollection.Category)
		{
			Text = CommonResources.FileShare_StopSharing,
			CanExecuteDelegate = (object x) => ((uint)shareInfoType & 0x80000000u) != 2147483648u,
			ExecuteDelegate = delegate
			{
				Delete(confirmation: true);
			}
		});
		clusterCommand.Finalizing += delegate
		{
			memberStopSharingCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand);
		ClusterCommand clusterCommand2 = WeakReferenceEx.ReturnInstance(ref memberRefreshShareCommandWeak, () => new ClusterCommand(null, "RefreshShare", ClusterCommandId.FileShareRefreshShare, ClusterCommandCollectionId.FileShareView)
		{
			Text = CommonResources.FileShare_RefreshShare,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				Refresh();
			}
		});
		clusterCommand2.Finalizing += delegate
		{
			memberRefreshShareCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand2);
		ClusterCommand item = new ClusterCommand(null, "SharesProperties", ClusterCommandId.FileShareProperties, ClusterCommandCollectionId.FileShare)
		{
			Text = CommandResources.Properties,
			ExecuteDelegate = delegate
			{
				OpenProperties();
			}
		};
		commandsCollection.Add(item);
	}

	internal static CommandCollection InitializeApplicationCommands(IEnumerable<FileShare> shares)
	{
		return new CommandCollection(ClusterCommandCollectionId.MultipleFileShare)
		{
			new ClusterCommand(null, "StopSharing", ClusterCommandId.FileShareStopSharing, ClusterCommandCollectionId.MultipleFileShare)
			{
				Text = CommonResources.FileShare_StopSharing,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					DeleteShares(shares, confirmation: true);
				}
			},
			new ClusterCommand(null, "RefreshShare", ClusterCommandId.FileShareRefreshShare, ClusterCommandCollectionId.FileShareView)
			{
				Text = CommonResources.FileShare_RefreshShare,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					RefreshShares(shares);
				}
			}
		};
	}

	internal static void DeleteShares(IEnumerable<FileShare> shares, bool confirmation = false)
	{
		if (new ConfirmationDialog
		{
			Icon = TaskDialogStandardIcon.Question,
			Caption = DialogResources.DeleteMultipleShares_Title,
			Header = DialogResources.DeleteMultipleShares_Header,
			Content = DialogResources.DeleteMultipleShares_Content.FormatCurrentCulture(shares.Count())
		}.ShowDialog() != TaskDialogResult.Yes)
		{
			return;
		}
		Cluster cluster = shares.Select((FileShare share) => share.Cluster).FirstOrDefault();
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).FileServer.DeleteShares(shares.Where((FileShare share) => !share.ShareInfoType.HasFlag(ShareInfoType.Special)));
		}, delegate(OperationResult operationResult)
		{
			cluster.Error = operationResult.Error;
		}, LockAccess.Reader);
	}

	internal static void RefreshShares(IEnumerable<FileShare> shares)
	{
		FileShare fileShare = shares.FirstOrDefault();
		if (fileShare == null)
		{
			return;
		}
		FileShareCollection owner = fileShare.Owner;
		if (owner != null)
		{
			if (owner.OwnerGroup != null)
			{
				owner.OwnerGroup.ClearFileShareErrors();
			}
			owner.UpdateShares();
			return;
		}
		foreach (FileShare share in shares)
		{
			share.Refresh();
		}
	}

	internal FileShare()
	{
	}

	public FileShare(FileShare source)
		: this()
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		name = source.name;
		CopyFrom(source);
	}

	public void OpenProperties()
	{
		PropertyPageType propertyPageType = PropertyPageType.NfsShare;
		propertyPageType = Protocol switch
		{
			FileShareProtocol.Nfs => PropertyPageType.NfsShare, 
			FileShareProtocol.Smb => PropertyPageType.SmbShare, 
			_ => throw new InvalidOperationException("Unknown Share Protocol"), 
		};
		try
		{
			ServerManagerProxy.OpenPropertyPage(propertyPageType, VcoFqdn, Name);
		}
		catch (ClusterDefaultException ex)
		{
			ClusterDialogException.ShowTaskDialog(ex);
		}
	}

	public void Delete(bool confirmation)
	{
		DeleteShares(new List<FileShare> { this }, confirmation);
	}

	public static void Delete(IEnumerable<FileShare> fileShares, bool confirmation)
	{
		DeleteShares(fileShares, confirmation);
	}

	public void Refresh()
	{
		FileShareCollection owner = Owner;
		if (owner != null)
		{
			owner.Clear();
			if (owner.OwnerGroup != null)
			{
				owner.OwnerGroup.ClearFileShareErrors();
			}
			owner.UpdateShares();
		}
	}

	public void Commit(Action<OperationResult> operationResult)
	{
		Cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).FileServer.SetShare(this);
		}, operationResult, LockAccess.Reader);
	}

	public static void GetShare(Cluster cluster, string serverName, string netName, Action<OperationResult<FileShare>> operationResult)
	{
		GetShare(ResultExecution.DoNotCare, cluster, serverName, netName, operationResult);
	}

	public static void GetShare(ResultExecution resultExecution, Cluster cluster, string serverName, string netName, Action<OperationResult<FileShare>> operationResult)
	{
		cluster.ExecuteMethod(resultExecution, delegate(ILockable lockObject)
		{
			FileShare share = ((PCluster)lockObject.Owner).FileServer.GetShare(serverName, netName);
			share.Cluster = cluster;
			return share;
		}, operationResult, LockAccess.Reader);
	}

	protected void OnPropertyChanged(string propertyName)
	{
		PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
		if (propertyChanged != null)
		{
			UIHelper.ExecuteOnDispatcher(delegate
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}, OperationType.Async);
		}
	}

	internal void CopyFrom(FileShare source)
	{
		ShareInfoType = source.shareInfoType;
		if (!string.IsNullOrWhiteSpace(source.path))
		{
			Path = source.path;
		}
		EventType = source.EventType;
		Remark = source.remark;
		MaxUses = source.maxUses;
		CurrentUses = source.currentUses;
		ServerName = source.serverName;
		Cluster = source.Cluster;
		ContinuousAvailability = source.continuousAvailability;
		ConnectionFqdn = source.ConnectionFqdn;
	}
}

