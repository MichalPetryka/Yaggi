﻿using System;
using System.Text;
using Yaggi.Core.IO;

namespace Yaggi.Core.Git.GitCommandline
{
	/// <summary>
	/// Git remote representation for the commandline git client
	/// </summary>
	public class GitCommandlineRemote : GitRemote
	{
		private string _name;

		/// <inheritdoc/>
		public override string Name
		{
			get => _name;
			set
			{
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentException("Invalid new branch name", nameof(value));
				CommandlineUtils.CreateProcess("git",
					$"remote rename {CommandlineUtils.EscapeArgument(_name)} {CommandlineUtils.EscapeArgument(value)}",
					Encoding.UTF8, Repository.Path);
				RenameRemote(_name, value);
				_name = value;
			}
		}

		/// <inheritdoc/>
		public override string Url
		{
			get
			{
				CommandlineUtils.CreateProcess("git", $"remote get-url {CommandlineUtils.EscapeArgument(_name)}",
					Encoding.UTF8, out string output, out _, Repository.Path);
				return output.Trim();
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentException("Invalid new url", nameof(value));
				CommandlineUtils.CreateProcess("git",
					$"remote set-url {CommandlineUtils.EscapeArgument(_name)} {CommandlineUtils.EscapeArgument(value)}",
					Encoding.UTF8, Repository.Path);
			}
		}

		internal GitCommandlineRemote(string name, GitRepository repository) : base(repository)
		{
			_name = name ?? throw new ArgumentNullException(nameof(name));
		}
	}
}
