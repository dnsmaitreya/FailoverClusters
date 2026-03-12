using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MS.Internal.ServerClusters;

[Serializable]
public class StoragePoolInvalidValidationStateException : ClusterBaseException
{
	private static string errorMessage = Resources.StoragePoolInvalidValidationStateException_Text;

	private static int errorCode = -2147019873;

	public override string Caption => Resources.StoragePoolInvalidValidationStateException_Caption_Text;

	public override string Header => Resources.StoragePoolInvalidValidationStateException_Header_Text;

	public override int NumericError => errorCode;

	public StoragePoolInvalidValidationStateException(Exception inner)
		: base(errorMessage, inner)
	{
		base.HResult = errorCode;
	}

	public StoragePoolInvalidValidationStateException()
		: base(errorMessage)
	{
		base.HResult = errorCode;
	}

	protected StoragePoolInvalidValidationStateException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
