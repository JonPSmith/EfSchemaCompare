#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: TestFileHelpers.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.IO;

namespace Tests.Helpers
{
    internal static class TestFileHelpers
    {
        private const string TestFileDirectoryName = @"\TestData";

        //-------------------------------------------------------------------

        internal static string GetTestFileFilePath(string searchPattern)
        {
            string[] fileList = GetTestFileFilesOfGivenName(searchPattern);

            if (fileList.Length != 1)
                throw new Exception(string.Format("GetTestFileFilePath: The searchString {0} found {1} file. Either not there or ambiguous",
                    searchPattern, fileList.Length));

            return fileList[0];
        }

        internal static string GetTestFileContent(string searchPattern)
        {
            var filePath = GetTestFileFilePath(searchPattern);
            return File.ReadAllText(filePath);
        }

        internal static bool TestFileDeleteIfPresent(string searchPattern)
        {
            var fileList = GetTestFileFilesOfGivenName(searchPattern);
            if (fileList.Length == 0) return false;
            if (fileList.Length != 1)
                throw new Exception(string.Format("TestFileDeleteIfPresent: The searchString {0} found {1} files!",
                    searchPattern, fileList.Length));

            File.Delete(fileList[0]);
            return true;
        }

        internal static void DeleteDirectoryAndAnyContent(string topDir)
        {
            if (!Directory.Exists(topDir)) return;
            Directory.Delete(topDir, true);
        }

        /// <summary>
        /// This deletes all files and directories (and subdirectories) in the given topDir.
        /// It does NOT delete the topDir directory
        /// </summary>
        /// <param name="topDir"></param>
        internal static void DeleteAllFilesAndSubDirsInDir(string topDir)
        {
            if (!Directory.Exists(topDir)) return;

            var files = Directory.GetFiles(topDir);
            foreach (var file in files)
                File.Delete(file);
            var dirs = Directory.GetDirectories(topDir);
            foreach (var dir in dirs)
                Directory.Delete(dir, true);
        }

        internal static string[] GetTestFileFilesOfGivenName(string searchPattern = "")
        {
            var directory = GetTestDataFileDirectory();
            if (searchPattern.Contains(@"\"))
            {
                //Has subdirectory in search pattern, so change directory
                directory = Path.Combine(directory, searchPattern.Substring(0, searchPattern.LastIndexOf('\\')));
                searchPattern = searchPattern.Substring(searchPattern.LastIndexOf('\\')+1);
            }

            string[] fileList = Directory.GetFiles(directory, searchPattern);

            return fileList;
        }


        //------------------------------------------------------------------------------

        public static string GetTestDataFileDirectory(string alternateTestDir = TestFileDirectoryName)
        {
            string pathToManipulate = Environment.CurrentDirectory;
            const string debugEnding = @"\bin\debug";
            const string releaseEnding = @"\bin\release";

            if (pathToManipulate.EndsWith(debugEnding, StringComparison.InvariantCultureIgnoreCase))
                return pathToManipulate.Substring(0, pathToManipulate.Length - debugEnding.Length) + alternateTestDir;
            if (pathToManipulate.EndsWith(releaseEnding, StringComparison.InvariantCultureIgnoreCase))
                return pathToManipulate.Substring(0, pathToManipulate.Length - releaseEnding.Length) + alternateTestDir;   
                
            throw new Exception("bad news guys. Not the expected path");

        }

        public static string GetSolutionDirectory()
        {
            string pathToManipulate = Environment.CurrentDirectory;
            const string debugEnding = @"\bin\debug";
            const string releaseEnding = @"\bin\release";

            string projectDir = null;
            if (pathToManipulate.EndsWith(debugEnding, StringComparison.InvariantCultureIgnoreCase))
                projectDir = pathToManipulate.Substring(0, pathToManipulate.Length - debugEnding.Length);
            if (pathToManipulate.EndsWith(releaseEnding, StringComparison.InvariantCultureIgnoreCase))
                projectDir = pathToManipulate.Substring(0, pathToManipulate.Length - releaseEnding.Length);

            if (projectDir == null)
                throw new Exception("bad news guys. Not the expected path");

            return projectDir.Substring(0, projectDir.LastIndexOf("\\", StringComparison.Ordinal));

        }

    }
}
