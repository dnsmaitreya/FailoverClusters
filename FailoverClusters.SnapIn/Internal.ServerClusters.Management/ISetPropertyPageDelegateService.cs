using System;

namespace MS.Internal.ServerClusters.Management;

internal interface ISetPropertyPageDelegateService
{
	void SetPropertyPageDelegate(Action<object> showPropertyPageDelegate);
}
