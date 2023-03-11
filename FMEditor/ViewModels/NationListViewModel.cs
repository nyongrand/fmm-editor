using DynamicData;
using FMELibrary;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMEditor.ViewModels
{
    public class NationListViewModel : ReactiveObject
    {
        [Reactive] public NationParser NationParser { get; set; }
        [Reactive] public Nation SelectedNation { get; set; }

        public ObservableCollection<Nation> Nations { get; }

        public ReactiveCommand<List<Nation>, Unit> LoadItems { get; private set; }
        public ReactiveCommand<Nation, Unit> Navigate { get; private set; }

        public NationListViewModel(NationParser nationParser)
        {
            NationParser = nationParser;

            Nations = new ObservableCollection<Nation>();
            SelectedNation = null;

            LoadItems = ReactiveCommand.Create<List<Nation>>(LoadItemsImpl);
            Navigate = ReactiveCommand.CreateFromTask<Nation>(NavigateImpl);

            this.WhenAnyValue(vm => vm.NationParser)
                .WhereNotNull()
                .Delay(TimeSpan.FromMilliseconds(200))
                .Select(x => x.Items)
                .InvokeCommand(LoadItems);
        }

        private void LoadItemsImpl(List<Nation> items)
        {
            Nations.Clear();
            Nations.AddRange(items.OrderBy(x => x.Name));
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
