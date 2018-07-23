using NeoSharp.VM.Attributes;

namespace NeoSharp.VM
{
    public enum EVMOpCode : byte
    {
        // Constants

        /// <summary>
        /// An empty array of bytes is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH0 = 0x00,
        /// <summary>
        /// 0x01 The next opcode bytes is data to be pushed onto the stack
        /// </summary>
        [InstructionWithPayload(FixedQuantity = 0x01)]
        PUSHBYTES1 = 0x01,
        /// <summary>
        /// 0x02 The next opcode bytes is data to be pushed onto the stack
        /// </summary>
        [InstructionWithPayload(FixedQuantity = 0x02)]
        PUSHBYTES2 = 0X02,// 0x01-0x4B The next opcode bytes is data to be pushed onto the stack
        [InstructionWithPayload(FixedQuantity = 0x03)]
        PUSHBYTES3 = 0X03,
        [InstructionWithPayload(FixedQuantity = 0x04)]
        PUSHBYTES4 = 0X04,
        [InstructionWithPayload(FixedQuantity = 0x05)]
        PUSHBYTES5 = 0X05,
        [InstructionWithPayload(FixedQuantity = 0x06)]
        PUSHBYTES6 = 0X06,
        [InstructionWithPayload(FixedQuantity = 0x07)]
        PUSHBYTES7 = 0X07,
        [InstructionWithPayload(FixedQuantity = 0x08)]
        PUSHBYTES8 = 0X08,
        [InstructionWithPayload(FixedQuantity = 0x09)]
        PUSHBYTES9 = 0X09,
        [InstructionWithPayload(FixedQuantity = 0x0A)]
        PUSHBYTES10 = 0X0A,
        [InstructionWithPayload(FixedQuantity = 0x0B)]
        PUSHBYTES11 = 0X0B,
        [InstructionWithPayload(FixedQuantity = 0x0C)]
        PUSHBYTES12 = 0X0C,
        [InstructionWithPayload(FixedQuantity = 0x0D)]
        PUSHBYTES13 = 0X0D,
        [InstructionWithPayload(FixedQuantity = 0x0E)]
        PUSHBYTES14 = 0X0E,
        [InstructionWithPayload(FixedQuantity = 0x0F)]
        PUSHBYTES15 = 0X0F,
        [InstructionWithPayload(FixedQuantity = 0x10)]
        PUSHBYTES16 = 0X10,
        [InstructionWithPayload(FixedQuantity = 0x11)]
        PUSHBYTES17 = 0X11,
        [InstructionWithPayload(FixedQuantity = 0x12)]
        PUSHBYTES18 = 0X12,
        [InstructionWithPayload(FixedQuantity = 0x13)]
        PUSHBYTES19 = 0X13,
        [InstructionWithPayload(FixedQuantity = 0x14)]
        PUSHBYTES20 = 0X14,
        [InstructionWithPayload(FixedQuantity = 0x15)]
        PUSHBYTES21 = 0X15,
        [InstructionWithPayload(FixedQuantity = 0x16)]
        PUSHBYTES22 = 0X16,
        [InstructionWithPayload(FixedQuantity = 0x17)]
        PUSHBYTES23 = 0X17,
        [InstructionWithPayload(FixedQuantity = 0x18)]
        PUSHBYTES24 = 0X18,
        [InstructionWithPayload(FixedQuantity = 0x19)]
        PUSHBYTES25 = 0X19,
        [InstructionWithPayload(FixedQuantity = 0x1A)]
        PUSHBYTES26 = 0X1A,
        [InstructionWithPayload(FixedQuantity = 0x1B)]
        PUSHBYTES27 = 0X1B,
        [InstructionWithPayload(FixedQuantity = 0x1C)]
        PUSHBYTES28 = 0X1C,
        [InstructionWithPayload(FixedQuantity = 0x1D)]
        PUSHBYTES29 = 0X1D,
        [InstructionWithPayload(FixedQuantity = 0x1E)]
        PUSHBYTES30 = 0X1E,
        [InstructionWithPayload(FixedQuantity = 0x1F)]
        PUSHBYTES31 = 0X1F,
        [InstructionWithPayload(FixedQuantity = 0x20)]
        PUSHBYTES32 = 0X20,
        [InstructionWithPayload(FixedQuantity = 0x21)]
        PUSHBYTES33 = 0X21,
        [InstructionWithPayload(FixedQuantity = 0x22)]
        PUSHBYTES34 = 0X22,
        [InstructionWithPayload(FixedQuantity = 0x23)]
        PUSHBYTES35 = 0X23,
        [InstructionWithPayload(FixedQuantity = 0x24)]
        PUSHBYTES36 = 0X24,
        [InstructionWithPayload(FixedQuantity = 0x25)]
        PUSHBYTES37 = 0X25,
        [InstructionWithPayload(FixedQuantity = 0x26)]
        PUSHBYTES38 = 0X26,
        [InstructionWithPayload(FixedQuantity = 0x27)]
        PUSHBYTES39 = 0X27,
        [InstructionWithPayload(FixedQuantity = 0x28)]
        PUSHBYTES40 = 0X28,
        [InstructionWithPayload(FixedQuantity = 0x29)]
        PUSHBYTES41 = 0X29,
        [InstructionWithPayload(FixedQuantity = 0x2A)]
        PUSHBYTES42 = 0X2A,
        [InstructionWithPayload(FixedQuantity = 0x2B)]
        PUSHBYTES43 = 0X2B,
        [InstructionWithPayload(FixedQuantity = 0x2C)]
        PUSHBYTES44 = 0X2C,
        [InstructionWithPayload(FixedQuantity = 0x2D)]
        PUSHBYTES45 = 0X2D,
        [InstructionWithPayload(FixedQuantity = 0x2E)]
        PUSHBYTES46 = 0X2E,
        [InstructionWithPayload(FixedQuantity = 0x2F)]
        PUSHBYTES47 = 0X2F,
        [InstructionWithPayload(FixedQuantity = 0x30)]
        PUSHBYTES48 = 0X30,
        [InstructionWithPayload(FixedQuantity = 0x31)]
        PUSHBYTES49 = 0X31,
        [InstructionWithPayload(FixedQuantity = 0x32)]
        PUSHBYTES50 = 0X32,
        [InstructionWithPayload(FixedQuantity = 0x33)]
        PUSHBYTES51 = 0X33,
        [InstructionWithPayload(FixedQuantity = 0x34)]
        PUSHBYTES52 = 0X34,
        [InstructionWithPayload(FixedQuantity = 0x35)]
        PUSHBYTES53 = 0X35,
        [InstructionWithPayload(FixedQuantity = 0x36)]
        PUSHBYTES54 = 0X36,
        [InstructionWithPayload(FixedQuantity = 0x37)]
        PUSHBYTES55 = 0X37,
        [InstructionWithPayload(FixedQuantity = 0x38)]
        PUSHBYTES56 = 0X38,
        [InstructionWithPayload(FixedQuantity = 0x39)]
        PUSHBYTES57 = 0X39,
        [InstructionWithPayload(FixedQuantity = 0x3A)]
        PUSHBYTES58 = 0X3A,
        [InstructionWithPayload(FixedQuantity = 0x3B)]
        PUSHBYTES59 = 0X3B,
        [InstructionWithPayload(FixedQuantity = 0x3C)]
        PUSHBYTES60 = 0X3C,
        [InstructionWithPayload(FixedQuantity = 0x3D)]
        PUSHBYTES61 = 0X3D,
        [InstructionWithPayload(FixedQuantity = 0x3E)]
        PUSHBYTES62 = 0X3E,
        [InstructionWithPayload(FixedQuantity = 0x3F)]
        PUSHBYTES63 = 0X3F,
        [InstructionWithPayload(FixedQuantity = 0x40)]
        PUSHBYTES64 = 0X40,
        [InstructionWithPayload(FixedQuantity = 0x41)]
        PUSHBYTES65 = 0X41,
        [InstructionWithPayload(FixedQuantity = 0x42)]
        PUSHBYTES66 = 0X42,
        [InstructionWithPayload(FixedQuantity = 0x43)]
        PUSHBYTES67 = 0X43,
        [InstructionWithPayload(FixedQuantity = 0x44)]
        PUSHBYTES68 = 0X44,
        [InstructionWithPayload(FixedQuantity = 0x45)]
        PUSHBYTES69 = 0X45,
        [InstructionWithPayload(FixedQuantity = 0x46)]
        PUSHBYTES70 = 0X46,
        [InstructionWithPayload(FixedQuantity = 0x47)]
        PUSHBYTES71 = 0X47,
        [InstructionWithPayload(FixedQuantity = 0x48)]
        PUSHBYTES72 = 0X48,
        [InstructionWithPayload(FixedQuantity = 0x49)]
        PUSHBYTES73 = 0X49,
        [InstructionWithPayload(FixedQuantity = 0x4A)]
        PUSHBYTES74 = 0X4A,
        [InstructionWithPayload(FixedQuantity = 0x4B)]
        PUSHBYTES75 = 0x4B,
        /// <summary>
        /// The next byte contains the number of bytes to be pushed onto the stack.
        /// </summary>
        [InstructionWithPayload(DynamicQuantity = InstructionWithPayloadAttribute.EQuantity.Byte)]
        PUSHDATA1 = 0x4C,
        /// <summary>
        /// The next two bytes contain the number of bytes to be pushed onto the stack.
        /// </summary>
        [InstructionWithPayload(DynamicQuantity = InstructionWithPayloadAttribute.EQuantity.UInt16)]
        PUSHDATA2 = 0x4D,
        /// <summary>
        /// The next four bytes contain the number of bytes to be pushed onto the stack.
        /// </summary>
        [InstructionWithPayload(DynamicQuantity = InstructionWithPayloadAttribute.EQuantity.Int32)]
        PUSHDATA4 = 0x4E,
        /// <summary>
        /// The number -1 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSHM1 = 0x4F,
        /// <summary>
        /// The number 1 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH1 = 0x51,
        /// <summary>
        /// The number 2 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH2 = 0x52,
        /// <summary>
        /// The number 3 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH3 = 0x53,
        /// <summary>
        /// The number 4 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH4 = 0x54,
        /// <summary>
        /// The number 5 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH5 = 0x55,
        /// <summary>
        /// The number 6 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH6 = 0x56,
        /// <summary>
        /// The number 7 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH7 = 0x57,
        /// <summary>
        /// The number 8 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH8 = 0x58,
        /// <summary>
        /// The number 9 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH9 = 0x59,
        /// <summary>
        /// The number 10 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH10 = 0x5A,
        /// <summary>
        /// The number 11 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH11 = 0x5B,
        /// <summary>
        /// The number 12 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH12 = 0x5C,
        /// <summary>
        /// The number 13 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH13 = 0x5D,
        /// <summary>
        /// The number 14 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH14 = 0x5E,
        /// <summary>
        /// The number 15 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH15 = 0x5F,
        /// <summary>
        /// The number 16 is pushed onto the stack.
        /// </summary>
        [Instruction]
        PUSH16 = 0x60,

