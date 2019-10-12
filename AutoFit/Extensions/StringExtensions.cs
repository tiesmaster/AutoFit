namespace AutoFit
{
    public static class StringExtensions
    {
        public static string Capitalize(this string source)
        {
            return source.Length < 2 ? source : char.ToUpper(source[0]) + source.Substring(1);
        }
    }
}