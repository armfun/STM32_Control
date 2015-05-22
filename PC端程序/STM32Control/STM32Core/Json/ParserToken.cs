using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Core
{
    internal enum ParserToken
    {
        // Lexer tokens
        None = System.Char.MaxValue + 1,
        Number,
        True,
        False,
        Null,
        CharSeq,
        // Single char
        Char,

        // Parser Rules
        Text,
        Object,
        ObjectPrime,
        Pair,
        PairRest,
        Array,
        ArrayPrime,
        Value,
        ValueRest,
        String,

        // End of input
        End,

        // The empty rule
        Epsilon
    }
}
