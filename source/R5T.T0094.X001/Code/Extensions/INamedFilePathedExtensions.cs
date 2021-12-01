using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0094;


namespace System
{
    public static class INamedFilePathedExtensions
    {
        public static Dictionary<string, List<string>> ToFilePathsByName(this IEnumerable<IGrouping<string, INamedFilePathed>> namedFilePathedGroups)
        {
            var output = namedFilePathedGroups
                .ToDictionary(
                    xGroup => xGroup.Key,
                    xGroup => xGroup
                        .Select(xNamedFilePathed => xNamedFilePathed.FilePath)
                        .ToList());

            return output;
        }

        public static Dictionary<string, List<string>> GetFilePathsByNameForDuplicateNames(this IEnumerable<INamedFilePathed> namedFilePatheds)
        {
            var output = namedFilePatheds
                .WhereDuplicateNames()
                .ToFilePathsByName();

            return output;
        }

        public static string GetNamedFilePathedInformation(this INamedFilePathed namedFilePathed)
        {
            var output = $"{namedFilePathed.Name}: {namedFilePathed.FilePath}";
            return output;
        }

        public static void VerifyDistinctByNamedFilePathedData<T>(this IEnumerable<T> extensionMethodBases)
            where T : INamedFilePathed
        {
            extensionMethodBases.Cast<INamedFilePathed>().VerifyDistinct(NamedFilePathedEqualityComparer.Instance);
        }
    }
}