        // Flow control

        /// <summary>
        /// Does nothing.
        /// </summary>
        [Instruction]
        NOP = 0x61,
        [InstructionWithPayload(FixedQuantity = 2)]
        JMP = 0x62,
        [InstructionWithPayload(FixedQuantity = 2)]
        JMPIF = 0x63,
        [InstructionWithPayload(FixedQuantity = 2)]
        JMPIFNOT = 0x64,
        [InstructionWithPayload(FixedQuantity = 2)]
        CALL = 0x65,
        [Instruction]
        RET = 0x66,
        [InstructionWithPayload(FixedQuantity = 20)]
        APPCALL = 0x67,
        [InstructionWithPayload(DynamicQuantity = InstructionWithPayloadAttribute.EQuantity.String)]
        SYSCALL = 0x68,
        [InstructionWithPayload(FixedQuantity = 20)]
        TAILCALL = 0x69,

        // Stack
        [Instruction]
        DUPFROMALTSTACK = 0x6A,
        [Instruction]
        TOALTSTACK = 0x6B, // Puts the input onto the top of the alt stack. Removes it from the main stack.
        [Instruction]
        FROMALTSTACK = 0x6C, // Puts the input onto the top of the main stack. Removes it from the alt stack.
        [Instruction]
        XDROP = 0x6D,
        [Instruction]
        XSWAP = 0x72,
        [Instruction]
        XTUCK = 0x73,
        [Instruction]
        DEPTH = 0x74, // Puts the number of stack items onto the stack.
        [Instruction]
        DROP = 0x75, // Removes the top stack item.
        [Instruction]
        DUP = 0x76, // Duplicates the top stack item.
        [Instruction]
        NIP = 0x77, // Removes the second-to-top stack item.
        [Instruction]
        OVER = 0x78, // Copies the second-to-top stack item to the top.
        [Instruction]
        PICK = 0x79, // The item n back in the stack is copied to the top.
        [Instruction]
        ROLL = 0x7A, // The item n back in the stack is moved to the top.
        [Instruction]
        ROT = 0x7B, // The top three items on the stack are rotated to the left.
        [Instruction]
        SWAP = 0x7C, // The top two items on the stack are swapped.
        [Instruction]
        TUCK = 0x7D, // The item at the top of the stack is copied and inserted before the second-to-top item.

