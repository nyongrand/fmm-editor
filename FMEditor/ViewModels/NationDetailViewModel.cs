using FMELibrary;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMEditor.ViewModels
{
    public class NationDetailViewModel : ReactiveObject, IQueryAttributable
    {
        [Reactive] public NationParser NationParser { get; set; }
        [Reactive] public Nation Nation { get; set; }

        public NationDetailViewModel(NationParser nationParser)
        {
            NationParser = nationParser;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Nation = query["Nation"] as Nation;
        }
    }
}
