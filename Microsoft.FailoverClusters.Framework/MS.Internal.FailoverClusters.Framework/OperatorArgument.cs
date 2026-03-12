using System;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class OperatorArgument : IClusterQueryArgument
{
	private readonly OperatorType operatorType;

	public string Name => null;

	public OperatorType OperatorType => operatorType;

	public OperatorArgument(OperatorType operatorType)
	{
		this.operatorType = operatorType;
	}

	public override string ToString()
	{
		switch (this.operatorType)
		{
		case OperatorType.Contains:
			return "CONTAINS";
		case OperatorType.StartsWith:
			return "STARTSWITH";
		case OperatorType.EndsWith:
			return "ENDWITH";
		case OperatorType.And:
			return "AND";
		case OperatorType.Equal:
			return "=";
		case OperatorType.GreaterThan:
			return ">";
		case OperatorType.GreaterThanOrEqual:
			return ">=";
		case OperatorType.Is:
			return "IS";
		case OperatorType.IsNot:
			return "IS NOT";
		case OperatorType.LessThan:
			return "<";
		case OperatorType.LessThanOrEqual:
			return "<=";
		case OperatorType.NotEqual:
			return "!=";
		case OperatorType.Or:
			return "OR";
		default:
		{
			string uknownOperatorType = ExceptionResources.UknownOperatorType;
			OperatorType operatorType = this.operatorType;
			throw new ArgumentOutOfRangeException("operatorType", uknownOperatorType.FormatCurrentCulture(operatorType.ToString()));
		}
		}
	}
}
