using System;
using System.ComponentModel;
using System.Windows.Forms;
using FailoverClusters.Framework;
using ManagementConsole;
using KDDSL.ServerClusters;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.WinForms;

internal class ClusterSnapinPropertyPage : PropertyPage
{
	private PropertyPageNotifyUser notifyUser;

	private SnapinPropertyPageControlBase _snapinPropertyPageControl;

	public void SetControl(SnapinPropertyPageControlBase control)
	{
		try
		{
			_snapinPropertyPageControl = control;
			base.Control = (Control)(object)_snapinPropertyPageControl;
			base.Title = control.Title;
			_ = control.HelpTopic != Guid.Empty;
			_snapinPropertyPageControl.DirtyChanged += OnDirtyChanged;
		}
		catch (ClusterException e)
		{
			ReportUnexpectedError(e);
		}
	}

	protected override void OnDestroy()
	{
		try
		{
			if (_snapinPropertyPageControl != null)
			{
				_snapinPropertyPageControl.DirtyChanged -= OnDirtyChanged;
				((Component)(object)_snapinPropertyPageControl).Dispose();
			}
		}
		catch (ClusterException e)
		{
			ReportUnexpectedError(e);
		}
	}

	protected override void OnInitialize()
	{
		try
		{
			if (_snapinPropertyPageControl != null)
			{
				notifyUser = new PropertyPageNotifyUser(base.ParentSheet);
				_snapinPropertyPageControl.Initialize((INotifyUser)(object)notifyUser);
			}
		}
		catch (ClusterException e)
		{
			ReportUnexpectedError(e);
		}
	}

	private void OnDirtyChanged(object sender, EventArgs e)
	{
		if (_snapinPropertyPageControl != null)
		{
			base.Dirty = _snapinPropertyPageControl.IsDirty;
		}
	}

	protected override bool OnApply()
	{
		try
		{
			if (!_snapinPropertyPageControl.IsDirty)
			{
				return true;
			}
			if (!_snapinPropertyPageControl.ApplyChanges())
			{
				return false;
			}
			_snapinPropertyPageControl.IsDirty = false;
			return true;
		}
		catch (ClusterException e)
		{
			ReportUnexpectedError(e);
			return false;
		}
	}

	protected override bool OnOK()
	{
		return OnApply();
	}

	protected override void OnCancel()
	{
		base.OnCancel();
		_snapinPropertyPageControl.Cancel();
	}

	protected override bool OnKillActive()
	{
		try
		{
			return true;
		}
		catch (ClusterException e)
		{
			ReportUnexpectedError(e);
		}
		return false;
	}

	protected override bool QueryCancel()
	{
		try
		{
			return true;
		}
		catch (ClusterException e)
		{
			ReportUnexpectedError(e);
		}
		return false;
	}

	private static void ReportUnexpectedError(Exception e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ClusterDialogException.ShowTaskDialog(e);
	}
}

