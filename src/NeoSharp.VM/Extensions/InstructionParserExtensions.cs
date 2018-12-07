using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoSharp.VM.Attributes;

namespace NeoSharp.VM.Extensions
{
    public static class InstructionParserExtensions
    {
        /// <summary>
        /// Cache
        /// </summary>
        private static readonly Dictionary<EVMOpCode, InstructionAttribute> Cache = new Dictionary<EVMOpCode, InstructionAttribute>();

        /// <summary>
        /// Cache attribute
        /// </summary>
        static InstructionParserExtensions()
        {
            // TODO #392: ReflectionCache?

            var enumType = typeof(EVMOpCode);
            foreach (var t in Enum.GetValues(enumType))
            {
                // Get enum member
                var memInfo = enumType.GetMember(t.ToString());
                if (memInfo == null || memInfo.Length != 1)
                    throw new FormatException();

                // Get attribute
                var attribute = memInfo[0].GetCustomAttributes(false)
                    .Where(u => typeof(InstructionAttribute).IsAssignableFrom(((Attribute)u).GetType()))
                    .Cast<InstructionAttribute>()
                    .FirstOrDefault();

                if (attribute == null)
                    continue;

                // Append to cache
                Cache.Add((EVMOpCode)t, attribute);
            }
        }

        /// <summary>
        /// Parse instruction into script
        /// </summary>
        /// <param name="instructions">Instruction</param>
        /// <returns>Return byte array</returns>
        public static byte[] ToScript(this IEnumerable<Instruction> instructions)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                foreach (var instruction in instructions)
                {
                    ms.Write(instruction.ToByteArray());
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Parse script to instructions
        /// </summary>
        /// <param name="script">Script</param>
        public static IEnumerable<Instruction> DecompileScript(this byte[] script)
        {
            var index = 0;
            var offset = 0L;
            var opType = typeof(EVMOpCode);

            using (var ms = new MemoryStream(script))
            using (var reader = new BinaryReader(ms))
            {
                while (offset < ms.Length)
                {
                    var location = new InstructionLocation()
                    {
                        Index = index,
                        Offset = offset
                    };

                    var opRead = reader.ReadByte();
                    if (!Enum.IsDefined(opType, opRead)) yield break;

                    var opcode = (EVMOpCode)Enum.ToObject(opType, opRead);
                    if (!Cache.TryGetValue(opcode, out var attr)) yield break;

                    var i = attr.New(location, opcode);

                    if (!attr.Fill(reader, i)) yield break;

                    offset = ms.Position;
                    index++;

                    yield return i;
                }
            }
        }
    }
}