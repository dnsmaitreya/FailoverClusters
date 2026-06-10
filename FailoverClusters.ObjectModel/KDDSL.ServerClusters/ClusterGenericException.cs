using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterGenericException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_Generic_Text;

	private static int errorCode = 0;

	private string caption;

	private string header;

	private string content;

	private string footer;

	private string details;

	private TaskDialogIcon icon;

	public override TaskDialogIcon Icon
	{
		get
		{
			return icon;
		}
		set
		{
			icon = value;
		}
	}

	public override string Details
	{
		get
		{
			string exceptionReportMessage = base.Details;
			if (exceptionReportMessage == null && InnerException != null)
			{
				string[] args = new string[0];
				exceptionReportMessage = ExceptionHelp.GetExceptionReportMessage(InnerException, args);
			}
			if (!string.Equals(exceptionReportMessage, content) && !string.IsNullOrEmpty(exceptionReportMessage))
			{
				return exceptionReportMessage;
			}
			return null;
		}
		set
		{
			details = value;
		}
	}

	public override string Footer
	{
		get
		{
			return footer;
		}
		set
		{
			footer = value;
		}
	}

	public override string Content
	{
		get
		{
			return content;
		}
		set
		{
			content = value;
		}
	}

	public override string Header
	{
		get
		{
			return header;
		}
		set
		{
			header = value;
		}
	}

	public override string Caption
	{
		get
		{
			return caption;
		}
		set
		{
			caption = value;
		}
	}

	public ClusterGenericException(string content, Exception inner)
		: base(content, inner)
	{
		caption = Resources.TaskDialog_WindowTitle_Generic_Text;
		header = Resources.Exception_Generic_Header_Text;
		this.content = content;
		icon = TaskDialogIcon.Error;
		base.HResult = errorCode;
	}

	public ClusterGenericException(string content)
		: base(content)
	{
		caption = Resources.TaskDialog_WindowTitle_Generic_Text;
		header = Resources.Exception_Generic_Header_Text;
		this.content = content;
		icon = TaskDialogIcon.Error;
		base.HResult = errorCode;
	}

	public ClusterGenericException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		caption = Resources.TaskDialog_WindowTitle_Generic_Text;
		header = Resources.Exception_Generic_Header_Text;
		content = string.Empty;
		icon = TaskDialogIcon.Error;
		base.HResult = errorCode;
	}

	protected ClusterGenericException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		caption = info.GetString("Caption");
		header = info.GetString("Header");
		content = info.GetString("Content");
		footer = info.GetString("Footer");
		icon = (TaskDialogIcon)info.GetInt32("Icon");
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("Caption", caption);
		info.AddValue("Header", header);
		info.AddValue("Content", content);
		info.AddValue("Footer", footer);
		info.AddValue("Icon", (int)icon);
	}
}
