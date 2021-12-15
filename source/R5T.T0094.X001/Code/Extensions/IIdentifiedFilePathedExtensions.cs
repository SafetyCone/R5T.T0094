using System;
using System.Collections.Generic;

using R5T.Magyar;

using R5T.T0094;


namespace System
{
    public static class IIdentifiedFilePathedExtensions
    {
        public static string GetInformation(this IIdentifiedFilePathed identifiedFilePathed)
        {
            var output = $"({identifiedFilePathed.Identity}): {identifiedFilePathed.FilePath}";
            return output;
        }
    }
}


namespace System.Linq
{
    public static class IIdentifiedFilePathedExtensions
    {
        public static WasFound<T> FindSingleByIdentityOrThenFilePath<T>(this IEnumerable<T> identifiedFilePatheds, T identifiedFilePathed)
            where T : IIdentifiedFilePathed
        {
            var output = identifiedFilePatheds.FindSingleByIdentityOrThenFilePath(
                identifiedFilePathed.Identity,
                identifiedFilePathed.FilePath);

            return output;
        }

        public static WasFound<T> FindSingleByIdentityOrThenFilePath<T>(this IEnumerable<T> identifiedFilePatheds,
            Guid identity, string filePath)
            where T : IIdentifiedFilePathed
        {
            var wasFoundByIdentity = identifiedFilePatheds.FindSingleByIdentity(identity);
            if (wasFoundByIdentity)
            {
                return wasFoundByIdentity;
            }

            var wasFoundByFilePath = identifiedFilePatheds.FindSingleByFilePath(filePath);
            return wasFoundByFilePath;
        }
    }
}
