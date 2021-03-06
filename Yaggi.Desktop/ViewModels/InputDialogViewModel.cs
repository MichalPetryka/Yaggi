using Avalonia.Collections;
using Avalonia.Controls;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using Yaggi.Desktop.Dialogs;

namespace Yaggi.Desktop.ViewModels
{
	/// <summary>
	/// ViewModel of the <see cref="InputDialog"/>
	/// </summary>
	public class InputDialogViewModel : ViewModelBase
	{
		private AvaloniaList<Control> _items = new();
		private string _title;
		private string _header;

		/// <summary>
		/// Title of the window
		/// </summary>
		public string Title
		{
			get => _title;
			set => this.RaiseAndSetIfChanged(ref _title, value, nameof(Title));
		}

		/// <summary>
		/// Header text
		/// </summary>
		public string Header
		{
			get => _header;
			set
			{
				this.RaiseAndSetIfChanged(ref _header, value, nameof(Header));
				this.RaisePropertyChanged(nameof(HeaderVisible));
			}
		}

		/// <summary>
		/// Whether header should be visible or not
		/// </summary>
		public bool HeaderVisible => !string.IsNullOrWhiteSpace(Header);

		/// <summary>
		/// List of alternating textboxes and textblocks <br/>
		/// More info: <see cref="InputEntries"/>
		/// </summary>
		public AvaloniaList<Control> Items
		{
			get => _items;
			set => this.RaiseAndSetIfChanged(ref _items, value);
		}

		/// <summary>
		/// Set inputs
		/// </summary>
		public IEnumerable<InputDialogEntry> InputEntries
		{
			set
			{
				//create an enumerable of alternating textboxes and textblocks
				//(for each item in InputEntries create one TextBlock for an description and a TextBox for the user input)
				var items = value
					.Select(ie => new TextBlock { Text = ie.Label })
					.Zip(
						value.Select(ie => new TextBox
						{
							//set a default value of a input if any was provided
							Text = !string.IsNullOrWhiteSpace(ie.InitialValue) ? ie.InitialValue : "",
							// show '*' instead of letters if that input should have masked input
							PasswordChar = ie.MaskInput ? '*' : '\0',
							//RevealPassword = ie.MaskInput
						}),
						(l, m) => new Control[] { l, m })
					.SelectMany(x => x);
				//remove any existing items and populate Items
				Items.Clear();
				Items.AddRange(items);
			}
		}
	}
}
