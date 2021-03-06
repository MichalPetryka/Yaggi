using Yaggi.Desktop.Views;

namespace Yaggi.Desktop.ViewModels
{
	/// <summary>
	/// The ViewModel of the <see cref="MainWindow"/>
	/// </summary>
	public class MainWindowViewModel : ViewModelBase
	{
		/// <summary>
		/// Welcome text message
		/// </summary>
		public string Greeting => "Welcome to Avalonia!";
	}
}
