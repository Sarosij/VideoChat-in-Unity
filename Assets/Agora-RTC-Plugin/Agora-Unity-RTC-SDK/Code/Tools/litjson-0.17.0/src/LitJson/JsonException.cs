#region Header
/**
 * JsonException.cs
 *   Base class throwed by Agora.Rtc.LitJson when a parsing error occurs.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion
#define AGORA_RTC


using System;

#if AGORA_RTC
namespace Agora.Rtc.LitJson
#elif AGORA_RTM
namespace Agora.Rtm.LitJson
#endif
{
    public class JsonException :
#if NETSTANDARD1_5
        Exception
#else
        ApplicationException
#endif
    {
        public JsonException() : base()
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


        public JsonException(string message) : base(message)
        {
        }

        public JsonException(string message, Exception inner_exception) :
            base(message, inner_exception)
        {
        }
    }
}