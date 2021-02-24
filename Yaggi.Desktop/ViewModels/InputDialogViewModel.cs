using Avalonia.Collections;
using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using Yaggi.Desktop.Dialogs;

namespace Yaggi.Desktop.ViewModels
{
	public class InputDialogViewModel : ViewModelBase
	{

		/// <summary>
		/// Title of the window
		/// </summary>
		public string Title
		{
			get => title;
			set => this.RaiseAndSetIfChanged(ref title, value);
		}
		private string title;

		/// <summary>
		/// Whether header should be visible or not
		/// </summary>
		public bool HeaderVisible => !string.IsNullOrWhiteSpace(Header);

		/// <summary>
		/// Header text
		/// </summary>
		public string Header
		{
			get => header;
			set
			{
				this.RaiseAndSetIfChanged(ref header, value);
				this.RaisePropertyChanged(nameof(HeaderVisible));
			}
		}
		private string header;

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
						value.Select(ie => new TextBox {
							//set a default value of a input if any was provided
							Text = !string.IsNullOrWhiteSpace(ie.DefaultValue) ? ie.DefaultValue : "",
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

		/// <summary>
		/// List of alternating textboxes and textblocks <br/>
		/// More info: <see cref="InputEntries"/>
		/// </summary>
		public AvaloniaList<Control> Items
		{
			get => items;
			set => this.RaiseAndSetIfChanged(ref items, value);
		}
		private AvaloniaList<Control> items = new AvaloniaList<Control>();

		public string Input { get; set; }
	}
}
