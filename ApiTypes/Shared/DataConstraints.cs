namespace ApiTypes.Shared
{
    public static class DataConstraints
    {
        public static bool IsLoginLegal(string login)
        {
            return login.Length < 128 && login.Length > 3;
        }

        public static bool IsPasswordLegal(string password)
        {
            return password.Length < 128 && password.Length > 6;
        }

        public static bool IsNameLegal(string name)
        {
            return name.Length < 128 && name.Length > 3;
        }

        public static bool IsMessageLegal(string message)
        {
            return message.Length < 512 && message.Length > 0;
        }
    }
}
