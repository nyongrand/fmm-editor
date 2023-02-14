using FMELibrary;
using System.Text;


var file = "D:\\Downloads\\database\\club.dat";
var parser = new ClubParser(file);
//await parser.ParseAsync();

var bytes = (await File.ReadAllBytesAsync(file)).ToList();
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

