using Avalonia;
using Avalonia.Controls;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace SpacedGrid.Avalonia
{
	public class SpacedGrid : Grid
	{
		public static readonly StyledProperty<double> RowSpacingProperty = AvaloniaProperty.Register<SpacedGrid, double>(nameof(RowSpacing), 3);
		public static readonly StyledProperty<double> ColumnSpacingProperty = AvaloniaProperty.Register<SpacedGrid, double>(nameof(ColumnSpacing), 3);

		public double RowSpacing
		{
			get => GetValue(RowSpacingProperty);
			set
			{
				SetValue(RowSpacingProperty, value);
				RecalculateRowSpacing();
			}
		}

		public double ColumnSpacing
		{
			get => GetValue(ColumnSpacingProperty);
			set
			{
				SetValue(ColumnSpacingProperty, value);
				RecalculateColumnSpacing();
			}
		}

		public SpacedGrid()
		{
			RowDefinitions.CollectionChanged += delegate { UpdateSpacedRows(); };
			ColumnDefinitions.CollectionChanged += delegate { UpdateSpacedColumns(); };

			Children.CollectionChanged += Children_CollectionChanged;
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();

			UpdateSpacedRows();
			UpdateSpacedColumns();

			UpdateChildren(Children);
		}

		private void UpdateSpacedRows()
		{
			var oldRowDefinitions = new RowDefinitions();
			oldRowDefinitions.AddRange(RowDefinitions.Where(x => !(x is ISpacingDefinition)));

			var newRowDefinitions = new RowDefinitions();

			int currentUserDefinition = 0;
			int currentActualDefinition = 0;

			while (currentUserDefinition < oldRowDefinitions.Count)
			{
				if ((currentActualDefinition + 1) % 2 == 0 && RowSpacing > 0)
					newRowDefinitions.Add(new SpacingRowDefinition(RowSpacing));
				else
				{
					newRowDefinitions.Add(oldRowDefinitions[currentUserDefinition]);
					currentUserDefinition++;
				}

				currentActualDefinition++;
			}

			RowDefinitions = newRowDefinitions;
		}

		private void UpdateSpacedColumns()
		{
			var oldColumnDefinitions = new ColumnDefinitions();
			oldColumnDefinitions.AddRange(ColumnDefinitions.Where(x => !(x is ISpacingDefinition)));

			var newColumnDefinitions = new ColumnDefinitions();

			int currentUserDefinition = 0;
			int currentActualDefinition = 0;

			while (currentUserDefinition < oldColumnDefinitions.Count)
			{
				if ((currentActualDefinition + 1) % 2 == 0 && ColumnSpacing > 0)
					newColumnDefinitions.Add(new SpacingColumnDefinition(ColumnSpacing));
				else
				{
					newColumnDefinitions.Add(oldColumnDefinitions[currentUserDefinition]);
					currentUserDefinition++;
				}

				currentActualDefinition++;
			}

			ColumnDefinitions = newColumnDefinitions;
		}

		private void UpdateChildren(IList children)
		{
			if (RowSpacing > 0 || ColumnSpacing > 0)
				foreach (Control child in children)
				{
					if (RowSpacing > 0)
					{
						SetRow(child, GetRow(child) * 2);
						SetRowSpan(child, GetRowSpan(child) * 2 - 1);
					}

					if (ColumnSpacing > 0)
					{
						SetColumn(child, GetColumn(child) * 2);
						SetColumnSpan(child, GetColumnSpan(child) * 2 - 1);
					}
				}
		}

		private void RecalculateRowSpacing()
		{
			foreach (ISpacingDefinition spacingRow in RowDefinitions.Where(x => x is ISpacingDefinition))
				spacingRow.Spacing = RowSpacing;
		}

		private void RecalculateColumnSpacing()
		{
			foreach (ISpacingDefinition spacingColumn in ColumnDefinitions.Where(x => x is ISpacingDefinition))
				spacingColumn.Spacing = ColumnSpacing;
		}

		private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
				UpdateChildren(e.NewItems);
		}
	}
}
