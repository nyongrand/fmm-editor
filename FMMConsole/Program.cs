using FMEConsole;
using FMMConsole;
using FMMLibrary;

#region Add indonesian names from players.csv

//var fparser = await NameParser.Load("../../../db/db_archive_2603/first_names.dat");
//var lparser = await NameParser.Load("../../../db/db_archive_2603/second_names.dat");

//await foreach (var line in File.ReadLinesAsync("../../../txt/players.csv"))
//{
//    var p = Scripts.ParseCsvLine(line);
//    if (int.TryParse(p[0], out var pid) && short.Parse(p[3]) == 94)
//    {
//        var playerName = p[1].Split(' ');
//        if (playerName.Length == 2)
//        {
//            if (!playerName[0].Contains('.') && fparser.Items.FirstOrDefault(x => x.Value == playerName[0]) == null)
//                fparser.Add(new Name(0, 113, 1, 5, playerName[0]));

//            if (!playerName[1].Contains('.') && fparser.Items.FirstOrDefault(x => x.Value == playerName[1]) == null)
//                lparser.Add(new Name(0, 113, 1, 225, playerName[1]));
//        }
//        else if (playerName.Length == 3)
//        {
//            if (fparser.Items.FirstOrDefault(x => x.Value == playerName[0] + " " + playerName[1]) == null)
//                fparser.Add(new Name(0, 113, 1, 5, playerName[0] + " " + playerName[1]));

//            if (!playerName[2].Contains('.') && fparser.Items.FirstOrDefault(x => x.Value == playerName[2]) == null)
//                lparser.Add(new Name(0, 113, 1, 225, playerName[2]));
//        }

//        //var birthdate = DateTime.Parse(p[2]);
//        //var nationId = short.Parse(p[3]);
//        //var clubId = int.Parse(p[4]);
//        //var preferredFoot = p[5];
//        //var leftFoot = p[6];
//        //var rightFoot = p[7];
//        //var height = short.Parse(p[8]);
//        //var weight = short.Parse(p[9]);
//    }
//}

//await fparser.Save();
//await lparser.Save();

#endregion

//await VerifyDatFileAsync();
//return;

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

//var file = "../../../db/db_archive_2603/player.dat";
//var parser = await Scripts.SwitchNationality(file, 70);

// Save changes back to file
//await parser.Save();


//var str = "../../../db/db_archive_2603/eng.lng";
//var strParser = await ResourceParser.Load(str);
//var cities = strParser.Regions.OrderBy(x => x.Name).ToList();

//var j = from c in parser.Items
//        join s in strParser.Regions on c.Id equals s.Uid
//        select new 
//        {
//            City = c, 
//            Str = s 
//        };

//var par = await Scripts.SwitchNationality("../../../db/db_archive_2603/people.dat", 58);
//await par.Save();

var peopleParser = await PeopleParser.Load("../../../db/db_archive_2604/people.dat");
var playerParser = await PlayerParser.Load("../../../db/db_archive_2604/players.dat");
var clubParser = await ClubParser.Load("../../../db/db_archive_2604/club.dat");
var nationParser = await NationParser.Load("../../../db/db_archive_2604/nation.dat");
var fnameParser = await NameParser.Load("../../../db/db_archive_2604/first_names.dat");
var lnameParser = await NameParser.Load("../../../db/db_archive_2604/second_names.dat");

var query = from people in peopleParser.Items
            join club in clubParser.Items on people.ClubId equals club.Id
            join player in playerParser.Items on people.PlayerId equals player.Id
            join nation in nationParser.Items on people.NationId equals nation.Id
            join fname in fnameParser.Items on people.FirstNameId equals fname.Id
            join lname in lnameParser.Items on people.LastNameId equals lname.Id
            select new
            {
                people.Uid,
                FirstName = fname.Value,
                LastName = lname.Value,

                People = people,
                Player = player,
                Club = club,
                Nation = nation.Name,
            };

var players = query.ToList();
var grouped = players
    .Where(x => x.People.NationId == 771)
    //.OrderBy(x => x.People.DateOfBirth)
    //.GroupBy(x => x.People.OtherNationalityCount)
    //.OrderBy(x => x.Key)
    .ToList();

#region Switch W.GER to GER

