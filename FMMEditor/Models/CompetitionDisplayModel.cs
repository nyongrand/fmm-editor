using FMMLibrary;

namespace FMMEditor.Models
{
    /// <summary>
    /// Display model for Competition with resolved names
    /// </summary>
    public class CompetitionDisplayModel(Competition competition)
    {
        public Competition Competition { get; set; } = competition;
        public short Id => Competition.Id;
        public int Uid => Competition.Uid;
        public string FullName => Competition.FullName;
        public string ShortName => Competition.ShortName;
        public string CodeName => Competition.CodeName;
        public string NationName { get; set; } = "";
        public byte Type => Competition.Type;
        public string TypeText => Type switch
        {
            0 => "Domestic Top Division",
            1 => "Domestic Division",
            2 => "Domestic Main Cup",
            3 => "Domestic League Cup",
            4 => "Domestic Cup",
            5 => "Super Cup",
            6 => "Reserve Division",
            7 => "U23 Division",
            8 => "U22 Division",
            9 => "U21 Division",
            10 => "U20 Division",
            11 => "U19 Division",
            12 => "U18 Division",
            13 => "Reserve Cup",
            _ => $"Type: {Type}"
        };
        public short Reputation => Competition.Reputation;
        public byte Level => Competition.Level;
        public bool IsWomen => Competition.IsWomen;
    }
}
