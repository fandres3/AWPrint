using Android.Bluetooth;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AWPrint
{
    public partial class App : Application
    {
        public static BluetoothManager mBTManager = null;
        public static BluetoothAdapter mBluetoothAdapter = null;
        public static BluetoothDevice mDevice = null;
        public static BluetoothSocket mSocket = null;
 

        public App()
        {
            InitializeComponent();

            IniciaSettings();
            //  var ResultadoBT = FindBT();
            //  if (ResultadoBT == "Bluetooth Ok") OpenBT();

            //MainPage = new AWPrint.MainPage(ResultadoBT);
            MainPage = new AWPrint.MainPage("");
        }

        private void IniciaSettings()
        {
            if (!Application.Current.Properties.ContainsKey("CaminoAFichero"))
            {   
                // Settings Generales
                Application.Current.Properties["CaminoAFichero"] = "/storage/";
                Application.Current.Properties["Fichero"] = "fichero.txt";
                Application.Current.Properties["Impresora"] = "PrinterBT";
                // Settings FTP
                Application.Current.Properties["FTPServer"] = "ftp.servidor.com";
                Application.Current.Properties["FTPUser"] = "usuario";
                Application.Current.Properties["FTPPassword"] = "1234";
                Application.Current.Properties["FTPCarpeta"] = "carpeta remota FTP";
                Application.Current.Properties["FTPSSL"] = "0";
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
