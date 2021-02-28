using Avalonia;
using Avalonia.ReactiveUI;
#if !DEBUG
using Yaggi.Core.Security;
#endif

namespace Yaggi.Desktop
{
	internal class Program
	{
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		public static void Main(string[] args)
		{
#if !DEBUG
			ProcessDescriptors.SecureProcess();
#endif
			BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
		}

		// Avalonia configuration, don't remove; also used by visual designer.
		private static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.LogToTrace()
				.UseReactiveUI();
	}
}
