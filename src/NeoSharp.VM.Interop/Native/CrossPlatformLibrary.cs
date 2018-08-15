using System;
using System.Runtime.InteropServices;
using NeoSharp.VM.Interop.Enums;

namespace NeoSharp.VM.Interop.Interfaces
{
    internal abstract class CrossPlatformLibrary : IDisposable
    {
        /// <summary>
        /// Architecture
        /// </summary>
        public readonly EArchitecture Architecture = IntPtr.Size == 8 ? EArchitecture.x64 : EArchitecture.x86;

        /// <summary>
        /// Platform
        /// </summary>
        public readonly EPlatform Platform;

        /// <summary>
        /// LibraryExtension
        /// </summary>
        public readonly string LibraryExtension;

        /// <summary>
        /// Library Handle
        /// </summary>
        protected IntPtr _nativeHandle;

        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="platform">Platform</param>
        /// <param name="libraryExtension">Library extension</param>
        protected CrossPlatformLibrary(EPlatform platform, string libraryExtension)
        {
            Platform = platform;
            LibraryExtension = libraryExtension;
            _nativeHandle = IntPtr.Zero;
        }

        #region CrossPlatform Support

        #region Abstracts
        
        /// <summary>
        /// Internal load library
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <param name="handle">Handle</param>
        /// <returns>Return true if correct</returns>
        protected abstract bool InternalLoadLibrary(string fileName, out IntPtr handle);
        
        /// <summary>
        /// Internal Free library
        /// </summary>
        /// <returns>Return true if correct</returns>
        protected abstract bool InternalFreeLibrary();
        
        /// <summary>
        /// Get address of method
        /// </summary>
        /// <param name="name">Method name</param>
        /// <returns>Return handle of method</returns>
        protected abstract IntPtr GetProcAddress(string name);

        #endregion

        /// <summary>
        /// Load native library
        /// </summary>
        /// <param name="fileName">Library filename</param>
        /// <returns>Return the library handle</returns>
        public bool LoadLibrary(string fileName)
        {
            if (_nativeHandle != IntPtr.Zero)
                throw (new NotSupportedException("Library is already loaded"));

            if (!InternalLoadLibrary(fileName, out IntPtr h))
            {
                _nativeHandle = IntPtr.Zero;
                return false;
            }

            _nativeHandle = h;
            return true;
        }

        /// <summary>
        /// Free Current library
        /// </summary>
        /// <returns>Return false if fail</returns>
        public bool FreeLibrary()
        {
            if (_nativeHandle == IntPtr.Zero)
                throw (new NotSupportedException("Library not loaded"));

            if (InternalFreeLibrary())
            {
                _nativeHandle = IntPtr.Zero;
                return true;
            }

            return false;
        }

        #endregion

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (_nativeHandle == IntPtr.Zero) return;

            // Free unmanaged resources (unmanaged objects) and override a finalizer below and set large fields to null.
            FreeLibrary();
        }

        /// <summary>
        /// override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        /// </summary>
        ~CrossPlatformLibrary()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // The finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Gets a delegate to a method in an unmanaged module.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="delegateType">The type of the delegate to return.</param>
        /// <returns>A delegate to the method.</returns>
        public Delegate GetDelegate(string methodName, Type delegateType)
        {
            var procaddress = GetProcAddress(methodName);
            if (procaddress == IntPtr.Zero) return null;

            return Marshal.GetDelegateForFunctionPointer(procaddress, delegateType);
        }

        /// <summary>
        /// Gets a delegate to a method in an unmanaged module.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate to return.</typeparam>
        /// <param name="methodName">The name of the method.</param>
        /// <returns>A delegate to the method.</returns>
        public TDelegate GetDelegate<TDelegate>(string methodName) where TDelegate : class
        {
            return GetDelegate(methodName, typeof(TDelegate)) as TDelegate;
        }
    }
}