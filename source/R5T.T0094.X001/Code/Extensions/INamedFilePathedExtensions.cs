using System;
using System.Collections.Generic;
using System.Linq;

using R5T.Magyar;

using R5T.T0094;

using Instances = R5T.T0094.X001.Instances;


namespace System
{
    public static class INamedFilePathedExtensions
    {
        public static string AsNamedFilePathedRepresentation(this INamedFilePathed namedFilePathed)
        {
            var output = $"{namedFilePathed.Name}:\n\t{namedFilePathed.FilePath}";
            return output;
        }

        public static T[] FindArrayByNameAndFilePath<T>(this IEnumerable<T> namedFiledPatheds,
            string name, string filePath)
            where T : INamedFilePathed
        {
            var output = namedFiledPatheds.FindArray(Instances.Predicate.NameAndFilePathIs<T>(name, filePath));
            return output;
        }

        public static WasFound<T> FindSingleByNameAndFilePath<T>(this IEnumerable<T> namedFiledPatheds,
            string name, string filePath)
            where T : INamedFilePathed
        {
            var output = namedFiledPatheds.FindSingle(Instances.Predicate.NameAndFilePathIs<T>(name, filePath));
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

        public static void VerifyDistinctByNamedFilePathedData<T>(this IEnumerable<T> extensionMethodBases)
            where T : INamedFilePathed
        {
            extensionMethodBases.Cast<INamedFilePathed>().VerifyDistinct(NamedFilePathedEqualityComparer.Instance);
        }
    }
}
