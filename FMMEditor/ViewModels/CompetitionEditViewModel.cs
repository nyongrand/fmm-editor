using FMMEditor.Collections;
using FMMEditor.Models;
using FMMLibrary;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FMMEditor.ViewModels
{
    public class CompetitionEditViewModel : ReactiveObject
    {
        [Reactive] public bool IsAddMode { get; set; }
        public string WindowTitle => IsAddMode ? "Add New Competition" : "Edit Competition";

        public BulkObservableCollection<Nation> Nations { get; }

        public List<TypeOption> TypeOptions { get; } =
        [
            new TypeOption { Value = 0, DisplayName = "Domestic Top Division" },
            new TypeOption { Value = 1, DisplayName = "Domestic Division" },
            new TypeOption { Value = 2, DisplayName = "Domestic Main Cup" },
            new TypeOption { Value = 3, DisplayName = "Domestic League Cup" },
            new TypeOption { Value = 4, DisplayName = "Domestic Cup" },
            new TypeOption { Value = 5, DisplayName = "Super Cup" },
            new TypeOption { Value = 6, DisplayName = "Reserve Division" },
            new TypeOption { Value = 7, DisplayName = "U23 Division" },
            new TypeOption { Value = 8, DisplayName = "U22 Division" },
            new TypeOption { Value = 9, DisplayName = "U21 Division" },
            new TypeOption { Value = 10, DisplayName = "U20 Division" },
            new TypeOption { Value = 11, DisplayName = "U19 Division" },
            new TypeOption { Value = 12, DisplayName = "U18 Division" },
            new TypeOption { Value = 13, DisplayName = "Reserve Cup" }
        ];

        // Competition fields
        [Reactive] public int? Uid { get; set; }
        [Reactive] public string? FullName { get; set; }
        [Reactive] public byte FullNameTerminator { get; set; }
        [Reactive] public string? ShortName { get; set; }
        [Reactive] public byte ShortNameTerminator { get; set; }
        [Reactive] public string? CodeName { get; set; }
        [Reactive] public byte Type { get; set; }
        [Reactive] public short ContinentId { get; set; }
        [Reactive] public short? NationId { get; set; }
        [Reactive] public Color ForegroundColor { get; set; }
        [Reactive] public Color BackgroundColor { get; set; }
        [Reactive] public short Reputation { get; set; }
        [Reactive] public byte Level { get; set; }
        [Reactive] public short ParentCompetitionId { get; set; }
        [Reactive] public byte[][] Qualifiers { get; set; } = [];
        [Reactive] public int Rank1 { get; set; }
        [Reactive] public int Rank2 { get; set; }
        [Reactive] public int Rank3 { get; set; }
        [Reactive] public short Year1 { get; set; }
        [Reactive] public short Year2 { get; set; }
        [Reactive] public short Year3 { get; set; }
        [Reactive] public byte Unknown3 { get; set; }
        [Reactive] public bool IsWomen { get; set; }

        public CompetitionEditViewModel(BulkObservableCollection<Nation> nations)
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

        public void InitializeForEdit(CompetitionDisplayModel competition)
        {
            if (competition == null) return;

            IsAddMode = false;
            LoadFromCompetition(competition);
        }

        public void InitializeForCopy(CompetitionDisplayModel competition)
        {
            if (competition == null) return;

            IsAddMode = true;
            LoadFromCompetition(competition);
            Uid = null;
        }

        private void LoadFromCompetition(CompetitionDisplayModel competition)
        {
            var c = competition.Competition;

            Uid = c.Uid;
            FullName = c.FullName;
            FullNameTerminator = c.FullNameTerminator;
            ShortName = c.ShortName;
            ShortNameTerminator = c.ShortNameTerminator;
            CodeName = c.CodeName;
            Type = c.Type;
            ContinentId = c.ContinentId;
            NationId = c.NationId;
            ForegroundColor = c.ForegroundColor;
            BackgroundColor = c.BackgroundColor;
            Reputation = c.Reputation;
            Level = c.Level;
            ParentCompetitionId = c.ParentCompetitionId;
            Qualifiers = c.Qualifiers ?? [];
            Rank1 = c.Rank1;
            Rank2 = c.Rank2;
            Rank3 = c.Rank3;
            Year1 = c.Year1;
            Year2 = c.Year2;
            Year3 = c.Year3;
            Unknown3 = c.Unknown3;
            IsWomen = c.IsWomen;
        }

        private void ResetToDefaults()
        {
            Uid = null;
            FullName = "";
            FullNameTerminator = 0;
            ShortName = "";
            ShortNameTerminator = 0;
            CodeName = "";
            Type = 0;
            ContinentId = 0;
            NationId = null;
            ForegroundColor = Color.White;
            BackgroundColor = Color.Black;
            Reputation = 0;
            Level = 1;
            ParentCompetitionId = -1;
            Qualifiers = [];
            Rank1 = 0;
            Rank2 = 0;
            Rank3 = 0;
            Year1 = 0;
            Year2 = 0;
            Year3 = 0;
            Unknown3 = 0;
            IsWomen = false;
        }

        public bool Validate() => !string.IsNullOrWhiteSpace(FullName) && NationId != null;
    }

    public class TypeOption
    {
        public byte Value { get; set; }
        public string DisplayName { get; set; } = "";
    }
}
