using System;
using System.Runtime.InteropServices;
using NeoSharp.VM.Interop.Enums;
using NeoSharp.VM.Interop.Interfaces;

namespace NeoSharp.VM.Interop.Native
{
    internal class WindowsCore : CrossPlatformLibrary
    {
        const string NativeLibrary = "kernel32.dll";

        public WindowsCore() : base(EPlatform.Windows, ".dll") { }

        #region Windows

        [DllImport(NativeLibrary, EntryPoint = "LoadLibrary", SetLastError = true)]
        static extern IntPtr _LoadLibrary(string fileName);

        [DllImport(NativeLibrary, EntryPoint = "FreeLibrary", SetLastError = true)]
        static extern bool _FreeLibrary(IntPtr hModule);

        [DllImport(NativeLibrary, EntryPoint = "GetProcAddress", SetLastError = true)]
        static extern IntPtr _GetProcAddress(IntPtr handle, string procedureName);

#if DEBUG

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

#endif

        #endregion

        #region Internals

        protected override bool InternalFreeLibrary()
        {
            return _FreeLibrary(_nativeHandle);
        }

        protected override IntPtr GetProcAddress(string name)
        {
            return _GetProcAddress(_nativeHandle, name);
        }

        protected override bool InternalLoadLibrary(string fileName, out IntPtr handle)
        {
            handle = _LoadLibrary(fileName);

            if (handle == IntPtr.Zero)
            {
#if DEBUG
                var err = GetLastError();
                System.Diagnostics.Debugger.Log(0, "NATIVE_ERROR", err.ToString());
#endif
                return false;
            }

            return true;
        }

        #endregion
    }
}