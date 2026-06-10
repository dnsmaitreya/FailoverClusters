using System;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;

namespace FailoverClusters.ClusterSnapIn;

public class ViewModelData
{
	public string NodeName { get; private set; }

	public Guid Id { get; private set; }

	public string HelpTopic { get; set; }

	public IViewCommandsProvider ViewCommandsProvider { get; set; }

	public event EventHandler Refresh;

	public event EventHandler TargetedRefresh;

	public event EventHandler Refreshed;

	public ViewModelData(Guid id, string nodeName)
	{
		Id = id;
		NodeName = nodeName;
	}

	public ViewModelData(Guid id)
		: this(id, null)
	{
	}

	public ViewModelData()
		: this(Guid.Empty)
	{
	}

	public void ClusterRefreshed(object sender, EventArgs e)
	{
		this.Refreshed.SafeCall(sender, e);
	}

	public void ProcessMessage(ViewModelDataSignal signal)
	{
		switch (signal)
		{
		case ViewModelDataSignal.Refresh:
			this.Refresh.SafeCall(this, new EventArgs());
			break;
		case ViewModelDataSignal.TargetedRefresh:
			this.TargetedRefresh.SafeCall(this, new EventArgs());
			break;
		default:
			throw new ArgumentException(Extensions.FormatCurrentCulture("Unknow signal {0} send to the view model", (object)signal.ToString()), "signal");
		}
	}
}

