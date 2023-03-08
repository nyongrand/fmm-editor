using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace FMEditor.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        [Reactive] public string Welcome { get; set; } = "Welcome to .NET Multi-platform App UI";
        [Reactive] public int ClickCount { get; set; }
        [Reactive] public string ButtonText { get; set; } = "Click me";


        public ReactiveCommand<Unit, Unit> Load { get; private set; }

        public MainViewModel()
        {
            Load = ReactiveCommand.Create(() => { ClickCount++; });

            this.WhenAnyValue(vm => vm.ClickCount)
                .Subscribe(x => ButtonText = $"Clicked {x} times");
        }
    }
}
