using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace Yaggi.Desktop.Views
{
	public class MainWindow : Window
	{
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
						(s, d) => Debug.WriteLine($"{s} {d:P}"), (s, tuples) =>
						{
							string[] a = InputDialog.Show(s, s,
								tuples.Select(tuple => new InputDialogEntry(tuple.Item1, tuple.Item2, tuple.Item3))
									.ToArray());
							return (a != null, a);
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
