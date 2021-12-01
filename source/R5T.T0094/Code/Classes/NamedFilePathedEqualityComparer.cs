using System;
using System.Collections.Generic;


namespace R5T.T0094
{
    /// <summary>
    /// Implements an identity for an <see cref="INamedFilePathed"/> based on the name and file path data values.
    /// </summary>
    public class NamedFilePathedEqualityComparer : IEqualityComparer<INamedFilePathed>
    {
        #region Static

        public static NamedFilePathedEqualityComparer Instance { get; } = new();

        #endregion


        public bool Equals(INamedFilePathed x, INamedFilePathed y)
        {
            var output = true
                && x.Name == y.Name
                && x.FilePath == y.FilePath
                ;

            return output;
        }

        public int GetHashCode(INamedFilePathed obj)
        {
            var output = HashCode.Combine(
                obj.Name,
                obj.FilePath);

            return output;
        }
    }
}
