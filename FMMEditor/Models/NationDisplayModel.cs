using FMMLibrary;
using System.Linq;

namespace FMMEditor.Models
{
    /// <summary>
    /// Display model for Nation with resolved and formatted values
    /// </summary>
    public class NationDisplayModel
    {
        public NationDisplayModel(Nation nation)
        {
            Nation = nation;
        }

        public Nation Nation { get; set; }
        public int Uid => Nation.Uid;
        public short Id => Nation.Id;
        public string Name => Nation.Name;
        public string Nationality => Nation.Nationality;
        public string CodeName => Nation.CodeName;
        public short ContinentId => Nation.ContinentId;
        public string ContinentName { get; set; } = "";
        public short CapitalId => Nation.CapitalId;
        public short StadiumId => Nation.StadiumId;
        public byte StateOfDevelopment => Nation.StateOfDevelopment;
        public string StateOfDevelopmentText => StateOfDevelopment switch
        {
            1 => "Developed",
            2 => "Developing",
            3 => "Third World",
            _ => $"State {StateOfDevelopment}"
        };
        public short Region => Nation.Region;
        public byte LanguageCount => (byte)(Nation.Languages?.Length ?? 0);
        public string Languages => Nation.Languages?.Length > 0
            ? string.Join(", ", Nation.Languages.Select(l => $"{l.Id} ({l.Proficiency})"))
            : string.Empty;
        public bool HasMaleTeam => Nation.HasMaleTeam;
        public bool HasFemaleTeam => Nation.HasFemaleTeam;
    }
}
