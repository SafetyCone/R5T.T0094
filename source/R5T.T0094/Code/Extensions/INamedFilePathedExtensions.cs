using System;

using R5T.T0094;


namespace System
{
    public static class INamedFilePathedExtensions
    {
        public static NamedFilePathed GetNamedFilePathed(this INamedFilePathed namedFilePathed)
        {
            var output = new NamedFilePathed
            {
                FilePath = namedFilePathed.FilePath,
                Name = namedFilePathed.Name,
            };

            return output;
        }
    }
}
