using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HFM.Proteins
{
    /// <summary>
    /// Represents a serializer capable of serializing and deserializing a collection of <see cref="Protein"/> objects to and from tab delimited text.
    /// </summary>
    public class TabDelimitedTextSerializer : IProteinCollectionSerializer
    {
        /// <summary>
        /// Deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
        /// </summary>
        public ICollection<Protein> Deserialize(Stream stream)
        {
            var collection = new List<Protein>();

            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var p = ParseProtein(line);
                    if (Protein.IsValid(p))
                    {
                        collection.Add(p);
                    }
                }
            }

            return collection;
        }

        /// <summary>
        /// Asynchronously deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
        /// </summary>
        public async Task<ICollection<Protein>> DeserializeAsync(Stream stream)
        {
            var collection = new List<Protein>();

            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    var p = ParseProtein(line);
                    if (Protein.IsValid(p))
                    {
                        collection.Add(p);
                    }
                }
            }

            return collection;
        }

        private static Protein ParseProtein(string line)
        {
            try
            {
                // Parse the current line from the CSV file
                var p = new Protein();
                string[] lineData = line.Split(new[] { '\t' }, StringSplitOptions.None);
                p.ProjectNumber = Int32.Parse(lineData[0], CultureInfo.InvariantCulture);
                p.ServerIP = lineData[1].Trim();
                p.WorkUnitName = lineData[2].Trim();
                p.NumberOfAtoms = Int32.Parse(lineData[3], CultureInfo.InvariantCulture);
                p.PreferredDays = Double.Parse(lineData[4], CultureInfo.InvariantCulture);
                p.MaximumDays = Double.Parse(lineData[5], CultureInfo.InvariantCulture);
                p.Credit = Double.Parse(lineData[6], CultureInfo.InvariantCulture);
                p.Frames = Int32.Parse(lineData[7], CultureInfo.InvariantCulture);
                p.Core = lineData[8];
                p.Description = lineData[9];
                p.Contact = lineData[10];
                p.KFactor = Double.Parse(lineData[11], CultureInfo.InvariantCulture);
                return p;
            }
            catch (Exception)
            {
                Debug.Assert(false);
            }
            return null;
        }

        /// <summary>
        /// Serializes a collection of <see cref="Protein"/> objects to a <see cref="Stream"/>.
        /// </summary>
        public void Serialize(Stream stream, ICollection<Protein> collection)
        {
            using (var writer = new StreamWriter(stream))
            {
                foreach (Protein protein in collection.OrderBy(x => x.ProjectNumber))
                {
                    string line = WriteProtein(protein);
                    writer.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Asynchronously serializes a collection of <see cref="Protein"/> objects to a <see cref="Stream"/>.
        /// </summary>
        public async Task SerializeAsync(Stream stream, ICollection<Protein> collection)
        {
            using (var writer = new StreamWriter(stream))
            {
                foreach (Protein protein in collection.OrderBy(x => x.ProjectNumber))
                {
                    string line = WriteProtein(protein);
                    await writer.WriteLineAsync(line).ConfigureAwait(false);
                }
            }
        }

        private static string WriteProtein(Protein protein)
        {
            // Project Number, Server IP, Work Unit Name, Number of Atoms, Preferred (days),
            // Final Deadline (days), Credit, Frames, Code, Description, Contact, KFactor

            return String.Format(CultureInfo.InvariantCulture,
               "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}",
                  /*  0 */ protein.ProjectNumber,
                  /*  1 */ protein.ServerIP,
                  /*  2 */ protein.WorkUnitName,
                  /*  3 */ protein.NumberOfAtoms,
                  /*  4 */ protein.PreferredDays,
                  /*  5 */ protein.MaximumDays,
                  /*  6 */ protein.Credit,
                  /*  7 */ protein.Frames,
                  /*  8 */ protein.Core,
                  /*  9 */ protein.Description,
                  /* 10 */ protein.Contact,
                  /* 11 */ protein.KFactor);
        }
    }
}
