namespace DevOpsMetrics.Service
{
    public static class Utility
    {
        public static string EncodePartitionKey(string text)
        {
            text = text.Replace("/", "_");

            //The forward slash(/) character
            //The backslash(\) character
            //The number sign(#) character
            //The question mark (?) character

            //Control characters from U+0000 to U+001F, including:
            //The horizontal tab(\t) character
            //The linefeed(\n) character
            //The carriage return (\r) character
            //Control characters from U + 007F to U+009F

            return text.Replace("/", "_");
        }

        public static string DecodePartitionKey(string text)
        {
            return text.Replace("_", "/");
        }
    }
}
