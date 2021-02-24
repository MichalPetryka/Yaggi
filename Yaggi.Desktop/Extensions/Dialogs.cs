using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Threading.Tasks;

namespace Yaggi.Desktop.Extensions
{
	public static class Dialogs
	{
		/// <summary>
		/// Showing a dialog without specifying a parent window<br/>
		/// The parent of the window will be set to the main window of the app
		/// </summary>
		/// <typeparam name="TResult">Result of the window</typeparam>
		/// <param name="dialog">Dialog Window that you want to show</param>
		/// <returns></returns>
		public static Task<TResult> ShowDialog<TResult>(this Window dialog)
		{
			if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
			{
				throw new InvalidOperationException("The dialog box can only be opened in the desktop application");
			}

			return dialog.ShowDialog<TResult>(desktop.MainWindow);
		}
		/// <summary>
		/// Showing a dialog without specifying a parent window<br/>
		/// The parent of the window will be set to the main window of the app
		/// </summary>
		/// <param name="dialog">Dialog Window that you want to show</param>
		/// <returns></returns>
		public static async Task ShowDialog(this Window dialog)
		{
			if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
			{
				throw new InvalidOperationException("The dialog box can only be opened in the desktop application");
			}

			await dialog.ShowDialog(desktop.MainWindow);
		}
	}
}
