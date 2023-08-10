using System.Reflection;

namespace EnexConverter.Tests;

internal static class PathHelpers
{
    public static string? GetExecutingPath()
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}