using FMM.Models;
using FMMLibrary;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace FMM.ViewModels;

public partial class ClubEditViewModel : ViewModelBase
{
    [Reactive] private bool _isAddMode;
    [Reactive] private Stadium? _selectedStadium;
    [Reactive] private Competition? _selectedCompetition;
    [Reactive] private Competition? _selectedLastLeague;

    [Reactive] private int? _uid;
    [Reactive] private string? _fullName;
    [Reactive] private byte _fullNameTerminator;
    [Reactive] private string? _shortName;
    [Reactive] private byte _shortNameTerminator;
    [Reactive] private string? _sixLetterName;
    [Reactive] private string? _threeLetterName;
    [Reactive] private short? _basedId;
    [Reactive] private short? _nationId;
    [Reactive] private byte _status;
    [Reactive] private byte? _academy;
    [Reactive] private byte? _facilities;
    [Reactive] private short? _attAvg;
    [Reactive] private short? _attMin;
    [Reactive] private short? _attMax;
    [Reactive] private byte? _reserves;
    [Reactive] private short? _leagueId;
    [Reactive] private short _otherDivision;
    [Reactive] private byte _otherLastPosition;
    [Reactive] private byte? _leaguePos;
    [Reactive] private short? _reputation;
    [Reactive] private short? _stadium;
    [Reactive] private short? _lastLeague;
    [Reactive] private int? _mainClub;
    [Reactive] private byte _type;
    [Reactive] private byte _gender;

    [Reactive] private Color _color1;
    [Reactive] private Color _color2;
    [Reactive] private Color _color3;
    [Reactive] private Color _color4;
    [Reactive] private Color _color5;
    [Reactive] private Color _color6;

    [Reactive] private Kit[] _kits = [];
    [Reactive] private Affiliate[] _affiliates = [];

    [Reactive] private bool _unknown4Flag;
    [Reactive] private byte[] _unknown4 = [];
    [Reactive] private byte[] _unknown5 = [];
    [Reactive] private byte[] _unknown6 = [];
    [Reactive] private int[] _unknown7 = [];
    [Reactive] private byte[] _unknown8 = [];
    [Reactive] private byte[] _unknown9 = [];

    public string WindowTitle => IsAddMode ? "Add New Club" : "Edit Club";

    public ObservableCollection<Nation> Nations { get; }
    public ObservableCollection<Stadium> Stadiums { get; }
    public ObservableCollection<Competition> Competitions { get; }
    public ObservableCollection<PlayerInfo> Players { get; } = [];

    private readonly Dictionary<int, People> peopleLookup;
    private readonly Dictionary<int, string> firstNameLookup;
    private readonly Dictionary<int, string> lastNameLookup;
    private readonly Dictionary<int, string> commonNameLookup;

    public List<StatusOption> StatusOptions { get; } =
    [
        new StatusOption { Value = 0, DisplayName = "National" },
        new StatusOption { Value = 1, DisplayName = "Professional" },
        new StatusOption { Value = 2, DisplayName = "Semi-Pro" },
        new StatusOption { Value = 3, DisplayName = "Amateur" },
        new StatusOption { Value = 22, DisplayName = "Unknown" }
    ];

    public event Action<ClubEditViewModel, bool>? CloseRequested;

