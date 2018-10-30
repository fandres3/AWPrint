using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using System.IO;
using Android.Bluetooth;
using Java.Util;

namespace AWPrint
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsGeneralPage : ContentPage
    {

        private BluetoothSocket _socket;
        // byte[] buffer;
        System.IO.Stream mmOutputStream;
        System.IO.Stream mmInputStream;

        public SettingsGeneralPage()
        {
            InitializeComponent();
            CargaSettings();
        }

        void CargaSettings()
        {
            if (Application.Current.Properties.ContainsKey("CaminoAFichero"))
                txtCaminoAFichero.Text = Application.Current.Properties["CaminoAFichero"] as string;
            if (Application.Current.Properties.ContainsKey("Fichero"))
                txtFichero.Text = Application.Current.Properties["Fichero"] as string;
            if (Application.Current.Properties.ContainsKey("Impresora"))
                txtImpresora.Text = Application.Current.Properties["Impresora"] as string;
         
        }

        void GrabaSettings(object sender, EventArgs args)
        {
            Application.Current.Properties["CaminoAFichero"] = txtCaminoAFichero.Text;
            Application.Current.Properties["Fichero"] = txtFichero.Text;
            Application.Current.Properties["Impresora"] = txtImpresora.Text;
            App.Current.SavePropertiesAsync();
        }

     


        void BtnPruebaImpresionClicked(object sender, EventArgs args)
        {



            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
                throw new Exception("No Bluetooth adapter found.");

            if (!adapter.IsEnabled)
                throw new Exception("Bluetooth adapter is not enabled.");

            BluetoothDevice device = (from bd in adapter.BondedDevices
                                      where bd.Name == "T9 BT Printer"
                                      select bd).FirstOrDefault();

            if (device == null)
                throw new Exception("Named device not found.");

            _socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            _socket.Connect();
            String message = "Linea de prueba";
            Encoding u8 = Encoding.UTF8;
            byte[] buffer = u8.GetBytes(message);
            // Read data from the device
            _socket.InputStream.ReadAsync(buffer, 0, buffer.Length);

            // https://brianpeek.com/connect-to-a-bluetooth-device-with-xamarinandroid/
            // https://forums.xamarin.com/discussion/6576/how-to-send-data-to-printer
            // https://stackoverflow.com/questions/33775823/send-data-to-bluetooth-printer

            buffer = u8.GetBytes(message);

            // Write data to the device
            _socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            mmOutputStream = _socket.OutputStream;
            mmInputStream = _socket.InputStream;

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

    }
}