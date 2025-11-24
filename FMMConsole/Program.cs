using FMEConsole;
using FMMLibrary;

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

//var cparser = await ClubParser.Load("../../../db/db_archive_2603/club.dat");
//var rangers = cparser.Items.Where(x => x.FullName.Contains("Rangers"));

//var nparser = await NationParser.Load("../../../db/db_archive_2603/nation.dat");
//var fparser = await NameParser.Load("../../../db/db_archive_2603/first_names.dat");
//var lparser = await NameParser.Load("../../../db/db_archive_2603/second_names.dat");

//var nf = lparser.Items.Where(x => x.NationUid == 113).ToList();
//fparser.Add(new Name(0, 113, 1, 5, "Ernando"));
//lparser.Add(new Name(0, 113, 1, 225, "Sutaryadi"));

//fparser.Add(new Name(0, 113, 1, 5, "Nadeo"));
//lparser.Add(new Name(0, 113, 1, 225, "Argawinata"));

//fparser.Add(new Name(0, 113, 1, 5, "Ramadhan"));
//lparser.Add(new Name(0, 113, 1, 225, "Sananta"));

//fparser.Add(new Name(0, 113, 1, 5, "Cahya"));
//lparser.Add(new Name(0, 113, 1, 225, "Supriadi"));

//fparser.Add(new Name(0, 113, 1, 5, "Yance"));
//lparser.Add(new Name(0, 113, 1, 225, "Sayuri"));

//fparser.Add(new Name(0, 113, 1, 5, "Pratama"));
//lparser.Add(new Name(0, 113, 1, 225, "Arhan"));

//fparser.Add(new Name(0, 113, 1, 5, "Muhammad"));
//lparser.Add(new Name(0, 113, 1, 225, "Ferarri"));

//fparser.Add(new Name(0, 113, 1, 5, "Achmad"));
//lparser.Add(new Name(0, 113, 1, 225, "Maulana"));

//fparser.Add(new Name(0, 113, 1, 5, "Arkhan"));
//lparser.Add(new Name(0, 113, 1, 225, "Fikri"));

//fparser.Add(new Name(0, 113, 1, 5, "Hokky"));
//lparser.Add(new Name(0, 113, 1, 225, "Caraka"));

//fparser.Add(new Name(0, 113, 1, 5, "Yakob"));
//lparser.Add(new Name(0, 113, 1, 225, "Kaka"));

//fparser.Add(new Name(0, 113, 1, 5, "Kakang"));
//lparser.Add(new Name(0, 113, 1, 225, "Rudianto"));

//fparser.Add(new Name(0, 113, 1, 5, "Ricky"));
//lparser.Add(new Name(0, 113, 1, 225, "Maulana"));

//fparser.Add(new Name(0, 113, 1, 5, "Fajar"));
//lparser.Add(new Name(0, 113, 1, 225, "Maulana"));

//fparser.Add(new Name(0, 113, 1, 5, "Bambang"));
//lparser.Add(new Name(0, 113, 1, 225, "Darwis"));

//fparser.Add(new Name(0, 113, 1, 5, "Kurnia"));
//lparser.Add(new Name(0, 113, 1, 225, "Putra"));

//fparser.Add(new Name(0, 113, 1, 5, "Septian"));
//lparser.Add(new Name(0, 113, 1, 225, "Hasan"));

//await fparser.Save();
//await lparser.Save();

//var query = from p in parser.Items
//            join n in nparser.Items on p.NationId equals n.Id
//            join f in fparser.Items on p.FirstNameId equals f.Id
//            join l in lparser.Items on p.LastNameId equals l.Id
//            select new
//            {
//                FirstName = f.Value,
//                LastName = l.Value,
//                Nation = n.Name,
//                People = p,
//            };

//var peoples = query.ToArray();

//var indonesian = peoples
//    .GroupBy(x => x.People.Unknown2)
//    .OrderBy(x => x.Key)
//    //.Where(x => x.People.Unknown2 == 805)
//    //.Where(x => x.People.Type == 1)
//    //.Where(x => x.People.NationalCaps == 0)
//    //.Where(x => x.People.NationId != 58)
//    //.Where(x => x.People.NationId == 58 || x.People.OtherNationalities.Contains(58))
//    //.GroupBy(x => x.Nation)
//    //.Where(x => x.People.MainLanguages.Select(x => x.Id).ToList().Contains(76) || x.People.OtherLanguages.Select(x => x.Id).ToList().Contains(76))
//    .ToList();

//var g4 = peoples.GroupBy(x => x.Ethnicity).ToList();
//var g5 = peoples.GroupBy(x => x.People.UnknownDate).ToList();

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

var peopleParser = await PeopleParser.Load("../../../db/db_archive_2603/people.dat");
var playerParser = await PlayerParser.Load("../../../db/db_archive_2603/players.dat");
var clubParser = await ClubParser.Load("../../../db/db_archive_2603/club.dat");
var nationParser = await NationParser.Load("../../../db/db_archive_2603/nation.dat");
var fnameParser = await NameParser.Load("../../../db/db_archive_2603/first_names.dat");
var lnameParser = await NameParser.Load("../../../db/db_archive_2603/second_names.dat");

var query = from people in peopleParser.Items
            join club in clubParser.Items on people.ClubId equals club.Id
            join player in playerParser.Items on people.PlayerId equals player.Id
            join nation in nationParser.Items on people.NationId equals nation.Id
            join fname in fnameParser.Items on people.FirstNameId equals fname.Id
            join lname in lnameParser.Items on people.LastNameId equals lname.Id
            select new
            {
                FirstName = fname.Value,
                LastName = lname.Value,
                People = people,
                Player = player,
                Club = club,
                Nation = nation.Name,
            };

var players = query.ToList();
var grouped = players.GroupBy(x => x.Player.Unknown1).ToList();

Console.ReadLine();

//await VerifyDatFileAsync();

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