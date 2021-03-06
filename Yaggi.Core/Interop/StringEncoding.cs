// ReSharper disable InconsistentNaming

namespace Yaggi.Core.Interop
{
	/// <summary>
	/// String encoding used in native string representation
	/// </summary>
	public enum StringEncoding
	{
		/// <summary>
		/// System encoding on Windows, UTF8 on other platforms
		/// </summary>
		ANSI,
		/// <summary>
		/// UTF16
		/// </summary>
		Unicode,
		/// <summary>
		/// UTF8
		/// </summary>
		UTF8
	}
}
