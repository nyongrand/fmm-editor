using FMMLibrary;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FMMEditor.Models
{
    /// <summary>
    /// Display model for Club with resolved names
    /// </summary>
    public class ClubDisplayModel(Club club) : INotifyPropertyChanged
    {
        private bool isAlwaysLoad;

        public Club Club { get; set; } = club;
        public int Id => Club.Id;
        public int Uid => Club.Uid;
        public string FullName => Club.FullName;
        public string ShortName => Club.ShortName;
        public string SixLetterName => Club.SixLetterName;
        public string ThreeLetterName => Club.ThreeLetterName;
        public string BasedName { get; set; } = "";
        public string NationName { get; set; } = "";
        public string CompetitionName { get; set; } = "";
        public byte Status => Club.Status;
        public string StatusText => Status switch
        {
            0 => "National",
            1 => "Professional",
            2 => "Semi-Pro",
            3 => "Amateur",
            _ => $"Status: {Status}"
        };
        public sbyte Academy => Club.Academy;
        public sbyte Facilities => Club.Facilities;
        public short AttAvg => Club.AttAvg;
        public short AttMin => Club.AttMin;
        public short AttMax => Club.AttMax;
        public byte Reserves => Club.Reserves;
        public short LeagueId => Club.LeagueId;
        public byte LeaguePos => Club.LeaguePos;
        public short Reputation => Club.Reputation;
        public int MainClub => Club.MainClub;
        public byte Type => Club.Type;
        public string TypeText => Type switch
        {
            0 => "Club",
            1 => "National",
            2 => "Unknown",
            _ => $"Type: {Type}"
        };
        public byte Gender => Club.Gender;
        public string GenderText => Gender == 1 ? "Yes" : "No";
        public int PlayerCount => Club.Players?.Length ?? 0;

        public bool IsAlwaysLoad
        {
            get => isAlwaysLoad;
            set
            {
                if (isAlwaysLoad != value)
                {
                    isAlwaysLoad = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
