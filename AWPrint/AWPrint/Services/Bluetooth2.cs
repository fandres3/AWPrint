using Android.Bluetooth;
using Android.OS;
using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWPrint.Services
{


    public class Bluetooth2
    {

        public static BluetoothManager mBTManager = null;
        public static BluetoothAdapter mBluetoothAdapter = null;
        public static BluetoothDevice mDevice = null;
        public static BluetoothSocket mSocket = null;
        public static BluetoothAdapter mAdapter = null;
        public static String mNombreDispositivo = null;
        static System.IO.Stream mmOutputStream;
        static System.IO.Stream mmInputStream;
        public Boolean estado = false;
        public String mensaje = "";

        public Bluetooth2(String nombreDispositivo)
        {

        }

        public async Task Imprime(String nombreDispositivo, String carpetaAFichero, String fichero)
        {
            await ImprimeBluetooth(nombreDispositivo, carpetaAFichero, fichero);
        }

        private async Task z(String msg)
        {
            mensaje = msg;
            await Task.Delay( 1);
        }

        public async Task ImprimeBluetooth(String nombreDispositivo, String carpetaAFichero, String fichero)
        {
            estado = true;

            if (!(mSocket==null))
            {
                if (mSocket.IsConnected) { await z(""); return; }
            }

            mAdapter = BluetoothAdapter.DefaultAdapter;

            if (mAdapter == null)
            {
                estado = false;
                await z("No encontrado adaptador Bluetooth");
            }

            if (!mAdapter.IsEnabled)
            {
                mAdapter.Enable();
            }

            if (!mAdapter.IsEnabled)
            {
                estado = false;
                await z("Adaptador Bluetooth no conectado");
            }

            mDevice = (from bd in mAdapter.BondedDevices
                       where bd.Name == nombreDispositivo
                       select bd).FirstOrDefault();
            if (mDevice == null)
            {
                estado = false;
                await z("Dispositivo " + nombreDispositivo + " no encontrado");
            }



            // mSocket = null;
            //mSocket = mDevice.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            // 00001101-0000-1000-8000-00805f9b34fb Original
            // 0000111f-0000-1000-8000-00805f9b34fb     
            // 00001132-0000-1000-8000-00805f9b34fb
            // 00000000-deca-fade-deca-deafdecacafe

            mSocket = mDevice.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            //mSocket = mDevice.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            //mSocket.Connect();
            await mSocket.ConnectAsync();
            //ParcelUuid[] uuids = null;
            //if (mDevice.FetchUuidsWithSdp())
            //{
            //    uuids = mDevice.GetUuids();
            //}

            //if ((uuids != null) && (uuids.Length > 0))
            //{
            //    foreach (var uuid in uuids)
            //    {
            //        try
            //        {
            //            mSocket = mDevice.CreateRfcommSocketToServiceRecord(uuid.Uuid);
            //            // mSocket = mDevice.CreateInsecureRfcommSocketToServiceRecord(uuid.Uuid);
            //            mSocket.Connect();
            //            if (mSocket.IsConnected)
            //            {
            //                break;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            mSocket = mDevice.CreateInsecureRfcommSocketToServiceRecord(uuid.Uuid);
            //            mSocket.Connect();
            //            if (mSocket.IsConnected)
            //            {
            //                break;
            //            }
            //            //mSocket=(BluetoothSocket) mDevice.getClass().getMethod("createRfcommSocket", new Class[] { int.class}).invoke(mDevice,1);
            //            // return "Error: " + ex.Message;
            //        }
            //    }
            //}

            if (!mSocket.IsConnected)
            {
                estado = false;
                await z("Socket no conectado");
                return;
            }
            Console.WriteLine("paso 21");
            // if (estado == true) BluetoothEnviarFichero(carpetaAFichero, fichero, nombreDispositivo);
            //await z("");
        }

        private async Task BluetoothEnviarFichero(String carpetaAFichero, String fichero, String nombreDispositivo)
        {
            Console.WriteLine("dEEEEEEEEEENTRO 1");
            String fileName = Path.Combine(carpetaAFichero, fichero);

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

            int nAlbaran = message.IndexOf("NRO ALBARAN");
            string strAlbaran = "";
            if (nAlbaran > -1)
            {
                strAlbaran = message.Substring(nAlbaran + 14, 12); // Extrae el albarán para mostrarlo en pantalla antes de imprimir
            }

            int final = message.IndexOf("...");


            if (final > -1)
            {
                message = message.Substring(0, final);
            }

            message = message + "\r\n";
            // ----
            Encoding u8 = Encoding.UTF8;
            byte[] buffer = u8.GetBytes(message);
            // Read data from the device
            Console.WriteLine("dEEEEEEEEEENTRO 2");
            int x = await mSocket.InputStream.ReadAsync(buffer, 0, buffer.Length);
            Console.WriteLine("111111111111111111111111");
            // https://brianpeek.com/connect-to-a-bluetooth-device-with-xamarinandroid/
            // https://forums.xamarin.com/discussion/6576/how-to-send-data-to-printer
            // https://stackoverflow.com/questions/33775823/send-data-to-bluetooth-printer
            // https://www.androidcode.ninja/android-bluetooth-tutorial/

            buffer = u8.GetBytes(message);

            // Write data to the device
            await mSocket.OutputStream.WriteAsync(buffer, 0, buffer.Length);

            Console.WriteLine("222222222222222222222222222222222");
            mmOutputStream = mSocket.OutputStream;
            mmInputStream = mSocket.InputStream;

            mmOutputStream.Close();
            mmInputStream.Close();
            mSocket.Close();
           // mSocket = null;
            mensaje = "Correcto";
            //return "";
            Console.WriteLine("paso 31");
            await z("");
        }
    }





  

}


