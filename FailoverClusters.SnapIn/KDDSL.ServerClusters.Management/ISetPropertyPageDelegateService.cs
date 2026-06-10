using System;

namespace KDDSL.ServerClusters.Management;

internal interface ISetPropertyPageDelegateService
{
	void SetPropertyPageDelegate(Action<object> showPropertyPageDelegate);
}
