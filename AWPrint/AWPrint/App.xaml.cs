using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AWPrint
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            if (!Application.Current.Properties.ContainsKey("CaminoAFichero"))
                { Application.Current.Properties["CaminoAFichero"] = "/storage/"; }

            if (!Application.Current.Properties.ContainsKey("Fichero"))
                { Application.Current.Properties["Fichero"] = "fichero.txt"; }

            MainPage = new AWPrint.MainPage();
		}

		protected override void OnStart ()
		{
            // Handle when your app starts

        }

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
