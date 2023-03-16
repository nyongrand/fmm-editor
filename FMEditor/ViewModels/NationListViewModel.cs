using DynamicData;
using FMEditor.Database;
using FMELibrary;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace FMEditor.ViewModels
{
    public class NationListViewModel : ReactiveObject
    {
        //[Reactive] public NationParser NationParser { get; set; }
        [Reactive] public Nation SelectedNation { get; set; }

        public ObservableCollection<Nation> Nations { get; }

        public ReactiveCommand<Unit, Unit> LoadItems { get; private set; }
        public ReactiveCommand<Nation, Unit> Navigate { get; private set; }

        FmmDatabase database;

        public NationListViewModel(FmmDatabase db, NationParser nationParser)
        {
            database = db;
            //NationParser = nationParser;

            Nations = new ObservableCollection<Nation>();
            SelectedNation = null;

            LoadItems = ReactiveCommand.CreateFromTask(LoadItemsImpl);
            //LoadItems = ReactiveCommand.Create<List<Nation>>(LoadItemsImpl);
            Navigate = ReactiveCommand.CreateFromTask<Nation>(NavigateImpl);

            //this.WhenAnyValue(vm => vm.NationParser)
            //    .WhereNotNull()
            //    .Delay(TimeSpan.FromMilliseconds(200))
            //    .Select(x => x.Items)
            //    .InvokeCommand(LoadItems);

            LoadItems.Execute();
        }

        private async Task LoadItemsImpl()
        {
            var entities = await database.GetItemsAsync();

            Nations.Clear();
            Nations.AddRange(entities.Select(Database.EntityNation.ToModel));
        }

        private async Task NavigateImpl(Nation item)
        {
            if (item != null)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Nation", item }
                };

                await Shell.Current.GoToAsync("NationDetail", navigationParameter);
            }
        }
    }
}
