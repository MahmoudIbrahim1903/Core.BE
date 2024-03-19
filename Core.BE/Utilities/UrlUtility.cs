
namespace Emeint.Core.BE.Utilities
{
    public static class UrlUtility
    {
        public static bool IsUrl(string text)
        {
            if (text != null && text.StartsWith("http"))
            {
                return true;
            }
            return false;
        }

        public static bool IsFacebookProfileUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (IsUrl(url))
                {
                    if (url.ToLower().Contains("facebook"))
                    {
                        return true;
                    }

                }
            }
            return false;
        }

        public static bool IsGoogleProfileUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (IsUrl(url))
                {
                    if (url.ToLower().Contains("google"))
                    {
                        return true;
                    }

                }
            }
            return false;
        }
    }
}