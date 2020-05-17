namespace DevOpsMetrics.Service
{
    public static class Utility
    {
        public static string EncodePartitionKey(string url)
        {
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(url);
            var base64 = System.Convert.ToBase64String(keyBytes);
            return base64.Replace('/', '_');
        }

        public static string DecodePartitionKey(string encodedKey)
        {
            var base64 = encodedKey.Replace('_', '/');
            byte[] bytes = System.Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
