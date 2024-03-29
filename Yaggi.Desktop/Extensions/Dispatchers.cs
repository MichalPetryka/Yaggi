﻿using Avalonia.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Yaggi.Desktop.Extensions
{
	/// <summary>
	/// Provides extensions methods for working with Dispatchers
	/// </summary>
	public static class Dispatchers
	{
		/// <summary>
		/// Invoke a dispatcher synchronously
		/// </summary>
		/// <param name="dispatcher">A dispatcher you want to invoke</param>
		/// <param name="action">An <see cref="Action"/> that is going to be run inside a <paramref name="dispatcher"/></param>
		/// <param name="priority">The priority with which to invoke the <paramref name="action"/></param>
		/// <param name="sleepTimeMilliseconds">Time in milliseconds between checking that the <paramref name="dispatcher"/> is finished</param>
		public static void Invoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority = DispatcherPriority.Normal, int sleepTimeMilliseconds = 100)
		{
			bool finished = false;

			dispatcher.Post(() =>
			{
				action();
				finished = true;
			}, priority);

			while (!finished)
				Thread.Sleep(sleepTimeMilliseconds);
		}

		/// <summary>
		/// Invoke a dispatcher synchronously
		/// </summary>
		/// <param name="dispatcher">A dispatcher you want to invoke</param>
		/// <param name="function">A <see cref="Func{Task}"/> that is going to be run inside a <paramref name="dispatcher"/></param>
		/// <param name="priority">The priority with which to invoke the <paramref name="function"/></param>
		/// <param name="sleepTimeMilliseconds">Time in milliseconds between checking that the <paramref name="dispatcher"/> is finished</param>
		public static void Invoke(this Dispatcher dispatcher, Func<Task> function, DispatcherPriority priority = DispatcherPriority.Normal, int sleepTimeMilliseconds = 100)
		{
			bool finished = false;

			dispatcher.InvokeAsync(async () =>
			{
				await function();
				finished = true;
			}, priority);

			while (!finished)
				Thread.Sleep(sleepTimeMilliseconds);
		}
	}
}
