using System;

namespace Microsoft.FailoverClusters.Framework;

public class WeakLazy<T> : WeakLazyBase<T> where T : class
{
	private readonly Func<T> generateFunc;

	private readonly Func<WeakLazy<T>, T> generateFuncPlus;

	public WeakLazy(Func<T> generateFunc)
	{
		this.generateFunc = generateFunc;
	}

	public WeakLazy(Func<WeakLazy<T>, T> generateFunc)
	{
		generateFuncPlus = generateFunc;
	}

	protected override T GenerateInstance()
	{
		if (generateFunc == null)
		{
			return generateFuncPlus(this);
		}
		return generateFunc();
	}
}
