using FMMEditor.Converters;
using FMMLibrary;

namespace FMMEditor.Models
{
    /// <summary>
    /// Display model for People with resolved names
    /// </summary>
    public class PeopleDisplayModel(People person)
    {
        public People Person { get; set; } = person;
        public int Uid => Person.Uid;
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string CommonName { get; set; } = "";
        public string FullName => !string.IsNullOrEmpty(CommonName) ? CommonName : $"{FirstName} {LastName}".Trim();
        public string NationName { get; set; } = "";
        public string ClubName { get; set; } = "";
        public string DateOfBirth => DateOfBirthConverter.ToDateString(Person.DateOfBirth);
        public string PersonType => Person.Type switch { 0 => "Player", 1 => "Staff", _ => "Other" };
        public short NationalCaps => Person.NationalCaps;
        public short NationalGoals => Person.NationalGoals;
        public byte Adaptability => Person.Adaptability;
        public byte Ambition => Person.Ambition;
        public byte Loyalty => Person.Loyality;
        public byte Pressure => Person.Pressure;
        public byte Professionalism => Person.Professionalism;
        public byte Temperament => Person.Temperament;
    }
}
