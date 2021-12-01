using System;

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
