using FMMLibrary;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FMM.Models;

public class ClubDisplayModel : INotifyPropertyChanged
{
    private bool isAlwaysLoad;

    public ClubDisplayModel(Club club)
    {
        Club = club;
    }

    public Club Club { get; }
    public int Id => Club.Id;
    public int Uid => Club.Uid;
    public string FullName => Club.FullName;
    public string ShortName => Club.ShortName;
    public string SixLetterName => Club.SixLetterName;
    public string ThreeLetterName => Club.ThreeLetterName;
    public string BasedName { get; set; } = string.Empty;
    public string NationName { get; set; } = string.Empty;
    public string CompetitionName { get; set; } = string.Empty;
    public byte Status => Club.Status;
    public string StatusText => Status switch
    {
        0 => "National",
        1 => "Professional",
        2 => "Semi-Pro",
        3 => "Amateur",
        _ => $"Status: {Status}"
    };
    public byte Academy => Club.Academy;
    public byte Facilities => Club.Facilities;
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
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
