using Microsoft.FailoverClusters.UI.Common.Services;
using Microsoft.FailoverClusters.UIFramework;
using Microsoft.ManagementConsole;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.SnapIn;

internal static class Services
{
	public static void Initialize(SnapInBase snapIn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		SelectionService val = new SelectionService();
		ServiceContainer.Container.RegisterServiceInstance(typeof(ICommandsProvider), (object)new CommandsProvider());
		ServiceContainer.Container.RegisterServiceInstance(typeof(ISelectionContextService), (object)val);
		ServiceContainer.Container.RegisterServiceInstance(typeof(ISelectionChangedService), (object)val);
		ServiceContainer.Container.RegisterServiceInstance(typeof(IUiActionProducer), (object)new UiActionProducer());
		ServiceContainer.Container.RegisterServiceInstance(typeof(IPropertyChangedEventFilter), (object)new PropertyChangedEventFilter());
		ShowPropertyPageService showPropertyPageService = new ShowPropertyPageService();
		ServiceContainer.Container.RegisterServiceInstance(typeof(IShowPropertyPageService), (object)showPropertyPageService);
		ServiceContainer.Container.RegisterServiceInstance(typeof(ISetPropertyPageDelegateService), (object)showPropertyPageService);
		ServiceContainer.Container.RegisterServiceInstance(typeof(IClusterEventsMonitorRegistry), (object)new ClusterEventsMonitorRegistry());
	}
}
