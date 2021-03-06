﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Build
{
    partial class Program
    {
        static string ProPackagesFolder => Path.Combine(Root, "pro-packages");
        static string ProPackagesTempZipDir => Path.Combine(TemporaryFilesRoot, "pro-packages-zip");

        static bool IsProPackage(string packageId)
        {
            return packageId.StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase) &&
                (packageId.StartsWith("Serenity.Pro", StringComparison.OrdinalIgnoreCase) ||
                 packageId.Contains("Advanced", StringComparison.OrdinalIgnoreCase) ||
                 packageId.Contains("Premium", StringComparison.OrdinalIgnoreCase));
        }

        static void CopyProPackagesToTempZipDir(List<Tuple<string, string>> packages)
        {
            CleanDirectory(ProPackagesTempZipDir, ensure: true);

            foreach (var pair in packages)
            {
                if (IsProPackage(pair.Item1))
                {
                    var idLower = pair.Item1.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                    var sourceFolder = Path.Combine(Path.Combine(ProPackagesFolder, idLower), pair.Item2);
                    var targetFolder = Path.Combine(Path.Combine(ProPackagesTempZipDir, idLower), pair.Item2);
                    
                    Directory.CreateDirectory(targetFolder);

                    foreach (var file in Directory.GetFiles(sourceFolder))
                    {
                        File.Copy(file, Path.Combine(targetFolder, Path.GetFileName(file)));
                    }
                }
            }
        }

        static void UpdateProPackages()
        {
            if (!HasProPackages)
                return;

            var packages = ParsePackages(ProjectFile);
            foreach (var package in packages)
            {
                if (IsProPackage(package.Item1))
                {
                    var proVer = GetLatestVersionOf(package.Item1);
                    if (proVer != null)
                        PatchPackageVersion(package.Item1, proVer.ToString());
                }
            }
        }
    }
}
