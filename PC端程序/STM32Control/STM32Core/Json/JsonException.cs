using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Core
{
    /// <summary> 
    /// dyJson错误处理
    /// </summary> 
    public class JsonException : Exception
    {
        /// <summary> 
        /// dyJson错误处理
        /// </summary> 
        public JsonException()
            : base()
        {
        }

        internal JsonException(ParserToken token) :
            base(String.Format(
                   "Invalid token '{0}' in input string", token))
        {
        }

        internal JsonException(ParserToken token,
                                Exception inner_exception) :
            base(String.Format(
                    "Invalid token '{0}' in input string", token),
                inner_exception)
        {
        }

        internal JsonException(int c) :
            base(String.Format(
                   "Invalid character '{0}' in input string", (char)c))
        {
        }

        internal JsonException(int c, Exception inner_exception) :
            base(String.Format(
                   "Invalid character '{0}' in input string", (char)c),
               inner_exception)
        {
        }

        /// <summary> 
        /// dyJson错误处理
        /// </summary> 
        public JsonException(string message)
            : base(message)
        {
        }
        /// <summary> 
        /// dyJson错误处理
        /// </summary> 
        public JsonException(string message, Exception inner_exception) :
            base(message, inner_exception)
        {
        }
    }
}
