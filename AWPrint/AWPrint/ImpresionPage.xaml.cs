using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AWPrint;
using Android.Bluetooth;
using Java.Net;
using Java.Util;
using System.Net;
using System.IO;
using AWPrint.Services;


namespace AWPrint
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImpresionPage : ContentPage
	{

        public static FTP Ftp;
        public static Bluetooth BT;
        double width;
        double height;
        public static bool NetworkAvailable = true;

        public ImpresionPage()
		{
			InitializeComponent ();
         //   lblStatus.Text = MensajeInicial;
         //   if (MensajeInicial.StartsWith("ERROR:")) lblStatus.TextColor = Color.Red;
         //       else lblStatus.TextColor = Color.Green;

        }

        protected override void OnAppearing()
        {
    
            base.OnAppearing();
            lblStatus.Text = "";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            lblStatus.Text = "";
        }

        void BtnImprimirClicked(object sender, System.EventArgs e)
        {

            // PASO 0 -> Comprueba conectividad con internet

            // PASO 1 -> Descarga por FTP ----------------------------------------------------------
            String sw = Application.Current.Properties["FTPSSL"] as string;
            Boolean swFTPSSL = (sw != "0");
            Ftp = new FTP(Application.Current.Properties["FTPServer"] as string,
                    Application.Current.Properties["FTPUser"] as string,
                    Application.Current.Properties["FTPPassword"] as string,
                    swFTPSSL,
                    Application.Current.Properties["FTPCarpeta"] as string);

            Color c = Color.Gray;
            lblStatus.BackgroundColor = c;
            lblStatus.Text = "Descargando fichero " + Application.Current.Properties["Fichero"];

            if (!Ftp.FTPDescargaFichero(Application.Current.Properties["FTPCarpeta"] as string,
                Application.Current.Properties["Fichero"] as string,
                Application.Current.Properties["CaminoAFichero"] as string,
                Application.Current.Properties["Fichero"] as string))
            {
                c = Color.Red;
                lblStatus.BackgroundColor = c;
                lblStatus.Text = Ftp.mensaje;
                return;
            }

            String camino = Application.Current.Properties["CaminoAFichero"] as string;
            String fichero = Application.Current.Properties["Fichero"] as string;
            String fileName = Path.Combine(camino, fichero);
            if (!File.Exists(fileName))
            {
                c = Color.Red;
                lblStatus.BackgroundColor = c;
                lblStatus.Text = "No existe " + fileName;
                return;
            }

            // PASO 2 -> Conexión por Bluetooth ----------------------------------------------------
            lblStatus.Text = "Conectando Bluetooth";

            String impresora = Application.Current.Properties["Impresora"] as string;
            BT = new Bluetooth(impresora);
            BT.BluetoothConecta(impresora);
            if (BT.mensaje !="")
            {
                c = Color.Red;
                lblStatus.BackgroundColor = c;
                lblStatus.Text = BT.mensaje;
                return;
            }

            // PASO 3 -> Impresión por Bluetooth ----------------------------------------------------
            lblStatus.Text = "Enviando a Bluetooth";
            String Resultado = BT.BluetoothEnviarFichero(camino, fileName, impresora);
            if (BT.estado == false)
            {
                c = Color.Red;
                lblStatus.BackgroundColor = c;
                lblStatus.Text = BT.mensaje;
                return;
            }

        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;
                if (width > height)
                {
                    outerStack.Orientation = StackOrientation.Horizontal;
                }
                else
                {
                    outerStack.Orientation = StackOrientation.Vertical;
                }
            }
        }
    }
}