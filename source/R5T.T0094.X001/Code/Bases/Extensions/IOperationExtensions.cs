using System;
using System.Linq;

using R5T.T0094;
using R5T.T0098;


namespace System
{
    public static class IOperationExtensions
    {
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
