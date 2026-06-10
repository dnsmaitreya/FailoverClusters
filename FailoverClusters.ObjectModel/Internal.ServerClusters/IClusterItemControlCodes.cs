namespace MS.Internal.ServerClusters;

internal interface IClusterItemControlCodes
{
	uint GetNameCode { get; }

	uint GetIdCode { get; }

	uint GetFlagsCode { get; }

	uint GetCharacteristicsCode { get; }
}
