namespace FailoverClusters.Framework;

public enum VirtualDiskAccess : ushort
{
	Unknown,
	Readable,
	Writeable,
	ReadWrite,
	WriteOnce
}