        // Splice
        [Instruction]
        CAT = 0x7E, // Concatenates two strings.
        [Instruction]
        SUBSTR = 0x7F, // Returns a section of a string.
        [Instruction]
        LEFT = 0x80, // Keeps only characters left of the specified point in a string.
        [Instruction]
        RIGHT = 0x81, // Keeps only characters right of the specified point in a string.
        [Instruction]
        SIZE = 0x82, // Returns the length of the input string.

        // Bitwise logic
        [Instruction]
        INVERT = 0x83, // Flips all of the bits in the input.
        [Instruction]
        AND = 0x84, // Boolean and between each bit in the inputs.
        [Instruction]
        OR = 0x85, // Boolean or between each bit in the inputs.
        [Instruction]
        XOR = 0x86, // Boolean exclusive or between each bit in the inputs.
        [Instruction]
        EQUAL = 0x87, // Returns 1 if the inputs are exactly equal, 0 otherwise.
                      //OP_EQUALVERIFY = 0x88, // Same as OP_EQUAL, but runs OP_VERIFY afterward.
                      //OP_RESERVED1 = 0x89, // Transaction is invalid unless occuring in an unexecuted OP_IF branch
                      //OP_RESERVED2 = 0x8A, // Transaction is invalid unless occuring in an unexecuted OP_IF branch

