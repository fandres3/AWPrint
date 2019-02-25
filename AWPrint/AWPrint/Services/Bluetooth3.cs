using Android.Bluetooth;
using Java.IO;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace AWPrint.Services
{
    public class Bluetooth3
    {

        BluetoothSocket socket = null;
        BufferedReader inReader = null;
        BufferedWriter outReader = null;
        public static BluetoothDevice mDevice = null;
        public static BluetoothSocket mSocket = null;
        public static BluetoothAdapter mAdapter = null;
        public static String mNombreDispositivo = null;
        public Boolean estado = true;
        public String mensaje = "";

        public Bluetooth3(String nombreDispositivo)
        {

        }

        public async Task Imprime(String nombreDispositivo, String carpetaAFichero, String fichero)
        {
            await ImprimeBluetooth(nombreDispositivo, carpetaAFichero, fichero);
        }

        private async Task z(String msg)
        {
            mensaje = msg;
            await Task.Delay(1);
        }

        public async Task ImprimeBluetooth(String nombreDispositivo, String carpetaAFichero, String fichero)
        {
            mAdapter = BluetoothAdapter.DefaultAdapter;
            if (mAdapter == null) { estado = false; return; }

            if (mAdapter != null)
            {

                if (!mAdapter.IsEnabled)
                {
                    mAdapter.Enable();
                }

                if (!mAdapter.IsEnabled)
                {
                    estado = false;
                    await z("Adaptador Bluetooth no conectado");
                    return;
                }

                mDevice = (from bd in mAdapter.BondedDevices
                           where bd.Name == nombreDispositivo
                           select bd).FirstOrDefault();
                if (mDevice == null)
                {
                    estado = false;
                    await z("Dispositivo " + nombreDispositivo + " no encontrado");
                    return;
                }


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


                //BluetoothDevice hxm = BluetoothAdapter.DefaultAdapter.GetRemoteDevice (bt_printer);
                UUID applicationUUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");


                socket = mDevice.CreateRfcommSocketToServiceRecord(applicationUUID);
                socket.Connect();
                inReader = new BufferedReader(new InputStreamReader(socket.InputStream));
                outReader = new BufferedWriter(new OutputStreamWriter(socket.OutputStream));
                outReader.Write(message);


                outReader.Flush();
                Thread.Sleep(5 * 1000);
                var s = inReader.Ready();
                inReader.Skip(0);
                //close all
                inReader.Close();
                socket.Close();
                outReader.Close();
                estado = true;
            }
        }
    }
}

     