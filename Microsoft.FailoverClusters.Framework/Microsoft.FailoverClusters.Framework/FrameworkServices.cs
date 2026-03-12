using Microsoft.FailoverClusters.UI.Common.Services;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public static class FrameworkServices
{
	public static void Initialize()
	{
		ServiceContainer.Container.RegisterServiceInstance(typeof(IClusterExecutor), new ClusterExecutor());
		ServiceContainer.Container.RegisterServiceInstance(typeof(IResourceExecutor), new ResourceExecutor());
		ServiceContainer.Container.RegisterService(typeof(IConnectionAdapter), typeof(ClusApiAdapter));
		ServiceContainer.Container.RegisterService(typeof(IVirtualizationAdapter), typeof(WmiVmAdapter));
		ServiceContainer.Container.RegisterService(typeof(IWin32Adapter), typeof(Win32Adapter));
		ServiceContainer.Container.RegisterService(typeof(IFileShareAdapter), typeof(FileShareAdapter));
		ServiceContainer.Container.RegisterService(typeof(NotificationManager));
		ServiceContainer.Container.RegisterServiceInstance(typeof(ICimUtilities), new CimUtilities());
		ClusterDataService serviceInstance = new ClusterDataService();
		ServiceContainer.Container.RegisterServiceInstance(typeof(IClusterDataChangedService), serviceInstance);
		ServiceContainer.Container.RegisterServiceInstance(typeof(IClusterDataService), serviceInstance);
	}
}
