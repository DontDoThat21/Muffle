using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Muffle.Data.Models;

namespace Muffle.ViewModels
{
    public class MainPageViewModel
    {
        private readonly MainPageModel MainPage;
        public ObservableCollection<Server> Servers { get; set; }
    }
}
