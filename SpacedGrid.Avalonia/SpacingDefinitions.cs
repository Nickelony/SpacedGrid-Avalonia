using Avalonia.Controls;

namespace SpacedGrid.Avalonia
{
	public class SpacingRowDefinition : RowDefinition
	{
		public SpacingRowDefinition(double height) : base(height, GridUnitType.Pixel)
		{ }
	}

	public class SpacingColumnDefinition : ColumnDefinition
	{
		public SpacingColumnDefinition(double width) : base(width, GridUnitType.Pixel)
		{ }
	}
}
