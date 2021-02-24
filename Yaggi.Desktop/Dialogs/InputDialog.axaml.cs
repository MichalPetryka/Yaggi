using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using Yaggi.Desktop.Extensions;
using Avalonia.Interactivity;
using System.Linq;
using Avalonia.Layout;

namespace Yaggi.Desktop.Dialogs
{
	public class InputDialog : Window
	{
		/// <summary>
		/// Pls don't use this
		/// </summary>
		public InputDialog()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}
		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs args)
		{
			Close(null);
		}

		private void ConfirmButton_Click(object sender, RoutedEventArgs args)
		{
			
			var dc = (ViewModels.InputDialogViewModel)DataContext;
			//get input as strings from TextBoxes, if textbox text was null, replace it with empty string
			Close(dc.Items.Where(x => x.GetType() == typeof(TextBox)).Select(x => ((TextBox)x).Text ?? "").ToArray());
		}


		/// <summary>
		/// Shows dialog and returns user input
		/// </summary>
		/// <code>
		/// var output = await Dialogs.InputDialog.Show(
		///		"Window Title",
		///		"Dialog header",
		///		new Dialogs.InputDialogEntry[] {
		///			new Dialogs.InputDialogEntry("Input 1", false),
		///			new Dialogs.InputDialogEntry("Input 2", true),
		///	});
		/// </code>
		///
		/// <param name="title">Title of the dialog</param>
		/// <param name="header">Header of the dialog</param>
		/// <param name="inputEntries">Array of Input entries</param>
		/// <returns>
		/// Array of user inputs (length of that array is equal to <paramref name="inputEntries"/>)<br/>
		/// null if user clicked cancel or closed the window
		/// </returns>
		public static async Task<string[]> Show(string title, string header, InputDialogEntry[] inputEntries)
		{
			
			var dlg = new InputDialog();
			if (inputEntries is null || inputEntries.Length == 0)
			{
				throw new ArgumentException($"Parameter \"{nameof(inputEntries)}\" of type {typeof(InputDialogEntry[])} was null or empty");
			}
			//fill the dialog
			var dc = new ViewModels.InputDialogViewModel();
			dlg.DataContext = dc;
			dc.Title = title;
			dc.Header = header;
			dc.InputEntries = inputEntries;

			return await dlg.ShowDialog<string[]>();
		}
	}

	/// <summary>
	/// Represents a single entry in <see cref="InputDialog"/> window
	/// </summary>
	public struct InputDialogEntry
	{
		/// <summary>
		/// Represents a single entry in <see cref="InputDialog"/> window
		/// </summary>
		/// <param name="label">Description of the input</param>
		/// <param name="maskInput">Should be set to true if input contain a sensitive data such as password or API key</param>
		public InputDialogEntry(string label, bool maskInput)
		{
			Label = label;
			MaskInput = maskInput;
			DefaultValue = null;
		}
		public InputDialogEntry(string label, string defaultValue, bool maskInput) : this(label, maskInput)
		{
			DefaultValue = defaultValue;
		}
		public readonly string Label;
		public readonly string DefaultValue;
		public readonly bool MaskInput;
	}
}
