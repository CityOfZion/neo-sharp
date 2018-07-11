namespace NeoSharp.VM
{
    public enum EVMOpCode : byte
    {
        // Constants

        /// <summary>
        /// An empty array of bytes is pushed onto the stack.
        /// </summary>
        PUSH0 = 0x00,
        /// <summary>
        /// 0x01 The next opcode bytes is data to be pushed onto the stack
        /// </summary>
        PUSHBYTES1 = 0x01,
        /// <summary>
        /// 0x02 The next opcode bytes is data to be pushed onto the stack
        /// </summary>
        PUSHBYTES2 = 0X02,// 0x01-0x4B The next opcode bytes is data to be pushed onto the stack
        PUSHBYTES3 = 0X03,
        PUSHBYTES4 = 0X04,
        PUSHBYTES5 = 0X05,
        PUSHBYTES6 = 0X06,
        PUSHBYTES7 = 0X07,
        PUSHBYTES8 = 0X08,
        PUSHBYTES9 = 0X09,
        PUSHBYTES10 = 0X0A,
        PUSHBYTES11 = 0X0B,
        PUSHBYTES12 = 0X0C,
        PUSHBYTES13 = 0X0D,
        PUSHBYTES14 = 0X0E,
        PUSHBYTES15 = 0X0F,
        PUSHBYTES16 = 0X10,
        PUSHBYTES17 = 0X11,
        PUSHBYTES18 = 0X12,
        PUSHBYTES19 = 0X13,
        PUSHBYTES20 = 0X14,
        PUSHBYTES21 = 0X15,
        PUSHBYTES22 = 0X16,
        PUSHBYTES23 = 0X17,
        PUSHBYTES24 = 0X18,
        PUSHBYTES25 = 0X19,
        PUSHBYTES26 = 0X1A,
        PUSHBYTES27 = 0X1B,
        PUSHBYTES28 = 0X1C,
        PUSHBYTES29 = 0X1D,
        PUSHBYTES30 = 0X1E,
        PUSHBYTES31 = 0X1F,
        PUSHBYTES32 = 0X20,
        PUSHBYTES33 = 0X21,
        PUSHBYTES34 = 0X22,
        PUSHBYTES35 = 0X23,
        PUSHBYTES36 = 0X24,
        PUSHBYTES37 = 0X25,
        PUSHBYTES38 = 0X26,
        PUSHBYTES39 = 0X27,
        PUSHBYTES40 = 0X28,
        PUSHBYTES41 = 0X29,
        PUSHBYTES42 = 0X2A,
        PUSHBYTES43 = 0X2B,
        PUSHBYTES44 = 0X2C,
        PUSHBYTES45 = 0X2D,
        PUSHBYTES46 = 0X2E,
        PUSHBYTES47 = 0X2F,
        PUSHBYTES48 = 0X30,
        PUSHBYTES49 = 0X31,
        PUSHBYTES50 = 0X32,
        PUSHBYTES51 = 0X33,
        PUSHBYTES52 = 0X34,
        PUSHBYTES53 = 0X35,
        PUSHBYTES54 = 0X36,
        PUSHBYTES55 = 0X37,
        PUSHBYTES56 = 0X38,
        PUSHBYTES57 = 0X39,
        PUSHBYTES58 = 0X3A,
        PUSHBYTES59 = 0X3B,
        PUSHBYTES60 = 0X3C,
        PUSHBYTES61 = 0X3D,
        PUSHBYTES62 = 0X3E,
        PUSHBYTES63 = 0X3F,
        PUSHBYTES64 = 0X40,
        PUSHBYTES65 = 0X41,
        PUSHBYTES66 = 0X42,
        PUSHBYTES67 = 0X43,
        PUSHBYTES68 = 0X44,
        PUSHBYTES69 = 0X45,
        PUSHBYTES70 = 0X46,
        PUSHBYTES71 = 0X47,
        PUSHBYTES72 = 0X48,
        PUSHBYTES73 = 0X49,
        PUSHBYTES74 = 0X4A,
        PUSHBYTES75 = 0x4B,
        /// <summary>
        /// The next byte contains the number of bytes to be pushed onto the stack.
        /// </summary>
        PUSHDATA1 = 0x4C,
        /// <summary>
        /// The next two bytes contain the number of bytes to be pushed onto the stack.
        /// </summary>
        PUSHDATA2 = 0x4D,
        /// <summary>
        /// The next four bytes contain the number of bytes to be pushed onto the stack.
        /// </summary>
        PUSHDATA4 = 0x4E,
        /// <summary>
        /// The number -1 is pushed onto the stack.
        /// </summary>
        PUSHM1 = 0x4F,
        /// <summary>
        /// The number 1 is pushed onto the stack.
        /// </summary>
        PUSH1 = 0x51,
        /// <summary>
        /// The number 2 is pushed onto the stack.
        /// </summary>
        PUSH2 = 0x52,
        /// <summary>
        /// The number 3 is pushed onto the stack.
        /// </summary>
        PUSH3 = 0x53,
        /// <summary>
        /// The number 4 is pushed onto the stack.
        /// </summary>
        PUSH4 = 0x54,
        /// <summary>
        /// The number 5 is pushed onto the stack.
        /// </summary>
        PUSH5 = 0x55,
        /// <summary>
        /// The number 6 is pushed onto the stack.
        /// </summary>
        PUSH6 = 0x56,
        /// <summary>
        /// The number 7 is pushed onto the stack.
        /// </summary>
        PUSH7 = 0x57,
        /// <summary>
        /// The number 8 is pushed onto the stack.
        /// </summary>
        PUSH8 = 0x58,
        /// <summary>
        /// The number 9 is pushed onto the stack.
        /// </summary>
        PUSH9 = 0x59,
        /// <summary>
        /// The number 10 is pushed onto the stack.
        /// </summary>
        PUSH10 = 0x5A,
        /// <summary>
        /// The number 11 is pushed onto the stack.
        /// </summary>
        PUSH11 = 0x5B,
        /// <summary>
        /// The number 12 is pushed onto the stack.
        /// </summary>
        PUSH12 = 0x5C,
        /// <summary>
        /// The number 13 is pushed onto the stack.
        /// </summary>
        PUSH13 = 0x5D,
        /// <summary>
        /// The number 14 is pushed onto the stack.
        /// </summary>
        PUSH14 = 0x5E,
        /// <summary>
        /// The number 15 is pushed onto the stack.
        /// </summary>
        PUSH15 = 0x5F,
        /// <summary>
        /// The number 16 is pushed onto the stack.
        /// </summary>
        PUSH16 = 0x60,

        // Flow control

        /// <summary>
        /// Does nothing.
        /// </summary>
        NOP = 0x61,
        JMP = 0x62,
        JMPIF = 0x63,
        JMPIFNOT = 0x64,
        CALL = 0x65,
        RET = 0x66,
        APPCALL = 0x67,
        SYSCALL = 0x68,
        TAILCALL = 0x69,

        // Stack
        DUPFROMALTSTACK = 0x6A,
        TOALTSTACK = 0x6B, // Puts the input onto the top of the alt stack. Removes it from the main stack.
        FROMALTSTACK = 0x6C, // Puts the input onto the top of the main stack. Removes it from the alt stack.
        XDROP = 0x6D,
        XSWAP = 0x72,
        XTUCK = 0x73,
        DEPTH = 0x74, // Puts the number of stack items onto the stack.
        DROP = 0x75, // Removes the top stack item.
        DUP = 0x76, // Duplicates the top stack item.
        NIP = 0x77, // Removes the second-to-top stack item.
        OVER = 0x78, // Copies the second-to-top stack item to the top.
        PICK = 0x79, // The item n back in the stack is copied to the top.
        ROLL = 0x7A, // The item n back in the stack is moved to the top.
        ROT = 0x7B, // The top three items on the stack are rotated to the left.
        SWAP = 0x7C, // The top two items on the stack are swapped.
        TUCK = 0x7D, // The item at the top of the stack is copied and inserted before the second-to-top item.

        // Splice
        CAT = 0x7E, // Concatenates two strings.
        SUBSTR = 0x7F, // Returns a section of a string.
        LEFT = 0x80, // Keeps only characters left of the specified point in a string.
        RIGHT = 0x81, // Keeps only characters right of the specified point in a string.
        SIZE = 0x82, // Returns the length of the input string.

        // Bitwise logic
        INVERT = 0x83, // Flips all of the bits in the input.
        AND = 0x84, // Boolean and between each bit in the inputs.
        OR = 0x85, // Boolean or between each bit in the inputs.
        XOR = 0x86, // Boolean exclusive or between each bit in the inputs.
        EQUAL = 0x87, // Returns 1 if the inputs are exactly equal, 0 otherwise.
                      //OP_EQUALVERIFY = 0x88, // Same as OP_EQUAL, but runs OP_VERIFY afterward.
                      //OP_RESERVED1 = 0x89, // Transaction is invalid unless occuring in an unexecuted OP_IF branch
                      //OP_RESERVED2 = 0x8A, // Transaction is invalid unless occuring in an unexecuted OP_IF branch

        // Arithmetic
        // Note: Arithmetic inputs are limited to signed 32-bit integers, but may overflow their output.
        INC = 0x8B, // 1 is added to the input.
        DEC = 0x8C, // 1 is subtracted from the input.
        SIGN = 0x8D,
        NEGATE = 0x8F, // The sign of the input is flipped.
        ABS = 0x90, // The input is made positive.
        NOT = 0x91, // If the input is 0 or 1, it is flipped. Otherwise the output will be 0.
        NZ = 0x92, // Returns 0 if the input is 0. 1 otherwise.
        ADD = 0x93, // a is added to b.
        SUB = 0x94, // b is subtracted from a.
        MUL = 0x95, // a is multiplied by b.
        DIV = 0x96, // a is divided by b.
        MOD = 0x97, // Returns the remainder after dividing a by b.
        SHL = 0x98, // Shifts a left b bits, preserving sign.
        SHR = 0x99, // Shifts a right b bits, preserving sign.
        BOOLAND = 0x9A, // If both a and b are not 0, the output is 1. Otherwise 0.
        BOOLOR = 0x9B, // If a or b is not 0, the output is 1. Otherwise 0.
        NUMEQUAL = 0x9C, // Returns 1 if the numbers are equal, 0 otherwise.
        NUMNOTEQUAL = 0x9E, // Returns 1 if the numbers are not equal, 0 otherwise.
        LT = 0x9F, // Returns 1 if a is less than b, 0 otherwise.
        GT = 0xA0, // Returns 1 if a is greater than b, 0 otherwise.
        LTE = 0xA1, // Returns 1 if a is less than or equal to b, 0 otherwise.
        GTE = 0xA2, // Returns 1 if a is greater than or equal to b, 0 otherwise.
        MIN = 0xA3, // Returns the smaller of a and b.
        MAX = 0xA4, // Returns the larger of a and b.
        WITHIN = 0xA5, // Returns 1 if x is within the specified range (left-inclusive), 0 otherwise.

        // Crypto
        //RIPEMD160 = 0xA6, // The input is hashed using RIPEMD-160.
        SHA1 = 0xA7, // The input is hashed using SHA-1.
        SHA256 = 0xA8, // The input is hashed using SHA-256.
        HASH160 = 0xA9,
        HASH256 = 0xAA,
        CHECKSIG = 0xAC,
        VERIFY = 0xAD,
        CHECKMULTISIG = 0xAE,

        // Array
        ARRAYSIZE = 0xC0,
        PACK = 0xC1,
        UNPACK = 0xC2,
        PICKITEM = 0xC3,
        SETITEM = 0xC4,
        NEWARRAY = 0xC5, //用作引用類型
        NEWSTRUCT = 0xC6, //用作值類型
        NEWMAP = 0xC7,
        APPEND = 0xC8,
        REVERSE = 0xC9,
        REMOVE = 0xCA,
        HASKEY = 0xCB,
        KEYS = 0xCC,
        VALUES = 0xCD,

        // Stack isolation

        /// <summary>
        /// The instruction CALL_I is very similar to the old instruction CALL. The difference is that CALL_I requires an operand behind the instruction for representing the number of parameters and return values to copy.
        /// </summary>
        CALL_I = 0xE0,
        /// <summary>
        /// The instruction CALL_E is very similar to the old instruction APPCALL for static invocations. The difference is that CALL_E requires an operand behind the instruction for representing the number of parameters and return values to copy.
        /// </summary>
        CALL_E = 0xE1,
        /// <summary>
        /// The instruction CALL_ED is very similar to the old instruction APPCALL for dynamic invocations. The difference is that CALL_ED requires an operand behind the instruction for representing the number of parameters and return values to copy.
        /// </summary>
        CALL_ED = 0xE2,
        /// <summary>
        /// The instruction CALL_ET is very similar to the instruction CALL_E. The difference is that CALL_ET will start a tail call.
        /// </summary>
        CALL_ET = 0xE3,
        /// <summary>
        /// The instruction CALL_EDT is very similar to the instruction CALL_ED. The difference is that CALL_EDT will start a tail call.
        /// </summary>
        CALL_EDT = 0xE4,

        // Exceptions
        THROW = 0xF0,
        THROWIFNOT = 0xF1
    }
}