using System;
using System.Collections.Generic;
using System.Linq;

using R5T.Magyar;

using R5T.T0092;
using R5T.T0094;

using Instances = R5T.T0094.X001.Instances;


namespace System
{
    public static class INamedIdentifiedFilePathedExtensions
    {
        public static HashSet<T> GetHashSetByDataValuesKeepFirst<T>(this IEnumerable<T> namedIdentifiedFilePatheds)
            where T : INamedIdentifiedFilePathed
        {
            var output = new HashSet<T>(NamedFilePathedEqualityComparer<T>.Instance)
                .AddRangeKeepFirst(namedIdentifiedFilePatheds)
                ;

            return output;
        }

        /// <summary>
        /// Fill identities for a set of <see cref="INamedIdentifiedFilePathed"/>s from a corresponding source set of <see cref="INamedIdentifiedFilePathed"/>s, matching using data values (name and file path), or generate new identities if a corresponding instance is not found.
        /// The all items in the source set must have identities, but their identities need not be unique. In the case of duplicate identities, the identity of the first item with matching data values is used.
        /// </summary>
        public static void FillIdentitiesFromSourceOrSetNew<T>(this IEnumerable<T> namedIdentifiedFilePatheds,
            IEnumerable<T> sourceNamedIdentifiedFilePatheds)
            where T : INamedIdentifiedFilePathed, IMutableIdentified
        {
            // The source named-identifieds should all have identities.
            sourceNamedIdentifiedFilePatheds.VerifyAllIdentitiesAreSet();

            var sourceHashSet = sourceNamedIdentifiedFilePatheds.GetHashSetByDataValuesKeepFirst();

            foreach (var namedIdentifiedFilePathed in namedIdentifiedFilePatheds)
            {
                var existsInSource = sourceHashSet.TryGetValue(namedIdentifiedFilePathed, out var sourceNamedIdentifiedFilePathed);
                if(existsInSource)
                {
                    // Match found, set the identity.
                    namedIdentifiedFilePathed.Identity = sourceNamedIdentifiedFilePathed.Identity;
                }
                else
                {
                    // No match found.
                    namedIdentifiedFilePathed.SetIdentityIfNotSet();
                }
            }


            //// 

            //// Cannot just use name as the key since there might be a duplicates, but it's a good place to start.
            //var currentGroupsByName = namedIdentifiedFilePatheds
            //    .GroupBy(x => x.Name)
            //    .ToDictionary(
            //        x => x.Key);

            //var repositoryGroupsByName = sourceNamedIdentifiedFilePatheds
            //    .GroupBy(x => x.Name)
            //    .ToDictionary(
            //        x => x.Key);

            //foreach (var groupPair in currentGroupsByName)
            //{
            //    var repositoryContainsNameGroup = repositoryGroupsByName.ContainsKey(groupPair.Key);
            //    if (repositoryContainsNameGroup)
            //    {
            //        var repositoryGroup = repositoryGroupsByName[groupPair.Key];
            //        foreach (var currentNamedIdentifiedFilePathed in groupPair.Value)
            //        {
            //            // Find the corresponding repository extension method base, if it exists.
            //            var repositoryExtensionMethodBaseOrDefault = repositoryGroup
            //                .Where(Instances.Predicate.NameAndFilePathIs<T>(currentNamedIdentifiedFilePathed.Name, currentNamedIdentifiedFilePathed.FilePath))
            //                .SingleOrDefault();

            //            var wasFound = repositoryExtensionMethodBaseOrDefault is object;
            //            if (wasFound)
            //            {
            //                currentNamedIdentifiedFilePathed.Identity = repositoryExtensionMethodBaseOrDefault.Identity;
            //            }
            //            else
            //            {
            //                // Fill in an identity.
            //                currentNamedIdentifiedFilePathed.SetIdentityIfNotSet();
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // Fill in identities for all extension method bases in this group.
            //        groupPair.Value.SetIdentitiesIfNotSet();
            //    }
            //}
        }

