using Muffle.Data.Models;

namespace Muffle.Views;

public partial class ServerTopBarUIView : ContentView
{
	public ServerTopBarUIView(Server server)
	{
		InitializeComponent();
		BindingContext = server;
	}
}