using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using R5T.T0092;
using R5T.T0094;
using R5T.T0098;


namespace System
{
    public static class IOperationExtensions
    {
        public static void WriteSelectedNameChangesSummaryFile<TNamedIdentifiedFilePathed, TNamedIdentified>(this IOperation _,
            StreamWriter output,
            IList<(TNamedIdentified, string)> newNameSelectionsWithReasons,
            IList<(TNamedIdentified, string)> departedNameSelectionsWithReasons,
            IEnumerable<TNamedIdentifiedFilePathed> namedIdentifiedFilePatheds)
            where TNamedIdentifiedFilePathed : INamedIdentifiedFilePathed
            where TNamedIdentified : IMutableNamedIdentified
        {
            var namedIdentifiedFilePathedsByIdentity = namedIdentifiedFilePatheds.ToDictionaryByIdentity();

            var newNameSelectionsCount = newNameSelectionsWithReasons.Count;

            output.WriteLine($"New selected names ({newNameSelectionsCount}):");
            output.WriteLine();

            if (newNameSelectionsWithReasons.None())
            {
                output.WriteLine("<none> (ok)");
            }
            else
            {
                var newNameSelectionsByReason = newNameSelectionsWithReasons
                    .GroupBy(x => x.Item2)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(y => y.Item1).ToArray());

                foreach (var pair in newNameSelectionsByReason)
                {
                    // Reason
                    output.WriteLine($"# {pair.Key}");

                    foreach (var nameSelection in pair.Value)
                    {
                        var namedIdentifiedFilePathed = namedIdentifiedFilePathedsByIdentity[nameSelection.Identity];

                        output.WriteLine($"{namedIdentifiedFilePathed.Name}: {namedIdentifiedFilePathed.FilePath}");
                    }
                    output.WriteLine();
                }
            }
            output.WriteLine("\n***\n");

            var departedNameSelectionsCount = departedNameSelectionsWithReasons.Count;

            output.WriteLine($"Departed selected names ({departedNameSelectionsCount}):");
            output.WriteLine();

            if (departedNameSelectionsWithReasons.None())
            {
                output.WriteLine("<none> (ok)");
            }
            else
            {
                var departedNameSelectionsByReason = departedNameSelectionsWithReasons
                    .GroupBy(x => x.Item2)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(y => y.Item1).ToArray());

