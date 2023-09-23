using System.Text.RegularExpressions;

namespace ApiTypes.Shared
{
    public static class DataConstraints
    {
        public static bool IsSearchQueryValid(string request)
        {
            if (string.IsNullOrEmpty(request) || request.Length < 3)
                return false;
            return true;
        }
        public static bool IsLoginLegal(string login)
        {
            return Regex.IsMatch(login, "^[a-zA-Z0-9]{3,128}$"); ;
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
