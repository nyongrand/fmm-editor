using FMMEditor.Collections;
using FMMEditor.Models;
using FMMLibrary;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FMMEditor.ViewModels
{
    public class ClubEditViewModel : ReactiveObject
    {
        [Reactive] public bool IsAddMode { get; set; }
        public string WindowTitle => IsAddMode ? "Add New Club" : "Edit Club";

        public BulkObservableCollection<Nation> Nations { get; }
        public List<StatusOption> StatusOptions { get; } = new()
        {
            new StatusOption { Value = 0, DisplayName = "National" },
            new StatusOption { Value = 1, DisplayName = "Professional" },
            new StatusOption { Value = 2, DisplayName = "Semi-Pro" },
            new StatusOption { Value = 3, DisplayName = "Amateur" },
            new StatusOption { Value = 22, DisplayName = "Unknown" }
        };

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

        public ClubEditViewModel(BulkObservableCollection<Nation> nations)
        {
            Nations = nations;

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
        }

        public bool Validate() => !string.IsNullOrWhiteSpace(FullName) && NationId != null;
    }

    public class StatusOption
    {
        public byte Value { get; set; }
        public string DisplayName { get; set; } = "";
    }
}