                foreach (var pair in departedNameSelectionsByReason)
                {
                    // Reason
                    output.WriteLine($"# {pair.Key}");

                    foreach (var nameSelection in pair.Value)
                    {
                        var namedIdentifiedFilePathedForIdentityExists = namedIdentifiedFilePathedsByIdentity.ContainsKey(nameSelection.Identity);
                        if (namedIdentifiedFilePathedForIdentityExists)
                        {
                            var namedIdentifiedFilePathed = namedIdentifiedFilePathedsByIdentity[nameSelection.Identity];

                            output.WriteLine($"{namedIdentifiedFilePathed.Name}: {namedIdentifiedFilePathed.FilePath}");
                        }
                        else
                        {
                            output.WriteLine($"{nameSelection.Name}: No file path exists (departed).");
                        }
                    }
                    output.WriteLine();
                }
            }
            output.WriteLine("\n***\n");
        }

        public static (
            TNamedIdentified[] newNameSelections,
            TNamedIdentified[] departedNameSelections,
            List<(TNamedIdentified, string)> newNameSelectionsWithReasons,
            List<(TNamedIdentified, string)> departedNameSelectionsWithReasons)
        GetNewAndDepartedNameSelectionsWithReasons<TNamedIdentifiedFilePathed, TNamedIdentified>(this IOperation _,
            IEnumerable<TNamedIdentifiedFilePathed> namedIdentifiedPatheds,
            IEnumerable<TNamedIdentified> priorDuplicateNameSelections,
            IEnumerable<TNamedIdentified> priorNameSelections,
            IEnumerable<TNamedIdentified> nameSelections,
            IEnumerable<string> priorIgnoredNames,
            IEnumerable<string> ignoredNames)
            where TNamedIdentifiedFilePathed : INamedIdentifiedFilePathed
            where TNamedIdentified : IMutableNamedIdentified, new()
        {
            var newNameSelections = nameSelections.Except(priorNameSelections, NamedIdentifiedEqualityComparer<TNamedIdentified>.Instance).Now();
            var departedNameSelections = priorNameSelections.Except(nameSelections, NamedIdentifiedEqualityComparer<TNamedIdentified>.Instance).Now();

            // Determine reasons for appearance/disappearance of name selections.
            var embsByIdentity = namedIdentifiedPatheds.ToDictionaryByIdentity();
            var repositoryDuplicateNameSelectionsByName = priorDuplicateNameSelections.ToDictionaryByName();
            var duplicateNameSelectionsByName = priorNameSelections.ToDictionaryByName();
            var repositoryIgnoredNamesHash = new HashSet<string>(priorIgnoredNames);
            var ignoredNamesHash = new HashSet<string>(ignoredNames);

            var defaultReason = "<Unknown> (Should not happen.)";

            var newNameSelectionsWithReasons = new List<(TNamedIdentified, string)>();
            foreach (var newNameSelection in newNameSelections)
            {
                var identity = newNameSelection.Identity;
                var name = newNameSelection.Name;

                // In order of least specific to most specific.
                var reason = defaultReason;

                // Is the project new? (Project is present in projects list.)
                var nameExists = embsByIdentity.ContainsKey(newNameSelection.Identity);
                if (nameExists)
                {
                    reason = "Name is new.";
                }

                // Is the project newly unignored? (Project name is present in initial repository ignored file, but not in modified ignored file.)
                var nameExistsInRepositoryIgnoredNames = repositoryIgnoredNamesHash.Contains(name);
                var nameExistsInIgnoredNames = ignoredNamesHash.Contains(name);
                if (nameExistsInRepositoryIgnoredNames && !nameExistsInIgnoredNames)
                {
                    reason = "Name is newly unignored";
                }

                // Did the choice of duplicate project name selection change? (Project name is assigned to one identity in modified duplicates file, but a different identity in the initial duplicates file.)
                var nameExistsInRepositoryDuplicateNameSelection = repositoryDuplicateNameSelectionsByName.ContainsKey(name);
                var nameExistsInDuplicateNameSelection = duplicateNameSelectionsByName.ContainsKey(name);
                if (nameExistsInRepositoryDuplicateNameSelection || nameExistsInDuplicateNameSelection)
                {
                    var repositoryDuplicateNameSelection = nameExistsInRepositoryDuplicateNameSelection
                        ? repositoryDuplicateNameSelectionsByName[newNameSelection.Name]
                        : new TNamedIdentified() // Just use an empty.
                        ;

                    var identityChanged = identity != repositoryDuplicateNameSelection.Identity;
                    if (identityChanged)
                    {
                        reason = "Duplicate selection for name changed.";
                    }
                }

                newNameSelectionsWithReasons.Add((newNameSelection, reason));
            }

            var departedNameSelectionsWithReasons = new List<(TNamedIdentified, string)>();
            foreach (var departedNameSelection in departedNameSelections)
            {
                var identity = departedNameSelection.Identity;
                var name = departedNameSelection.Name;

                // In order of least specific to most specific.
                var reason = defaultReason;

                // Did the project departed? (Project is not present in projects list.)
                var nameDoesNotExist = !embsByIdentity.ContainsKey(identity);
                if (nameDoesNotExist)
                {
                    reason = "Name departed.";
                }

                // Is the project newly unignored? (Project name is present in modified ignored file, but not in initial repository ignored file.)
                var nameExistsInIgnoredNames = ignoredNamesHash.Contains(name);
                if (nameExistsInIgnoredNames)
                {
                    reason = "Name is ignored.";
                }

                // Did the choice of duplicate project name selection change? (Project name is assigned to one identity in modified duplicates file, but a different identity in the initial duplicates file.)
                var nameExistsInRepositoryDuplicateNameSelection = repositoryDuplicateNameSelectionsByName.ContainsKey(name);
                var nameExistsInDuplicateNameSelection = duplicateNameSelectionsByName.ContainsKey(name);
                if (nameExistsInRepositoryDuplicateNameSelection || nameExistsInDuplicateNameSelection)
                {
                    var repositoryDuplicateNameSelection = nameExistsInRepositoryDuplicateNameSelection
                        ? repositoryDuplicateNameSelectionsByName[name]
                        : new TNamedIdentified() // Just use an empty.
                        ;

                    var identityChanged = identity != repositoryDuplicateNameSelection.Identity;
                    if (identityChanged)
                    {
                        reason = "Duplicate selection for name changed.";
                    }
                }

                departedNameSelectionsWithReasons.Add((departedNameSelection, reason));
            }

            return (
                newNameSelections,
                departedNameSelections,
                newNameSelectionsWithReasons,
                departedNameSelectionsWithReasons);
        }

        public static (
            TNamedIdentified[] nameSelections,
            Dictionary<string, TNamedIdentifiedFilePathed[]> unspecifiedDuplicateNameSets)
        GetSelectedNames<TNamedIdentifiedFilePathed, TNamedIdentified>(this IOperation _,
            IList<TNamedIdentifiedFilePathed> namedIdentifiedFilePatheds,
            IList<string> ignoredNames,
            IList<TNamedIdentified> duplicateNameSelections)
            where TNamedIdentifiedFilePathed : INamedIdentifiedFilePathed
            where TNamedIdentified : IMutableNamedIdentified, new()
        {
            // Verify inputs.
            namedIdentifiedFilePatheds.VerifyDistinctByIdentity();
            namedIdentifiedFilePatheds.VerifyDistinctByFilePath();
            duplicateNameSelections.VerifyDistinctNamesAndDistinctIdentities();

            var ignoredNamesHash = new HashSet<string>(ignoredNames);

            // Only work with projects with unignored names.
            var unignored = namedIdentifiedFilePatheds
                .Where(xProject => !ignoredNamesHash.Contains(xProject.Name))
                .Now();

            var groupsByName = unignored
                .GroupBy(x => x.Name)
                ;

            // Handle non-duplicate projects.
            var nonDuplicateNameSelections = groupsByName
                .Where(xGroup => xGroup.Count() == 1)
                .SelectMany(xGroup => xGroup
                    .Select(xProject =>
                    {
                        var nameSelection = new TNamedIdentified
                        {
                            Identity = xProject.Identity,
                            Name = xProject.Name,
                        };

                        return nameSelection;
                    }))
                ;

            // Handle duplicates.
            var duplicateGroupsByName = groupsByName
                .Where(xGroup => xGroup.Count() > 1);

            var duplicateNameSelectionsNamesHash = new HashSet<string>(duplicateNameSelections.GetAllNames());

            // Get specified and unspecified duplicate name selections.
            var specifiedDuplicateNameGroups = duplicateGroupsByName
                .Where(xGroup => duplicateNameSelectionsNamesHash.Contains(xGroup.Key));

            var duplicateNameSelectionsByName = duplicateNameSelections.ToDictionaryByName();

            var duplicateProjectNameSelections = specifiedDuplicateNameGroups
                .Select(xGroup =>
                {
                    var duplicateNameSelection = duplicateNameSelectionsByName[xGroup.Key];
                    return duplicateNameSelection;
                });

            var unspecifiedDuplicateProjectNameSets = duplicateGroupsByName
                .Where(xGroup => !duplicateNameSelectionsNamesHash.Contains(xGroup.Key))
                .ToDictionary(
                    x => x.Key,
                    x => x.ToArray());

            var nameSelections = nonDuplicateNameSelections.Concat(duplicateProjectNameSelections).Now();

            return (nameSelections, unspecifiedDuplicateProjectNameSets);
        }

        public static (
            T[] newExtensionMethodBases,
            T[] departedExtensionMethodBases)
        GetNewAndDepartedByNameAndFilePath<T>(this IOperation _,
            T[] baseNamedFilePatheds,
            T[] modifedNamedFilePatheds)
            where T : INamedFilePathed
        {
            // No need to check data values, because we are using a data value-based equality comparer, and the only other field is identity, which will not have been set for the local extension method base objects.
            var newExtensionMethodBases = modifedNamedFilePatheds.Except(baseNamedFilePatheds, NamedFilePathedEqualityComparer<T>.Instance)
                .ToArray();

            var departedExtensionMethodBases = baseNamedFilePatheds.Except(modifedNamedFilePatheds, NamedFilePathedEqualityComparer<T>.Instance)
                .ToArray();

            return (newExtensionMethodBases, departedExtensionMethodBases);
        }
    }
}
