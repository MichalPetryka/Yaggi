using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using Yaggi.Desktop.ViewModels;

namespace Yaggi.Desktop
{
	/// <summary>
	/// ViewLocator defines a data template which converts view models into views.
	/// </summary>
	public class ViewLocator : IDataTemplate
	{
#pragma warning disable CA1822
		/// <summary>
		/// Gets a value indicating whether the data template supports recycling of the generated control.
		/// </summary>
		public bool SupportsRecycling => false;
#pragma warning restore CA1822

		/// <inheritdoc/>
		public IControl Build(object data)
		{
			string name = data.GetType().FullName!.Replace("ViewModel", "View");
			Type type = Type.GetType(name);

			if (type != null)
			{
				return (Control)Activator.CreateInstance(type)!;
			}

			return new TextBlock { Text = "Not Found: " + name };
		}

		/// <inheritdoc/>
		public bool Match(object data)
		{
			return data is ViewModelBase;
		}
	}
}
