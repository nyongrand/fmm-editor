using FMMLibrary;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FMM.ViewModels;

public enum NameType
{
    FirstName,
    SecondName,
    CommonName
}

public partial class NameListViewModel : ViewModelBase
{
    [Reactive] private string? _folderPath;
    [Reactive] private bool _isDatabaseLoaded;
    [Reactive] private string _searchQuery = string.Empty;
    [Reactive] private Name? _selectedName;

    [Reactive] private bool _showDialog;
    [Reactive] private int? _selectedId;
    [Reactive] private int? _selectedGenderIndex;
    [Reactive] private Nation? _selectedNation;
    [Reactive] private short? _selectedUnknown2;
    [Reactive] private byte? _selectedUnknown3;
    [Reactive] private string? _selectedValue;
    [Reactive] private NameType _activeNameType;
    [Reactive] private string _statusMessage = string.Empty;

    public ObservableCollection<Nation> Nations { get; } = [];
    public ObservableCollection<Name> FirstNames { get; } = [];
    public ObservableCollection<Name> SecondNames { get; } = [];
    public ObservableCollection<Name> CommonNames { get; } = [];

    public IEnumerable<Name> FilteredFirstNames => FilterBySearch(FirstNames);
    public IEnumerable<Name> FilteredSecondNames => FilterBySearch(SecondNames);
    public IEnumerable<Name> FilteredCommonNames => FilterBySearch(CommonNames);

    public bool HasSearchQuery => !string.IsNullOrWhiteSpace(SearchQuery);
    public bool IsDatabaseEmpty => !IsDatabaseLoaded;
    public bool IsAddMode => SelectedId == -1;
    public bool IsEditMode => SelectedId is not null && SelectedId != -1;
    public string DialogTitle => IsAddMode ? "Add Names" : "Edit Name";

    private NationParser? nationParser;
    private NameParser? firstNameParser;
    private NameParser? secondNameParser;
    private NameParser? commonNameParser;

    readonly IObservable<bool> _canSave;