var ger = nationParser.Items.First(x => x.Uid == 771);
var wger = nationParser.Items.First(x => x.Uid == 584);

// change all w.ger data with ger data except except name and id
wger.Name = ger.Name;
wger.Nationality = ger.Nationality;
wger.CodeName = ger.CodeName;
wger.ContinentId = ger.ContinentId;
wger.CapitalId = ger.CapitalId;
wger.StadiumId = ger.StadiumId;
wger.StateOfDevelopment = ger.StateOfDevelopment;
wger.Unknown1 = ger.Unknown1;
wger.Unknown2 = ger.Unknown2;
wger.Region = ger.Region;
wger.Unknown3 = ger.Unknown3;
wger.Languages = ger.Languages;
wger.HasMaleTeam = ger.HasMaleTeam;
wger.MaleTeam = ger.MaleTeam;
wger.HasFemaleTeam = ger.HasFemaleTeam;
wger.FemaleTeam = ger.FemaleTeam;

await nationParser.Save();

//var germanyPlayers = players
//    .Where(x => x.People.NationId == ger.Id)
//    .ToList();

//var germanyPlayersOther = players
//    .Where(x => x.People.OtherNationalities.Contains(ger.Id))
//    .ToList();


// change all ger people to w.ger
foreach (var people in peopleParser.Items.Where(x => x.NationId == ger.Id))
{
    people.NationId = wger.Id;
}
await peopleParser.Save();

var cityParser = await CityParser.Load("../../../db/db_archive_2604/city.dat");
foreach (var city in cityParser.Items.Where(x => x.NationId == ger.Id))
{
    city.NationId = wger.Id;
}
await cityParser.Save();

foreach (var club in clubParser.Items.Where(x => x.BasedId == ger.Id))
{
    club.NationId = wger.Id;
    club.BasedId = wger.Id;
}
await clubParser.Save();

var competitionParser = await CompetitionParser.Load("../../../db/db_archive_2604/competition.dat");
foreach (var competition in competitionParser.Items.Where(x => x.NationId == ger.Id))
{
    competition.NationId = wger.Id;
}
await competitionParser.Save();

var languageParser = await LanguageParser.Load("../../../db/db_archive_2604/languages.dat");
foreach (var language in languageParser.Items.Where(x => x.NationId == ger.Id))
{
    language.NationId = wger.Id;
}
await languageParser.Save();

var regionParser = await RegionParser.Load("../../../db/db_archive_2604/regions.dat");
foreach (var region in regionParser.Items.Where(x => x.NationId == ger.Id))
{
    region.NationId = wger.Id;
}
await regionParser.Save();

#endregion

var indonesian = players
    //.GroupBy(x => x.People.Unknown2)
    //.OrderBy(x => x.Key)
    .Where(x => x.People.Type == 1)
    //.Where(x => x.People.NationalCaps == 0)
    //.Where(x => x.People.NationId != 58)
    .Where(x => x.People.NationId == 58 || x.People.OtherNationalities.Contains(58))
    //.Where(x => x.Uid != 37059452)
    //.Where(x => x.Uid != 85029078)
    .GroupBy(x => x.Club.NationId)
    .OrderBy(x => x.Count())
    //.Where(x => x.People.DefaultLanguages.Select(x => x.Id).ToList().Contains(76) || x.People.OtherLanguages.Select(x => x.Id).ToList().Contains(76))
    .ToList();

Console.ReadLine();

static async Task VerifyDatFileAsync()
{
    var file = "../../../db/db_archive_2603/players.dat";
    var parser = await PlayerParser.Load("../../../db/db_archive_2603/players.dat");

    var bytes1 = parser.ToBytes();
    var bytes2 = await File.ReadAllBytesAsync(file);

    for (int i = 0; i < bytes2.Length; i++)
    {
        var byte1 = bytes1[i];
        var byte2 = bytes2[i];
        if (byte1 != byte2)
        {
            // difference handling placeholder
            throw new Exception($"Byte mismatch at position {i}: {byte1} != {byte2}");
        }
    }

    var equal = bytes1.SequenceEqual(bytes2);
    var count = parser.Count;
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
        //.Where(x => x.IsWomanFlag == 0)
        .Where(x => x.MainClub == -1)
        //.Where(x => x.IsNational == 0)
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