        // Arithmetic
        // Note: Arithmetic inputs are limited to signed 32-bit integers, but may overflow their output.
        [Instruction]
        INC = 0x8B, // 1 is added to the input.
        [Instruction]
        DEC = 0x8C, // 1 is subtracted from the input.
        [Instruction]
        SIGN = 0x8D,
        [Instruction]
        NEGATE = 0x8F, // The sign of the input is flipped.
        [Instruction]
        ABS = 0x90, // The input is made positive.
        [Instruction]
        NOT = 0x91, // If the input is 0 or 1, it is flipped. Otherwise the output will be 0.
        [Instruction]
        NZ = 0x92, // Returns 0 if the input is 0. 1 otherwise.
        [Instruction]
        ADD = 0x93, // a is added to b.
        [Instruction]
        SUB = 0x94, // b is subtracted from a.
        [Instruction]
        MUL = 0x95, // a is multiplied by b.
        [Instruction]
        DIV = 0x96, // a is divided by b.
        [Instruction]
        MOD = 0x97, // Returns the remainder after dividing a by b.
        [Instruction]
        SHL = 0x98, // Shifts a left b bits, preserving sign.
        [Instruction]
        SHR = 0x99, // Shifts a right b bits, preserving sign.
        [Instruction]
        BOOLAND = 0x9A, // If both a and b are not 0, the output is 1. Otherwise 0.
        [Instruction]
        BOOLOR = 0x9B, // If a or b is not 0, the output is 1. Otherwise 0.
        [Instruction]
        NUMEQUAL = 0x9C, // Returns 1 if the numbers are equal, 0 otherwise.
        [Instruction]
        NUMNOTEQUAL = 0x9E, // Returns 1 if the numbers are not equal, 0 otherwise.
        [Instruction]
        LT = 0x9F, // Returns 1 if a is less than b, 0 otherwise.
        [Instruction]
        GT = 0xA0, // Returns 1 if a is greater than b, 0 otherwise.
        [Instruction]
        LTE = 0xA1, // Returns 1 if a is less than or equal to b, 0 otherwise.
        [Instruction]
        GTE = 0xA2, // Returns 1 if a is greater than or equal to b, 0 otherwise.
        [Instruction]
        MIN = 0xA3, // Returns the smaller of a and b.
        [Instruction]
        MAX = 0xA4, // Returns the larger of a and b.
        [Instruction]
        WITHIN = 0xA5, // Returns 1 if x is within the specified range (left-inclusive), 0 otherwise.

