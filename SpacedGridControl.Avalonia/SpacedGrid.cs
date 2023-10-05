using Avalonia;
using Avalonia.Controls;
using System.Collections.Specialized;

namespace SpacedGridControl.Avalonia;

public class SpacedGrid : Grid
{
	public static readonly StyledProperty<double> RowSpacingProperty = AvaloniaProperty.Register<SpacedGrid, double>(nameof(RowSpacing), 3);
	public static readonly StyledProperty<double> ColumnSpacingProperty = AvaloniaProperty.Register<SpacedGrid, double>(nameof(ColumnSpacing), 3);

	public double RowSpacing
	{
		get => GetValue(RowSpacingProperty);
		set => SetValue(RowSpacingProperty, value);
	}

	public double ColumnSpacing
	{
		get => GetValue(ColumnSpacingProperty);
		set => SetValue(ColumnSpacingProperty, value);
	}

	public SpacedGrid()
		=> Children.CollectionChanged += Children_CollectionChanged;

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);

		if (change.Property == RowSpacingProperty || change.Property == ColumnSpacingProperty)
			RecalculateMarginsOfChildren();
	}

	private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.NewItems is not null)
		{
			foreach (Control item in e.NewItems)
				item.Initialized += Item_Initialized; // Wait for item to initialize, otherwise GetRow() and GetColumn() will always return 0
		}
	}

	private void Item_Initialized(object sender, System.EventArgs e)
	{
		var item = sender as Control;
		item.Initialized -= Item_Initialized;

		RecalculateMarginsOfChildren();
	}

	private void RecalculateMarginsOfChildren()
	{
		for (int i = 0; i < Children.Count; i++)
		{
			Control child = Children[i];

			if (child is not SpacedGridItem item)
			{
				item = new SpacedGridItem();

				int childIndex = Children.IndexOf(child),
					row = GetRow(child),
					column = GetColumn(child),
					rowSpan = GetRowSpan(child),
					columnSpan = GetColumnSpan(child);

				Children.Remove(child);

				item.Child = child;
				SetRow(item, row);
				SetColumn(item, column);
				SetRowSpan(item, rowSpan);
				SetColumnSpan(item, columnSpan);

				Children.Insert(childIndex, item);
			}

			RecalculateMargin(item);
		}
	}

	private void RecalculateMargin(SpacedGridItem item)
	{
		int row = GetRow(item),
			column = GetColumn(item),
			rowSpan = GetRowSpan(item),
			columnSpan = GetColumnSpan(item),
			rowCount = RowDefinitions.Count,
			columnCount = ColumnDefinitions.Count;

		double halfRowSpacing = RowSpacing / 2,
			halfColumnSpacing = ColumnSpacing / 2;

		double left, top, right, bottom;

		left = column == 0 ? 0 : halfColumnSpacing;
		top = row == 0 ? 0 : halfRowSpacing;
		right = column + columnSpan == columnCount ? 0 : halfColumnSpacing;
		bottom = row + rowSpan == rowCount ? 0 : halfRowSpacing;

		item.Margin = new Thickness(left, top, right, bottom);
	}
}
