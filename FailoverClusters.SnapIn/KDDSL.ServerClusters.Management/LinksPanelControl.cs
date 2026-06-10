using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class LinksPanelControl : SnapinUserControl
{
	private delegate void OnChildAddedDelegate(object sender, ChildAddedEventArgs e);

	private delegate void OnChildDeletedDelegate(object sender, ChildDeletedEventArgs e);

	private IContainer components;

	private Collection<Control> items;

	private ManagementConsole.View view;

	private IScopeNodeContext parentContext;

	private readonly bool twoColumns;

	private StartPageContainerControl baseFormControl;

	public LinksPanelControl()
	{
		((Control)(object)this).Font = SystemFonts.DefaultFont;
		InitializeComponent();
	}

	public LinksPanelControl(StartPageContainerControl baseFormControl, string name)
		: this(baseFormControl, name, twoColumns: false)
	{
	}

	public LinksPanelControl(StartPageContainerControl baseFormControl, string name, bool twoColumns)
		: this()
	{
		items = new Collection<Control>();
		InitializeComponent();
		this.baseFormControl = baseFormControl;
		((Control)(object)this).BackColor = SystemColors.Window;
		((Control)this).Name = name;
		view = this.baseFormControl.View;
		this.twoColumns = twoColumns;
	}

	public static LinksPanelControl CreateShortcutPanel(StartPageContainerControl baseFormControl, IScopeNodeContext parentContext)
	{
		LinksPanelControl linksPanelControl = new LinksPanelControl(baseFormControl, "shortcutPanel", twoColumns: true);
		linksPanelControl.parentContext = parentContext;
		try
		{
			linksPanelControl.parentContext.ChildAdded += linksPanelControl.OnChildContextAdded;
			linksPanelControl.parentContext.ChildDeleted += linksPanelControl.OnChildContextDeleted;
			linksPanelControl.PopulateChildContexts();
		}
		catch (ObjectDisposedException ex)
		{
			ClusterLog.LogException((Exception)ex, "Context was disposed whilst adding children to the links panel.");
		}
		return linksPanelControl;
	}

	private void OnChildContextAdded(object sender, ChildAddedEventArgs e)
	{
		if (SynchronizeInvoke.InvokeRequired)
		{
			SynchronizeInvoke.BeginInvoke((Delegate)new OnChildAddedDelegate(OnChildContextAdded), new object[2] { sender, e });
		}
		else
		{
			AddShortcutLink(e.ChildContext);
		}
	}

	private void OnChildContextDeleted(object sender, ChildDeletedEventArgs e)
	{
		if (((Control)this).InvokeRequired)
		{
			((Control)this).BeginInvoke((Delegate)new OnChildDeletedDelegate(OnChildContextDeleted), new object[2] { sender, e });
		}
		else
		{
			RemoveShortcutLink(e.ChildName);
		}
	}

	private void PopulateChildContexts()
	{
		List<IContext> childContexts = parentContext.GetChildContexts();
		items.Clear();
		((Control)this).Controls.Clear();
		foreach (IScopeNodeContext item in childContexts)
		{
			AddShortcutLink(item);
		}
	}

	public static LinksPanelControl CreateMoreInformationPanel(StartPageContainerControl baseFormControl)
	{
		LinksPanelControl linksPanelControl = new LinksPanelControl(baseFormControl, "informationPanel");
		linksPanelControl.AddUrlLink(Resources.FailoverTopics_Title_Text, "https://go.com/fwlink/?LinkID=230640");
		linksPanelControl.AddUrlLink(Resources.FailoverCommunities_Title_Text, "https://go.com/fwlink/?LinkID=230641");
		linksPanelControl.AddUrlLink(Resources.MicrosoftSupport_Title_Text, "https://go.com/fwlink/?LinkID=230642");
		return linksPanelControl;
	}

	public static LinksPanelControl CreateActionPanel(StartPageContainerControl baseFormControl, ActionsPaneItemCollection actions, ActionTypes actionTypes, string panelText)
	{
		LinksPanelControl linksPanelControl = new LinksPanelControl(baseFormControl, "actionPanel");
		linksPanelControl.AddText(panelText);
		linksPanelControl.AddActionCollection(actions, actionTypes);
		return linksPanelControl;
	}

	private void AddActionCollection(ActionsPaneItemCollection actions, ActionTypes actionTypes)
	{
		foreach (ActionsPaneItem action2 in actions)
		{
			if (action2 is ActionBase actionBase)
			{
				if (actionBase.Tag != null)
				{
					ActionData.GetActionData(actionBase);
					ActionTypes actionTypes2 = ActionTypes.None;
					actionTypes2 = ActionTypes.Standard;
					if ((actionTypes & actionTypes2) != 0)
					{
						ActionBase action = (SyncAction)Utilities.CloneActions((SyncAction)actionBase);
						AddActionLink(action);
					}
				}
			}
			else if (action2 is ActionGroup actionGroup)
			{
				AddActionCollection(actionGroup.Items, actionTypes);
			}
		}
	}

	public void AddText(string panelText)
	{
		Label label = new Label();
		label.Text = panelText;
		label.AutoSize = false;
		AddChildControl(label);
	}

	public void AddActionLink(ActionBase action)
	{
		AddChildControl(LinksPanelActionControl.CreateActionLink(view, action));
	}

	public void AddUrlLink(string linkText, string url)
	{
		AddChildControl(LinksPanelLinkControl.CreateUrlLink(linkText, url));
	}

	private void RemoveShortcutLink(string shortCutName)
	{
		Control control = null;
		foreach (LinksPanelLinkControl item in items)
		{
			if (item.Compare(shortCutName))
			{
				control = (Control)(object)item;
				break;
			}
		}
		if (control != null)
		{
			items.Remove(control);
			((Control)this).Controls.Remove(control);
			LayoutChildren();
		}
	}

	private void AddShortcutLink(IContext context)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		UIThreadHandlerV val = (UIThreadHandlerV)delegate
		{
			AddChildControl(LinksPanelLinkControl.CreateShortcutLink(view, context, null));
		};
		if (SynchronizeInvoke.InvokeRequired)
		{
			SynchronizeInvoke.Invoke((Delegate)(object)val);
		}
		else
		{
			val.Invoke();
		}
	}

	internal void AddShortcutLink(IContext context, string shortcutText)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		UIThreadHandlerV val = (UIThreadHandlerV)delegate
		{
			AddChildControl(LinksPanelLinkControl.CreateShortcutLink(view, context, shortcutText));
		};
		if (SynchronizeInvoke.InvokeRequired)
		{
			SynchronizeInvoke.Invoke((Delegate)(object)val);
		}
		else
		{
			val.Invoke();
		}
	}

	private void AddChildControl(Control childControl)
	{
		if (!items.Contains(childControl))
		{
			childControl.Location = new Point(StartPageLayout.Padding, 0);
			items.Add(childControl);
			((Control)this).Controls.Add(childControl);
			LayoutChildren();
		}
	}

	private void LayoutChildren()
	{
		if (items != null && items.Count > 0)
		{
			int num = StartPageLayout.Padding;
			int num2 = 0;
			bool flag = true;
			((Control)this).SuspendLayout();
			int num3 = ((Control)this).Width / 2 + StartPageLayout.Padding;
			foreach (Control item in items)
			{
				if (twoColumns)
				{
					if (flag)
					{
						item.Location = new Point(StartPageLayout.Padding, 0);
						item.Width = num3 - item.Location.X - StartPageLayout.Padding;
					}
					else
					{
						item.Width = ((Control)this).Width - num3 - StartPageLayout.Padding;
						item.Location = new Point(num3 + StartPageLayout.Padding, 0);
					}
				}
				else
				{
					item.Width = ((Control)this).Width - item.Location.X - StartPageLayout.Padding;
				}
				item.Top = num;
				if (item is Label label)
				{
					label.Height = TextRenderer.MeasureText(label.Text, label.Font, new Size(label.Width, int.MaxValue), TextFormatFlags.WordBreak).Height;
				}
				if (twoColumns)
				{
					if (flag)
					{
						flag = false;
						num2 = item.Height + StartPageLayout.Padding;
					}
					else
					{
						flag = true;
						num += Math.Max(item.Height + StartPageLayout.Padding, num2);
					}
				}
				else
				{
					num += item.Height + StartPageLayout.Padding;
				}
			}
			if (twoColumns && !flag)
			{
				num += num2;
			}
			((Control)this).Height = num;
			((Control)this).ResumeLayout(performLayout: true);
		}
		else
		{
			((Control)this).Height = 0;
		}
	}

	protected override void OnResize(EventArgs e)
	{
		LayoutChildren();
		((UserControl)this).OnResize(e);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (components != null)
			{
				components.Dispose();
			}
			if (parentContext != null)
			{
				parentContext.ChildAdded -= OnChildContextAdded;
				parentContext.ChildDeleted -= OnChildContextDeleted;
				parentContext = null;
			}
		}
		((SnapinUserControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LinksPanelControl));
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).BackColor = SystemColors.ControlLightLight;
		((Control)this).Name = "LinksPanelControl";
		((Control)this).ResumeLayout(performLayout: false);
	}
}

