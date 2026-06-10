using System.Globalization;

namespace KDDSL.FailoverClusters.Framework;

internal class Counter
{
	private int value;

	public int Value => value;

	public Counter()
	{
	}

	public Counter(int initValue)
	{
		value = initValue;
	}

	public void Increment()
	{
		value++;
	}

	public bool Decrement()
	{
		return --value == 0;
	}

	public override string ToString()
	{
		return value.ToString(CultureInfo.CurrentCulture);
	}
}
