using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FailoverClusters.UI.Common;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterPropertyCollection : IEnumerable<ClusterProperty>, IEnumerable
{
	private readonly ConcurrentDictionary<string, ClusterProperty> dictionary = new ConcurrentDictionary<string, ClusterProperty>(StringComparer.CurrentCultureIgnoreCase);

	private readonly ClusterIdentityType identityType;

	private readonly Guid clusterId = Guid.Empty;

	private readonly Guid clusterObjectId = Guid.Empty;

	private bool compacting;

	public bool CommonPropertiesLoaded { get; internal set; }

	public bool PrivatePropertiesLoaded { get; internal set; }

	public bool Partial { get; set; }

	public int Count => dictionary.Count;

	public ClusterProperty this[string name]
	{
		get
		{
			dictionary.TryGetValue(name, out var value);
			return value;
		}
	}

	public ClusterPropertyCollection(Guid clusterId, Guid clusterObjectId, ClusterIdentityType identityType)
	{
		this.clusterId = clusterId;
		this.clusterObjectId = clusterObjectId;
		this.identityType = identityType;
	}

	public void AddOrUpdate(ClusterPropertyCollection sourceCollection)
	{
		Exceptions.ThrowIfNull(sourceCollection, "sourceCollection");
		if (!sourceCollection.Partial)
		{
			if (sourceCollection.CommonPropertiesLoaded)
			{
				CommonPropertiesLoaded = true;
			}
			if (sourceCollection.PrivatePropertiesLoaded)
			{
				PrivatePropertiesLoaded = true;
			}
		}
		sourceCollection.ForEach(delegate(ClusterProperty item)
		{
			AddOperation((ClusterProperty)item.Clone());
		});
	}

	public bool Contains(string name)
	{
		return dictionary.ContainsKey(name);
	}

	internal void Compact()
	{
		compacting = true;
		try
		{
			ClusterProperty currentProperty;
			dictionary.Where((KeyValuePair<string, ClusterProperty> item) => item.Value.IsDeleted).ToList().ForEach(delegate(KeyValuePair<string, ClusterProperty> item)
			{
				dictionary.TryRemove(item.Key, out currentProperty);
			});
		}
		finally
		{
			compacting = false;
		}
	}

	public void Rollback()
	{
		dictionary.ForEach(delegate(KeyValuePair<string, ClusterProperty> item)
		{
			item.Value.Rollback();
		});
	}

	public void Commit()
	{
		Commit(delegate(OperationResult commitResult)
		{
			commitResult.Sender.Error = commitResult.Error;
		});
	}

	public void Commit(Action<OperationResult> operationResult)
	{
		Commit(operationResult, null);
	}

	internal void Commit(Action<OperationResult> operationResult, PClusterObject privateClusterObject)
	{
		if (clusterId == Guid.Empty || clusterObjectId == Guid.Empty)
		{
			operationResult.SafeCall(new OperationResult(null, new ClusterDefaultException(new InvalidOperationException(ExceptionResources.PropertyCollectionObjectIdNotSet))));
			return;
		}
		Worker.Start(delegate
		{
			OperationResult obj = null;
			try
			{
				if (privateClusterObject == null)
				{
					using (ClusterLock clusterLock = CacheManager.ClusterLockById(clusterId, LockAccess.Reader))
					{
						if (clusterLock == null)
						{
							obj = new OperationResult(null, new ClusterObjectNotFoundException(null, clusterId, typeof(Cluster)));
						}
						else
						{
							PCluster pCluster = (PCluster)clusterLock.Owner;
							if (identityType == ClusterIdentityType.Cluster)
							{
								obj = SaveProperties(pCluster);
							}
							else
							{
								ClusterLock clusterLock2 = pCluster.CacheManager.Get(clusterObjectId, identityType, LockAccess.Writer);
								if (clusterLock2 != null)
								{
									try
									{
										obj = SaveProperties(clusterLock2.Owner);
										return;
									}
									finally
									{
										clusterLock2.UnlockWriter();
									}
								}
								obj = new OperationResult(null, new ClusterObjectNotFoundException(null, clusterObjectId, identityType));
							}
						}
						return;
					}
				}
				obj = SaveProperties(privateClusterObject);
			}
			finally
			{
				if (operationResult != null)
				{
					operationResult(obj);
				}
			}
		});
	}

	private OperationResult SaveProperties(PClusterObject privateClusterObject)
	{
		OperationResult operationResult = null;
		try
		{
			privateClusterObject.SaveProperties(this);
			Rollback();
			return new OperationResult(null, null);
		}
		catch (ClusterException error)
		{
			return new OperationResult(null, error);
		}
	}

	public void Add(ClusterProperty item)
	{
		Exceptions.ThrowIfNull(item, "item");
		AddOperation(item);
	}

	public void Remove(ClusterProperty item)
	{
		Exceptions.ThrowIfNull(item, "item");
		RemoveOperation(item.Name);
	}

	public void Remove(ClusterPropertyKind propertyKind)
	{
		if ((propertyKind == ClusterPropertyKind.Common && !CommonPropertiesLoaded) || (propertyKind == ClusterPropertyKind.Private && !PrivatePropertiesLoaded))
		{
			return;
		}
		foreach (ClusterProperty item in dictionary.Values.Where((ClusterProperty property) => property.PropertyKind == propertyKind).ToList())
		{
			item.IsDeleted = true;
		}
		if (propertyKind == ClusterPropertyKind.Common)
		{
			CommonPropertiesLoaded = false;
		}
		if (propertyKind == ClusterPropertyKind.Private)
		{
			PrivatePropertiesLoaded = false;
		}
	}

	public void Clear()
	{
		ClearOperation();
	}

	protected virtual void AddOperation(ClusterProperty item)
	{
		while (!dictionary.TryAdd(item.Name, item))
		{
			ClusterProperty value = dictionary[item.Name];
			if (value.PropertyKind == item.PropertyKind)
			{
				dictionary[item.Name] = item;
				break;
			}
			if (item.PropertyKind == ClusterPropertyKind.Private)
			{
				item.Name += "_";
				continue;
			}
			string text = value.Name + "_";
			while (dictionary.ContainsKey(text))
			{
				text += "_";
			}
			dictionary.TryRemove(value.Name, out value);
			value.Name = text;
			dictionary.TryAdd(value.Name, value);
		}
	}

	protected virtual bool RemoveOperation(string name)
	{
		if (!compacting)
		{
			if (dictionary.TryGetValue(name, out var value))
			{
				value.IsDeleted = true;
				return true;
			}
			return false;
		}
		ClusterProperty value2;
		return dictionary.TryRemove(name, out value2);
	}

	protected virtual void ClearOperation()
	{
		dictionary.Clear();
	}

	public IEnumerator<ClusterProperty> GetEnumerator()
	{
		return dictionary.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

