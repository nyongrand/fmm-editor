using FMMEditor.Collections;
using FMMEditor.Models;
using FMMLibrary;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace FMMEditor.ViewModels
{
    public class ClubEditViewModel : ReactiveObject
    {
        [Reactive] public bool IsAddMode { get; set; }
        public string WindowTitle => IsAddMode ? "Add New Club" : "Edit Club";

        public BulkObservableCollection<Nation> Nations { get; }
        public ObservableCollection<PlayerInfo> Players { get; } = new();
        
        private readonly Dictionary<int, People> peopleLookup;
        private readonly Dictionary<int, Player> playerLookup;
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

        // Club fields
        [Reactive] public int? Uid { get; set; }
        [Reactive] public string? FullName { get; set; }
        [Reactive] public string? ShortName { get; set; }
        [Reactive] public string? SixLetterName { get; set; }
        [Reactive] public string? ThreeLetterName { get; set; }
        [Reactive] public short? BasedId { get; set; }
        [Reactive] public short? NationId { get; set; }
        [Reactive] public byte Status { get; set; }
        [Reactive] public byte? Academy { get; set; }
        [Reactive] public byte? Facilities { get; set; }
        [Reactive] public short? AttAvg { get; set; }
        [Reactive] public short? AttMin { get; set; }
        [Reactive] public short? AttMax { get; set; }
        [Reactive] public byte? Reserves { get; set; }
        [Reactive] public short? LeagueId { get; set; }
        [Reactive] public byte? LeaguePos { get; set; }
        [Reactive] public short? Reputation { get; set; }
        [Reactive] public short? Stadium { get; set; }
        [Reactive] public short? LastLeague { get; set; }
        [Reactive] public int? MainClub { get; set; }
        [Reactive] public short IsNational { get; set; }
        [Reactive] public short IsWomanFlag { get; set; }

        // Club colors (6 colors)
        [Reactive] public Color Color1 { get; set; }
        [Reactive] public Color Color2 { get; set; }
        [Reactive] public Color Color3 { get; set; }
        [Reactive] public Color Color4 { get; set; }
        [Reactive] public Color Color5 { get; set; }
        [Reactive] public Color Color6 { get; set; }

        // Unknown fields
        [Reactive] public bool Unknown4Flag { get; set; }
        [Reactive] public byte[] Unknown4 { get; set; }
        [Reactive] public byte[] Unknown5 { get; set; }
        [Reactive] public byte[] Unknown6 { get; set; }
        [Reactive] public int[] Unknown7 { get; set; }
        [Reactive] public byte[] Unknown8 { get; set; }
        [Reactive] public byte[] Unknown9 { get; set; }

        public ClubEditViewModel(
            BulkObservableCollection<Nation> nations,
            Dictionary<int, People> peopleLookup,
            Dictionary<int, Player> playerLookup,
            Dictionary<int, string> firstNameLookup,
            Dictionary<int, string> lastNameLookup,
            Dictionary<int, string> commonNameLookup)
        {
            Nations = nations;
            this.peopleLookup = peopleLookup;
            this.playerLookup = playerLookup;
            this.firstNameLookup = firstNameLookup;
            this.lastNameLookup = lastNameLookup;
            this.commonNameLookup = commonNameLookup;

            this.WhenAnyValue(x => x.IsAddMode)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));
        }

        public void InitializeForAdd()
        {
            IsAddMode = true;
            ResetToDefaults();
        }

        public void InitializeForEdit(ClubDisplayModel club)
        {
            if (club == null) return;

            IsAddMode = false;
            LoadFromClub(club);
        }

        public void InitializeForCopy(ClubDisplayModel club)
        {
            if (club == null) return;

            IsAddMode = true;
            LoadFromClub(club);
            Uid = null;
        }

        private void LoadFromClub(ClubDisplayModel club)
        {
            var c = club.Club;

            Uid = c.Uid;
            FullName = c.FullName;
            ShortName = c.ShortName;
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
            LeaguePos = c.LeaguePos;
            Reputation = c.Reputation;
            Stadium = c.Stadium;
            LastLeague = c.LastLeague;
            MainClub = c.MainClub;
            IsNational = c.IsNational;
            IsWomanFlag = c.IsWomanFlag;
            
            LoadColors(c.Colors);
            LoadUnknownFields(c);
            LoadPlayers(c.Players);
        }

        private void ResetToDefaults()
        {
            Uid = null;
            FullName = "";
            ShortName = "";
            SixLetterName = "";
            ThreeLetterName = "";
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
            LeaguePos = 0;
            Reputation = 0;
            Stadium = -1;
            LastLeague = -1;
            MainClub = -1;
            IsNational = 0;
            IsWomanFlag = 0;
            
            ResetColors();
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

        private void LoadUnknownFields(Club club)
        {
            Unknown4Flag = club.Unknown4Flag;
            Unknown4 = club.Unknown4 ?? [];
            Unknown5 = club.Unknown5 ?? [];
            Unknown6 = club.Unknown6 ?? new byte[20];
            Unknown7 = club.Unknown7 ?? new int[11];
            Unknown8 = club.Unknown8 ?? new byte[33];
            Unknown9 = club.Unknown9 ?? new byte[40];
        }

        private void ResetUnknownFields()
        {
            Unknown4Flag = false;
            Unknown4 = [];
            Unknown5 = [];
            Unknown6 = new byte[20];
            Unknown7 = new int[11];
            Unknown8 = new byte[33];
            Unknown9 = new byte[40];
        }
        
        private void LoadPlayers(int[] playerIds)
        {
            Players.Clear();
            
            if (playerIds == null || playerIds.Length == 0)
                return;
                
            for (int i = 0; i < playerIds.Length; i++)
            {
                var playerId = playerIds[i];
                var playerName = ResolvePlayerName(playerId);
                
                Players.Add(new PlayerInfo
                {
                    Index = i + 1,
                    PlayerId = playerId,
                    PlayerName = playerName
                });
            }
        }
        
        private string ResolvePlayerName(int playerId)
        {
            if (!peopleLookup.TryGetValue(playerId, out var person))
                return $"Unknown (ID: {playerId})";
            
            var firstName = firstNameLookup.GetValueOrDefault(person.FirstNameId, "");
            var lastName = lastNameLookup.GetValueOrDefault(person.LastNameId, "");
            var commonName = commonNameLookup.GetValueOrDefault(person.CommonNameId, "");
            
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
        public string DisplayName { get; set; } = "";
    }
    
    public class PlayerInfo
    {
        public int Index { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = "";
    }
}
