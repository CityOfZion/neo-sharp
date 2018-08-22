using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoSharp.VM.Attributes;

namespace NeoSharp.VM.Types
{
    public class InstructionParser
    {
        /// <summary>
        /// Cache
        /// </summary>
        static Dictionary<EVMOpCode, InstructionAttribute> _cache = new Dictionary<EVMOpCode, InstructionAttribute>();

        /// <summary>
        /// Cache attribute
        /// </summary>
        static InstructionParser()
        {
            // TODO: ReflectionCache?

            var enumType = typeof(EVMOpCode);
            foreach (var t in Enum.GetValues(enumType))
            {
                // Get enumn member
                var memInfo = enumType.GetMember(t.ToString());
                if (memInfo == null || memInfo.Length != 1)
                    throw (new FormatException());

                // Get attribute
                var attribute = memInfo[0].GetCustomAttributes(false)
                    .Where(u => typeof(InstructionAttribute).IsAssignableFrom(((Attribute)u).GetType()))
                    .Cast<InstructionAttribute>()
                    .FirstOrDefault();

                if (attribute == null)
                    continue;

                // Append to cache
                _cache.Add((EVMOpCode)t, attribute);
            }
        }

        /// <summary>
        /// Parse script to instructions
        /// </summary>
        /// <param name="script">Script</param>
        public IEnumerable<Instruction> Parse(byte[] script)
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
                    if (!_cache.TryGetValue(opcode, out var attr)) yield break;

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