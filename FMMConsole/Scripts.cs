using FMMLibrary;

namespace FMEConsole
{
    public static class Scripts
    {
        public static async Task<PeopleParser> SwitchNationality(string databaseFile, short nationId)
        {
            var parser = await PeopleParser.Load(databaseFile);

            foreach (var item in parser.Items)
            {
                if (item.Type == 1 && item.NationalCaps == 0 && item.NationId != nationId && item.OtherNationalities.Contains(nationId))
                {
                    // Switch current nation
                    var oldNation = item.NationId;
                    item.NationId = nationId;
                    // Remove Indonesia from other nationalities
                    item.OtherNationalities.Remove(nationId);
                    // Add old nation to other nationalities if not already present
                    if (!item.OtherNationalities.Contains(oldNation))
                    {
                        item.OtherNationalities.Insert(0, oldNation);
                    }
                }
            }

            return parser;
        }

        public static async Task<NameParser> AddNames(string databaseFile, Name sample, string[] names)
        {
            var parser = await NameParser.Load(databaseFile);

            foreach (var name in names)
            {
                parser.Add(new Name(sample.Gender, sample.NationUid, sample.Unknown2, sample.Unknown3, name));
            }

            return parser;
        }
    }
}
