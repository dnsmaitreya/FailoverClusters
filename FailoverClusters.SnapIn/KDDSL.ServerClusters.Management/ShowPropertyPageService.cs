using System;
using FailoverClusters.Framework;
using FailoverClusters.UIFramework;

namespace KDDSL.ServerClusters.Management;

internal class ShowPropertyPageService : IShowPropertyPageService, ISetPropertyPageDelegateService
{
	private Action<object> ShowPropertyPageDelegate { get; set; }

	public void SetPropertyPageDelegate(Action<object> showPropertyPageDelegate)
	{
		ShowPropertyPageDelegate = showPropertyPageDelegate;
	}

	public void Show(object dataItem)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		if (ShowPropertyPageDelegate != null)
		{
			((ClusterCommand)new UICommandProperties("ViewProperties", dataItem, ShowPropertyPageDelegate)).Execute();
		}
	}
}

