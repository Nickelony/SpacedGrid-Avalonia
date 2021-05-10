using Avalonia.Controls;

namespace SpacedGrid.Avalonia
{
	public class SpacingRowDefinition : RowDefinition, ISpacingDefinition
	{
		public double Spacing
		{
			get => Height.Value;
			set => Height = new GridLength(value, GridUnitType.Pixel);
		}

		public SpacingRowDefinition(double height) : base(height, GridUnitType.Pixel)
		{ }
	}
}
