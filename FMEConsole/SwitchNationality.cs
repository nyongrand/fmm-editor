using FMELibrary;

namespace FMEConsole
{
    public static class SwitchNationality
    {
        public static async Task<PeopleParser> Run(string databaseFile, short nationId)
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
    }
}
