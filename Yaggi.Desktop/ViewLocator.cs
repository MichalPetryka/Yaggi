using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using Yaggi.Desktop.ViewModels;

namespace Yaggi.Desktop
{
	public class ViewLocator : IDataTemplate
	{
#pragma warning disable CA1822
		public bool SupportsRecycling => false;
#pragma warning restore CA1822

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

		public bool Match(object data)
		{
			return data is ViewModelBase;
		}
	}
}