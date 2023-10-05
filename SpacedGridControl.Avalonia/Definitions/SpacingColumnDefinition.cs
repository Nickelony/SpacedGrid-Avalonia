using Avalonia.Controls;
using SpacedGridControl.Avalonia.Interfaces;

namespace SpacedGridControl.Avalonia.Definitions
{
	public class SpacingColumnDefinition : ColumnDefinition, ISpacingDefinition
	{
		public double Spacing
		{
			get => Width.Value;
			set => Width = new GridLength(value, GridUnitType.Pixel);
		}

		public SpacingColumnDefinition(double width) : base(width, GridUnitType.Pixel)
		{ }
	}
}
