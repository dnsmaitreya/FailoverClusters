using System;
using System.Runtime.Serialization;
using System.Text;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterDefaultException : ClusterDialogException
{
	public ClusterDefaultException()
		: this(string.Empty)
	{
	}

	public ClusterDefaultException(string message)
		: this(message, null)
	{
	}

	public ClusterDefaultException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.Header = ExceptionResources.Unknown_Header;
	}

	public ClusterDefaultException(string message, string header, Exception innerException)
		: base(message, innerException)
	{
		base.Header = header;
	}

	public ClusterDefaultException(Exception innerException)
		: base((innerException != null) ? innerException.Message : CommonResources.ClusterException_UnknownError, innerException)
	{
		base.Header = ExceptionResources.Unknown_Header;
	}

	protected ClusterDefaultException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override string ToString()
	{
		if (base.InnerException == null)
		{
			return Message;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Message);
		stringBuilder.Append("\t");
		stringBuilder.Append(base.InnerException.Message);
		return stringBuilder.ToString();
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

