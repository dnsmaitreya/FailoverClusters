namespace KDDSL.ServerClusters;

internal interface ICommonControlCodes
{
	uint UnknownCode { get; }

	uint ValidatePrivatePropertiesCode { get; }

	uint SetPrivatePropertiesCode { get; }

	uint GetPrivatePropertiesFormatCode { get; }

	uint GetReadOnlyPrivatePropertiesCode { get; }

	uint GetPrivatePropertiesCode { get; }

	uint EnumPrivatePropertiesCode { get; }

	uint ValidateCommonPropertiesCode { get; }

	uint SetCommonPropertiesCode { get; }

	uint GetCommonPropertiesFormatCode { get; }

	uint GetReadOnlyCommonPropertiesCode { get; }

	uint GetCommonPropertiesCode { get; }

	uint EnumCommonPropertiesCode { get; }
}
