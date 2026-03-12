using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterCommand : ICommand, INotifyPropertyChanged
{
	private readonly WeakEventHandler canExecuteChanged = WeakEvent.Create();

	private ClusterObject clusterObject;

	private readonly string name;

	private readonly ClusterCommandId id;

	private readonly ClusterCommandCollectionId category;

	private object commandParameter;

	private bool visible = true;

	private Action<object> customExecuteDelegate;

	private Predicate<object> customCanExecuteDelegate;

	private object customExecuteParameter;

	private string cannotExecuteReason;

	private EventHandler<ClusterProcessingEventArgs> updateCanExecute;

	private readonly StrongToWeakEvent<ClusterProcessingEventArgs> processingWeakEvent;

	public object CommandParameter
	{
		get
		{
			return commandParameter;
		}
		set
		{
			commandParameter = value;
		}
	}

	public ClusterCommandCollectionId Category => category;

	public ClusterObject ClusterObject => clusterObject;

	public bool Visible
	{
		get
		{
			return visible;
		}
		internal set
		{
			visible = value;
		}
	}

	public bool IgnoreIsProcessing { get; set; }

	internal Predicate<object> CanExecuteDelegate { get; set; }

	internal Action<object> ExecuteDelegate { get; set; }

	public string CannotExecuteReason
	{
		get
		{
			return cannotExecuteReason;
		}
		set
		{
			cannotExecuteReason = value;
			OnPropertyChanged("CannotExecuteReason");
		}
	}

	public string Text { get; set; }

	public string Description { get; set; }

	public string Name => name;

	public ClusterCommandId Id => id;

	public virtual string FullName => ToString();

	public virtual IInputParameter InputParameters { get; internal set; }

	public event EventHandler CanExecuteChanged
	{
		add
		{
			canExecuteChanged.Add(value);
		}
		remove
		{
			canExecuteChanged.Remove(value);
		}
	}

	public event EventHandler Finalizing;

	public event PropertyChangedEventHandler PropertyChanged;

	public ClusterCommand(ClusterObject clusterObject, string name, ClusterCommandId commandId, ClusterCommandCollectionId category, Action<object> execute, Predicate<object> canExecute)
		: this(clusterObject, name, commandId, category)
	{
		customExecuteDelegate = execute;
		customCanExecuteDelegate = canExecute;
	}

	public ClusterCommand(ClusterObject clusterObject, string name, ClusterCommandId commandId, ClusterCommandCollectionId category, Action<object> execute, Predicate<object> canExecute, object executeParameter)
		: this(clusterObject, name, commandId, category)
	{
		customExecuteDelegate = execute;
		customCanExecuteDelegate = canExecute;
		customExecuteParameter = executeParameter;
	}

	public ClusterCommand(ClusterObject clusterObject, string name, ClusterCommandId commandId)
		: this(clusterObject, name, commandId, ClusterCommandCollectionId.None)
	{
	}

	public ClusterCommand(ClusterObject clusterObject, string name, ClusterCommandId commandId, ClusterCommandCollectionId category)
	{
		updateCanExecute = CanExecuteUpdate;
		processingWeakEvent = new StrongToWeakEvent<ClusterProcessingEventArgs>(updateCanExecute);
		PerformanceCounters.Increment("Commands");
		this.name = name;
		id = commandId;
		this.category = category;
		IgnoreIsProcessing = false;
		if (clusterObject != null)
		{
			this.clusterObject = clusterObject;
			this.clusterObject.Processing += processingWeakEvent;
		}
	}

	~ClusterCommand()
	{
		if (clusterObject != null)
		{
			clusterObject.Processing -= processingWeakEvent;
			processingWeakEvent.Dispose();
		}
		EventHandler finalizing = this.Finalizing;
		if (finalizing != null)
		{
			try
			{
				finalizing(this, EventArgs.Empty);
			}
			catch (Exception)
			{
			}
		}
		PerformanceCounters.Decrement("Commands");
	}

	public bool CanExecute()
	{
		return CanExecute(commandParameter);
	}

	public void SetClusterObject(ClusterObject newClusterObject)
	{
		Exceptions.ThrowIfNull(newClusterObject, "newClusterObject");
		if (clusterObject != null)
		{
			clusterObject.Processing -= processingWeakEvent;
		}
		clusterObject = newClusterObject;
		clusterObject.Processing += processingWeakEvent;
	}

	public bool CanExecute(object parameter)
	{
		if (clusterObject != null && !IgnoreIsProcessing && clusterObject.IsProcessing && !(clusterObject is Cluster))
		{
			return false;
		}
		bool flag = true;
		if (customCanExecuteDelegate != null)
		{
			flag = customCanExecuteDelegate(customExecuteParameter ?? parameter);
		}
		else if (CanExecuteDelegate != null)
		{
			flag = CanExecuteDelegate((parameter != null) ? parameter : commandParameter);
		}
		if (flag)
		{
			CannotExecuteReason = string.Empty;
		}
		return flag;
	}

	public virtual void Execute()
	{
		Execute(commandParameter);
	}

	public virtual void Execute(object parameter)
	{
		if (customExecuteDelegate != null)
		{
			customExecuteDelegate(customExecuteParameter ?? parameter);
		}
		else if (ExecuteDelegate != null)
		{
			ExecuteDelegate(parameter);
		}
	}

	public void RegisterExecuteDelegate(Action<object> execute)
	{
		customExecuteDelegate = execute;
	}

	public void RegisterCanExecuteDelegate(Predicate<object> canExecute)
	{
		customCanExecuteDelegate = canExecute;
	}

	public void RegisterExecuteParameter(object parameter)
	{
		customExecuteParameter = parameter;
	}

	public void CanExecuteUpdate(object sender, EventArgs e)
	{
		canExecuteChanged(sender, e);
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.CurrentCulture, "ClusterCommand: '{0}'/'{1}' of type '{2}'", name, id, GetType().ToString());
	}

	public static IList<CommandCollection> GetCommands(IList objects)
	{
		Exceptions.ThrowIfNull(objects, "objects");
		return ClusterMultiCommand.GetCommands(objects);
	}

	public static IList<CommandCollection> GetCommands(IEnumerable<Group> groups)
	{
		Exceptions.ThrowIfNull(groups, "groups");
		return ClusterMultiCommand.GetCommands(groups);
	}

	public static IList<CommandCollection> GetCommands(IEnumerable<Resource> resources)
	{
		Exceptions.ThrowIfNull(resources, "resources");
		return ClusterMultiCommand.GetCommands(resources);
	}

	public static IList<CommandCollection> GetCommands(IEnumerable<FileShare> shares)
	{
		Exceptions.ThrowIfNull(shares, "shares");
		return ClusterMultiCommand.GetCommands(shares);
	}

	public static IList<CommandCollection> GetCommands(IEnumerable<Node> nodes)
	{
		Exceptions.ThrowIfNull(nodes, "nodes");
		return ClusterMultiCommand.GetCommands(nodes);
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
}
