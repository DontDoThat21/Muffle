using Muffle.Views;

namespace Muffle
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            bool isMobile = DeviceInfo.Current.Platform == DevicePlatform.Android
                         || DeviceInfo.Current.Platform == DevicePlatform.iOS;

            if (isMobile)
            {
                bool isTablet = DeviceInfo.Current.Idiom == DeviceIdiom.Tablet;

                var shellContent = Items.OfType<ShellItem>()
                    .SelectMany(i => i.Items.OfType<ShellSection>())
                    .SelectMany(s => s.Items.OfType<ShellContent>())
                    .FirstOrDefault();

                if (shellContent != null)
                {
                    shellContent.ContentTemplate = isTablet
                        ? new DataTemplate(typeof(TabletMuffleMainPage))
                        : new DataTemplate(typeof(MobileMuffleMainPage));
                }
            }
        }
    }
}
