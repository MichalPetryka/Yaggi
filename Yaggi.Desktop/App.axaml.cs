using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Yaggi.Desktop.ViewModels;
using Yaggi.Desktop.Views;

namespace Yaggi.Desktop
{
	/// <inheritdoc/>
	public class App : Application
	{
		/// <inheritdoc/>
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		/// <inheritdoc/>
		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = new MainWindow
				{
					DataContext = new MainWindowViewModel()
				};
			}

			base.OnFrameworkInitializationCompleted();
		}
	}
}
