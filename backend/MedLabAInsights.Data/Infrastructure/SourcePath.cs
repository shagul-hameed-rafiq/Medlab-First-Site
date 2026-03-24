namespace MedLabAInsights.Data.Infrastructure
{
    public static class SourcePath
    {
        public static string GetFilePath(string fileName)
        {
            var normalized = fileName.Replace('\\', Path.DirectorySeparatorChar)
                                     .Replace('/', Path.DirectorySeparatorChar);

            var publishedPath = Path.Combine(AppContext.BaseDirectory, normalized);
            if (File.Exists(publishedPath))
            {
                return publishedPath;
            }

            // Local development fallback: AppContext.BaseDirectory -> .../bin/Debug/net8.0/
            var projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            return Path.Combine(projectDir, "MedLabAInsights.Data", normalized);
        }
    }
}
