using System;


namespace R5T.T0094
{
    public class NamedIdentifiedFilePathed : INamedIdentifiedFilePathed
    {
        public Guid Identity { get; set; }

        public string Name { get; set; }
        public string FilePath { get; set; }


        public override string ToString()
        {
            var representation = $"{this.Name} ({this.FilePath})";
            return representation;
        }
    }
}
