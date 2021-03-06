using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yaggi.Desktop.Extensions;

namespace Yaggi.Desktop.Dialogs
{
	/// <summary>
	/// Input dialog providing multiple fields when needed.
	/// </summary>
	public class InputDialog : Window
	{
		private string[] _result;

		/// <summary>
		/// Pls don't use this<br/>
		/// Use <see cref="ShowAsync(string, string, InputDialogEntry[])"/> or <see cref="Show(string, string, InputDialogEntry[])"/> instead
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
			_result = dc.Items
					.Where(controls => controls is TextBox)
					.Select(input => ((TextBox)input).Text ?? "")
					.ToArray();

			Close(_result);
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
		/// <param name="inputEntries">Input entries</param>
		/// <returns>
		/// Array of user inputs (length of that array is equal to <paramref name="inputEntries"/>)<br/>
		/// null if user clicked cancel or closed the window
		/// </returns>
		public static Task<string[]> ShowAsync(string title, string header, IEnumerable<InputDialogEntry> inputEntries) =>
			ShowAsync(title, header, inputEntries.ToArray());

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
		public static Task<string[]> ShowAsync(string title, string header, params InputDialogEntry[] inputEntries)
		{
			var dlg = new InputDialog();
			if (inputEntries == null || inputEntries.Length == 0)
				throw new ArgumentException($"Parameter \"{nameof(inputEntries)}\" of type {typeof(InputDialogEntry[])} was null or empty", nameof(inputEntries));

			//fill the dialog
			var dc = new ViewModels.InputDialogViewModel();
			dlg.DataContext = dc;
			dc.Title = title;
			dc.Header = header;
			dc.InputEntries = inputEntries;

			return dlg.ShowDialog<string[]>();
		}

		/// <summary>
		/// Shows dialog synchronously and returns user input
		/// </summary>
		/// <code>
		/// var output = Dialogs.InputDialog.ShowSync(
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
		/// <param name="inputEntries">Input entries</param>
		/// <returns>
		/// Array of user inputs (length of that array is equal to <paramref name="inputEntries"/>)<br/>
		/// null if user clicked cancel or closed the window
		/// </returns>
		public static string[] Show(string title, string header, IEnumerable<InputDialogEntry> inputEntries) =>
			Show(title, header, inputEntries.ToArray());

		/// <summary>
		/// Shows dialog synchronously and returns user input
		/// </summary>
		/// <code>
		/// var output = Dialogs.InputDialog.ShowSync(
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
		public static string[] Show(string title, string header, params InputDialogEntry[] inputEntries)
		{
			if (inputEntries == null || inputEntries.Length == 0)
				throw new ArgumentException($"Parameter \"{nameof(inputEntries)}\" of type {typeof(InputDialogEntry[])} was null or empty", nameof(inputEntries));

			string[] result = null;

			void MakeWindow()
			{
				var dlg = new InputDialog();

				//fill the dialog
				var dc = new ViewModels.InputDialogViewModel();
				dlg.DataContext = dc;
				dc.Title = title;
				dc.Header = header;
				dc.InputEntries = inputEntries;

				dlg.WaitForDialog();
				result = dlg._result;
			}

			if (Dispatcher.UIThread.CheckAccess())
				MakeWindow();
			else
				Dispatcher.UIThread.Invoke(MakeWindow);

			return result;
		}
	}

	/// <summary>
	/// Represents a single entry in <see cref="InputDialog"/> window
	/// </summary>
	public readonly struct InputDialogEntry
	{
		/// <summary>
		/// Input field description
		/// </summary>
		public readonly string Label;
		/// <summary>
		/// Initial value of the input field
		/// </summary>
		public readonly string InitialValue;
		/// <summary>
		/// Should the input field content be visible when typing
		/// </summary>
		public readonly bool MaskInput;

		/// <summary>
		/// Represents a single entry in <see cref="InputDialog"/> window
		/// </summary>
		/// <param name="label">Description of the input</param>
		/// <param name="initialValue">Initial value of the input</param>
		/// <param name="maskInput">Should be set to true if input contain a sensitive data such as password or API key</param>
		public InputDialogEntry(string label, string initialValue = null, bool maskInput = false)
		{
			Label = label;
			MaskInput = maskInput;
			InitialValue = initialValue;
		}
	}
}
