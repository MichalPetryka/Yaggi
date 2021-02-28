using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Yaggi.Core.Security
{
	public static unsafe class ProcessDescriptors
	{
		// ReSharper disable UnusedMember.Local
		[Flags]
		private enum ProcessAccessRights
		{
			ProcessCreateProcess = 0x0080, //  Required to create a process.
			ProcessCreateThread = 0x0002, //  Required to create a thread.
			ProcessDupHandle = 0x0040, // Required to duplicate a handle using DuplicateHandle.
			ProcessQueryInformation = 0x0400, //  Required to retrieve certain information about a process, such as its token, exit code, and priority class (see OpenProcessToken, GetExitCodeProcess, GetPriorityClass, and IsProcessInJob).
			ProcessQueryLimitedInformation = 0x1000, //  Required to retrieve certain information about a process (see QueryFullProcessImageName). A handle that has the PROCESS_QUERY_INFORMATION access right is automatically granted PROCESS_QUERY_LIMITED_INFORMATION. Windows Server 2003 and Windows XP/2000:  This access right is not supported.
			ProcessSetInformation = 0x0200, //    Required to set certain information about a process, such as its priority class (see SetPriorityClass).
			ProcessSetQuota = 0x0100, //  Required to set memory limits using SetProcessWorkingSetSize.
			ProcessSuspendResume = 0x0800, // Required to suspend or resume a process.
			ProcessTerminate = 0x0001, //  Required to terminate a process using TerminateProcess.
			ProcessVmOperation = 0x0008, //   Required to perform an operation on the address space of a process (see VirtualProtectEx and WriteProcessMemory).
			ProcessVmRead = 0x0010, //    Required to read memory in a process using ReadProcessMemory.
			ProcessVmWrite = 0x0020, //   Required to write to memory in a process using WriteProcessMemory.
			Delete = 0x00010000, // Required to delete the object.
			ReadControl = 0x00020000, //   Required to read information in the security descriptor for the object, not including the information in the SACL. To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right. For more information, see SACL Access Right.
			Synchronize = 0x00100000, //    The right to use the object for synchronization. This enables a thread to wait until the object is in the signaled state.
			WriteDac = 0x00040000, //  Required to modify the DACL in the security descriptor for the object.
			WriteOwner = 0x00080000, //    Required to change the owner in the security descriptor for the object.
			StandardRightsRequired = 0x000f0000,
			ProcessAllAccess = StandardRightsRequired | Synchronize | 0xFFF //    All possible access rights for a process object.
		}

		[Flags]
		private enum SecurityInformation : uint
		{
			OwnerSecurityInformation = 0x00000001,
			GroupSecurityInformation = 0x00000002,
			DaclSecurityInformation = 0x00000004,
			SaclSecurityInformation = 0x00000008,
			UnprotectedSaclSecurityInformation = 0x10000000,
			UnprotectedDaclSecurityInformation = 0x20000000,
			ProtectedSaclSecurityInformation = 0x40000000,
			ProtectedDaclSecurityInformation = 0x80000000
		}

		[Flags]
		private enum TokenAccess : uint
		{
			StandardRightsRequired = 0x000F0000,
			TokenAssignPrimary = 0x0001,
			TokenDuplicate = 0x0002,
			TokenImpersonate = 0x0004,
			TokenQuery = 0x0008,
			TokenQuerySource = 0x0010,
			TokenAdjustPrivileges = 0x0020,
			TokenAdjustGroups = 0x0040,
			TokenAdjustDefault = 0x0080,
			TokenAdjustSessionid = 0x0100,
			TokenAllAccess = StandardRightsRequired | TokenAssignPrimary | TokenDuplicate | TokenImpersonate | TokenQuery | TokenQuerySource | TokenAdjustPrivileges | TokenAdjustGroups | TokenAdjustDefault
		}
		// ReSharper restore UnusedMember.Local

		public static void SecureProcess()
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return;

			IntPtr hProcessToken = IntPtr.Zero;
			try
			{
				if (OpenProcessToken(GetCurrentProcess(), TokenAccess.TokenAllAccess, out hProcessToken) == 0)
					throw new Win32Exception();

				// Get security descriptor associated with the kernel object and modify it.
				_ = GetKernelObjectSecurity(hProcessToken, SecurityInformation.DaclSecurityInformation, null, 0, out uint returnLength);

				byte[] sdBytes = new byte[returnLength];

				fixed (byte* ptr = sdBytes)
					if (GetKernelObjectSecurity(hProcessToken, SecurityInformation.DaclSecurityInformation, ptr, (uint)sdBytes.Length, out returnLength) == 0)
						throw new Win32Exception();

				RawSecurityDescriptor rawSecurityDescriptor = new(sdBytes, 0);

				if (rawSecurityDescriptor.DiscretionaryAcl == null)
					throw new NullReferenceException();

				const int rights = (int)
					(ProcessAccessRights.ProcessCreateProcess | ProcessAccessRights.ProcessCreateThread |
					ProcessAccessRights.ProcessDupHandle | ProcessAccessRights.ProcessSetInformation |
					ProcessAccessRights.ReadControl | ProcessAccessRights.ProcessSetQuota |
					ProcessAccessRights.ProcessVmRead | ProcessAccessRights.ProcessVmWrite |
					ProcessAccessRights.ProcessVmOperation | ProcessAccessRights.WriteDac | ProcessAccessRights.WriteOwner);

				rawSecurityDescriptor.DiscretionaryAcl.InsertAce(0,
					new CommonAce(AceFlags.None, AceQualifier.AccessDenied, rights,
						new SecurityIdentifier(WellKnownSidType.WorldSid, null), false, null));

				sdBytes = new byte[rawSecurityDescriptor.BinaryLength];
				rawSecurityDescriptor.GetBinaryForm(sdBytes, 0);

				fixed (byte* ptr = sdBytes)
					if (SetKernelObjectSecurity(hProcessToken, SecurityInformation.DaclSecurityInformation, ptr) == 0)
						throw new Win32Exception();
			}
			finally
			{
#pragma warning disable CA2219
				if (hProcessToken != IntPtr.Zero)
					if (CloseHandle(hProcessToken) == 0)
						throw new Win32Exception();
#pragma warning restore CA2219
			}
		}

		[DllImport("Kernel32", EntryPoint = "GetCurrentProcess", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("Advapi32", EntryPoint = "OpenProcessToken", ExactSpelling = true, SetLastError = true)]
		private static extern int OpenProcessToken(IntPtr processHandle, TokenAccess desiredAccess, out IntPtr tokenHandle);

		[DllImport("Advapi32", EntryPoint = "GetKernelObjectSecurity", ExactSpelling = true, SetLastError = true)]
		private static extern int GetKernelObjectSecurity(IntPtr handle, SecurityInformation requestedInformation, byte* pSecurityDescriptor, uint nLength, out uint lpnLengthNeeded);

		[DllImport("Advapi32", EntryPoint = "SetKernelObjectSecurity", ExactSpelling = true, SetLastError = true)]
		private static extern int SetKernelObjectSecurity(IntPtr handle, SecurityInformation securityInformation, byte* securityDescriptor);

		[DllImport("Kernel32", EntryPoint = "CloseHandle", ExactSpelling = true, SetLastError = true)]
		private static extern int CloseHandle(IntPtr handle);
	}
}
