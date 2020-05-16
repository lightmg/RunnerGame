using System;
using System.IO;

namespace Game.Helpers
{
    public static class PathHelpers
    {
        public static string RootPath =>
            Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)!, @"..");
    }
}