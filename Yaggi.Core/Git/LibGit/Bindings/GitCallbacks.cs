using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;
using Yaggi.Core.Git.LibGit.Bindings.Structures;

namespace Yaggi.Core.Git.LibGit.Bindings
{
	internal static unsafe class GitCallbacks
	{
		private const CallingConvention Convention = CallingConvention.Cdecl;

		[UnmanagedFunctionPointer(Convention)]
		public delegate int IndexerProgressCallback(GitIndexerProgress* stats, IntPtr payload);

		[UnmanagedFunctionPointer(Convention)]
		public delegate void CheckoutProgressCallback(IntPtr path, nuint completedSteps, nuint totalSteps, IntPtr payload);

		[UnmanagedFunctionPointer(Convention)]
		public delegate GitErrorCode CredentialAcquireCallback(GitCredential** outData, IntPtr url, IntPtr usernameFromUrl, GitCredentialType allowedTypes, IntPtr payload);

		[UnmanagedFunctionPointer(Convention)]
		public delegate void CredentialSshInteractiveCallback(IntPtr name, int nameLength, IntPtr instruction, int instructionLength, int numPrompts, IntPtr prompts, IntPtr* responses, void** abstractData);

		[UnmanagedFunctionPointer(Convention)]
		public delegate void CredentialSignCallback(IntPtr session, byte** sig, nuint sigLength, byte* data, nuint dataLength, void** abstractData);
	}
}
