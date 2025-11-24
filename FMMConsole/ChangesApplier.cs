using FMMLibrary;
using System.Text.RegularExpressions;

namespace FMEConsole
{
    public partial class ChangesApplier
    {
        public static async Task ApplyChangesTxt(string databaseFile, string changesFile)
        {
            var competitionParser = await CompetitionParser.Load(Path.Combine(databaseFile, "competition.dat"));
            var nationParser = await NationParser.Load(Path.Combine(databaseFile, "nation.dat"));
            var clubParser = await ClubParser.Load(Path.Combine(databaseFile, "club.dat"));
            var lines = File.ReadLinesAsync(changesFile);

            var clubChanged = 0;
            var competitionChanged = 0;

            await foreach (var line in lines)
            {
                // Extract quoted strings
                var matches = QuotedStringRegex().Matches(line);
                if (matches.Count == 0) continue;

                string header = matches[0].Groups[1].Value;
                if (header == "CLUB")
                {
                    var changed = ApplyClubChange(nationParser, clubParser, matches);
                    clubChanged += changed ? 1 : 0;
                }
                if (header == "COMPETITION")
                {
                    var changed = ApplyCompetitionnChange(competitionParser, matches);
                    competitionChanged += changed ? 1 : 0;
                }
            }

            // Save to new file to preserve original
            if (clubChanged > 0)
            {
                await clubParser.Save(Path.Combine(databaseFile, "club.dat"));
                Console.WriteLine($"Applied {clubChanged} club name changes to: {Path.Combine(databaseFile, "club.dat")}");
            }

            if (competitionChanged > 0)
            {
                await competitionParser.Save(Path.Combine(databaseFile, "competition.dat"));
                Console.WriteLine($"Applied {competitionChanged} competition name changes to: {Path.Combine(databaseFile, "competition.dat")}");
            }
        }


        // "CLUB" "Blu-neri Milano" "Italy" "FC Internazionale Milano" "Inter" "" ""
        private static bool ApplyClubChange(NationParser nationParser, ClubParser clubParser, MatchCollection matches)
        {
            var isChanged = false;
            var originalName = matches[1].Groups[1].Value;
            var club = clubParser.Items
                .Where(x => x.FullName == originalName || x.ShortName == originalName)
                .FirstOrDefault();

            if (club != null)
            {
                var nation = nationParser.Items
                    .Where(x => x.Id == club.NationId)
                    .FirstOrDefault();

                var nationName = matches[2].Groups[1].Value;
                if (nation?.Name == nationName)
                {
                    isChanged = true;

                    if (!string.IsNullOrEmpty(matches[3].Groups[1].Value))
                        club.FullName = matches[3].Groups[1].Value;

                    if (!string.IsNullOrEmpty(matches[4].Groups[1].Value))
                        club.ShortName = matches[4].Groups[1].Value;
                }
            }

            return isChanged;
        }

        // "COMPETITION" "Italian Serie A" "Serie A Enilive" "" ""
        private static bool ApplyCompetitionnChange(CompetitionParser parser, MatchCollection matches)
        {
            var isChanged = false;
            var originalName = matches[1].Groups[1].Value;
            var competition = parser.Items
                .Where(x => x.FullName == originalName || x.ShortName == originalName)
                .FirstOrDefault();

            if (competition != null)
            {
                isChanged = true;
                competition.FullName = matches[2].Groups[1].Value;
                competition.ShortName = matches[3].Groups[1].Value;
                competition.CodeName = matches[4].Groups[1].Value;
            }

            return isChanged;
        }

        [GeneratedRegex("\"([^\"]*)\"")]
        private static partial Regex QuotedStringRegex();
    }
}
