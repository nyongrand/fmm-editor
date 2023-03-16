using FMELibrary;
using SQLite;

namespace FMEditor.Database
{
    [Table("Nation")]
    public class EntityNation
    {
        [Indexed]
        [Column("uid")]
        public int Uid { get; set; }

        [PrimaryKey]
        [Column("id")]
        public short Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("unknown1")]
        public byte Unknown1 { get; set; }

        [Column("nationality")]
        public string Nationality { get; set; }

        [Column("unknown2")]
        public byte Unknown2 { get; set; }

        [Column("code_name")]
        public string CodeName { get; set; }

        [Column("continent")]
        public short Continent { get; set; }

        [Column("city")]
        public short City { get; set; }

        [Column("stadium")]
        public short Stadium { get; set; }

        [Column("unknown3")]
        public byte Unknown3 { get; set; }

        [Column("color1")]
        public short Color1 { get; set; }

        [Column("unknown4")]
        public int Unknown4 { get; set; }

        [Column("color2")]
        public short Color2 { get; set; }

        [Column("unknown5")]
        public byte Unknown5 { get; set; }

        [Column("unknown6")]
        public byte Unknown6 { get; set; }

        [Column("unknown7")]
        public short Unknown7 { get; set; }

        [Column("unknown8")]
        public short Unknown8 { get; set; }

        [Column("unknown9")]
        public byte Unknown9 { get; set; }

        [Column("unknown10")]
        public short Unknown10 { get; set; }

        [Column("ranked")]
        public bool IsRanked { get; set; }

        [Column("ranking")]
        public short Ranking { get; set; }

        [Column("points")]
        public short Points { get; set; }

        [Column("unknown11")]
        public int Unknown11 { get; set; }

        [Column("coefficients")]
        public byte[] Coefficients { get; set; }

        [Column("extra_names")]
        public byte[] ExtraNames { get; set; }

        [Column("languages")]
        public byte[] Languages { get; set; }

        [Column("unknown12")]
        public byte[] Unknown12 { get; set; }

        public static EntityNation FromModel(FMELibrary.Nation model)
        {
            return new EntityNation
            {
                Uid = model.Uid,
                Id = model.Id,
                Name = model.Name,
                Unknown1 = model.Unknown1,
                Nationality = model.Nationality,
                Unknown2 = model.Unknown2,
                CodeName = model.CodeName,
                Continent = model.Continent,
                City = model.City,
                Stadium = model.Stadium,
                Unknown3 = model.Unknown3,
                Color1 = model.Color1,
                Unknown4 = model.Unknown4,
                Color2 = model.Color2,
                Unknown5 = model.Unknown5,
                Unknown6 = model.Unknown6,
                Unknown7 = model.Unknown7,
                Unknown8 = model.Unknown8,
                Unknown9 = model.Unknown9,
                Unknown10 = model.Unknown10,
                IsRanked = model.IsRanked,
                Ranking = model.Ranking,
                Points = model.Points,
                Unknown11 = model.Unknown11,
                Coefficients = ToBytes(model.Coefficients),
                ExtraNames = model.ExtraNames.SelectMany(x => x.ToBytes()).ToArray(),
                Languages = model.Languages.SelectMany(x => x.ToBytes()).ToArray(),
                Unknown12 = model.Unknown12,
            };
        }

        public static FMELibrary.Nation ToModel(EntityNation entity)
        {
            return new FMELibrary.Nation
            {
                Uid = entity.Uid,
                Id = entity.Id,
                Name = entity.Name,
                Unknown1 = entity.Unknown1,
                Nationality = entity.Nationality,
                Unknown2 = entity.Unknown2,
                CodeName = entity.CodeName,
                Continent = entity.Continent,
                City = entity.City,
                Stadium = entity.Stadium,
                Unknown3 = entity.Unknown3,
                Color1 = entity.Color1,
                Unknown4 = entity.Unknown4,
                Color2 = entity.Color2,
                Unknown5 = entity.Unknown5,
                Unknown6 = entity.Unknown6,
                Unknown7 = entity.Unknown7,
                Unknown8 = entity.Unknown8,
                Unknown9 = entity.Unknown9,
                Unknown10 = entity.Unknown10,
                IsRanked = entity.IsRanked,
                Ranking = entity.Ranking,
                Points = entity.Points,
                Unknown11 = entity.Unknown11,
                Coefficients = ToFloats(entity.Coefficients),
                ExtraNames = ToExtraNames(entity.ExtraNames),
                Languages = ToLanguages(entity.Languages),
                Unknown12 = entity.Unknown12,
            };
        }

        private static byte[] ToBytes(float[] floats)
        {
            return floats.SelectMany(BitConverter.GetBytes).ToArray();
        }

        private static float[] ToFloats(byte[] bytes)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentException("Byte array length must be a multiple of 4");

            float[] floats = new float[bytes.Length / 4];

            for (int i = 0; i < floats.Length; i++)
            {
                byte[] floatBytes = new byte[4];
                Buffer.BlockCopy(bytes, i * 4, floatBytes, 0, 4);
                floats[i] = BitConverter.ToSingle(floatBytes, 0);
            }

            return floats;
        }

        private static ExtraName[] ToExtraNames(byte[] bytes)
        {
            using var ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;

            using var reader = new BinaryReader(ms);
            var items = new List<ExtraName>();

            while (ms.Position < ms.Length)
            {
                var item = new ExtraName(reader);
                items.Add(item);
            }

            return items.ToArray();
        }

        private static Language[] ToLanguages(byte[] bytes)
        {
            using var ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;

            using var reader = new BinaryReader(ms);
            var items = new List<Language>();

            while (ms.Position < ms.Length)
            {
                var item = new Language(reader);
                items.Add(item);
            }

            return items.ToArray();
        }
    }
}
