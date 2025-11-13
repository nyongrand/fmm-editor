using FMELibrary;
using System.Text.RegularExpressions;

//await ApplyChangesTxt();
await SwitchNationClubsWithTopContinentClubsAsync(131, 3, 61); // Belgium
await SwitchNationClubsWithTopContinentClubsAsync(150, 0, 100); // Italy
await SwitchNationClubsWithTopContinentClubsAsync(162, 5, 112); // Portugal
await SwitchNationClubsWithTopContinentClubsAsync(139, 1, 164); // England
await SwitchNationClubsWithTopContinentClubsAsync(170, 2, 172); // Spain
//await VerifyDatFileAsync();

static async Task VerifyDatFileAsync()
{
    var file = "../../../db/db_archive_2603/club.dat";
    var parser = await ClubParser.Load(file);

    var match = parser.Count == parser.Items.Count;
    var bytes1 = parser.ToBytes();
    var bytes2 = await File.ReadAllBytesAsync(file);

    for (int i = 0; i < bytes2.Length; i++)
    {
        if (bytes1[i] != bytes2[i])
        {
            // difference handling placeholder
        }
    }

    var equal = bytes1.SequenceEqual(bytes2);
    var count = parser.Count;
}

static async Task ApplyChangesTxt()
{
    var clubParser = await ClubParser.Load("C:\\Users\\SIRS\\Documents\\fm26\\db_archive_2603\\club.dat");

    var replacementMap = new Dictionary<string, string>();
    var lines = File.ReadLinesAsync("C:\\Users\\SIRS\\Documents\\fm26\\changes.txt");
    await foreach (var line in lines)
    {
        // Extract quoted strings
        var matches = Regex.Matches(line, "\"([^\"]*)\"");
        if (matches.Count == 0) continue;

        //"CLUB" "Current Name" "" "New Name" "New Short Name" "" ""

        string header = matches[0].Groups[1].Value;
        if (header == "CLUB")
        {
            var clubName = matches[1].Groups[1].Value;
            var newClubName = matches[3].Groups[1].Value;
            var newShortName = matches[4].Groups[1].Value;
        }

        var values = new List<string>();

        for (int i = 1; i < matches.Count; i++)
        {
            values.Add(matches[i].Groups[1].Value);
        }
    }
}

//0 - CAF
//1 - AFC
//2 - UEFA
//3 - CONCACAF
//4 - OFC
//5 - CONMEBOL
static async Task SwitchNationClubsWithTopContinentClubsAsync(int nationId, int continentId, int max)
{
    var nationParser = await NationParser.Load("C:\\Users\\nyong\\Data\\fm26\\superliga\\nation.dat");
    var clubParser = await ClubParser.Load("C:\\Users\\nyong\\Data\\fm26\\superliga\\club.dat");

    // Nations in a continent
    var uefaNationIds = nationParser.Items
        .Where(x => x.ContinentId == continentId)
        .Select(x => x.Id)
        .ToHashSet();

    // Filter top continent clubs - limit to maximum 10 per nation
    var topClubs = clubParser.Items
        .Where(x => x.IsWomanFlag == 0)
        .Where(x => x.MainClub == -1)
        .Where(x => x.IsNational == 0)
        .Where(x => uefaNationIds.Contains(x.NationId))
        .GroupBy(x => x.NationId)
        .OrderByDescending(g => g.Max(c => c.Reputation))
        .SelectMany((g, i) => g.OrderByDescending(c => c.Reputation).Take(7 - (i / 5)))
        .OrderByDescending(x => x.Reputation)
        .Take(max)
        .ToList();

    // national clubs
    var nationalClubs = clubParser.Items
        .Where(x => x.IsWomanFlag == 0)
        .Where(x => x.IsNational == 0)
        .Where(x => x.LeagueId != -1)
        .Where(x => x.NationId == nationId)
        .OrderByDescending(x => x.Reputation)
        .ToList();

    if (nationalClubs.Count == 0 || topClubs.Count == 0)
    {
        Console.WriteLine("No clubs found to switch.");
        return;
    }

    // Take same count from top continent list (already capped per nation)
    var pairCount = Math.Min(nationalClubs.Count, topClubs.Count);

    Console.WriteLine($"Switching {pairCount} clubs with top clubs...");

    for (int i = 0; i < pairCount; i++)
    {
        var european = clubParser.Items.First(x => x.Uid == topClubs[i].Uid);
        var national = clubParser.Items.First(x => x.Uid == nationalClubs[i].Uid);

        // Swap BasedId and LeagueId as per earlier intent
        var tmpBased = national.BasedId;
        var tmpLeague = national.LeagueId;

        national.BasedId = european.BasedId;
        national.LeagueId = european.LeagueId;

        european.BasedId = tmpBased;
        european.LeagueId = tmpLeague;

        Console.WriteLine($"Switched: {national.FullName} <-> {european.FullName}");
    }

    // Save to new file to preserve original
    var outputPath = "C:\\Users\\nyong\\Data\\fm26\\superliga\\club.dat";
    await clubParser.Save(outputPath);
    Console.WriteLine($"Saved switched club data to: {outputPath}");
}

// Existing commented exploratory code retained below
//var compParser = new CompetitionParser("D:\\Downloads\\database\\competition.dat");
//var clubParser = new ClubParser("D:\\Downloads\\database\\club.dat");

//var competitions = compParser.Items;
//var clubs = compParser.Items;

//var q = from comp in compParser.Items
// join club in clubParser.Items on comp.Id equals club.League into grouping
// select new { Competition = comp, Clubs = grouping.ToList() };

//Console.WriteLine(""));

//while (true)
//{
// var name = Console.ReadLine();
// if (string.IsNullOrEmpty(name))
// break;
//}

