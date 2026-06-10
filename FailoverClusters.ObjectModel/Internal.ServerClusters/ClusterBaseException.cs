using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace MS.Internal.ServerClusters;

[Serializable]
public abstract class ClusterBaseException : Exception, ITaskDialogResource
{
	private string diagnose;

	public virtual TaskDialogIcon Icon
	{
		get
		{
			return TaskDialogIcon.Error;
		}
		set
		{
		}
	}

	public virtual string Diagnose
	{
		get
		{
			if (DebugLog.ExtraExceptionData)
			{
				if (diagnose != null)
				{
					return diagnose;
				}
				return ToString();
			}
			return null;
		}
		set
		{
			diagnose = value;
		}
	}

	public virtual string Details
	{
		get
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(this);
			if (firstException != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} 0x{1:x}\n{2}", Resources.TaskDialog_ErrorCode_Text, firstException.NativeErrorCode, firstException.Message);
			}
			return null;
		}
		set
		{
		}
	}

	public virtual string Footer
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public virtual string Content
	{
		get
		{
			return Message;
		}
		set
		{
		}
	}

	public virtual string Header
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	public virtual string Caption
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	public virtual int NumericError => 0;

	protected ClusterBaseException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	protected ClusterBaseException(string message, Exception inner)
		: base(message, inner)
	{
	}

	protected ClusterBaseException(string message)
		: base(message)
	{
	}

	protected ClusterBaseException()
	{
	}
}
