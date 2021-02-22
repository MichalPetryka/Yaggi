using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable HeapView.BoxingAllocation

namespace Yaggi.Core.Git.GitCommandline
{
	/// <summary>
	/// Thrown when a process returns a non zero exit code
	/// </summary>
	public class ProcessException : Exception
	{
		/// <summary>
		/// Executed command
		/// </summary>
		public string Command { get; }
		/// <summary>
		/// Exit code
		/// </summary>
		public int ExitCode { get; }
		/// <summary>
		/// Process output
		/// </summary>
		public string Output { get; }
		/// <summary>
		/// Process error
		/// </summary>
		public string Error { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="ProcessException"/>
		/// </summary>
		/// <param name="command">Executed command</param>
		/// <param name="exitCode">Exit code</param>
		/// <param name="output">Process output</param>
		/// <param name="error">Process error</param>
		internal ProcessException(string command, int exitCode, string output = null, string error = null) : base(
			$"Process \"{command}\" exitted with 0x{exitCode:X8}{(string.IsNullOrEmpty(output) ? "" : $"\nOutput: {output}")}{(string.IsNullOrEmpty(error) ? "" : $"\nError: {error}")}")
		{
			Command = command;
			Output = output;
			ExitCode = exitCode;
			Error = error;
		}
	}
}
