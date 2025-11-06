using FMELibrary;
using System.Collections;


//var compParser = new CompetitionParser("D:\\Downloads\\database\\competition.dat");
//var clubParser = new ClubParser("D:\\Downloads\\database\\club.dat");

//var competitions = compParser.Items;
//var clubs = compParser.Items;

//var q = from comp in compParser.Items
//        join club in clubParser.Items on comp.Id equals club.League into grouping
//        select new { Competition = comp, Clubs = grouping.ToList() };

//Console.WriteLine("");


var file = "../../../db/db_archive_2603/nation.dat";
var parser = await NationParser.Load(file);

var bytes1 = parser.ToBytes();
var bytes2 = await File.ReadAllBytesAsync(file);

for (int i = 0; i < bytes2.Length; i++)
{
    if (bytes1[i] != bytes2[i])
    {

    }
}

var equal = bytes1.SequenceEqual(bytes2);
var count = parser.Count;

//while (true)
//{
//    //Console.WriteLine("Enter new name to add!");

//    var name = Console.ReadLine();
//    if (string.IsNullOrEmpty(name))
//        break;

//    //var bytes = new List<byte> { 0, 0, 0, 0 };
//    bytes.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
//    bytes.AddRange(BitConverter.GetBytes(count++));
//    bytes.AddRange(new byte[] { 0x00, 0x71, 0x00, 0x00 });
//    bytes.AddRange(new byte[] { 0x00, 0x02, 0x00, 0xff });
//    bytes.AddRange(BitConverter.GetBytes(name.Length));
//    bytes.AddRange(Encoding.UTF8.GetBytes(name));

//    //hex += BitConverter.ToString(bytes.ToArray()).Replace("-", " ") + " ";
//    //var hex = BitConverter.ToString(bytes.ToArray()).Replace("-", " ");
//    //Console.WriteLine(hex);

//    //var h = "            ^^                                  ^^          ";
//    //Console.WriteLine(h.PadRight(hex.Length, '^'));
//}

//await File.WriteAllBytesAsync(file.Replace(".dat", "_new.dat"), bytes.ToArray());

