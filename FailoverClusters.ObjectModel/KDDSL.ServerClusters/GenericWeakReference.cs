using System;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

internal class GenericWeakReference<T>
{
	private WeakReference m_reference;

	internal T Target => (T)m_reference.Target;

	internal bool IsNull
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_reference.Target == null;
		}
	}

	internal GenericWeakReference(T target)
	{
		m_reference = new WeakReference(target);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool Equals(object obj)
	{
		//Discarded unreachable code: IL_001d
		GenericWeakReference<T> obj2 = obj as GenericWeakReference<T>;
		if (obj != null)
		{
			return Equals(obj2);
		}
		throw new ArgumentNullException("obj");
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public virtual bool Equals(GenericWeakReference<T> obj)
	{
		T target = Target;
		T target2 = obj.Target;
		return target?.Equals(target2) ?? (target2 == null);
	}
}
