using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NeoSharp.VM.Interop.Interfaces;
using NeoSharp.VM.Interop.Native;
using NeoSharp.VM.Interop.Types;

namespace NeoSharp.VM.Interop
{
    public class NeoVM : IVMFactory
    {
        #region Private fields

        internal const byte TRUE = 0x01;
        internal const byte FALSE = 0x00;

        private const string LibraryName = "NeoVM";

        /// <summary>
        /// Library core
        /// </summary>
        private readonly static CrossPlatformLibrary Core;

        #endregion

        #region Public fields

        /// <summary>
        /// Library path
        /// </summary>
        public readonly static string LibraryPath;

        /// <summary>
        /// Version
        /// </summary>
        public readonly static Version LibraryVersion;

        /// <summary>
        /// Is loaded
        /// </summary>
        public static readonly bool IsLoaded = false;

        /// <summary>
        /// Last error
        /// </summary>
        public static readonly string LastError;

        #endregion

        #region Core cache

        #region Delegates

        // https://www.codeproject.com/Tips/318140/How-to-make-a-callback-to-Csharp-from-C-Cplusplus

        #region Callbacks

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void OnStepIntoCallback(IntPtr item);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void OnStackChangeCallback(IntPtr item, int index, byte operation);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate byte InvokeInteropCallback(IntPtr ptr, byte size);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate byte LoadScriptCallback([MarshalAs(UnmanagedType.LPArray, SizeConst = 20)]byte[] scriptHash, byte isDynamicInvoke, int rvcount);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate int GetMessageCallback(uint iteration, out IntPtr script);

        #endregion

        // Shared

        internal delegate int delInt_Handle(IntPtr pointer);
        internal delegate void delVoid_Handle(IntPtr pointer);
        internal delegate byte delByte_Handle(IntPtr pointer);
        internal delegate ulong delUInt64_Handle(IntPtr pointer);
        internal delegate IntPtr delHandle_Handle(IntPtr pointer);
        internal delegate void delVoid_RefHandle(ref IntPtr pointer);

        internal delegate void delVoid_OutIntOutIntOutIntOutInt(out int i1, out int i2, out int i3, out int i4);

        internal delegate int delInt_HandleInt(IntPtr pointer, int value);
        internal delegate void delVoid_HandleUInt(IntPtr pointer, uint value);
        internal delegate void delVoid_HandleInt(IntPtr pointer, int value);
        internal delegate int delInt_HandleHandle(IntPtr handle, IntPtr item);
        internal delegate byte delByte_HandleRefInt(IntPtr item, out int size);
        internal delegate IntPtr delHandle_HandleInt(IntPtr pointer, int value);
        internal delegate byte delByte_HandleUInt64(IntPtr pointer, ulong value);
        internal delegate void delVoid_HandleHandle(IntPtr pointer1, IntPtr pointer2);
        internal delegate byte delByte_HandleHandle(IntPtr pointer1, IntPtr pointer2);
        internal delegate IntPtr delHandle_HandleHandle(IntPtr pointer1, IntPtr pointer2);
        internal delegate byte delByte_HandleIntInt(IntPtr handle, int index, int index2);
        internal delegate IntPtr delHandle_ByteHandleInt(byte type, IntPtr data, int size);
        internal delegate void delVoid_HandleIntByte(IntPtr handle, int index, byte dispose);
        internal delegate int delInt_HandleHandleInt(IntPtr pointer1, IntPtr pointer2, int value);
        internal delegate void delVoid_HandleHandleInt(IntPtr pointer1, IntPtr pointer2, int value);
        internal delegate int delInt_HandleHandleIntInt(IntPtr pointer1, IntPtr pointer2, int value, int value2);
        internal delegate void delVoid_HandleHandleHandle(IntPtr pointer1, IntPtr pointer2, IntPtr pointer3);

        // Specific

        internal delegate void delVoid_HandleOnStepIntoCallback(IntPtr handle, OnStepIntoCallback callback);
        internal delegate void delVoid_HandleOnStackChangeCallback(IntPtr item, OnStackChangeCallback callback);
        internal delegate IntPtr delCreateExecutionEngine
            (
            InvokeInteropCallback interopCallback, LoadScriptCallback scriptCallback, GetMessageCallback getMessageCallback,
            out IntPtr invocationHandle, out IntPtr resultStack
            );

        internal delegate void delExecutionContextClaim
            (
            IntPtr handle, out IntPtr invocationHandle, out IntPtr resultStack
            );

        #endregion

        #region Cache

#pragma warning disable CS0649

        internal static delVoid_OutIntOutIntOutIntOutInt GetVersion;

        internal static delCreateExecutionEngine ExecutionEngine_Create;
        internal static delVoid_RefHandle ExecutionEngine_Free;
        internal static delInt_HandleHandleIntInt ExecutionEngine_LoadScript;
        internal static delByte_HandleIntInt ExecutionEngine_LoadCachedScript;
        internal static delByte_Handle ExecutionEngine_Execute;
        internal static delByte_HandleUInt64 ExecutionEngine_ExecuteUntil;
        internal static delVoid_Handle ExecutionEngine_StepInto;
        internal static delVoid_Handle ExecutionEngine_StepOver;
        internal static delVoid_Handle ExecutionEngine_StepOut;
        internal static delByte_Handle ExecutionEngine_GetState;
        internal static delUInt64_Handle ExecutionEngine_GetConsumedGas;
        internal static delVoid_HandleUInt ExecutionEngine_Clean;
        internal static delVoid_HandleOnStepIntoCallback ExecutionEngine_AddLog;

