using FMMLibrary;

namespace FMMConsole
{
    public static class Scripts
    {
        public static async Task<PeopleParser> SwitchNationality(string databaseFile, short nationId)
        {
            var parser = await PeopleParser.Load(databaseFile);

            int[] uids = [
        37076007,
        37076311,
        37077859,
        37084510,
        2000068026,
        2000305950,
        2000314450,
        2000350893,
        2000353431,
        2000422879];

            foreach (var item in parser.Items)
            {
                if (item.Uid == 37059452 || item.Uid == 85029078)
                    continue;

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

        public static List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuotes = false;
            string currentField = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField.Trim());
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }

            fields.Add(currentField.Trim());
            return fields;
        }
    }
}
