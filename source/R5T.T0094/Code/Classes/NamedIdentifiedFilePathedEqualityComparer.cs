using System;
using System.Collections.Generic;


namespace R5T.T0094
{
    /// <summary>
    /// Implements an identity for an <see cref="INamedFilePathed"/> based on the name and file path data values.
    /// </summary>
    public class NamedIdentifiedFilePathedEqualityComparer<T> : IEqualityComparer<T>
        where T: INamedIdentifiedFilePathed
    {
        #region Static

        public static NamedIdentifiedFilePathedEqualityComparer<T> Instance { get; } = new();

        #endregion


        public bool Equals(T x, T y)
        {
            var output = true
                && x.Identity == y.Identity
                && x.Name == y.Name
                && x.FilePath == y.FilePath
                ;

            return output;
        }

        public int GetHashCode(T obj)
        {
            var output = HashCode.Combine(
                obj.Identity,
                obj.Name,
                obj.FilePath);

            return output;
        }
    }
}
