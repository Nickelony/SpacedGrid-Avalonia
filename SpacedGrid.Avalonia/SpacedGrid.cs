using Avalonia;
using Avalonia.Controls;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace SpacedGrid.Avalonia
{
	public class SpacedGrid : Grid
	{
		#region Properties

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

		#endregion Properties

		#region Construction

		public SpacedGrid()
		{
			RowDefinitions.CollectionChanged += delegate { UpdateSpacedRows(); };
			ColumnDefinitions.CollectionChanged += delegate { UpdateSpacedColumns(); };

			Children.CollectionChanged += Children_CollectionChanged;
		}

		#endregion Construction

		#region Override methods

		protected override void OnInitialized()
		{
			base.OnInitialized();

			UpdateSpacedRows();
			UpdateSpacedColumns();

			UpdateChildren(Children);
		}

		#endregion Override methods

		#region Events

		private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
				UpdateChildren(e.NewItems);
		}

		#endregion Events

		#region Other methods

		private void UpdateSpacedRows()
		{
			var userRowDefinitions = new RowDefinitions(); // User-defined rows (e.g. the ones defined in XAML files)
			userRowDefinitions.AddRange(RowDefinitions.Where(x => !(x is ISpacingDefinition))); // Exclude spacing rows

			var actualRowDefinitions = new RowDefinitions(); // User-defined + spacing rows

			int currentUserDefinition = 0, currentActualDefinition = 0;

			while (currentUserDefinition < userRowDefinitions.Count)
			{
				if (currentActualDefinition % 2 == 0) // Even rows are user-defined rows (0, 2, 4, 6, 8, 10, ...)
				{
					actualRowDefinitions.Add(userRowDefinitions[currentUserDefinition]);
					currentUserDefinition++;
				}
				else // Odd rows are spacing rows (1, 3, 5, 7, 9, 11, ...)
					actualRowDefinitions.Add(new SpacingRowDefinition(RowSpacing));

				currentActualDefinition++;
			}

			RowDefinitions = actualRowDefinitions;
		}

		private void UpdateSpacedColumns()
		{
			var userColumnDefinitions = new ColumnDefinitions(); // User-defined columns (e.g. the ones defined in XAML files)
			userColumnDefinitions.AddRange(ColumnDefinitions.Where(x => !(x is ISpacingDefinition))); // Exclude spacing columns

			var actualColumnDefinitions = new ColumnDefinitions(); // User-defined + spacing columns

			int currentUserDefinition = 0, currentActualDefinition = 0;

			while (currentUserDefinition < userColumnDefinitions.Count)
			{
				if (currentActualDefinition % 2 == 0) // Even columns are user-defined columns (0, 2, 4, 6, 8, 10, ...)
				{
					actualColumnDefinitions.Add(userColumnDefinitions[currentUserDefinition]);
					currentUserDefinition++;
				}
				else // Odd columns are spacing columns (1, 3, 5, 7, 9, 11, ...)
					actualColumnDefinitions.Add(new SpacingColumnDefinition(ColumnSpacing));

				currentActualDefinition++;
			}

			ColumnDefinitions = actualColumnDefinitions;
		}

		/// <summary>
		/// Updates the following parameters of passed children, so they match the new Row and Column definitions:<br />
		/// <c>Grid.Row</c><br />
		/// <c>Grid.Column</c><br />
		/// <c>Grid.RowSpan</c><br />
		/// <c>Grid.ColumnSpan</c>
		/// </summary>
		private void UpdateChildren(IList children)
		{
			foreach (Control child in children)
			{
				SetRow(child, GetRow(child) * 2); // 1 -> 2 or 2 -> 4
				SetRowSpan(child, GetRowSpan(child) * 2 - 1); // 2 -> 3 or 3 -> 5

				SetColumn(child, GetColumn(child) * 2); // 1 -> 2 or 2 -> 4
				SetColumnSpan(child, GetColumnSpan(child) * 2 - 1); // 2 -> 3 or 3 -> 5
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

		#endregion Other methods
	}
}
