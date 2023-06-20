using Avalonia;
using Avalonia.Controls;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace AvaloniaSpacedGrid
{
	public class SpacedGrid : Grid
	{
		#region Properties

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

		/// <summary>
		/// Returns an enumerable of all the grid's row definitions, <u>excluding</u> spacing rows.
		/// </summary>
		public IEnumerable<RowDefinition> UserDefinedRowDefinitions =>
			from definition in RowDefinitions
			where !(definition is ISpacingDefinition)
			select definition;

		/// <summary>
		/// Returns an enumerable of all the grid's column definitions, <u>excluding</u> spacing columns.
		/// </summary>
		public IEnumerable<ColumnDefinition> UserDefinedColumnDefinitions =>
			from definition in ColumnDefinitions
			where !(definition is ISpacingDefinition)
			select definition;

		#endregion Properties

		#region Construction

		public SpacedGrid()
			=> Children.CollectionChanged += Children_CollectionChanged;

		#endregion Construction

		#region Override methods

		protected override void OnInitialized()
		{
			base.OnInitialized();

			RowDefinitions.CollectionChanged += delegate { UpdateSpacedRows(); };
			ColumnDefinitions.CollectionChanged += delegate { UpdateSpacedColumns(); };

			UpdateSpacedRows();
			UpdateSpacedColumns();
		}

		protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
		{
			base.OnPropertyChanged(change);

			switch (change.Property.Name)
			{
				case nameof(RowSpacing):
					RecalculateRowSpacing();
					break;

				case nameof(ColumnSpacing):
					RecalculateColumnSpacing();
					break;
			}
		}

		#endregion Override methods

		#region Events

		private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (Control item in e.NewItems)
					item.Initialized += Item_Initialized;
			}
		}

		private void Item_Initialized(object sender, System.EventArgs e)
		{
			var item = sender as Control;
			item.Initialized -= Item_Initialized;

			SetRow(item, GetRow(item) * 2); // 1 -> 2 or 2 -> 4
			SetRowSpan(item, (GetRowSpan(item) * 2) - 1); // 2 -> 3 or 3 -> 5

			SetColumn(item, GetColumn(item) * 2); // 1 -> 2 or 2 -> 4
			SetColumnSpan(item, (GetColumnSpan(item) * 2) - 1); // 2 -> 3 or 3 -> 5
		}

		#endregion Events

		#region Other methods

		private void UpdateSpacedRows()
		{
			var userRowDefinitions = UserDefinedRowDefinitions.ToList(); // User-defined rows (e.g. the ones defined in XAML files)
			var actualRowDefinitions = new RowDefinitions(); // User-defined + spacing rows

			int currentUserDefinition = 0,
				currentActualDefinition = 0;

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
			RowDefinitions.CollectionChanged += delegate { UpdateSpacedRows(); };
		}

		private void UpdateSpacedColumns()
		{
			var userColumnDefinitions = UserDefinedColumnDefinitions.ToList(); // User-defined columns (e.g. the ones defined in XAML files)
			var actualColumnDefinitions = new ColumnDefinitions(); // User-defined + spacing columns

			int currentUserDefinition = 0,
				currentActualDefinition = 0;

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
			ColumnDefinitions.CollectionChanged += delegate { UpdateSpacedColumns(); };
		}

		private void RecalculateRowSpacing()
		{
			foreach (ISpacingDefinition spacingRow in RowDefinitions.OfType<ISpacingDefinition>())
				spacingRow.Spacing = RowSpacing;
		}

		private void RecalculateColumnSpacing()
		{
			foreach (ISpacingDefinition spacingColumn in ColumnDefinitions.OfType<ISpacingDefinition>())
				spacingColumn.Spacing = ColumnSpacing;
		}

		#endregion Other methods
	}
}