    public ClubEditViewModel(
        ObservableCollection<Nation> nations,
        ObservableCollection<Stadium> stadiums,
        ObservableCollection<Competition> competitions,
        Dictionary<int, People> peopleLookup,
        Dictionary<int, string> firstNameLookup,
        Dictionary<int, string> lastNameLookup,
        Dictionary<int, string> commonNameLookup)
    {
        Nations = nations;
        Stadiums = stadiums;
        Competitions = competitions;
        this.peopleLookup = peopleLookup;
        this.firstNameLookup = firstNameLookup;
        this.lastNameLookup = lastNameLookup;
        this.commonNameLookup = commonNameLookup;

        this.WhenAnyValue(x => x.IsAddMode)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));

        this.WhenAnyValue(x => x.SelectedStadium)
            .Subscribe(s => Stadium = s != null ? (short?)s.Id : (short?)-1);

        this.WhenAnyValue(x => x.SelectedCompetition)
            .Subscribe(c => LeagueId = c != null ? c.Id : (short?)-1);

        this.WhenAnyValue(x => x.SelectedLastLeague)
            .Subscribe(c => LastLeague = c != null ? c.Id : (short?)-1);
    }

    [ReactiveCommand]
    private void Save() => CloseRequested?.Invoke(this, true);

    [ReactiveCommand]
    private void Cancel() => CloseRequested?.Invoke(this, false);

    public void InitializeForAdd()
    {
        IsAddMode = true;
        ResetToDefaults();
    }

    public void InitializeForEdit(ClubDisplayModel club)
    {
        IsAddMode = false;
        LoadFromClub(club);
    }

    public void InitializeForCopy(ClubDisplayModel club)
    {
        IsAddMode = true;
        LoadFromClub(club);
        Uid = null;
    }

    private void LoadFromClub(ClubDisplayModel club)
    {
        var c = club.Club;

        Uid = c.Uid;
        FullName = c.FullName;
        FullNameTerminator = c.FullNameTerminator;
        ShortName = c.ShortName;
        ShortNameTerminator = c.ShortNameTerminator;
        SixLetterName = c.SixLetterName;
        ThreeLetterName = c.ThreeLetterName;
        BasedId = c.BasedId;
        NationId = c.NationId;
        Status = c.Status;
        Academy = c.Academy;
        Facilities = c.Facilities;
        AttAvg = c.AttAvg;
        AttMin = c.AttMin;
        AttMax = c.AttMax;
        Reserves = c.Reserves;
        LeagueId = c.LeagueId;
        SelectedCompetition = Competitions.FirstOrDefault(comp => comp.Id == c.LeagueId);
        OtherDivision = c.OtherDivision;
        OtherLastPosition = c.OtherLastPosition;
        LeaguePos = c.LeaguePos;
        Reputation = c.Reputation;
        Stadium = c.Stadium;
        SelectedStadium = Stadiums.FirstOrDefault(s => s.Id == c.Stadium);
        LastLeague = c.LastLeague;
        SelectedLastLeague = Competitions.FirstOrDefault(comp => comp.Id == c.LastLeague);
        MainClub = c.MainClub;
        Type = c.Type;
        Gender = c.Gender;

        LoadColors(c.Colors);
        LoadKits(c.Kits);
        LoadAffiliates(c.Affiliates);
        LoadUnknownFields(c);
        LoadPlayers(c.Players);
    }

    private void ResetToDefaults()
    {
        Uid = null;
        FullName = string.Empty;
        FullNameTerminator = 0;
        ShortName = string.Empty;
        ShortNameTerminator = 0;
        SixLetterName = string.Empty;
        ThreeLetterName = string.Empty;
        BasedId = null;
        NationId = null;
        Status = 1;
        Academy = 10;
        Facilities = 10;
        AttAvg = 0;
        AttMin = 0;
        AttMax = 0;
        Reserves = 0;
        LeagueId = -1;
        SelectedCompetition = null;
        OtherDivision = -1;
        OtherLastPosition = 0;
        LeaguePos = 0;
        Reputation = 0;
        Stadium = -1;
        SelectedStadium = null;
        LastLeague = -1;
        SelectedLastLeague = null;
        MainClub = -1;
        Type = 0;
        Gender = 0;

        ResetColors();
        ResetKits();
        ResetAffiliates();
        ResetUnknownFields();
        Players.Clear();
    }

    private void LoadColors(Color[] colors)
    {
        if (colors != null && colors.Length >= 6)
        {
            Color1 = colors[0];
            Color2 = colors[1];
            Color3 = colors[2];
            Color4 = colors[3];
            Color5 = colors[4];
            Color6 = colors[5];
        }
        else
        {
            ResetColors();
        }
    }

    private void ResetColors()
    {
        Color1 = Color.White;
        Color2 = Color.Black;
        Color3 = Color.White;
        Color4 = Color.Black;
        Color5 = Color.White;
        Color6 = Color.Black;
    }

    private void LoadKits(Kit[] kits)
    {
        if (kits != null && kits.Length >= 6)
        {
            Kits = new Kit[6];
            for (int i = 0; i < 6; i++)
            {
                Kits[i] = kits[i];
            }
        }
        else
        {
            ResetKits();
        }
    }

    private void ResetKits()
    {
        Kits = new Kit[6];
        for (int i = 0; i < 6; i++)
        {
            Kits[i] = new Kit();
        }
    }

    private void LoadAffiliates(Affiliate[] affiliates)
    {
        Affiliates = affiliates ?? [];
    }

    private void ResetAffiliates()
    {
        Affiliates = [];
    }

    private void LoadUnknownFields(Club club)
    {
        Unknown4Flag = club.Unknown4Flag;
        Unknown4 = club.Unknown4 ?? [];
        Unknown5 = club.Unknown5 ?? [];
        Unknown6 = club.Unknown6 ?? new byte[20];
        Unknown7 = club.Unknown7 ?? new int[11];
        Unknown8 = club.Unknown8 ?? new byte[34];
        Unknown9 = club.Unknown9 ?? new byte[41];
    }

    private void ResetUnknownFields()
    {
        Unknown4Flag = false;
        Unknown4 = [];
        Unknown5 = [];
        Unknown6 = new byte[20];
        Unknown7 = new int[11];
        Unknown8 = new byte[34];
        Unknown9 = new byte[41];
    }

    private void LoadPlayers(int[] players)
    {
        Players.Clear();
        if (players == null)
            return;

        for (int i = 0; i < players.Length; i++)
        {
            Players.Add(new PlayerInfo
            {
                Index = i + 1,
                PlayerId = players[i],
                PlayerName = ResolvePlayerName(players[i])
            });
        }
    }

    private string ResolvePlayerName(int playerId)
    {
        if (!peopleLookup.TryGetValue(playerId, out var person))
            return $"Unknown (ID: {playerId})";

        var firstName = firstNameLookup.GetValueOrDefault(person.FirstNameId, string.Empty);
        var lastName = lastNameLookup.GetValueOrDefault(person.LastNameId, string.Empty);
        var commonName = commonNameLookup.GetValueOrDefault(person.CommonNameId, string.Empty);

        if (!string.IsNullOrEmpty(commonName))
            return commonName;

        if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            return $"{firstName} {lastName}";

        if (!string.IsNullOrEmpty(lastName))
            return lastName;

        if (!string.IsNullOrEmpty(firstName))
            return firstName;

        return $"Unknown (ID: {playerId})";
    }

    public bool Validate() => !string.IsNullOrWhiteSpace(FullName) && NationId != null;
}

public class StatusOption
{
    public byte Value { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public class PlayerInfo
{
    public int Index { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
}
