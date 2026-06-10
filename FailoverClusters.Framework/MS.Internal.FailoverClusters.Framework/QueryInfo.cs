using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class QueryInfo
{
	private readonly List<ClusterObjectMetaDataMember> projectionFields = new List<ClusterObjectMetaDataMember>();

	private readonly List<ClusterObjectMetaDataMember> whereFields = new List<ClusterObjectMetaDataMember>();

	private readonly List<IClusterQueryArgument> whereSyntaxis = new List<IClusterQueryArgument>();

	private readonly List<OrderByItem> orderByFields = new List<OrderByItem>();

	private readonly List<OrderByItem> customOrderByFields = new List<OrderByItem>();

	private LambdaExpression whereLambda;

	private bool isCancel;

	private Type source;

	public bool IsCancel => isCancel;

	internal List<ClusterObjectMetaDataMember> ProjectionFields => projectionFields;

	internal List<ClusterObjectMetaDataMember> WhereFields => whereFields;

	public LambdaExpression WhereLambdaExpression
	{
		get
		{
			return whereLambda;
		}
		set
		{
			whereLambda = value;
			WhereLambdaExpressionFx = whereLambda.Compile();
		}
	}

	public Delegate WhereLambdaExpressionFx { get; set; }

	internal List<IClusterQueryArgument> WhereSyntaxis => whereSyntaxis;

	internal List<OrderByItem> OrderBy => orderByFields;

	internal List<OrderByItem> CustomOrderBy => customOrderByFields;

	public string QueryText { get; internal set; }

	public Type Source
	{
		get
		{
			return source;
		}
		set
		{
			Exceptions.ThrowIfNull(value, "value");
			source = value;
			if (typeof(Cluster).IsAssignableFrom(source))
			{
				IdentityType = ClusterIdentityType.Cluster;
				return;
			}
			if (typeof(Group).IsAssignableFrom(source))
			{
				IdentityType = ClusterIdentityType.Group;
				return;
			}
			if (typeof(Resource).IsAssignableFrom(source))
			{
				IdentityType = ClusterIdentityType.Resource;
				return;
			}
			if (typeof(Node).IsAssignableFrom(source))
			{
				IdentityType = ClusterIdentityType.Node;
				return;
			}
			if (typeof(Network).IsAssignableFrom(source))
			{
				IdentityType = ClusterIdentityType.Network;
				return;
			}
			if (typeof(NetworkInterface).IsAssignableFrom(source))
			{
				IdentityType = ClusterIdentityType.NetworkInterface;
				return;
			}
			if (typeof(ResourceType).IsAssignableFrom(source))
			{
				IdentityType = ClusterIdentityType.ResourceType;
				return;
			}
			throw new NotSupportedException("The type '{0}' is not valid source for a ClusterList".FormatCurrentCulture(value));
		}
	}

	public ClusterIdentityType IdentityType { get; private set; }

	public LambdaExpression LambdaExpression { get; set; }

	public QueryResultType ResultShape { get; internal set; }

	public bool UseDefault { get; set; }

	public QueryInfo()
	{
	}

	public QueryInfo(QueryResultType shape, string queryText)
	{
		ResultShape = shape;
		QueryText = queryText;
	}

	public void Cancel()
	{
		isCancel = true;
	}
}

