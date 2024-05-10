namespace Muffle
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.MainPageViewModel();
        }

        private void ServerButton_OnClicked(object? sender, EventArgs e)
        {
            // Clear existing content
            DynamicServerContent.Content = null;
            //Populate

        }
    }

}
