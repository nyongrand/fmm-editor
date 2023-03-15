using SQLite;

namespace FMEditor.Entities
{
    [Table("Nation")]
    public class Nation
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

        public static Nation FromModel(FMELibrary.Nation model)
        {
            return new Nation
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
                Coefficients = model.Coefficients.SelectMany(BitConverter.GetBytes).ToArray(),
                ExtraNames = model.ExtraNames.SelectMany(x => x.ToBytes()).ToArray(),
                Languages = model.Languages.SelectMany(x => x.ToBytes()).ToArray(),
                Unknown12 = model.Unknown12,
            };
        }
    }
}
