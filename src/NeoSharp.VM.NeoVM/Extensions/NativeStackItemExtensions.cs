using System;
using NeoSharp.VM.NeoVM.StackItems;

namespace NeoSharp.VM.NeoVM.Extensions
{
    internal static class NativeStackItemExtensions
    {
        private static readonly Type InteropType = typeof(InteropStackItem<>);

        public static StackItemBase ConvertFromNative(this Neo.VM.StackItem stackItem)
        {
            switch (stackItem)
            {
                case Neo.VM.Types.Array array:
                {
                    if (stackItem is Neo.VM.Types.Struct @struct)
                        return new StructStackItem(@struct);

                    return new ArrayStackItem(array);
                }
                case Neo.VM.Types.Map map:
                {
                    return new MapStackItem(map);
                }
                case Neo.VM.Types.Boolean boolean:
                {
                    return new BooleanStackItem(boolean);
                }
                case Neo.VM.Types.ByteArray byteArray:
                {
                    return new ByteArrayStackItem(byteArray);
                }
                case Neo.VM.Types.Integer integer:
                {
                    return new IntegerStackItem(integer);
                }
                case Neo.VM.Types.InteropInterface interop:
                {
                    var baseType = interop.GetType().GenericTypeArguments[0];
                    var type = InteropType.MakeGenericType(baseType);

                    return (StackItemBase) Activator.CreateInstance(type, interop);
                }
            }

            throw new ArgumentException(nameof(stackItem));
        }

        public static ExecutionContextWrapper ConvertFromNative(this Neo.VM.ExecutionContext context)
        {
            return new ExecutionContextWrapper(context);
        }
    }
}