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
    public partial class SettingsFTPPage : ContentPage
    {

        static private BluetoothSocket _socket;
        // byte[] buffer;
        static System.IO.Stream mmOutputStream;
        static System.IO.Stream mmInputStream;

        public SettingsFTPPage()
        {
            InitializeComponent();
            CargaSettings();


        }

        void CargaSettings()
        {
            if (Application.Current.Properties.ContainsKey("FTPServer"))
                txtFTPServer.Text = Application.Current.Properties["FTPServer"] as string;
            if (Application.Current.Properties.ContainsKey("FTPUser"))
                txtFTPUser.Text = Application.Current.Properties["FTPUser"] as string;
            if (Application.Current.Properties.ContainsKey("FTPPassword"))
                txtFTPPassword.Text = Application.Current.Properties["FTPPassword"] as string;
            if (Application.Current.Properties.ContainsKey("FTPCarpeta"))
                txtFTPCarpeta.Text = Application.Current.Properties["FTPCarpeta"] as string;
            if (Application.Current.Properties.ContainsKey("FTPSSL"))
            {
                String sw = Application.Current.Properties["FTPSSL"] as string;
                swFTPSSL.IsToggled = (sw != "0");
            }
        }

        void GrabaSettings(object sender, EventArgs args)
        {
            Application.Current.Properties["FTPServer"] = txtFTPServer.Text;
            Application.Current.Properties["FTPUser"] = txtFTPUser.Text;
            Application.Current.Properties["FTPPassword"] = txtFTPPassword.Text;
            Application.Current.Properties["FTPCarpeta"] = txtFTPCarpeta.Text;
            Application.Current.Properties["FTPSSL"] = (swFTPSSL.IsToggled == true) ? "1" : "0";
            App.Current.SavePropertiesAsync();
        }

        void BtnPruebaFTPClicked(object sender, EventArgs args)
        {

            string fichero = Application.Current.Properties["Fichero"] as string;
            //var uri = new Uri("ftp://" + Application.Current.Properties["FTPServer"] + "/test.txt");
            var uri = new Uri("ftp://" + Application.Current.Properties["FTPServer"] + "/" + fichero);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.EnableSsl = swFTPSSL.IsToggled;
            try
            {
                request.Credentials = new NetworkCredential(txtFTPUser.Text, txtFTPPassword.Text);

            }
            catch (Exception e)
            {
                lblStatus.TextColor = Color.Red;
                lblStatus.Text = e.Message;
                return;
            }

            FtpWebResponse response;
            try
            {
                response = (FtpWebResponse)request.GetResponse();

            }
            catch (System.Net.WebException e)
            {

                lblStatus.TextColor = Color.Red;
                lblStatus.Text = e.Message;
                return;
            }

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            int dataLength = (int)request.GetResponse().ContentLength;

            byte[] buffer = new byte[2048];
            String camino = Application.Current.Properties["CaminoAFichero"] as string;
            if (!Directory.Exists(camino))
            {
                lblStatus.TextColor = Color.Red;
                lblStatus.Text = "No existe " + camino;
                return;
            }

            //String fichero = Application.Current.Properties["Fichero"] as string;
            String fileName = Path.Combine(camino,fichero);
            File.Delete(fileName);

            FileStream fs;
            try
            {
                fs = new FileStream(fileName, FileMode.Create);
            }
            catch (System.IO.IOException e)
            {
                lblStatus.TextColor = Color.Red;
                lblStatus.Text = e.Message;
                return;
            }

            int ReadCount = responseStream.Read(buffer, 0, buffer.Length);
            while (ReadCount > 0)
            {
                fs.Write(buffer, 0, ReadCount);
                ReadCount = responseStream.Read(buffer, 0, buffer.Length);
            }
            //      ResponseDescription = response.StatusDescription;
            fs.Close();
            reader.Close();
            response.Close();
            if (!File.Exists (fileName))
            {
                lblStatus.TextColor = Color.Red;
                lblStatus.Text = "No descargado";
                return;
            }

            lblStatus.TextColor = Color.Green;
            lblStatus.Text = "Correcto";
            return;
        }


        void BtnPruebaImpresionClicked(object sender, EventArgs args)
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null) {
                lblStatus.TextColor = Color.Red;
                lblStatus.Text = "No encontrado adaptador Bluetooth";
                return;
            }
               
            if (!adapter.IsEnabled) {
                lblStatus.TextColor = Color.Red;
                lblStatus.Text = "Adaptador Bluetooth no conectado";
                return;
            }

            String impresora = Application.Current.Properties["Impresora"] as string;
            BluetoothDevice device = (from bd in adapter.BondedDevices
                                      where bd.Name == impresora
                                      select bd).FirstOrDefault();

            if (device == null)
            {
                lblStatus.TextColor = Color.Red;
                lblStatus.Text = "Impresora " + impresora + " no encontrada";
                return;
            }

            try
            {
                _socket = null;
                _socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                //_socket = device.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
   
                _socket.Connect();

            }
            catch (Exception e)
            {
                lblStatus.TextColor = Color.Red;
                //lblStatus.Text = "Impresora " + impresora + " no emparejada";
                lblStatus.Text = e.Message;
                return;
            }

            String camino = Application.Current.Properties["CaminoAFichero"] as string;
            String fichero = Application.Current.Properties["Fichero"] as string;
            String fileName = Path.Combine(camino, fichero);
            String message = "Linea de prueba";
            using (var streamReader = new StreamReader(fileName))
            {
                message = streamReader.ReadToEnd();
                //System.Diagnostics.Debug.WriteLine(content);
            }
            
            Encoding u8 = Encoding.UTF8;
            byte[] buffer = u8.GetBytes(message);
            // Read data from the device
            _socket.InputStream.ReadAsync(buffer, 0, buffer.Length);

            // https://brianpeek.com/connect-to-a-bluetooth-device-with-xamarinandroid/
            // https://forums.xamarin.com/discussion/6576/how-to-send-data-to-printer
            // https://stackoverflow.com/questions/33775823/send-data-to-bluetooth-printer
            // https://www.androidcode.ninja/android-bluetooth-tutorial/

            buffer = u8.GetBytes(message);

            // Write data to the device
            _socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            mmOutputStream = _socket.OutputStream;
            mmInputStream = _socket.InputStream;

            mmOutputStream.Close();
            mmInputStream.Close();
            _socket.Close();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            lblStatus.Text = "";
        }

    }
}