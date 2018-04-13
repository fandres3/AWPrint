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


namespace AWPrint
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImpresionPage : ContentPage
	{
		public ImpresionPage(String MensajeInicial)
		{
			InitializeComponent ();
            lblStatus.Text = MensajeInicial;
            if (MensajeInicial.StartsWith("ERROR:")) lblStatus.TextColor = Color.Red;
                else lblStatus.TextColor = Color.Green;
		}

        protected override void OnAppearing()
        {
    
            base.OnAppearing();
        }

        void BtnImprimirClicked(object sender, System.EventArgs e)
        {
          

            // Read data from the device
           // await _socket.InputStream.ReadAsync(buffer, 0, buffer.Length);

            // Write data to the device
            //await _socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}