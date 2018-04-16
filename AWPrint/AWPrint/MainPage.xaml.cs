using AWPrint.MenuItems;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AWPrint
{
    public partial class MainPage : MasterDetailPage
    {
        public List<MasterPageItem> menuList { get; set; }

        public MainPage(String MensajeInicial)
        {
            InitializeComponent();

            menuList = new List<MasterPageItem>();
            // Creating our pages for menu navigation
            // Here you can define title for item, 
            // icon on the left side, and page that you want to open after selection
            var page1 = new MasterPageItem() { Title = "Ajustes Generales", Icon = "ic_printer.png", TargetType = typeof(SettingsGeneralPage) };
            var page2 = new MasterPageItem() { Title = "Configuración FTP", Icon = "ic_settings.png", TargetType = typeof(SettingsFTPPage) };
            var page3 = new MasterPageItem() { Title = "Impresión", Icon = "ic_printer.png", TargetType = typeof(ImpresionPage) };

            // Adding menu items to menuList
            menuList.Add(page1);
            menuList.Add(page2);
            menuList.Add(page3);
            // Setting our list to be ItemSource for ListView in MainPage.xaml
            navigationDrawerList.ItemsSource = menuList;


            Detail = new NavigationPage(new ImpresionPage());
            IsPresented = false;
        }

        private void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            var item = (MasterPageItem)e.SelectedItem;
            Type page = item.TargetType;

            Detail = new NavigationPage((Page)Activator.CreateInstance(page));
            IsPresented = false;
        }


        void BtnImprimirClicked(object sender, System.EventArgs e)
        {
            Detail = new NavigationPage(new ImpresionPage());
            IsPresented = false;

        }

        void BtnAjustesClicked(object sender, System.EventArgs e)
        {
            Detail = new NavigationPage(new SettingsPage0());
            IsPresented = false;
        }
    }
}
