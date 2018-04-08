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
        public static BluetoothManager mBTManager;
        public static BluetoothAdapter mBluetoothAdapter;
        public static BluetoothDevice mDevice;
        public static BluetoothSocket mSocket;

        public App ()
		{
			InitializeComponent();

            IniciaSettings();

            var ResultadoBT = FindBT();
            if (ResultadoBT == "Bluetooth Ok") OpenBT();

            MainPage = new AWPrint.MainPage(ResultadoBT);
        }

        private void IniciaSettings()
        {
            if (!Application.Current.Properties.ContainsKey("CaminoAFichero"))
            { Application.Current.Properties["CaminoAFichero"] = "/storage/"; }

            if (!Application.Current.Properties.ContainsKey("Fichero"))
            { Application.Current.Properties["Fichero"] = "fichero.txt"; }

            if (!Application.Current.Properties.ContainsKey("Impresora"))
            { Application.Current.Properties["Impresora"] = "PrinterBT"; }
        }

        private String FindBT()
        {
            try
            {
                AWPrint.App.mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                if (AWPrint.App.mBluetoothAdapter == null)
                {
                    throw new Exception("No encuentro adaptador Bluetooth.");
                }
         
                if (AWPrint.App.mBluetoothAdapter == null)
                {
                    throw new Exception("No encuentro adaptador Bluetooth.");
                }
                if (!AWPrint.App.mBluetoothAdapter.IsEnabled)
                    throw new Exception("Adaptador Bluetooth desconectado.");

                var Impresora = Application.Current.Properties["Impresora"] as string;
                AWPrint.App.mDevice = (from bd in AWPrint.App.mBluetoothAdapter.BondedDevices
                                       where bd.Name == Impresora 
                                       select bd).FirstOrDefault();

                if (AWPrint.App.mDevice == null)
                    throw new Exception("Nombre de periférico no encontrado.");

                return "Bluetooth Ok";
            }
            catch (Exception e)
            {
                return "ERROR:" + e.Message;
                //DisplayAlert( throw e.Message ;
            }


        }

        private void OpenBT()
        {
            try
            {
                BluetoothSocket mSocket = AWPrint.App.mDevice.CreateRfcommSocketToServiceRecord(
                               UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                mSocket.ConnectAsync();
            }
            catch (Exception)
            {

                throw;
            }
           

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
