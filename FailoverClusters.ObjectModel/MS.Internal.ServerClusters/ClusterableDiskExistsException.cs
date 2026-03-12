using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MS.Internal.ServerClusters;

[Serializable]
public class ClusterableDiskExistsException : ClusterBaseException
{
	private static string errorMessage = Resources.ClusterableDiskExistsException_Message_Text;

	private static int errorCode = -2147024713;

	public override string Caption => Resources.ClusterableDiskExistsException_Caption_Text;

	public override string Header => Resources.ClusterableDiskExistsException_Header_Text;

	public override int NumericError => errorCode;

	public ClusterableDiskExistsException(Exception inner)
		: base(errorMessage, inner)
	{
		base.HResult = errorCode;
	}

	public ClusterableDiskExistsException()
		: base(errorMessage)
	{
		base.HResult = errorCode;
	}

	protected ClusterableDiskExistsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
