﻿using System;
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
using System.Threading;
using Xamarin.Essentials;
using System.Runtime.CompilerServices;

namespace AWPrint
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImpresionPage : ContentPage
    {

        public static FTP Ftp;
        public static Bluetooth3 BT;
        private double width;
        private double height;
        public static bool NetworkAvailable = true;
        public static Bluetooth3 BT3;
        public static bool flagImpreso;
        public static bool flagDosCopias;

        public ImpresionPage()
        {
            InitializeComponent();
            //   lblStatus.Text = MensajeInicial;
            //   if (MensajeInicial.StartsWith("ERROR:")) lblStatus.TextColor = Color.Red;
            //       else lblStatus.TextColor = Color.Green;

        }

        protected override void OnAppearing()
        {

            base.OnAppearing();
            lblStatus.Text = "";
            btnDescargar.Text = "Albarán a imprimir";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            lblStatus.Text = "";
            btnDescargar.Text = "Albarán a imprimir";
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

        async void BtnDescargarClicked(object sender, System.EventArgs e)
        {

            // Modo = 0 -> El fichero a imprimir ya se encuentra en la carpeta tsclient indicada en la sesión o través de parámetro FOLDER_FTP en el servidor
            // Modo = 1 -> El fichero baja por FTP
            int modo = 1;

            Color c = Color.Gray;
            lblStatus.BackgroundColor = c;

            if (modo == 1)
            {
                // PASO 0->Comprueba conectividad con internet 
                var current = Connectivity.NetworkAccess;

                if (current != NetworkAccess.Internet)
                {
                    // Connection to internet NOT is available
                    c = Color.Red;
                    lblStatus.BackgroundColor = c;
                    lblStatus.Text = "SIN CONEXIÓN. NO HAY COBERTURA";
                    await Task.Delay(2000);
                    c = Color.White;
                    lblStatus.Text = "";
                    lblStatus.BackgroundColor = c;
                    //////////////////////
                    return;
                    /////////////////////
                }
                else {
                    lblStatus.Text = "CONECTADO. DESCARGANDO ....";
                }
            }

            btnImprimirCopia.IsVisible = false;
            lblStatus.Text = "";
            // PASO 1 -> Descarga por FTP ----------------------------------------------------------
            // ---- Para pruebas, sin bajar el fichero del FTP. 
            // ---- Coge el fichero local que se encuentre en el terminal
            Boolean ficheroLocal = false; 
            // ----
            if (ficheroLocal == false)
            {
                lblStatus.Text = "Descargando " + Application.Current.Properties["Fichero"];
                lblStatus.BackgroundColor = Color.Gray;
                String sw = Application.Current.Properties["FTPSSL"] as string;
                Boolean swFTPSSL = (sw != "0");
                String sw1 = Application.Current.Properties["FTPPasivo"] as string;
                Boolean swFTPPasivo = (sw1 != "0");
                Ftp = new FTP(Application.Current.Properties["FTPServer"] as string,
                        Application.Current.Properties["FTPUser"] as string,
                        Application.Current.Properties["FTPPassword"] as string,
                        swFTPSSL,
                        Application.Current.Properties["FTPCarpeta"] as string,
                        swFTPPasivo);


                Console.WriteLine("paso 1");
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
                lblStatus.Text = "Descargado";
            }
            else
            {
                lblStatus.Text = "Fichero local. Sin FTP.";
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////

            String camino = Application.Current.Properties["CaminoAFichero"] as string;
            String fichero = Application.Current.Properties["Fichero"] as string;
            String fileName = Path.Combine(camino, fichero);

            //  WebClient webClient = new WebClient();
            //  webClient.DownloadFile("http://www.aniwin.com/portal/media/Agenda.pdf", fileName);

       

            if (!File.Exists(fileName))
            {
                c = Color.Red;
                lblStatus.BackgroundColor = c;
                //lblStatus.Text = "No existe " + fileName;
                lblStatus.Text = "No hay albarán a imprimir";
                return;
            }


            String message = null;
            using (var streamReader = new StreamReader(fileName))
            {
                message = streamReader.ReadToEnd();
            }
            // ---- Elimino lineas CR+LF del final del archivo (ya en la cadena message)
            //message = message.TrimEnd( System.Environment.NewLine.ToCharArray());
            message = message.TrimEnd('\r');
            message = message.TrimEnd('\n');
            //message = message.Replace("\r\n","");
            int cAlbaran = message.IndexOf("CLIENTE");
            string strcAlbaran = "";
            if (cAlbaran > -1)
            {
                strcAlbaran = message.Substring(cAlbaran + 9, 20); // Extrae el cliente para mostrarlo en pantalla antes de imprimir

            }

            string tipoDoc = "ALBARAN";
            int nAlbaran = message.IndexOf("ALBARAN");
            if (nAlbaran == -1) { nAlbaran = message.IndexOf("NRO FACTURA"); tipoDoc = "FACTURA"; }
            if (nAlbaran == -1) { nAlbaran = message.IndexOf("INFORME ALBA"); tipoDoc = "INFORME"; }
            string strAlbaran = "";
            if (nAlbaran > -1)
            {
                strAlbaran = message.Substring(nAlbaran + 8, 12); // Extrae el albarán para mostrarlo en pantalla antes de imprimir
            }
            else
            {
                tipoDoc = "?";
            }


            int tAlbaran = message.IndexOf("TOTAL ALBARAN");
            if (tAlbaran == -1) tAlbaran = message.IndexOf("TOTAL FACTURA");
            string strtAlbaran = "0";
            if (tAlbaran > -1)
            {
                strtAlbaran = message.Substring(tAlbaran + 14, 16); // Extrae el total albarán para mostrarlo en pantalla antes de imprimir
                //strtAlbaran = strtAlbaran.Replace(",", ".");
            }
            btnDescargar.Text = tipoDoc + "\n" + strcAlbaran + "\n" + strAlbaran + "\n" + string.Format("Total: {0,8:#,###.00}", Convert.ToDecimal(strtAlbaran));
            lblStatus.Text = "";

            // Modificación pedida el 03-07-07. Esto es nuevo.
            // Y sobre lo de las dos copias, AHORA SE IMPRIME EN EL albaran.sec: Iva no incluido. Venta a credito CUANDO LA FORMA DE PAGO ES DISTINTA A LA 5,
            // ASÍ QUE HEMOS QUEDADO QUE CUANDO APAREZCA ESE TEXTO, HACER QUE IMPRIMAN DOS COPIAS.
            // PARA QUE NO SALGAN LAS DOS COPIAS SEGUIDAS, ME HA COMENTADO A VER SI SE PUEDE HACER QUE EN ESTOS CASOS, CUANDO PULSE EN EL AWPRINT A IMPRIMIR 
            // SE IMPRIMA 1 COPIA, PERO QUE NO SE BORRE EL .SEC TODAVIA, PARA QUE PUEDA CORTAR EL PAPEL DE LA IMPRESORA DE ESA PRIMERA COPIA, 
            // Y CUANDO LUEGO LE PULSE A IMPRIMIR LA SEGUNDA COPIA, SE IMPRIMA LA SEGUNDA Y ENTONCES SÍ QUE SE BORRE EL.SEC
            flagDosCopias = false;
            int tDosCopias = message.IndexOf("Venta a cr");
            if (tDosCopias > -1)
            {
                flagDosCopias = true;
                btnImprimirCopia.IsVisible = true;
            }
            else
            {
                btnImprimirCopia.IsVisible = false;
            }

            await z("");

        }

        private async Task z(String msg)
        {
            await Task.Delay(1);
        }

        void BtnImprimirClicked(object sender, System.EventArgs e)
        {

            imprimir(false); // imprimir(true) no hace impresión y devuelve flagImpreso = true
            if (flagImpreso == false) return;
            if (flagDosCopias == true) { btnImprimirCopia.IsVisible = true; return; }

            // Borra albaran.sec cuando es una copia
            String camino = Application.Current.Properties["CaminoAFichero"] as string;
            String fichero = Application.Current.Properties["Fichero"] as string;
            String fileName = Path.Combine(camino, fichero);
            if (File.Exists(fileName))
            {
                try
                {
                    System.IO.File.Delete(fileName); 
                }
                catch (Exception ex)
                {
                    lblStatus.Text = ex.Message;
                    return;
                }
            }
        }

        void BtnImprimirCopiaClicked(object sender, System.EventArgs e)
        {
            imprimir(false); // imprimir(true) no hace impresión y devuelve flagImpreso = true
            if (flagImpreso)
            {
                btnImprimirCopia.IsVisible = false;
                String camino = Application.Current.Properties["CaminoAFichero"] as string;
                String fichero = Application.Current.Properties["Fichero"] as string;
                String fileName = Path.Combine(camino, fichero);
                if ((flagDosCopias==true) && (File.Exists(fileName)))
                {
                    try
                    {
                        System.IO.File.Delete(fileName); // 14-02-19 Borra el fichero del albarán ya impreso en la segunda copia
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Text = ex.Message;
                        return;
                    }
                }
            }

        }



        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    base.OnSizeAllocated(width, height);
        //    if (width != this.width || height != this.height)
        //    {
        //        this.width = width;
        //        this.height = height;
        //        if (width > height)
        //        {
        //            outerStack.Orientation = StackOrientation.Horizontal;
        //        }
        //        else
        //        {
        //            outerStack.Orientation = StackOrientation.Vertical;
        //        }
        //    }
        //}

        private async void imprimir (Boolean test)
        {
         

            if (test) { flagImpreso = true; return; }
            Color c = Color.Gray;
            flagImpreso = false;
            String camino = Application.Current.Properties["CaminoAFichero"] as string;
            String fichero = Application.Current.Properties["Fichero"] as string;
            String fileName = Path.Combine(camino, fichero);
            if (!File.Exists(fileName))
            {
                c = Color.Red;
                lblStatus.BackgroundColor = c;
                // lblStatus.Text = "No existe " + fileName;
                lblStatus.Text = "No hay albarán a imprimir";
                return;
            }

            // PASO 2 -> Conexión por Bluetooth ----------------------------------------------------
            //lblStatus.Text = "Conectando Bluetooth";
            Console.WriteLine("paso 22");
            String impresora = Application.Current.Properties["Impresora"] as string;
            BT = new Bluetooth3(impresora);
            // BT.BluetoothEnviarFichero(impresora, camino, fileName);
            Console.WriteLine("vuelve de bluetoothenviarfichero");
            await BT.Imprime(impresora, camino, fileName);
            //Console.WriteLine("paso 3");


            //BT3 = new Bluetooth3(impresora);
            //await BT3.Imprime(impresora, camino, fileName);

            Thread.Sleep(1000);
            if (BT.estado == false)
                {
                c = Color.Red;
                lblStatus.BackgroundColor = c;
                lblStatus.Text = BT.mensaje;
                return;
            }
            else
            {
                flagImpreso = true;
                Console.WriteLine("paso 3");
                //           BT.BluetoothEnviarFichero(camino, fileName, impresora);
                Console.WriteLine("vuelta aaaaaa");
                c = Color.Green;
                lblStatus.BackgroundColor = c;
                lblStatus.Text = "Albarán impreso";
                btnDescargar.Text = "Descargar albarán";

                if ((flagDosCopias==false) &&  (File.Exists(fileName)))
                {
                    try
                    {
                        System.IO.File.Delete(fileName); // 14-02-19 Borra el fichero del albarán ya impreso solo si no es con segunda copia
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Text = ex.Message;
                        return;
                    }
                }
            }

            //BT = new Bluetooth(impresora);
            //BT.BluetoothConecta(impresora);
            //if (BT.mensaje != "")
            //{
            //    c = Color.Red;
            //    lblStatus.BackgroundColor = c;
            //    lblStatus.Text = BT.mensaje;
            //    return;
            //}

            //// PASO 3 -> Impresión por Bluetooth ----------------------------------------------------
            //lblStatus.Text = "Enviando a Bluetooth";
            //String Resultado = BT.BluetoothEnviarFichero(camino, fileName, impresora);
            //if (BT.estado == false)
            //{
            //    c = Color.Red;
            //    lblStatus.BackgroundColor = c;
            //    lblStatus.Text = BT.mensaje;
            //    return;
            //}




        }

    }
}