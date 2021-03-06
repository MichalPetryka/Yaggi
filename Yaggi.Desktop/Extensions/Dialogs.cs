using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Yaggi.Desktop.Extensions
{
	/// <summary>
	/// Provides extension methods for working with dialogs
	/// </summary>
	public static class Dialogs
	{
		/// <summary>
		/// Showing a dialog without specifying a owner window<br/>
		/// The owner of the window will be set to the main window of the app
		/// </summary>
		/// <typeparam name="TResult">Result of the window</typeparam>
		/// <param name="dialog">Dialog Window that you want to show</param>
		/// <returns></returns>
		public static Task<TResult> ShowDialog<TResult>(this Window dialog)
		{
			if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
				throw new InvalidOperationException("The dialog box can only be opened in the desktop application");

			return dialog.ShowDialog<TResult>(desktop.MainWindow);
		}

		/// <summary>
		/// Showing a dialog without specifying a owner window<br/>
		/// The owner of the window will be set to the main window of the app
		/// </summary>
		/// <param name="dialog">Dialog Window that you want to show</param>
		/// <returns></returns>
		public static async Task ShowDialog(this Window dialog)
		{
			if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
				throw new InvalidOperationException("The dialog box can only be opened in the desktop application");

			await dialog.ShowDialog(desktop.MainWindow);
		}

		/// <summary>
		/// Showing a dialog synchronously without specifying a owner window<br/>
		/// The owner of the window will be set to the main window of the app
		/// </summary>
		/// <param name="dialog">Dialog Window that you want to show</param>
		public static void WaitForDialog(this Window dialog)
		{
			if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
				throw new InvalidOperationException("The dialog box can only be opened in the desktop application");

			using (var source = new CancellationTokenSource())
			{
				_ = dialog.ShowDialog(desktop.MainWindow).ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
				Dispatcher.UIThread.MainLoop(source.Token);
			}
		}

		/// <summary>
		/// Showing a dialog synchronously
		/// </summary>
		/// <param name="dialog">Dialog Window that you want to show</param>
		/// <param name="owner">Owner of the dialog</param>
		public static void WaitForDialog(this Window dialog, Window owner)
		{
			using (var source = new CancellationTokenSource())
			{
				_ = dialog.ShowDialog(owner).ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
				Dispatcher.UIThread.MainLoop(source.Token);
			}
		}
	}
}
