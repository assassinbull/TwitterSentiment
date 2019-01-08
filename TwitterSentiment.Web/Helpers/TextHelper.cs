using System.Text;

namespace TwitterSentiment.Web.Helpers
{
    public class TextHelper
    {
        public static readonly Encoding Utf8Encoder = Encoding.GetEncoding(
                                                            "UTF-8",
                                                            new EncoderReplacementFallback(string.Empty),
                                                            new DecoderExceptionFallback()
                                                        );
    }
}