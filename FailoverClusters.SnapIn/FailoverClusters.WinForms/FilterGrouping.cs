using System;
using System.Collections.Generic;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.WinForms;

internal class FilterGrouping
{
	private class FilterGroupNode
	{
		private int _row1;

		private int _row2;

		public int Row1
		{
			get
			{
				return _row1;
			}
			set
			{
				_row1 = value;
			}
		}

		public int Row2
		{
			get
			{
				return _row2;
			}
			set
			{
				_row2 = value;
			}
		}

		public FilterGroupNode(int row1, int row2)
		{
			_row1 = row1;
			_row2 = row2;
		}
	}

	private readonly List<FilterGroupNode> groups;

	internal FilterGrouping()
	{
		groups = new List<FilterGroupNode>();
	}

	internal Connection GetConnection(int row)
	{
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].Row1 == row)
			{
				return Connection.Down;
			}
			if (groups[i].Row2 == row)
			{
				return Connection.Up;
			}
			if (groups[i].Row1 < row && groups[i].Row2 > row)
			{
				return Connection.Across;
			}
		}
		return Connection.None;
	}

	internal void AddGrouping(int row1, int row2)
	{
		if (row1 >= row2 || row1 < 0)
		{
			throw new ArgumentOutOfRangeException("row1", row1, Resources.FilterGroupingRowsOutOfRange);
		}
		for (int i = 0; i < groups.Count; i++)
		{
			if ((groups[i].Row1 <= row1 && row1 <= groups[i].Row2) || (groups[i].Row1 <= row2 && row2 <= groups[i].Row2))
			{
				throw new ArgumentException(Resources.FilterGroupingCannotGroup);
			}
		}
		groups.Add(new FilterGroupNode(row1, row2));
	}

	internal void AddRow(int row)
	{
		if (row < 0)
		{
			throw new ArgumentOutOfRangeException("row", row, Resources.FilterGroupingRowOutOfRange);
		}
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].Row1 >= row)
			{
				groups[i].Row1 = groups[i].Row1 + 1;
			}
			if (groups[i].Row2 >= row)
			{
				groups[i].Row2 = groups[i].Row2 + 1;
			}
		}
	}

	internal void RemoveRow(int row)
	{
		if (row < 0)
		{
			throw new ArgumentOutOfRangeException("row", row, Resources.FilterGroupingRowOutOfRange);
		}
		int num = 0;
		while (num < groups.Count)
		{
			FilterGroupNode filterGroupNode = groups[num];
			if (filterGroupNode.Row1 == row)
			{
				if (filterGroupNode.Row2 == row + 1)
				{
					groups.RemoveAt(num);
				}
				else
				{
					filterGroupNode.Row2--;
				}
			}
			else if (filterGroupNode.Row2 == row)
			{
				if (filterGroupNode.Row1 == row - 1)
				{
					groups.RemoveAt(num);
				}
				else
				{
					filterGroupNode.Row2 = row - 1;
				}
			}
			else
			{
				if (filterGroupNode.Row1 > row)
				{
					filterGroupNode.Row1--;
				}
				if (filterGroupNode.Row2 > row)
				{
					filterGroupNode.Row2--;
				}
			}
			if (num < groups.Count && filterGroupNode == groups[num])
			{
				num++;
			}
		}
		int num2 = 0;
		while (num2 < groups.Count)
		{
			if (IsDuplicate(groups[num2]))
			{
				groups.RemoveAt(num2);
			}
			else
			{
				num2++;
			}
		}
	}

	internal bool RowIsInGroup(int row)
	{
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].Row1 <= row && row <= groups[i].Row2)
			{
				return true;
			}
		}
		return false;
	}

	internal bool AddRowToAdjacentGroup(int row)
	{
		if (row == 0)
		{
			return false;
		}
		int num = -1;
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].Row2 == row - 1)
			{
				num = i;
				break;
			}
		}
		int num2 = -1;
		for (int j = 0; j < groups.Count; j++)
		{
			if (groups[j].Row1 == row)
			{
				num2 = j;
				break;
			}
		}
		if (num2 == -1)
		{
			if (num == -1)
			{
				groups.Add(new FilterGroupNode(row - 1, row));
			}
			else
			{
				groups[num].Row2 = row;
			}
		}
		else if (num == -1)
		{
			groups[num2].Row1 = row - 1;
		}
		else
		{
			groups[num].Row2 = groups[num2].Row2;
			groups.RemoveAt(num2);
		}
		return true;
	}

	internal bool RemoveRowFromGroup(int row)
	{
		if (row < 0)
		{
			throw new ArgumentOutOfRangeException("row", row, Resources.FilterGroupingRowOutOfRange);
		}
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].Row1 == row)
			{
				if (groups[i].Row2 == row + 1)
				{
					groups.RemoveAt(i);
				}
				else
				{
					groups[i].Row1 = row + 1;
				}
				return true;
			}
			if (groups[i].Row2 == row)
			{
				if (groups[i].Row1 == row - 1)
				{
					groups.RemoveAt(i);
				}
				else
				{
					groups[i].Row2 = row - 1;
				}
				return true;
			}
			if (groups[i].Row1 < row && row < groups[i].Row2)
			{
				if (groups[i].Row1 < row - 1)
				{
					groups.Add(new FilterGroupNode(groups[i].Row1, row - 1));
				}
				groups[i].Row1 = row;
				return true;
			}
		}
		return false;
	}

	internal void Clear()
	{
		groups.Clear();
	}

	private bool IsDuplicate(FilterGroupNode node)
	{
		for (int i = 0; i < groups.Count; i++)
		{
			if (node != groups[i] && groups[i].Row1 == node.Row1 && groups[i].Row2 == node.Row2)
			{
				return true;
			}
		}
		return false;
	}
}