        public static Dictionary<string, List<string>> GetInformationByNameForDuplicateNames(this IEnumerable<INamedIdentifiedFilePathed> namedIdentifiedFilePatheds)
        {
            var output = namedIdentifiedFilePatheds
                .WhereDuplicateNames()
                .ToInformationByName();

            return output;
        }

        public static T[] GetUniqueItems<T>(this IEnumerable<T> items,
            Dictionary<string, T> itemsForDuplicateNamesByName,
            IEnumerable<string> ignoredNames)
            where T : INamedIdentifiedFilePathed
        {
            var uniqueIgnoredNames = new HashSet<string>(ignoredNames);

            var namesHandled = new HashSet<string>();
            var duplicateNamesHandled = new HashSet<string>();

            var uniqueItems = new List<T>();
            foreach (var item in items)
            {
                var name = item.Name;

                // Is the name ignored?
                if (uniqueIgnoredNames.Contains(name))
                {
                    // Ignore the entry for the name.
                    continue;
                }

                // Are there duplicate entries with the same name?
                if (itemsForDuplicateNamesByName.ContainsKey(name))
                {
                    if (!duplicateNamesHandled.Contains(name))
                    {
                        var duplicateNameEntrySelection = itemsForDuplicateNamesByName[name];

                        uniqueItems.Add(duplicateNameEntrySelection);

                        // Now it has been handled.
                        duplicateNamesHandled.Add(name);
                    }

                    // Finished handling this duplicate project name.
                    continue;
                }

                // Now handle the name.
                if (namesHandled.Contains(name))
                {
                    throw new Exception($"Duplicate name without specified entry detected: {name}");
                }

                uniqueItems.Add(item);

                namesHandled.Add(name);
            }

            return uniqueItems.ToArray();
        }

        public static T[] GetUniqueItems<T>(this IEnumerable<T> items,
            IEnumerable<INamedIdentified> duplicateNamedIdentifiedSelections,
            IEnumerable<string> ignoredNames)
            where T : INamedIdentifiedFilePathed
        {
            var duplicateNamedIdentifiedSelectionIdentities = new HashSet<Guid>(duplicateNamedIdentifiedSelections.GetIdentities());

            var itemsForDuplicateNamesByName = items
                .Where(xExtensionMethodBase => duplicateNamedIdentifiedSelectionIdentities.Contains(xExtensionMethodBase.Identity))
                .ToDictionary(
                    x => x.Name);

            var output = items.GetUniqueItems(
                itemsForDuplicateNamesByName,
                ignoredNames);

            return output;
        }

        public static Dictionary<string, List<string>> ToInformationByName(this IEnumerable<IGrouping<string, INamedIdentifiedFilePathed>> namedIdentifiedFilePathedGroups)
        {
            var output = namedIdentifiedFilePathedGroups
                .ToDictionary(
                    xGroup => xGroup.Key,
                    xGroup => xGroup
                        .Select(x => x.GetInformation())
                        .ToList());

            return output;
        }
    }
}


namespace System.Linq
{
    public static class INamedIdentifiedFilePathedExtensions
    {
        public static WasFound<T> FindSingleByIdentityOrThenNameAndFilePath<T>(this IEnumerable<T> namedIdentifiedFilePatheds,
            T namedIdentifiedFilePathed)
            where T : INamedIdentifiedFilePathed
        {
            var output = namedIdentifiedFilePatheds.FindSingleByIdentityOrThenNameAndFilePath(
                namedIdentifiedFilePathed.Identity,
                namedIdentifiedFilePathed.Name,
                namedIdentifiedFilePathed.FilePath);

            return output;
        }   

        public static WasFound<T> FindSingleByIdentityOrThenNameAndFilePath<T>(this IEnumerable<T> namedIdentifiedFilePatheds,
            Guid identity, string name, string filePath)
            where T : INamedIdentifiedFilePathed
        {
            var wasFoundByIdentity = namedIdentifiedFilePatheds.FindSingleByIdentity(identity);
            if (wasFoundByIdentity)
            {
                return wasFoundByIdentity;
            }

            var wasFoundByFilePath = namedIdentifiedFilePatheds.FindSingleByNameAndFilePath(name, filePath);
            return wasFoundByFilePath;
        }
    }
}
