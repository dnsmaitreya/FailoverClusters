using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.Management.Infrastructure;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public static class ExtensionMethods
{
	public static void DisposeSafe(this IDisposable component)
	{
		component?.Dispose();
	}

	public static T ValueOrDefault<T>(this T? value, T defaultValue = default(T)) where T : struct
	{
		if (value.HasValue)
		{
			return value.Value;
		}
		return defaultValue;
	}

	public static string TrimOrDefault(this string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return value.Trim();
		}
		return string.Empty;
	}

	public static T ValueOrDefault<T, T1>(this T1? value, T defaultValue = default(T)) where T1 : struct
	{
		if (value.HasValue)
		{
			return (T)Enum.Parse(typeof(T), value.Value.ToString());
		}
		return defaultValue;
	}

	public static T[] ToArrayOf<T>(this ushort[] array) where T : new()
	{
		return array?.Cast<object>().Cast<T>().ToArray();
	}

	public static T[] ToArrayOf<T>(this IList<ushort> array) where T : new()
	{
		return array?.Cast<object>().Cast<T>().ToArray();
	}

	public static T[] ToArrayOf<T>(this uint[] array) where T : new()
	{
		return array?.Cast<object>().Cast<T>().ToArray();
	}

	public static string GetRelativePath(this CimInstance cimInstance)
	{
		if (cimInstance == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(cimInstance.CimSystemProperties.ClassName);
		stringBuilder.Append(" (");
		stringBuilder.Append(string.Join(", ", cimInstance.CimInstanceProperties.Where((CimProperty property) => property.Value != null && (property.Flags & CimFlags.Key) == CimFlags.Key)));
		stringBuilder.Append(")");
		return stringBuilder.ToString();
	}

	private static TResult GetPropertyValue<T, TResult>(ClusterPropertyCollection propertyCollection, string propertyName, Func<T, TResult> property, bool callbackIfNull) where T : ClusterProperty
	{
		T val = (T)propertyCollection[propertyName];
		if (callbackIfNull || val != null)
		{
			return property.SafeCall(val);
		}
		return default(TResult);
	}

	internal static void Get<T>(this ClusterPropertyCollection propertyCollection, string propertyName, Action<T> property) where T : ClusterProperty
	{
		GetPropertyValue(propertyCollection, propertyName, (Func<T, object>)delegate(T propertyValue)
		{
			property(propertyValue);
			return null;
		}, callbackIfNull: false);
	}

	internal static TResult Get<T, TResult>(this ClusterPropertyCollection propertyCollection, string propertyName, Func<T, TResult> property) where T : ClusterProperty
	{
		return GetPropertyValue(propertyCollection, propertyName, property, callbackIfNull: false);
	}

	internal static void GetOrNull<T>(this ClusterPropertyCollection propertyCollection, string propertyName, Action<T> property) where T : ClusterProperty
	{
		GetPropertyValue(propertyCollection, propertyName, (Func<T, object>)delegate(T propertyValue)
		{
			property(propertyValue);
			return null;
		}, callbackIfNull: true);
	}

	internal static TResult GetOrNull<T, TResult>(this ClusterPropertyCollection propertyCollection, string propertyName, Func<T, TResult> property) where T : ClusterProperty
	{
		return GetPropertyValue(propertyCollection, propertyName, property, callbackIfNull: true);
	}

	internal static int AllLoadSelectionMask(this PClusterObject clusterObject)
	{
		if (!(clusterObject is PVirtualMachineResource))
		{
			if (!(clusterObject is PResource))
			{
				if (!(clusterObject is PGroup))
				{
					if (!(clusterObject is PNode))
					{
						if (!(clusterObject is PResourceType))
						{
							if (!(clusterObject is PCluster))
							{
								return 0;
							}
							return 4111;
						}
						return 15;
					}
					return 28679;
				}
				return 15;
			}
			return 4095;
		}
		return 524287;
	}

	internal static int AllLoadSelectionMask(this ClusterObject clusterObject)
	{
		if (!(clusterObject is VirtualMachineResource))
		{
			if (!(clusterObject is Resource))
			{
				if (!(clusterObject is Group))
				{
					if (!(clusterObject is Node))
					{
						if (!(clusterObject is ResourceType))
						{
							if (!(clusterObject is Cluster))
							{
								return 0;
							}
							return 4111;
						}
						return 15;
					}
					return 28679;
				}
				return 15;
			}
			return 4095;
		}
		return 524287;
	}

	public static StringCollection ToStringCollection(this IEnumerable<string> collection)
	{
		if (collection == null)
		{
			return null;
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.AddRange(collection.ToArray());
		return stringCollection;
	}

	public static void BreakAndStack()
	{
		BreakAndStack(null);
	}

	public static void BreakAndStack(Exception ex)
	{
		Utilities.UnreferencedParameter(new StackTrace(fNeedFileInfo: true));
		Utilities.UnreferencedParameter(ex);
	}

	public static bool SequenceEqual(this byte[] array, byte[] second)
	{
		if (array == null && second == null)
		{
			return true;
		}
		if ((array != null && second == null) || (array == null && second != null) || array.Length != second.Length)
		{
			return false;
		}
		return NativeMethods.MemoryCompare(array, second, new UIntPtr((uint)array.Length)) == 0;
	}

	public static void TryAdd<T>(this List<T> list, T item)
	{
		list?.Add(item);
	}

	public static void SafeCall(this Action action)
	{
		action?.Invoke();
	}

	public static T SafeCall<T>(this Func<T> action)
	{
		if (action != null)
		{
			return action();
		}
		return default(T);
	}

	public static TResult SafeCall<T, TResult>(this Func<T, TResult> action, T parameter)
	{
		if (action != null)
		{
			return action(parameter);
		}
		return default(TResult);
	}

	public static void SafeCall<T>(this Action<T> operationResultCallback, T operationResult)
	{
		operationResultCallback?.Invoke(operationResult);
	}

	public static void SafeCall<T1, T2>(this Action<T1, T2> action, T1 parameter1, T2 parameter2)
	{
		action?.Invoke(parameter1, parameter2);
	}

	public static void SafeCall(this EventHandler eventHandler, object sender, EventArgs args)
	{
		eventHandler?.Invoke(sender, args);
	}

	public static void SafeCall<T>(this EventHandler<T> eventHandler, object sender, T args) where T : EventArgs
	{
		eventHandler?.Invoke(sender, args);
	}

	public static ManagementObject GetFirst(this ManagementObjectCollection managementObjectCollection)
	{
		if (managementObjectCollection.Count == 0)
		{
			throw new ArgumentException("The collection contains no objects", "managementObjectCollection");
		}
		return managementObjectCollection.Cast<ManagementObject>().FirstOrDefault();
	}

	public static PropertyData GetPropertyOrDefault(this ManagementBaseObject managementObject, string propertyName)
	{
		if (managementObject == null)
		{
			return null;
		}
		foreach (PropertyData property in managementObject.Properties)
		{
			if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
			{
				return property;
			}
		}
		return null;
	}

	public static object GetPropertyValueOrDefault(this ManagementBaseObject managementObject, string propertyName)
	{
		if (managementObject == null)
		{
			return null;
		}
		foreach (PropertyData property in managementObject.Properties)
		{
			if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
			{
				return property.Value;
			}
		}
		return null;
	}

	public static ImageSource ToImageSource(this Icon icon)
	{
		MemoryStream memoryStream = new MemoryStream();
		icon.Save(memoryStream);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		return BitmapFrame.Create(memoryStream);
	}

	public static T[] ToArray<T>(this ReadOnlyCollection<T> list)
	{
		T[] array = new T[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			array[i] = list[i];
		}
		return array;
	}

	public static void ForEach<T>(this IQueryable<T> list, Action<T> action) where T : ClusterObject
	{
		foreach (T item in list)
		{
			action(item);
		}
	}

	public static void ForEach<T>(this ClusterList<T> list, Action<T> action) where T : ClusterObject
	{
		foreach (T item in list)
		{
			action(item);
		}
	}

	public static bool Exists<T>(this IList<T> list, Predicate<T> match)
	{
		return list.FindIndex(match) != -1;
	}

	public static int FindIndex<T>(this IList<T> list, Predicate<T> match)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (match(list[i]))
			{
				return i;
			}
		}
		return -1;
	}

	public static IList<TOutput> ConvertAll<TInput, TOutput>(this IList<TInput> list, Converter<TInput, TOutput> converter)
	{
		List<TOutput> list2 = new List<TOutput>(list.Count);
		foreach (TInput item in list)
		{
			list2.Add(converter(item));
		}
		return list2;
	}

	public static List<TOutput> ConvertAll<TOutput>(this IList list, Converter<object, TOutput> converter)
	{
		List<TOutput> list2 = new List<TOutput>(list.Count);
		foreach (object item in list)
		{
			list2.Add(converter(item));
		}
		return list2;
	}

	public static IEnumerable<TOutput> ConvertAll<TInput, TOutput>(this IEnumerable<TInput> enumerable, Converter<TInput, TOutput> converter)
	{
		foreach (TInput item in enumerable)
		{
			yield return converter(item);
		}
	}

	public static IEnumerable<TOutput> ConvertAll<TInput, TOutput>(this Array enumerable, Converter<TInput, TOutput> converter)
	{
		foreach (TInput item in enumerable)
		{
			yield return converter(item);
		}
	}

	public static IEnumerable<TOutput> FindAll<TOutput>(this Array enumerable, Predicate<TOutput> match)
	{
		if (match == null)
		{
			throw new ArgumentException("match");
		}
		for (int i = 0; i < enumerable.Length; i++)
		{
			TOutput val = (TOutput)enumerable.GetValue(i);
			if (match(val))
			{
				yield return val;
			}
		}
	}

	public static IEnumerable<TOutput> FindAll<TOutput>(this TOutput[] enumerable, Predicate<TOutput> match)
	{
		if (match == null)
		{
			throw new ArgumentException("match");
		}
		foreach (TOutput val in enumerable)
		{
			if (match(val))
			{
				yield return val;
			}
		}
	}

	public static ObservableCollection<TOutput> ConvertAll<TOutput>(this CommandCollection collection, Converter<ICommand, TOutput> converter)
	{
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		ObservableCollection<TOutput> observableCollection = new ObservableCollection<TOutput>();
		for (int i = 0; i < collection.Count; i++)
		{
			observableCollection.Add(converter(collection[i]));
		}
		return observableCollection;
	}

	public static ObservableCollection<TOutput> ConvertAll<TOutput, TInput>(this ObservableCollection<TInput> collection, Converter<TInput, TOutput> converter)
	{
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		ObservableCollection<TOutput> observableCollection = new ObservableCollection<TOutput>();
		for (int i = 0; i < collection.Count; i++)
		{
			observableCollection.Add(converter(collection[i]));
		}
		return observableCollection;
	}

	public static Collection<T> AddAll<T>(this Collection<T> collection, IEnumerable<T> toAdd)
	{
		Exceptions.ThrowIfNull(collection, "collection");
		Exceptions.ThrowIfNull(toAdd, "toAdd");
		foreach (T item in toAdd)
		{
			collection.Add(item);
		}
		return collection;
	}

	public static string Append(this string source, string target)
	{
		if (source == null)
		{
			return target;
		}
		if (target == null)
		{
			return source;
		}
		return source + target;
	}

	public static string AppendLineAndAddExtra(this string source, string target)
	{
		if (source == null)
		{
			return target;
		}
		if (target == null)
		{
			return source;
		}
		return source + Environment.NewLine + Environment.NewLine + target;
	}

	public static string AppendLine(this string source, string target)
	{
		if (source == null && target == null)
		{
			return Environment.NewLine;
		}
		if (source == null)
		{
			return target + Environment.NewLine;
		}
		if (target == null)
		{
			return source + Environment.NewLine;
		}
		return source + Environment.NewLine + target;
	}
}
