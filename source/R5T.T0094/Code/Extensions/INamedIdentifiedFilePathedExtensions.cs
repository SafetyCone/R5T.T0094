using System;

using R5T.T0094;


namespace System
{
    public static class INamedIdentifiedFilePathedExtensions
    {
        // <summary>
        /// Allows converting any <see cref="INamedIdentifiedFilePathed"/> to a general <see cref="NamedIdentifiedFilePathed"/>, with a specific conversion performed on the name.
        /// </summary>
        public static NamedIdentifiedFilePathed ToNamedIdentifiedFilePathed(this INamedIdentifiedFilePathed namedIdentifiedFilePathed,
            Func<string, string> nameModifier)
        {
            var output = new NamedIdentifiedFilePathed
            {
                FilePath = namedIdentifiedFilePathed.FilePath,
                Identity = namedIdentifiedFilePathed.Identity,
                Name = nameModifier(namedIdentifiedFilePathed.Name)
            };

            return output;
        }
    }
}
