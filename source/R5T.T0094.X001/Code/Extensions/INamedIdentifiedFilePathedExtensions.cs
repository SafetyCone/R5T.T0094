using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0092;
using R5T.T0094;


namespace System
{
    public static class INamedIdentifiedFilePathedExtensions
    {
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
