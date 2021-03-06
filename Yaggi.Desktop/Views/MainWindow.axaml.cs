using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace Yaggi.Desktop.Views
{
	/// <summary>
	/// Main <see cref="Window"/> of the application
	/// </summary>
	public class MainWindow : Window
	{
		/// <summary>
		/// Creates the window and initializes its content
		/// </summary>
		public MainWindow()
		{
			Opened += OnOpened;
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		private async void OnOpened(object sender, EventArgs e)
		{
			// test clone code
			/*await Task.Run(() =>
			{
				using (GitClient client = new GitCommandlineClient())
				{
					client.CloneRepository(@"I:\test", @"git@github.com:MichalPetryka/Yaggi.git",
						(s, d) => Debug.WriteLine($"{s} {d:P}"), (title, inputs) =>
						{
							string[] responses = InputDialog.Show(title, title,
								inputs.Select(input => new InputDialogEntry(input.prompt, input.defaultValue, input.confidential)));
							return (responses != null, responses);
						}).Dispose();
				}
			});*/
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
