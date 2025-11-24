using static FMMLibrary.StringResource;

namespace FMMLibrary
{
    /// <summary>
    /// Parses and manages club data from binary files.
    /// </summary>
    public class ResourceParser
    {
        /// <summary>
        /// File path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// File header, 8 bytes
        /// </summary>
        public byte[] Header { get; set; }

        /// <summary>
        /// List of all items
        /// </summary>
        public List<ClubName> Clubs { get; set; }

        public List<NationName> Nations { get; set; }

        public List<ContinentName> Continents { get; set; }

        public List<CompetitionName> Competitions { get; set; }

        public List<StadiumName> Stadiums { get; set; }

        public List<AwardName> Awards { get; set; }

        public List<OtherName> Agreements { get; set; }

        public List<OtherName> Rivalries { get; set; }

        public List<OtherName> Regions { get; set; }

        public byte[] UnknownData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the club data.</param>
        private ResourceParser(string path, BinaryReader reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);

            Clubs = [];
            Nations = [];
            Continents = [];
            Competitions = [];
            Stadiums = [];
            Awards = [];
            Agreements = [];
            Rivalries = [];
            Regions = [];
            UnknownData = [];
        }

        /// <summary>
        /// Asynchronously loads club data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load club data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="ClubParser"/> instance.</returns>
        public static async Task<ResourceParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;

            using var reader = new BinaryReader(ms);
            var parser = new ResourceParser(path, reader);

            await Task.Run(() =>
            {
                var clubCount = reader.ReadInt32();
                while (clubCount > parser.Clubs.Count)
                {
                    var item = new ClubName(reader);
                    parser.Clubs.Add(item);
                }

                var nationCount = reader.ReadInt32();
                while (nationCount > parser.Nations.Count)
                {
                    var item = new NationName(reader);
                    parser.Nations.Add(item);
                }

                var continentCount = reader.ReadInt32();
                while (continentCount > parser.Continents.Count)
                {
                    var item = new ContinentName(reader);
                    parser.Continents.Add(item);
                }

                var competitionCount = reader.ReadInt32();
                while (competitionCount > parser.Competitions.Count)
                {
                    var item = new CompetitionName(reader);
                    parser.Competitions.Add(item);
                }

                var stadiumCount = reader.ReadInt32();
                while (stadiumCount > parser.Stadiums.Count)
                {
                    var item = new StadiumName(reader);
                    parser.Stadiums.Add(item);
                }

                var awardsCount = reader.ReadInt32();
                while (awardsCount > parser.Awards.Count)
                {
                    var item = new AwardName(reader);
                    parser.Awards.Add(item);
                }

                var agreementsCount = reader.ReadInt32();
                while (agreementsCount > parser.Agreements.Count)
                {
                    var item = new OtherName(reader);
                    parser.Agreements.Add(item);
                }

                var citiesCount = reader.ReadInt32();
                while (citiesCount > parser.Rivalries.Count)
                {
                    var item = new OtherName(reader);
                    parser.Rivalries.Add(item);
                }

                var unknownsCount = reader.ReadInt32();
                while (unknownsCount > parser.Regions.Count)
                {
                    var item = new OtherName(reader);
                    parser.Regions.Add(item);
                }

                parser.UnknownData = reader.ReadBytes((int)(ms.Length - ms.Position));
            });

            return parser;
        }

        /// <summary>
        /// Converts the club data to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized club data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.Write(Header);

            writer.Write(Clubs.Count);
            foreach (var item in Clubs)
                item.Write(writer);

            writer.Write(Nations.Count);
            foreach (var item in Nations)
                item.Write(writer);

            writer.Write(Continents.Count);
            foreach (var item in Continents)
                item.Write(writer);

            writer.Write(Competitions.Count);
            foreach (var item in Competitions)
                item.Write(writer);

            writer.Write(Stadiums.Count);
            foreach (var item in Stadiums)
                item.Write(writer);

            writer.Write(Awards.Count);
            foreach (var item in Awards)
                item.Write(writer);

            writer.Write(Agreements.Count);
            foreach (var item in Agreements)
                item.Write(writer);

            writer.Write(Rivalries.Count);
            foreach (var item in Rivalries)
                item.Write(writer);

            writer.Write(Regions.Count);
            foreach (var item in Regions)
                item.Write(writer);

            writer.Write(UnknownData);

            return stream.ToArray();
        }

        /// <summary>
        /// Save data back to file path
        /// </summary>
        /// <param name="filepath">Optional file path. If null, saves to the original file path.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task Save(string? filepath = null)
        {
            var bytes = ToBytes();
            await File.WriteAllBytesAsync(filepath ?? FilePath, bytes);
        }
    }
}