        internal static delInt_Handle StackItems_Count;
        internal static delVoid_HandleHandle StackItems_Push;
        internal static delHandle_Handle StackItems_Pop;
        internal static delHandle_HandleInt StackItems_Peek;
        internal static delInt_HandleInt StackItems_Drop;
        internal static delVoid_HandleOnStackChangeCallback StackItems_AddLog;

        internal static delInt_Handle ExecutionContextStack_Count;
        internal static delInt_HandleInt ExecutionContextStack_Drop;
        internal static delHandle_HandleInt ExecutionContextStack_Peek;
        internal static delVoid_HandleOnStackChangeCallback ExecutionContextStack_AddLog;

        internal static delHandle_ByteHandleInt StackItem_Create;
        internal static delByte_HandleRefInt StackItem_SerializeInfo;
        internal static delInt_HandleHandleInt StackItem_Serialize;
        internal static delVoid_RefHandle StackItem_Free;
        internal static delVoid_Handle StackItem_Claim;

        internal static delInt_Handle MapStackItem_Count;
        internal static delVoid_Handle MapStackItem_Clear;
        internal static delByte_HandleHandle MapStackItem_Remove;
        internal static delHandle_HandleHandle MapStackItem_Get;
        internal static delVoid_HandleHandleHandle MapStackItem_Set;
        internal static delHandle_HandleInt MapStackItem_GetKey;
        internal static delHandle_HandleInt MapStackItem_GetValue;

        internal static delInt_Handle ArrayStackItem_Count;
        internal static delVoid_Handle ArrayStackItem_Clear;
        internal static delHandle_HandleInt ArrayStackItem_Get;
        internal static delVoid_HandleHandleInt ArrayStackItem_Set;
        internal static delVoid_HandleHandle ArrayStackItem_Add;
        internal static delInt_HandleHandle ArrayStackItem_IndexOf;
        internal static delVoid_HandleInt ArrayStackItem_RemoveAt;
        internal static delVoid_HandleHandleInt ArrayStackItem_Insert;

        internal static delInt_HandleHandleInt ExecutionContext_GetScriptHash;
        internal static delByte_Handle ExecutionContext_GetNextInstruction;
        internal static delInt_Handle ExecutionContext_GetInstructionPointer;
        internal static delVoid_RefHandle ExecutionContext_Free;
        internal static delExecutionContextClaim ExecutionContext_Claim;

#pragma warning restore CS0649

        #endregion

        #endregion

        /// <summary>
        /// Static constructor for load NativeCoreType
        /// </summary>
        static NeoVM()
        {
            // Detect OS
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    {
                        Core = new WindowsCore();
                        break;
                    }
                case PlatformID.Unix:
                case (PlatformID)128:
                    {
                        Core = new LinuxCore();
                        break;
                    }
                case PlatformID.MacOSX:
                    {
                        Core = new MacCore();
                        break;
                    }
            }

            // Check core
            if (Core == null)
            {
                LastError = "Native library not found";
                return;
            }

            // Load library
            LibraryPath = Path.Combine(AppContext.BaseDirectory, Core.Platform.ToString(),
                Core.Architecture.ToString(), LibraryName + Core.LibraryExtension);

            // Check Environment path
            if (!File.Exists(LibraryPath))
            {
                string nfile = Environment.GetEnvironmentVariable("NEO_VM_PATH");

                if (string.IsNullOrEmpty(nfile))
                {
                    LastError = "File not found: " + LibraryPath;
                    return;
                }

                LibraryPath = nfile;
                if (!File.Exists(LibraryPath))
                {
                    LastError = "File not found: " + LibraryPath;
                    return;
                }
            }

            if (!Core.LoadLibrary(LibraryPath))
            {
                LastError = "Wrong library file: " + LibraryPath;
                return;
            }

            // Static destructor
            AppDomain.CurrentDomain.ProcessExit += (o, e) =>
            {
                Core?.Dispose();
            };

            // Cache delegates using reflection

            var delegateType = typeof(MulticastDelegate);

            foreach (var fi in typeof(NeoVM).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(fi => fi.FieldType.BaseType == delegateType))
            {
                var del = Core.GetDelegate(fi.Name, fi.FieldType);

                if (del == null)
                {
                    LastError = "Method not found: " + fi.Name;
                    return;
                }

                fi.SetValue(null, del);
            }

            // Get version

            GetVersion(out int major, out int minor, out int build, out int revision);
            LibraryVersion = new Version(major, minor, build, revision);
            IsLoaded = true;
        }

        /// <summary>
        /// Create new Execution Engine
        /// </summary>
        /// <param name="e">Arguments</param>
        public IExecutionEngine Create(ExecutionEngineArgs e)
        {
            return new ExecutionEngine(e);
        }
    }
}