    public NameListViewModel()
    {
        _canSave = this.WhenAnyValue(vm => vm.IsDatabaseLoaded);

        FirstNames.CollectionChanged += (_, _) => this.RaisePropertyChanged(nameof(FilteredFirstNames));
        SecondNames.CollectionChanged += (_, _) => this.RaisePropertyChanged(nameof(FilteredSecondNames));
        CommonNames.CollectionChanged += (_, _) => this.RaisePropertyChanged(nameof(FilteredCommonNames));

        this.WhenAnyValue(vm => vm.SearchQuery)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(FilteredFirstNames));
                this.RaisePropertyChanged(nameof(FilteredSecondNames));
                this.RaisePropertyChanged(nameof(FilteredCommonNames));
                this.RaisePropertyChanged(nameof(HasSearchQuery));
            });

        this.WhenAnyValue(vm => vm.IsDatabaseLoaded)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(IsDatabaseEmpty)));

        this.WhenAnyValue(vm => vm.SelectedId)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(IsAddMode));
                this.RaisePropertyChanged(nameof(IsEditMode));
                this.RaisePropertyChanged(nameof(DialogTitle));
            });

        this.WhenAnyValue(vm => vm.FolderPath)
            .WhereNotNull()
            .InvokeCommand(LoadCommand);
    }

    private IEnumerable<Name> FilterBySearch(ObservableCollection<Name> names)
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return names;

        return names.Where(name =>
            name.Value.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            (name.NationName?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) == true));
    }

    [ReactiveCommand]
    private void ClearSearch() => SearchQuery = string.Empty;

    [ReactiveCommand]
    public async Task LoadAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        var nationPath = Path.Combine(path, "nation.dat");
        var firstPath = Path.Combine(path, "first_names.dat");
        var secondPath = Path.Combine(path, "second_names.dat");
        var commonPath = Path.Combine(path, "common_names.dat");

        if (!File.Exists(nationPath) || !File.Exists(firstPath) || !File.Exists(secondPath) || !File.Exists(commonPath))
        {
            StatusMessage = "Missing required name database files.";
            FolderPath = path;
            ClearData();
            return;
        }

        try
        {
            nationParser = await NationParser.Load(nationPath);
            firstNameParser = await NameParser.Load(firstPath);
            secondNameParser = await NameParser.Load(secondPath);
            commonNameParser = await NameParser.Load(commonPath);

            FolderPath = path;
            IsDatabaseLoaded = true;

            RefreshData();
            StatusMessage = "Database loaded";
        }
        catch (Exception ex)
        {
            FolderPath = path;
            ClearData();
            StatusMessage = $"Load error: {ex.Message}";
        }
    }

    [ReactiveCommand(CanExecute = nameof(_canSave))]
    private async Task SaveAsync()
    {
        if (!IsDatabaseLoaded)
            return;

        try
        {
            if (firstNameParser != null && secondNameParser != null && commonNameParser != null)
            {
                await firstNameParser.Save();
                await secondNameParser.Save();
                await commonNameParser.Save();
                StatusMessage = "Save successful";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save error: {ex.Message}";
        }
    }

    [ReactiveCommand(CanExecute = nameof(_canSave))]
    private async Task SaveAsAsync(string? path)
    {
        if (!IsDatabaseLoaded || string.IsNullOrWhiteSpace(path))
            return;

        try
        {
            if (firstNameParser != null && secondNameParser != null && commonNameParser != null)
            {
                await firstNameParser.Save(Path.Combine(path, "first_names.dat"));
                await secondNameParser.Save(Path.Combine(path, "second_names.dat"));
                await commonNameParser.Save(Path.Combine(path, "common_names.dat"));
                StatusMessage = "Save successful";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save error: {ex.Message}";
        }
    }

    [ReactiveCommand]
    private void Add(Name? name)
    {
        if (name == null)
            return;

        SelectedId = -1;
        SelectedGenderIndex = name.Gender;
        SelectedNation = Nations.FirstOrDefault(n => n.Uid == name.NationUid);
        SelectedUnknown2 = name.Unknown2;
        SelectedUnknown3 = name.Unknown3;
        SelectedValue = string.Empty;
        ActiveNameType = ResolveNameType(name);
        ShowDialog = true;
    }

    [ReactiveCommand]
    private void Edit(Name? name)
    {
        if (name == null)
            return;

        SelectedId = name.Id;
        SelectedGenderIndex = name.Gender;
        SelectedNation = Nations.FirstOrDefault(n => n.Uid == name.NationUid);
        SelectedUnknown2 = name.Unknown2;
        SelectedUnknown3 = name.Unknown3;
        SelectedValue = name.Value;
        ActiveNameType = ResolveNameType(name);
        ShowDialog = true;
    }

    [ReactiveCommand]
    private void Delete(Name? name)
    {
    }

    [ReactiveCommand]
    private void Cancel() => ResetDialog();

    [ReactiveCommand]
    private void Confirm()
    {
        if (string.IsNullOrWhiteSpace(SelectedValue)
            || SelectedGenderIndex is null || SelectedGenderIndex < 0
            || SelectedNation is null
            || SelectedUnknown2 is null
            || SelectedUnknown3 is null)
        {
            ResetDialog();
            return;
        }

        var collection = GetActiveCollection();
        var parser = GetActiveParser();
        if (collection == null || parser == null)
        {
            ResetDialog();
            return;
        }

        var nationName = SelectedNation.Name;
        var nationUid = SelectedNation.Uid;
        var gender = (byte)SelectedGenderIndex.Value;
        var unknown2 = SelectedUnknown2.Value;
        var unknown3 = SelectedUnknown3.Value;

        if (SelectedId == -1)
        {
            var names = SelectedValue
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => n.Trim())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();

            foreach (var nameValue in names)
            {
                var newName = new Name(gender, nationUid, unknown2, unknown3, nameValue)
                {
                    NationName = nationName
                };

                parser.Add(newName);
                collection.Add(newName);
            }

            StatusMessage = $"Added {names.Count} name(s)";
        }
        else
        {
            var existing = collection.FirstOrDefault(x => x.Id == SelectedId);
            if (existing != null)
            {
                existing.Gender = gender;
                existing.NationUid = nationUid;
                existing.Unknown2 = unknown2;
                existing.Unknown3 = unknown3;
                existing.Value = SelectedValue.Trim();
                existing.NationName = nationName;

                StatusMessage = "Name updated";
            }
        }

        this.RaisePropertyChanged(nameof(FilteredFirstNames));
        this.RaisePropertyChanged(nameof(FilteredSecondNames));
        this.RaisePropertyChanged(nameof(FilteredCommonNames));

        ResetDialog();
    }

    private void ResetDialog()
    {
        SelectedId = null;
        SelectedGenderIndex = null;
        SelectedNation = null;
        SelectedUnknown2 = null;
        SelectedUnknown3 = null;
        SelectedValue = null;
        ShowDialog = false;
    }

    private ObservableCollection<Name>? GetActiveCollection() => ActiveNameType switch
    {
        NameType.FirstName => FirstNames,
        NameType.SecondName => SecondNames,
        NameType.CommonName => CommonNames,
        _ => null
    };

    private NameParser? GetActiveParser() => ActiveNameType switch
    {
        NameType.FirstName => firstNameParser,
        NameType.SecondName => secondNameParser,
        NameType.CommonName => commonNameParser,
        _ => null
    };

    private NameType ResolveNameType(Name name)
    {
        if (FirstNames.Contains(name))
            return NameType.FirstName;
        if (SecondNames.Contains(name))
            return NameType.SecondName;
        return NameType.CommonName;
    }

    private void RefreshData()
    {
        Nations.Clear();
        if (nationParser != null)
        {
            foreach (var nation in nationParser.Items.OrderBy(n => n.Name))
            {
                Nations.Add(nation);
            }
        }

        var nationLookup = nationParser?.Items.ToDictionary(n => n.Uid, n => n.Name) ?? [];

        void ResetNames(ObservableCollection<Name> target, NameParser? parser)
        {
            target.Clear();
            if (parser == null)
                return;

            foreach (var name in parser.Items)
            {
                if (nationLookup.TryGetValue(name.NationUid, out var nationName))
                    name.NationName = nationName;
                else
                    name.NationName = null;

                target.Add(name);
            }
        }

        ResetNames(FirstNames, firstNameParser);
        ResetNames(SecondNames, secondNameParser);
        ResetNames(CommonNames, commonNameParser);

        this.RaisePropertyChanged(nameof(FilteredFirstNames));
        this.RaisePropertyChanged(nameof(FilteredSecondNames));
        this.RaisePropertyChanged(nameof(FilteredCommonNames));
    }

    private void ClearData()
    {
        nationParser = null;
        firstNameParser = null;
        secondNameParser = null;
        commonNameParser = null;

        Nations.Clear();
        FirstNames.Clear();
        SecondNames.Clear();
        CommonNames.Clear();

        IsDatabaseLoaded = false;
        SelectedName = null;
    }
}
