using FMMEditor.Collections;
using FMMLibrary;
using MaterialDesignThemes.Wpf;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FMMEditor.ViewModels
{
    public enum NameType
    {
        FirstName,
        SecondName,
        CommonName
    }

    public class NamesViewModel : ReactiveObject
    {
        public extern string? FolderPath { [ObservableAsProperty] get; }
        public extern bool IsDatabaseLoaded { [ObservableAsProperty] get; }


        [Reactive] public string SearchQuery { get; set; } = "";
        public extern bool ShowSearch { [ObservableAsProperty] get; }
        public ReactiveCommand<Unit, Unit> ClearSearch { get; private set; }


        public extern NationParser? NationParser { [ObservableAsProperty] get; }
        public extern NameParser? FirstNameParser { [ObservableAsProperty] get; }
        public extern NameParser? SecondNameParser { [ObservableAsProperty] get; }
        public extern NameParser? CommonNameParser { [ObservableAsProperty] get; }


        public BulkObservableCollection<Nation> Nations { get; } = [];
        public BulkObservableCollection<Name> FirstNames { get; } = [];
        public BulkObservableCollection<Name> SecondNames { get; } = [];
        public BulkObservableCollection<Name> CommonNames { get; } = [];


        public ReactiveCommand<Unit, string?> Load { get; private set; }
        public ReactiveCommand<Unit, Unit> Save { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAs { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNations { get; private set; }
        public ReactiveCommand<string, NameParser> ParseFirstName { get; private set; }
        public ReactiveCommand<string, NameParser> ParseSecondName { get; private set; }
        public ReactiveCommand<string, NameParser> ParseCommonName { get; private set; }

        #region Dialog

        public ReactiveCommand<Name, Unit> AddCommand { get; private set; }
        public ReactiveCommand<Name, Unit> EditCommand { get; private set; }
        public ReactiveCommand<Name, Unit> DeleteCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ConfirmCommand { get; private set; }

        [Reactive] public bool ShowDialog { get; set; }
        [Reactive] public Name? SelectedName { get; set; }
        [Reactive] public int? SelectedId { get; set; }
        [Reactive] public byte? SelectedGender { get; set; }
        [Reactive] public int? SelectedNationUid { get; set; }
        [Reactive] public short? SelectedUnknown2 { get; set; }
        [Reactive] public byte? SelectedUnknown3 { get; set; }
        [Reactive] public string? SelectedValue { get; set; }
        [Reactive] public NameType ActiveNameType { get; set; }

        #endregion

        [Reactive] public ISnackbarMessageQueue MessageQueue { get; set; }

        private readonly ICollectionView firstNamesView;
        private readonly ICollectionView secondNamesView;
        private readonly ICollectionView commonNamesView;
        private readonly IDialogService dialogService;

        public NamesViewModel(IDialogService dialogService, ISnackbarMessageQueue messageQueue)
        {
            this.dialogService = dialogService;
            MessageQueue = messageQueue;

            ClearSearch = ReactiveCommand.Create(() => { SearchQuery = ""; });

            firstNamesView = CollectionViewSource.GetDefaultView(FirstNames);
            firstNamesView.Filter = obj =>
            {
                if (obj is Name name)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                        || name.Value.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || name.NationName?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) == true;
                }

                return true;
            };

            secondNamesView = CollectionViewSource.GetDefaultView(SecondNames);
            secondNamesView.Filter = obj =>
            {
                if (obj is Name name)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                        || name.Value.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || name.NationName?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) == true;
                }

                return true;
            };

            commonNamesView = CollectionViewSource.GetDefaultView(CommonNames);
            commonNamesView.Filter = obj =>
            {
                if (obj is Name name)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                        || name.Value.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || name.NationName?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) == true;
                }

                return true;
            };

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNations = ReactiveCommand.CreateFromTask<string, NationParser>(NationParser.Load);
            ParseNations.ToPropertyEx(this, vm => vm.NationParser);

            ParseFirstName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseFirstName.ToPropertyEx(this, vm => vm.FirstNameParser);

            ParseSecondName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseSecondName.ToPropertyEx(this, vm => vm.SecondNameParser);

            ParseCommonName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseCommonName.ToPropertyEx(this, vm => vm.CommonNameParser);

            AddCommand = ReactiveCommand.Create<Name>(Add);
            EditCommand = ReactiveCommand.Create<Name>(Edit);
            DeleteCommand = ReactiveCommand.Create<Name>(Delete);

            CancelCommand = ReactiveCommand.Create(Cancel);
            ConfirmCommand = ReactiveCommand.Create(Confirm);

            Save = ReactiveCommand.CreateFromTask(SaveImpl);
            SaveAs = ReactiveCommand.CreateFromTask(SaveAsImpl);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\nation.dat")
                .InvokeCommand(ParseNations);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\first_names.dat")
                .InvokeCommand(ParseFirstName);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\second_names.dat")
                .InvokeCommand(ParseSecondName);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\common_names.dat")
                .InvokeCommand(ParseCommonName);

            this.WhenAnyValue(vm => vm.FolderPath)
                .Select(x => !string.IsNullOrEmpty(x))
                .ToPropertyEx(this, vm => vm.IsDatabaseLoaded);

            this.WhenAnyValue(vm => vm.NationParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    Nations.Reset(x.Items.OrderBy(y => y.Name));
                });

            this.WhenAnyValue(vm => vm.FirstNameParser, vm => vm.NationParser)
                .Subscribe(pair =>
                {
                    var names = pair.Item1?.Items;
                    var nationParser = pair.Item2;
                    if (names != null)
                    {
                        foreach (var name in names)
                        {
                            name.NationName = nationParser?.Items.FirstOrDefault(n => n.Uid == name.NationUid)?.Name;
                        }
                        FirstNames.Reset(names.OrderBy(x => x.NationUid).ThenBy(x => x.Value));
                    }
                    else
                    {
                        FirstNames.Clear();
                    }
                });

            this.WhenAnyValue(vm => vm.SecondNameParser, vm => vm.NationParser)
                .Subscribe(pair =>
                {
                    var names = pair.Item1?.Items;
                    var nationParser = pair.Item2;
                    if (names != null)
                    {
                        foreach (var name in names)
                        {
                            name.NationName = nationParser?.Items.FirstOrDefault(n => n.Uid == name.NationUid)?.Name;
                        }
                        SecondNames.Reset(names.OrderBy(x => x.NationUid).ThenBy(x => x.Value));
                    }
                    else
                    {
                        SecondNames.Clear();
                    }
                });

            this.WhenAnyValue(vm => vm.CommonNameParser, vm => vm.NationParser)
                .Subscribe(pair =>
                {
                    var names = pair.Item1?.Items;
                    var nationParser = pair.Item2;
                    if (names != null)
                    {
                        foreach (var name in names)
                        {
                            name.NationName = nationParser?.Items.FirstOrDefault(n => n.Uid == name.NationUid)?.Name;
                        }
                        CommonNames.Reset(names.OrderBy(x => x.NationUid).ThenBy(x => x.Value));
                    }
                    else
                    {
                        CommonNames.Clear();
                    }
                });

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x =>
                {
                    firstNamesView.Refresh();
                    secondNamesView.Refresh();
                    commonNamesView.Refresh();
                });
        }

        private string? LoadImpl()
        {
            var settings = new FolderBrowserDialogSettings();
            bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            return (success == true) ? settings.SelectedPath : null;
        }

        private void Add(Name name)
        {
            SelectedId = -1;
            SelectedGender = name.Gender;
            SelectedNationUid = name.NationUid;
            SelectedUnknown2 = name.Unknown2;
            SelectedUnknown3 = name.Unknown3;
            SelectedValue = null;

            // Determine which collection the name belongs to
            if (FirstNames.Contains(name))
                ActiveNameType = NameType.FirstName;
            else if (SecondNames.Contains(name))
                ActiveNameType = NameType.SecondName;
            else
                ActiveNameType = NameType.CommonName;

            ShowDialog = true;
        }

        private void Edit(Name name)
        {
            SelectedId = name.Id;
            SelectedGender = name.Gender;
            SelectedNationUid = name.NationUid;
            SelectedUnknown2 = name.Unknown2;
            SelectedUnknown3 = name.Unknown3;
            SelectedValue = name.Value;

            // Determine which collection the name belongs to
            if (FirstNames.Contains(name))
                ActiveNameType = NameType.FirstName;
            else if (SecondNames.Contains(name))
                ActiveNameType = NameType.SecondName;
            else
                ActiveNameType = NameType.CommonName;

            ShowDialog = true;
        }

        private void Delete(Name name)
        {
            //if (SelectedClub is not null)
            //{
            //    var oldLeagueId = SelectedClub.LeagueId;

            //    SelectedClub.BasedId = SelectedClub.NationId;
            //    SelectedClub.LeagueId = -1;

            //    // Update index
            //    if (oldLeagueId >= 0 && clubsByLeagueLookup.TryGetValue((short)oldLeagueId, out var clubs))
            //    {
            //        clubs.Remove(SelectedClub);
            //    }

            //    UpdateFilteredClubs();
            //}
        }

        private void Cancel()
        {
            SelectedId = null;
            SelectedGender = null;
            SelectedNationUid = null;
            SelectedUnknown2 = null;
            SelectedUnknown3 = null;
            SelectedValue = null;

            ShowDialog = false;
        }

        private void Confirm()
        {
            if (string.IsNullOrWhiteSpace(SelectedValue) || SelectedGender is null || SelectedNationUid is null || SelectedUnknown2 is null || SelectedUnknown3 is null)
            {
                ShowDialog = false;
                return;
            }

            var collection = ActiveNameType switch
            {
                NameType.FirstName => FirstNames,
                NameType.SecondName => SecondNames,
                NameType.CommonName => CommonNames,
                _ => null
            };

            if (collection is null)
            {
                ShowDialog = false;
                return;
            }

            var nationName = NationParser?.Items.FirstOrDefault(n => n.Uid == SelectedNationUid)?.Name;

            // Adding new names (SelectedId == -1)
            if (SelectedId == -1)
            {
                // Split by newlines and add each name
                var names = SelectedValue
                    .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => n.Trim())
                    .Where(n => !string.IsNullOrWhiteSpace(n));

                var maxId = collection.Count > 0 ? collection.Max(x => x.Id) : 0;

                foreach (var nameValue in names)
                {
                    var newName = new Name(
                        SelectedGender.Value,
                        SelectedNationUid.Value,
                        SelectedUnknown2.Value,
                        SelectedUnknown3.Value,
                        nameValue)
                    {
                        Id = ++maxId,
                        NationName = nationName
                    };

                    collection.Add(newName);
                }

                MessageQueue.Enqueue($"Added {names.Count()} name(s)");
            }
            // Editing existing name
            else
            {
                var existingName = collection.FirstOrDefault(x => x.Id == SelectedId);
                if (existingName is not null)
                {
                    existingName.Gender = SelectedGender.Value;
                    existingName.NationUid = SelectedNationUid.Value;
                    existingName.Unknown2 = SelectedUnknown2.Value;
                    existingName.Unknown3 = SelectedUnknown3.Value;
                    existingName.Value = SelectedValue.Trim();
                    existingName.NationName = nationName;

                    MessageQueue.Enqueue("Name updated");
                }
            }

            // Reset dialog state
            SelectedId = null;
            SelectedGender = null;
            SelectedNationUid = null;
            SelectedUnknown2 = null;
            SelectedUnknown3 = null;
            SelectedValue = null;

            ShowDialog = false;
        }

        private async Task SaveImpl()
        {
            try
            {
                if (FirstNameParser != null && SecondNameParser != null && CommonNameParser != null)
                {
                    FirstNameParser.Replace(FirstNames);
                    await FirstNameParser.Save();

                    SecondNameParser.Replace(SecondNames);
                    await SecondNameParser.Save();

                    CommonNameParser.Replace(CommonNames);
                    await CommonNameParser.Save();

                    MessageQueue.Enqueue("Save Successfull");
                }
            }
            catch (Exception e)
            {
                dialogService.ShowMessageBox(this, $"Save error: {e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SaveAsImpl()
        {
            var settings = new FolderBrowserDialogSettings();
            bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            if (success != true) return;

            try
            {
                if (FirstNameParser != null && SecondNameParser != null && CommonNameParser != null)
                {
                    FirstNameParser.Replace(FirstNames);
                    await FirstNameParser.Save(settings.SelectedPath + "\\first_name.dat");

                    SecondNameParser.Replace(SecondNames);
                    await SecondNameParser.Save(settings.SelectedPath + "\\second_name.dat");

                    CommonNameParser.Replace(CommonNames);
                    await CommonNameParser.Save(settings.SelectedPath + "\\common_name.dat");

                    MessageQueue.Enqueue("Save Successfull");
                }
            }
            catch (Exception e)
            {
                dialogService.ShowMessageBox(this, $"Save error: {e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
