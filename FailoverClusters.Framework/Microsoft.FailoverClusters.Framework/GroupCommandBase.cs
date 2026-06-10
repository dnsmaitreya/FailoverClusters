namespace FailoverClusters.Framework;

public abstract class GroupCommandBase : WeakLazyBase<ClusterCommand>
{
	protected bool UpdateCanExecuteOnStateChange { get; set; }

	protected bool UpdateCanExecuteOnApplicationStatusChange { get; set; }

	public Group ClusterGroup { get; set; }

	protected GroupCommandBase(Group group)
	{
		ClusterGroup = group;
	}

	protected override void DoPostGenerateInstance(ClusterCommand newValue)
	{
		if (UpdateCanExecuteOnStateChange)
		{
			ClusterGroup.StateChanged += UpdateCanExecute;
			newValue.Finalizing += delegate
			{
				ClusterGroup.StateChanged -= UpdateCanExecute;
			};
		}
		if (UpdateCanExecuteOnApplicationStatusChange)
		{
			ClusterGroup.ApplicationStatusChanged += UpdateCanExecute;
			newValue.Finalizing += delegate
			{
				ClusterGroup.ApplicationStatusChanged -= UpdateCanExecute;
			};
		}
		base.DoPostGenerateInstance(newValue);
	}

	protected void TryUpdateCanExecute()
	{
		TryUpdateCanExecute(TryGetInstance());
	}

	protected void TryUpdateCanExecute(ClusterCommand clusterCommand)
	{
		clusterCommand?.CanExecuteUpdate(null, null);
	}

	protected void UpdateCanExecute(object sender, ClusterEventArgs e)
	{
		TryUpdateCanExecute();
	}
}

