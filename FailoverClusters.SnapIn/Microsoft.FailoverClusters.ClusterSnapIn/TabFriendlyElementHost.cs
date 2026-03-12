using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.ClusterSnapIn;

public class TabFriendlyElementHost : ElementHost
{
	private static TabFriendlyElementHost instance;

	private static readonly object lockObject = new object();

	private const uint WM_KEYDOWN = 256u;

	private const uint GA_PARENT = 1u;

	public static TabFriendlyElementHost Current
	{
		get
		{
			if (instance == null)
			{
				lock (lockObject)
				{
					if (instance == null)
					{
						instance = new TabFriendlyElementHost();
					}
				}
			}
			return instance;
		}
	}

	private TabFriendlyElementHost()
	{
		base.Disposed += delegate
		{
			lock (lockObject)
			{
				instance = null;
			}
		};
	}

	public void ClearReflectParent()
	{
		MethodInfo method = typeof(Control).GetMethod("UpdateReflectParent", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (method != null)
		{
			method.Invoke(this, new object[1] { false });
		}
		else
		{
			ClusterLog.LogInfo("Method UpdateReflectParent was not found.");
		}
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern bool PostMessage(IntPtr hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	private static extern IntPtr GetAncestor(IntPtr hWnd, uint flags);

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Tab)
		{
			keyData = Keys.Shift;
		}
		if (keyData == Keys.Shift || keyData == (Keys.Tab | Keys.Shift))
		{
			IntPtr ancestor = GetAncestor(msg.HWnd, 1u);
			IInputElement focusedElement = Keyboard.FocusedElement;
			FrameworkElement obj = (FrameworkElement)base.Child;
			obj.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
			IInputElement focusedElement2 = Keyboard.FocusedElement;
			obj.MoveFocus(new TraversalRequest(FocusNavigationDirection.Last));
			IInputElement focusedElement3 = Keyboard.FocusedElement;
			bool flag = keyData == (Keys.Tab | Keys.Shift);
			Keyboard.Focus(focusedElement);
			bool result = base.ProcessCmdKey(ref msg, keyData);
			IInputElement focusedElement4 = Keyboard.FocusedElement;
			if ((flag && focusedElement3 == null) || focusedElement4 == (flag ? focusedElement3 : focusedElement2))
			{
				if (flag)
				{
					return PostMessage(ancestor, 256u, new IntPtr(65545), IntPtr.Zero);
				}
				Keyboard.Focus(focusedElement3);
				return PostMessage(ancestor, 256u, new IntPtr(65545), IntPtr.Zero);
			}
			return result;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}
}
