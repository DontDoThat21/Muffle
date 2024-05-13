using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Muffle.Data.Models;

namespace Muffle.Views;

public partial class ServerDetailsContentView : ContentView
{
    public ServerDetailsContentView(Server server)
    {
        InitializeComponent();
        BindingContext = server;
    }
}