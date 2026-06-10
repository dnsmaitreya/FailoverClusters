namespace KDDSL.FailoverClusters.Framework;

internal abstract class WmiTypeConverter<ClientType>
{
	public abstract ClientType ConvertFromWmiType(object value);

	public abstract object ConvertToWmiType(ClientType value);
}
