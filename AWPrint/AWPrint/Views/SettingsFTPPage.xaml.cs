﻿using System;
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
using AWPrint.Services;
using System.Threading;


namespace AWPrint
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsFTPPage : ContentPage
    {

     
        public static FTP Ftp;
        public static Bluetooth2 BT;

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
            lblStatus.Text = "";
            String sw = Application.Current.Properties["FTPSSL"] as string;
            Boolean swFTPSSL = (sw != "0");
            Ftp = new FTP(Application.Current.Properties["FTPServer"] as string,
                Application.Current.Properties["FTPUser"] as string,
                Application.Current.Properties["FTPPassword"] as string,
                swFTPSSL,
                Application.Current.Properties["FTPCarpeta"] as string);

            Color c = Color.Green;
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
            lblStatus.BackgroundColor = c;
            lblStatus.Text = Ftp.mensaje;
            Ftp = null;
        }


       async void BtnPruebaImpresionClickedAsync(object sender, EventArgs args)
        {
            lblStatus.Text = "";
            Color c = Color.Green;
            String impresora = Application.Current.Properties["Impresora"] as string;
            String camino = Application.Current.Properties["CaminoAFichero"] as string;
            String fichero = Application.Current.Properties["Fichero"] as string;
            BT = new Bluetooth2(impresora);
            ////BT.ImprimeBluetooth(impresora, camino, fileName);
            await BT.Imprime(impresora, camino, fichero);
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
                Console.WriteLine("paso 3");
                BT.BluetoothEnviarFichero(camino, fichero, impresora);
                Console.WriteLine("vuelta aaaaaa");
                lblStatus.Text = "Albarán impreso";
            }


        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            lblStatus.Text = "";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            lblStatus.Text = "";
        }

        async void BtnPruebaInternetClickedAsync(object sender, EventArgs args)
        {
            NetworkCheck CheckNetworkAccess = new NetworkCheck();

            if (CheckNetworkAccess.IsNetworkConnected())
            {
                lblStatus.Text = "Conexión correcta";
            }
            else
            {
                await DisplayAlert("AWPrint", "Revise conexión a internet", "OK");
            }
        }

    }
}