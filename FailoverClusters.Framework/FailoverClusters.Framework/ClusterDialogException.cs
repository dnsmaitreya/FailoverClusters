using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterDialogException : ClusterException, IErrorDialog
{
	private readonly ClusterException innerException;

	private string header;

	private string content;

	private string footer;

	private string details;

	private string diagnose;

	private string caption = CommonResources.Text_Error;

	private TaskDialogStandardButtons buttons = TaskDialogStandardButtons.Ok;

	private TaskDialogStandardIcon icon = TaskDialogStandardIcon.Error;

	public TaskDialogStandardButtons Buttons
	{
		get
		{
			return buttons;
		}
		internal set
		{
			buttons = value;
		}
	}

	public string Caption
	{
		get
		{
			return caption;
		}
		internal set
		{
			caption = value;
		}
	}

	public string Header
	{
		get
		{
			return header;
		}
		internal set
		{
			header = value;
		}
	}

	public string Content
	{
		get
		{
			return content;
		}
		internal set
		{
			content = value;
		}
	}

	public string Footer
	{
		get
		{
			return footer;
		}
		internal set
		{
			footer = value;
		}
	}

	public string Details
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (Exception ex = base.InnerException; ex != null; ex = ex.InnerException)
			{
				if (!(ex is ClusterControlCodeException))
				{
					if (ex is Win32Exception ex2 && ex2.NativeErrorCode != 0)
					{
						stringBuilder.AppendLine("{0} 0x{1:x}".FormatInvariantCulture(CommonResources.ErrorDialog_ErrorCode, NativeMethods.HRESULT_FROM_WIN32(ex2.NativeErrorCode)));
					}
					stringBuilder.AppendLine(ex.Message);
				}
			}
			if (stringBuilder.Length > 0)
			{
				details = stringBuilder.ToString();
			}
			return details;
		}
		internal set
		{
			details = value;
		}
	}

	public TaskDialogStandardIcon Icon
	{
		get
		{
			return icon;
		}
		internal set
		{
			icon = value;
		}
	}

	public string Diagnose
	{
		get
		{
			return diagnose;
		}
		internal set
		{
			diagnose = value;
		}
	}

	public ClusterDialogException()
	{
	}

	public ClusterDialogException(ClusterException clusterException)
		: base(clusterException?.Message, clusterException?.InnerException)
	{
		if (clusterException == null)
		{
			throw new ArgumentNullException("clusterException");
		}
		innerException = clusterException;
		header = ExceptionResources.Unknown_Header;
		content = clusterException.Message;
		diagnose = clusterException.StackTrace;
	}

	public ClusterDialogException(string message)
		: this(message, null)
	{
	}

	public ClusterDialogException(string message, Exception innerException)
		: base(message, innerException)
	{
		content = message;
	}

	public ClusterDialogException(string message, Exception innerException, string caption, TaskDialogStandardIcon icon, string header)
		: this(message, innerException)
	{
		Caption = caption;
		Icon = icon;
		Header = header;
	}

	protected ClusterDialogException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public static TaskDialogResult ShowTaskDialog(Exception ex)
	{
		return ShowTaskDialog(ex, IntPtr.Zero);
	}

	public static TaskDialogResult ShowTaskDialog(Exception ex, IntPtr owner)
	{
		if (ex == null)
		{
			return TaskDialogResult.Ok;
		}
		using TaskDialog taskDialog = CreateDialog(ex);
		return ShowTaskDialog(taskDialog, owner);
	}

	public static TaskDialogResult ShowTaskDialog(ClusterDialogException ex, string caption, string detailsText, TaskDialogStandardButtons buttons)
	{
		if (ex == null)
		{
			return TaskDialogResult.Ok;
		}
		ex.Buttons = buttons;
		using TaskDialog taskDialog = CreateDialog(ex);
		taskDialog.DetailsExpandedText = (string.IsNullOrEmpty(detailsText) ? string.Empty : detailsText);
		taskDialog.FooterIcon = TaskDialogStandardIcon.None;
		if (!string.IsNullOrEmpty(caption))
		{
			taskDialog.Caption = caption;
		}
		return ShowTaskDialog(taskDialog, IntPtr.Zero);
	}

	private static TaskDialogResult ShowTaskDialog(TaskDialog taskDialog, IntPtr owner)
	{
		try
		{
			if (owner != IntPtr.Zero)
			{
				return taskDialog.Show(owner);
			}
			return taskDialog.Show(Global.DefaultWindowHandle);
		}
		catch
		{
			try
			{
				return taskDialog.Show();
			}
			catch
			{
				return TaskDialogResult.Ok;
			}
		}
	}

	public static void ShowTaskDialogAsync(Exception ex)
	{
		ShowTaskDialogAsync(ex, IntPtr.Zero, null);
	}

	public static void ShowTaskDialogAsync(Exception ex, Action<ActionResult<TaskDialogResult>> actionResult)
	{
		ShowTaskDialogAsync(ex, IntPtr.Zero, actionResult);
	}

	public static void ShowTaskDialogAsync(Exception ex, IntPtr owner)
	{
		ShowTaskDialogAsync(ex, owner, null);
	}

	public static void ShowTaskDialogAsync(Exception ex, IntPtr owner, Action<ActionResult<TaskDialogResult>> actionResult)
	{
		Worker.Start(delegate
		{
			TaskDialogResult resultObject = TaskDialogResult.Ok;
			try
			{
				if (ex == null)
				{
					return;
				}
				using TaskDialog taskDialog = CreateDialog(ex);
				resultObject = ((!(owner != IntPtr.Zero)) ? taskDialog.Show(Global.DefaultWindowHandle) : taskDialog.Show(owner));
			}
			finally
			{
				if (actionResult != null)
				{
					actionResult(new ActionResult<TaskDialogResult>(resultObject, null));
				}
			}
		}, delegate(ClusterException exception)
		{
			ClusterLog.LogException(exception, "Error displaying the non-blocking task dialog");
			if (actionResult != null)
			{
				actionResult(new ActionResult<TaskDialogResult>(TaskDialogResult.Cancel, exception));
			}
		});
	}

	public static TaskDialog CreateDialog(Exception ex)
	{
		if (ex == null)
		{
			throw new ArgumentNullException("ex");
		}
		TaskDialog taskDialog = null;
		if (ex is ClusterDialogException)
		{
			return CreateDialog((ClusterDialogException)ex);
		}
		if (ex is ClusterException)
		{
			return CreateDialog(new ClusterDialogException((ClusterException)ex));
		}
		return CreateDialog(new ClusterDefaultException(ex));
	}

	private static TaskDialog CreateDialog(ClusterDialogException errorDialog)
	{
		TaskDialog taskDialog = new TaskDialog();
		try
		{
			taskDialog.ContentWidthArea = 250u;
			taskDialog.InstructionText = errorDialog.Header;
			taskDialog.Caption = errorDialog.Caption;
			taskDialog.Icon = errorDialog.Icon;
			taskDialog.Text = errorDialog.Content;
			taskDialog.Cancelable = true;
			if ((errorDialog.Buttons & TaskDialogStandardButtons.Ok) == TaskDialogStandardButtons.Ok)
			{
				TaskDialogButton taskDialogButton = new TaskDialogButton(TaskDialogStandardButtons.Ok);
				taskDialogButton.Default = true;
				taskDialog.Controls.Add(taskDialogButton);
			}
			if ((errorDialog.Buttons & TaskDialogStandardButtons.Cancel) == TaskDialogStandardButtons.Cancel)
			{
				TaskDialogButton item = new TaskDialogButton(TaskDialogStandardButtons.Cancel);
				taskDialog.Controls.Add(item);
			}
			if ((errorDialog.Buttons & TaskDialogStandardButtons.Yes) == TaskDialogStandardButtons.Yes)
			{
				TaskDialogButton taskDialogButton2 = new TaskDialogButton(TaskDialogStandardButtons.Yes);
				taskDialogButton2.Default = true;
				taskDialog.Controls.Add(taskDialogButton2);
			}
			if ((errorDialog.Buttons & TaskDialogStandardButtons.No) == TaskDialogStandardButtons.No)
			{
				TaskDialogButton item2 = new TaskDialogButton(TaskDialogStandardButtons.No);
				taskDialog.Controls.Add(item2);
			}
			if ((errorDialog.Buttons & TaskDialogStandardButtons.Retry) == TaskDialogStandardButtons.Retry)
			{
				TaskDialogButton item3 = new TaskDialogButton(TaskDialogStandardButtons.Retry);
				taskDialog.Controls.Add(item3);
			}
			if ((errorDialog.Buttons & TaskDialogStandardButtons.Close) == TaskDialogStandardButtons.Close)
			{
				TaskDialogButton item4 = new TaskDialogButton(TaskDialogStandardButtons.Close);
				taskDialog.Controls.Add(item4);
			}
			taskDialog.DetailsExpandedText = errorDialog.Details;
			taskDialog.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;
			taskDialog.DetailsCollapsedLabel = CommonResources.ErrorDialog_ShowDetails;
			taskDialog.DetailsExpandedLabel = CommonResources.ErrorDialog_HideDetails;
			return taskDialog;
		}
		catch (Exception)
		{
			taskDialog.Dispose();
			throw;
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

