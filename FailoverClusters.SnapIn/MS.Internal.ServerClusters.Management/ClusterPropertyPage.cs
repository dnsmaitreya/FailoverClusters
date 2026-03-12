using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class ClusterPropertyPage : PropertyPage
{
	private INotifyUser notifyUser;

	private PropertyPageControlBase propertyPageControl;

	public void ClearControl()
	{
		if (propertyPageControl != null)
		{
			propertyPageControl.DirtyChanged -= OnDirtyChanged;
		}
	}

	public void SetControl(PropertyPageControlBase propertyPage)
	{
		Exceptions.ThrowIfNotEmpty(propertyPage.HelpTopic, "propertyPage.HelpTopic");
		try
		{
			propertyPageControl = propertyPage;
			base.Control = (Control)(object)propertyPageControl;
			base.Title = propertyPage.Title;
			propertyPageControl.DirtyChanged += OnDirtyChanged;
		}
		catch (Exception e)
		{
			ReportUnexpectedError(e);
		}
	}

	protected override void OnDestroy()
	{
		try
		{
			if (propertyPageControl != null)
			{
				propertyPageControl.DirtyChanged -= OnDirtyChanged;
				((Component)(object)propertyPageControl).Dispose();
			}
		}
		catch (Exception e)
		{
			ReportUnexpectedError(e);
		}
	}

	protected override void OnInitialize()
	{
		try
		{
			if (propertyPageControl != null)
			{
				notifyUser = (INotifyUser)(object)new PropertyPageNotifyUser(base.ParentSheet);
				propertyPageControl.Initialize(notifyUser);
			}
		}
		catch (Exception e)
		{
			ReportUnexpectedError(e);
		}
	}

	internal void OnDirtyChanged(object sender, EventArgs e)
	{
		base.Dirty = propertyPageControl.IsDirty;
	}

	protected override bool OnApply()
	{
		try
		{
			if (!propertyPageControl.IsDirty)
			{
				return true;
			}
			if (!propertyPageControl.ApplyChanges())
			{
				return false;
			}
			propertyPageControl.IsDirty = false;
			return true;
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex);
			return false;
		}
	}

	protected override bool OnOK()
	{
		return OnApply();
	}

	protected override bool OnKillActive()
	{
		return true;
	}

	protected override bool QueryCancel()
	{
		try
		{
			propertyPageControl.Cancel();
			return true;
		}
		catch (Exception e)
		{
			ReportUnexpectedError(e);
		}
		return false;
	}

	internal void ReportUnexpectedError(Exception e)
	{
		ExceptionHelp.LogException(e, "An unexpected error occurred in property page");
		notifyUser.ShowError(e, Resources.UnexpectedError_Text);
	}
}
