using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NeoSharp.Application.Attributes;

namespace NeoSharp.Application.Providers
{
    public class FileAutoCompleteAttribute : AutoCompleteAttribute
    {
        private readonly bool _withFiles;

        public static bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;

        public FileAutoCompleteAttribute(bool withFiles) { _withFiles = withFiles; }

        public override IEnumerable<string> GetParameterValues(ParameterInfo parameter, string currentValue)
        {
            if (string.IsNullOrEmpty(currentValue))
            {
                try
                {
                    return DriveValues();
                }
                catch { }
            }
            else
            {
                // Files and folders

                if (IsWindows && currentValue.Length < 2)
                {
                    // Windows autocomplete when you write (c) or (c:)

                    currentValue = DriveValues()
                        .Where(u => u.StartsWith(currentValue, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();
                }

                var ret = new List<string>();

                try { ret.AddRange(DirValues(currentValue)); } catch { }

                if (_withFiles)
                {
                    try { ret.AddRange(FileValues(currentValue)); } catch { }
                }

                return ret.ToArray();
            }

            return new string[] { };
        }

        private IEnumerable<string> DirValues(string currentValue)
        {
            if (Directory.Exists(currentValue))
            {
                foreach (var dir in Directory.GetDirectories(currentValue)) yield return dir;
            }
            else
            {
                var dirName = Path.GetDirectoryName(currentValue);
                var comparer = IsWindows ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

                if (!string.IsNullOrEmpty(dirName) && Directory.Exists(dirName))
                {
                    foreach (var dir in Directory.GetDirectories(dirName))
                    {
                        if (!dir.StartsWith(currentValue, comparer)) continue;

                        foreach (var d in Directory.GetDirectories(dir)) yield return d;

                        yield return dir;
                    }
                }
            }
        }

        private IEnumerable<string> FileValues(string currentValue)
        {
            if (Directory.Exists(currentValue))
            {
                foreach (var dir in Directory.GetFiles(currentValue)) yield return dir;
            }
            else
            {
                var dirName = Path.GetDirectoryName(currentValue);
                var comparer = Environment.OSVersion.Platform == PlatformID.Win32NT ?
                    StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

                if (!string.IsNullOrEmpty(dirName) && Directory.Exists(dirName))
                {
                    foreach (var dir in Directory.GetFiles(dirName))
                    {
                        if (!dir.StartsWith(currentValue, comparer)) continue;

                        yield return dir;
                    }

                    foreach (var dir in Directory.GetDirectories(dirName))
                    {
                        if (!dir.StartsWith(currentValue, comparer)) continue;

                        foreach (var d in Directory.GetFiles(dir)) yield return d;
                    }
                }
            }
        }

        private IEnumerable<string> DriveValues()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                yield return drive.RootDirectory.FullName;
            }
        }
    }
}