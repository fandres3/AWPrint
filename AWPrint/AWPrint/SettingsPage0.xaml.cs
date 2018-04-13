using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AWPrint
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage0 : ContentPage
    {
        public SettingsPage0()
        {
            InitializeComponent();
        }

        async void ConfigGeneral(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new SettingsFTPPage());
        }

        async void ConfigFTP(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new SettingsFTPPage());
        }
    }
}