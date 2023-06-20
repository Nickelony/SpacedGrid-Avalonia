using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace AvaloniaSpacedGrid.Demo
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			spacedGrid = this.FindControl<SpacedGrid>("spacedGrid");
			textBlock_RowSpacing = this.FindControl<TextBlock>("textBlock_RowSpacing");
			textBlock_ColumnSpacing = this.FindControl<TextBlock>("textBlock_ColumnSpacing");
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void RowSpacingSliderPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.Property.Name.Equals("Value", StringComparison.OrdinalIgnoreCase))
			{
				spacedGrid.RowSpacing = (double)e.NewValue!;
				textBlock_RowSpacing.Text = e.NewValue.ToString();
			}
		}

		private void ColumnSpacingSliderPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.Property.Name.Equals("Value", StringComparison.OrdinalIgnoreCase))
			{
				spacedGrid.ColumnSpacing = (double)e.NewValue!;
				textBlock_ColumnSpacing.Text = e.NewValue.ToString();
			}
		}
	}
}
