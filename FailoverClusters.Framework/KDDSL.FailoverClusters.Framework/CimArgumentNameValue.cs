namespace KDDSL.FailoverClusters.Framework;

internal class CimArgumentNameValue
{
	public string Name { get; set; }

	public object Value { get; set; }

	public CimArgumentNameValue(string name, object value)
	{
		Name = name;
		Value = value;
	}
}
