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
		public MainPage(String MensajeInicial)
		{
			InitializeComponent();
            Detail = new NavigationPage(new Page1(MensajeInicial));
            IsPresented = false;
        }


        void BtnImprimirClicked(object sender, System.EventArgs e)
        {
            Detail = new NavigationPage(new Page1("Imprimiendo..."));
            IsPresented = false;

        }

        void BtnAjustesClicked(object sender, System.EventArgs e)
        {
            Detail = new NavigationPage(new Page2());
            IsPresented = false;
        }
    }
}
