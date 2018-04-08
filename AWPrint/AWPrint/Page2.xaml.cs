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
    public partial class Page2 : ContentPage
    {
        public Page2()
        {
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey("CaminoAFichero"))
                txtCaminoAFichero.Text = Application.Current.Properties["CaminoAFichero"] as string;
            if (Application.Current.Properties.ContainsKey("Fichero"))
                txtFichero.Text = Application.Current.Properties["Fichero"] as string;

        }
        
        void On_Completed(object sender, EventArgs args)
        {
            Application.Current.Properties["CaminoAFichero"] = txtCaminoAFichero.Text;
            Application.Current.Properties["Fichero"] = txtFichero.Text;
            App.Current.SavePropertiesAsync();
        }
    }
}