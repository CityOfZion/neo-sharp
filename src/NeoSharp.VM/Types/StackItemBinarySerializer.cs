using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NeoSharp.VM.Extensions;

namespace NeoSharp.VM
{
    public class StackItemBinarySerializer
    {
        /// <summary>
        /// Set Max Array Size
        /// </summary>
        public const uint MaxArraySize = 1024;

        internal class ContainerPlaceholder
        {
            public EStackItemType Type;
            public int ElementCount;
        }

        public void Serialize(StackItemBase stackItem, BinaryWriter writer)
        {
            var serialized = new List<StackItemBase>();
            var unserialized = new Stack<StackItemBase>();

            unserialized.Push(stackItem);

            while (unserialized.Count > 0)
            {
                stackItem = unserialized.Pop();

                switch (stackItem)
                {
                    case ByteArrayStackItemBase _:
                        {
                            writer.Write((byte)EStackItemType.ByteArray);
                            writer.WriteVarBytes(stackItem.ToByteArray());
                            break;
                        }
                    case BooleanStackItemBase b:
                        {
                            writer.Write((byte)EStackItemType.Bool);
                            writer.Write(b.Value);
                            break;
                        }
                    case IntegerStackItemBase _:
                        {
                            writer.Write((byte)EStackItemType.Integer);
                            writer.WriteVarBytes(stackItem.ToByteArray());
                            break;
                        }
                    case ArrayStackItemBase array:
                        {
                            if (serialized.Any(p => ReferenceEquals(p, array)))
                                throw new NotSupportedException();

                            serialized.Add(array);

                            if (array.IsStruct)
                                writer.Write((byte)EStackItemType.Struct);
                            else
                                writer.Write((byte)EStackItemType.Array);

                            writer.WriteVarInt(array.Count);

                            for (var i = array.Count - 1; i >= 0; i--)
                                unserialized.Push(array[i]);

                            break;
                        }
                    case MapStackItemBase map:
                        {
                            if (serialized.Any(p => ReferenceEquals(p, map)))
                                throw new NotSupportedException();

                            serialized.Add(map);
                            writer.Write((byte)EStackItemType.Map);
                            writer.WriteVarInt(map.Count);

                            foreach (var pair in map.Reverse())
                            {
                                unserialized.Push(pair.Value);
                                unserialized.Push(pair.Key);
                            }
                            break;
                        }
                    default: throw new NotSupportedException();
                }
            }
        }

        public StackItemBase Deserialize(Stack stack, BinaryReader reader)
        {
            var deserialized = new Stack<StackItemBase>();
            var undeserialized = 1;

            while (undeserialized-- > 0)
            {
                var type = (EStackItemType)reader.ReadByte();
                switch (type)
                {
                    case EStackItemType.ByteArray:
                        {
                            deserialized.Push(stack.CreateByteArray(reader.ReadVarBytes()));
                            break;
                        }
                    case EStackItemType.Bool:
                        {
                            deserialized.Push(stack.CreateBool(reader.ReadBoolean()));
                            break;
                        }
                    case EStackItemType.Integer:
                        {
                            deserialized.Push(stack.CreateInteger(new BigInteger(reader.ReadVarBytes())));
                            break;
                        }
                    case EStackItemType.Array:
                    case EStackItemType.Struct:
                        {
                            var count = (int)reader.ReadVarInt(MaxArraySize);

                            deserialized.Push(stack.CreateInterop(new ContainerPlaceholder
                            {
                                Type = type,
                                ElementCount = count
                            }));

                            undeserialized += count;
                        }
                        break;
                    case EStackItemType.Map:
                        {
                            var count = (int)reader.ReadVarInt(MaxArraySize);

                            deserialized.Push(stack.CreateInterop(new ContainerPlaceholder
                            {
                                Type = type,
                                ElementCount = count
                            }));

                            undeserialized += count * 2;
                        }
                        break;
                    default:
                        throw new FormatException();
                }
            }

            var stackTemp = new Stack<StackItemBase>();

            while (deserialized.Count > 0)
            {
                var item = deserialized.Pop();

                if (item is InteropStackItemBase<ContainerPlaceholder> interop)
                {
                    var placeholder = interop.Value;

                    switch (placeholder.Type)
                    {
                        case EStackItemType.Array:
                            {
                                var array = stack.CreateArray();

                                for (var i = 0; i < placeholder.ElementCount; i++)
                                {
                                    using (var val = stackTemp.Pop())
                                    {
                                        array.Add(val);
                                    }
                                }

                                item = array;
                                break;
                            }
                        case EStackItemType.Struct:
                            {
                                var @struct = stack.CreateStruct();

                                for (var i = 0; i < placeholder.ElementCount; i++)
                                {
                                    using (var val = stackTemp.Pop())
                                    {
                                        @struct.Add(val);
                                    }
                                }

                                item = @struct;
                                break;
                            }
                        case EStackItemType.Map:
                            {
                                var map = stack.CreateMap();

                                for (var i = 0; i < placeholder.ElementCount; i++)
                                {
                                    using (var key = stackTemp.Pop())
                                    using (var value = stackTemp.Pop())
                                    {
                                        map.Set(key, value);
                                    }
                                }

                                item = map;
                                break;
                            }
                    }
                }

                stackTemp.Push(item);
            }

            return stackTemp.Pop();
        }
    }
}