using System;

using R5T.T0060;
using R5T.T0094;


namespace System
{
    public static class IPredicateExtensions
    {
        public static Func<T, bool> NameAndFilePathIs<T>(this IPredicate _,
            string name, string filePath)
            where T : INamedFilePathed
        {
            return namedFilePathed => namedFilePathed.Name == name && namedFilePathed.FilePath == filePath;
        }
    }
}
