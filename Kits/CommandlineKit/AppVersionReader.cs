using System;
using System.Reflection;

namespace CommandlineKit;

public static class AppVersionReader
{
    private static string GetVersionFromGit()
    {
        var version = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        return version ?? "0.0.0.1";
    }

    public static string? GitCommitHash
    {
        get
        {
            var informationalVersion = GetVersionFromGit();

            if (informationalVersion.Contains('+'))
            {
                return informationalVersion[(informationalVersion.IndexOf('+') + 1)..];
            }

            return informationalVersion;
        }
    }

    public static string GetVersionFromGitNoComitHash
    {
        get
        {
            var informationalVersion = GetVersionFromGit();

            if (informationalVersion.Contains('+'))
            {
                return informationalVersion[..(informationalVersion.IndexOf('+'))];
            }

            return informationalVersion; // Or handle as appropriate if the hash is not present
        }
    }
}