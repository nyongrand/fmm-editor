using FMEConsole;
using FMELibrary;

//await ApplyChangesTxt();

//// Belgium - 3 levels
//await SwitchNationClubsWithTopContinentClubsAsync(
//    [0, 1, 1616, 1617, /* Non Playable */ 474, 475], (int)Confederation.CONMEBOL);

//// Italy - 3 levels
//await SwitchNationClubsWithTopContinentClubsAsync(
//    [20, 21, 763, 764, 765, /* Non Playable */ 306, 307, 308, 309, 310, 311, 312, 313, 314], (int)Confederation.CAF);

//// Portugal - 4 levels
//await SwitchNationClubsWithTopContinentClubsAsync(
//    [36, 37, 1451, 1452, 944, 945, 946, 947, /* Non Playable */ 1204, 1206, 1207, 1209, 1210, 1211, 1212, 1213, 1214, 1215, 1216, 1217, 1218, 1219, 1220, 1225, 1227, 1228], (int)Confederation.CONCACAF);

//// England - 6 levels
//await SwitchNationClubsWithTopContinentClubsAsync(
//    [5, 6, 7, 8, 83, 326, 327, /* Non Playable */ 323, 325, 1251, 1252], (int)Confederation.AFC, (int)Confederation.OFC);

//// Spain - 4 levels
//await SwitchNationClubsWithTopContinentClubsAsync(
//    [38, 39, 1385, 1386, 1411, 1412, 1413, 1414, 1415], (int)Confederation.UEFA);

await VerifyDatFileAsync();

static async Task VerifyDatFileAsync()
{
    var file = "../../../db/db_archive_2603/people.dat";
    var parser = await PeopleParser.Load(file);

    var g4 = parser.Items.GroupBy(x => x.Ethnicity).ToList();
    var g5 = parser.Items.GroupBy(x => x.Unknown3).ToList();

    var str = "../../../db/db_archive_2603/eng.lng";
    var strParser = await ResourceParser.Load(str);
    var cities = strParser.Regions.OrderBy(x => x.Name).ToList();

    var j = from c in parser.Items
            join s in strParser.Regions on c.Id equals s.Uid
            select new 
            {
                City = c, 
                Str = s 
            };

    var g = parser.Items.GroupBy(x => x.Unknown3).ToList();

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
    var count = parser.OriginalCount;
}

static async Task ApplyChangesTxt()
{
    await ChangesApplier.ApplyChangesTxt(
        "C:\\Users\\nyong\\Data\\fm26\\superliga",
        "C:\\Users\\nyong\\Data\\fm26\\changes.txt"
    );
}

static async Task SwitchNationClubsWithTopContinentClubsAsync(int[] leagueIds, params int[] continentId)
{
    var nationParser = await NationParser.Load("C:\\Users\\nyong\\Data\\fm26\\superliga\\nation.dat");
    var clubParser = await ClubParser.Load("C:\\Users\\nyong\\Data\\fm26\\superliga\\club.dat");

    // Clubs from a leagues that will be switched
    var clubsToSwitch = clubParser.Items
        .Where(x => leagueIds.Contains(x.LeagueId))
        .OrderByDescending(x => x.Reputation)
        .ToList();

    // Nations in a continent
    var nationIds = nationParser.Items
        .Where(x => continentId.Contains(x.ContinentId))
        .Select(x => x.Id)
        .ToHashSet();

    // Filter top continent clubs
    var topClubs = clubParser.Items
        .Where(x => x.IsWomanFlag == 0)
        .Where(x => x.MainClub == -1)
        .Where(x => x.IsNational == 0)
        .Where(x => nationIds.Contains(x.BasedId))
        .GroupBy(x => x.BasedId)
        .OrderByDescending(g => g.Max(c => c.Reputation))
        .SelectMany((g, i) => g.OrderByDescending(c => c.Reputation).Take(7 - Math.Min(i / 7, 6)))
        .OrderByDescending(x => x.Reputation)
        .Take(clubsToSwitch.Count)
        .ToList();

    if (clubsToSwitch.Count == 0 || topClubs.Count == 0)
    {
        Console.WriteLine("No clubs found to switch.");
        return;
    }

    // Take same count from top continent list (already capped per nation)
    var pairCount = Math.Min(clubsToSwitch.Count, topClubs.Count);

    Console.WriteLine($"Switching {pairCount} clubs with top clubs...");

    for (int i = 0; i < pairCount; i++)
    {
        var european = clubParser.Items.First(x => x.Uid == topClubs[i].Uid);
        var national = clubParser.Items.First(x => x.Uid == clubsToSwitch[i].Uid);

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
    var outputPath = "C:\\Users\\SIRS\\Documents\\fm26\\db_archive_2603\\club.dat";
    await clubParser.Save(outputPath);
    Console.WriteLine($"Saved switched club data to: {outputPath}");
}