using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace HFM.Proteins
{
    /// <summary>
    /// Deserializes a collection of <see cref="Protein"/> objects from the Folding@Home project summary json.
    /// This class does not support serialization through the <see cref="IProteinCollectionSerializer"/> interface. The serialize methods will throw <see cref="NotSupportedException"/>.
    /// </summary>
    public class ProjectSummaryJsonDeserializer : IProteinCollectionSerializer
    {
        /// <summary>
        /// Deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
        /// </summary>
        public ICollection<Protein> Deserialize(Stream stream)
        {
            string json;
            using (var reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            return DeserializeInternal(json);
        }

        /// <summary>
        /// Asynchronously deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
        /// </summary>
        public async Task<ICollection<Protein>> DeserializeAsync(Stream stream)
        {
            string json;
            using (var reader = new StreamReader(stream))
            {
                json = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            return DeserializeInternal(json);
        }

        private static ICollection<Protein> DeserializeInternal(string json)
        {
            const double secondsToDays = 86400.0;

            var collection = new List<Protein>();
            if (json.Length > 0)
            {
                foreach (var token in JArray.Parse(json))
                {
                    if (!token.HasValues)
                    {
                        continue;
                    }

                    var p = new Protein();
                    p.ProjectNumber = GetTokenValue<int>(token, "id");
                    p.ServerIP = GetTokenValue<string>(token, "ws");
                    p.WorkUnitName = @"p" + p.ProjectNumber;
                    p.NumberOfAtoms = GetTokenValue<int>(token, "atoms");
                    p.PreferredDays = Math.Round(GetTokenValue<int>(token, "timeout") / secondsToDays, 3, MidpointRounding.AwayFromZero);
                    p.MaximumDays = Math.Round(GetTokenValue<int>(token, "deadline") / secondsToDays, 3, MidpointRounding.AwayFromZero);
                    p.Credit = GetTokenValue<double>(token, "credit");
                    p.Frames = 100;
                    p.Core = GetTokenValue<string>(token, "type");
                    p.Description = @"https://apps.foldingathome.org/project.py?p=" + p.ProjectNumber;
                    p.Contact = GetTokenValue<string>(token, "contact");
                    p.KFactor = 0.75;
                    collection.Add(p);
                }
            }
            return collection;
        }

        private static T GetTokenValue<T>(JToken token, string path)
        {
            for (var selected = token.SelectToken(path); selected != null;)
            {
                return selected.Value<T>();
            }
            return default(T);
        }

        [ExcludeFromCodeCoverage]
        void IProteinCollectionSerializer.Serialize(Stream stream, ICollection<Protein> collection)
        {
            throw new NotSupportedException("Serialization is not supported.");
        }

        [ExcludeFromCodeCoverage]
        Task IProteinCollectionSerializer.SerializeAsync(Stream stream, ICollection<Protein> collection)
        {
            throw new NotSupportedException("Serialization is not supported.");
        }
    }
}
