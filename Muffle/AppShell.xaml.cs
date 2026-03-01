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
                var shellContent = Items.OfType<ShellItem>()
                    .SelectMany(i => i.Items.OfType<ShellSection>())
                    .SelectMany(s => s.Items.OfType<ShellContent>())
                    .FirstOrDefault();

                if (shellContent != null)
                    shellContent.ContentTemplate = new DataTemplate(typeof(MobileMuffleMainPage));
            }
        }
    }
}
