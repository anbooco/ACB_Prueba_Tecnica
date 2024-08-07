namespace ACB_Prueba_Tecnica.Utils
{
    public static class TokenStore
    {
        private static List<string> validTokens = new List<string>();

        public static void AddToken(string token)
        {
            validTokens.Add(token);
        }

        public static bool IsValidToken(string token)
        {
            return validTokens.Contains(token);
        }
    }

}