        // Crypto
        //RIPEMD160 = 0xA6, // The input is hashed using RIPEMD-160.
        [Instruction]
        SHA1 = 0xA7, // The input is hashed using SHA-1.
        [Instruction]
        SHA256 = 0xA8, // The input is hashed using SHA-256.
        [Instruction]
        HASH160 = 0xA9,
        [Instruction]
        HASH256 = 0xAA,
        [Instruction]
        CHECKSIG = 0xAC,
        [Instruction]
        VERIFY = 0xAD,
        [Instruction]
        CHECKMULTISIG = 0xAE,

        // Array
        [Instruction]
        ARRAYSIZE = 0xC0,
        [Instruction]
        PACK = 0xC1,
        [Instruction]
        UNPACK = 0xC2,
        [Instruction]
        PICKITEM = 0xC3,
        [Instruction]
        SETITEM = 0xC4,
        [Instruction]
        NEWARRAY = 0xC5, //用作引用類型
        [Instruction]
        NEWSTRUCT = 0xC6, //用作值類型
        [Instruction]
        NEWMAP = 0xC7,
        [Instruction]
        APPEND = 0xC8,
        [Instruction]
        REVERSE = 0xC9,
        [Instruction]
        REMOVE = 0xCA,
        [Instruction]
        HASKEY = 0xCB,
        [Instruction]
        KEYS = 0xCC,
        [Instruction]
        VALUES = 0xCD,

        // Stack isolation

        /// <summary>
        /// The instruction CALL_I is very similar to the old instruction CALL. The difference is that CALL_I requires an operand behind the instruction for representing the number of parameters and return values to copy.
        /// </summary>
        [InstructionWithPayload(FixedQuantity = 2)]
        CALL_I = 0xE0,
        /// <summary>
        /// The instruction CALL_E is very similar to the old instruction APPCALL for static invocations. The difference is that CALL_E requires an operand behind the instruction for representing the number of parameters and return values to copy.
        /// </summary>
        [InstructionWithPayload(FixedQuantity = 22)]
        CALL_E = 0xE1,
        /// <summary>
        /// The instruction CALL_ED is very similar to the old instruction APPCALL for dynamic invocations. The difference is that CALL_ED requires an operand behind the instruction for representing the number of parameters and return values to copy.
        /// </summary>
        [InstructionWithPayload(FixedQuantity = 2)]
        CALL_ED = 0xE2,
        /// <summary>
        /// The instruction CALL_ET is very similar to the instruction CALL_E. The difference is that CALL_ET will start a tail call.
        /// </summary>
        [InstructionWithPayload(FixedQuantity = 22)]
        CALL_ET = 0xE3,
        /// <summary>
        /// The instruction CALL_EDT is very similar to the instruction CALL_ED. The difference is that CALL_EDT will start a tail call.
        /// </summary>
        [InstructionWithPayload(FixedQuantity = 2)]
        CALL_EDT = 0xE4,

        // Exceptions
        [Instruction]
        THROW = 0xF0,
        [Instruction]
        THROWIFNOT = 0xF1
    }
}