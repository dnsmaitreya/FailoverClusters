using System.ComponentModel;

namespace KDDSL.ServerClusters.Management;

internal class ResourcePropertiesPage : PropertyPageControlBase
{
	private ResourceContext context;

	private bool saveRequiresRecycling;

	private IContainer components;

	protected ResourceContext Context => context;

	protected bool SaveRequiresRecycling
	{
		get
		{
			return saveRequiresRecycling;
		}
		set
		{
			saveRequiresRecycling = value;
		}
	}

	protected bool OfflineDependencies { get; set; }

	public ResourcePropertiesPage()
	{
		InitializeComponent();
	}

	protected ResourcePropertiesPage(ResourceContext context, string title)
		: base(title)
	{
		this.context = context;
		InitializeComponent();
	}

	protected void SaveProperties(PropertyCollection properties)
	{
		saveRequiresRecycling = properties.SaveChanges() == PropertySaveStatus.ResourceRequiresRecycle;
	}

	protected override void CompleteSaveProperties()
	{
		if (saveRequiresRecycling)
		{
			ClusterHelp.RecycleResourceAfterPropertiesSave(Context.Resource, base.NotifyUser, OfflineDependencies);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (context != null)
			{
				context = null;
			}
			if (components != null)
			{
				components.Dispose();
			}
		}
		((SnapinUserControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		components = new Container();
	}
}
