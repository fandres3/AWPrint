using Android.Bluetooth;
using Java.IO;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public Boolean estado = false;
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

            if (mAdapter == null)
            {
                estado = false;
                await z("No encontrado adaptador Bluetooth");


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



                //BluetoothDevice hxm = BluetoothAdapter.DefaultAdapter.GetRemoteDevice (bt_printer);
                UUID applicationUUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");


                socket = mDevice.CreateRfcommSocketToServiceRecord(applicationUUID);
                socket.Connect();
                inReader = new BufferedReader(new InputStreamReader(socket.InputStream));
                outReader = new BufferedWriter(new OutputStreamWriter(socket.OutputStream));
                outReader.Write("asdfasdfasd\r\n");


                outReader.Flush();
                Thread.Sleep(5 * 1000);
                var s = inReader.Ready();
                inReader.Skip(0);
                //close all
                inReader.Close();
                socket.Close();
                outReader.Close();

            }
        }
    }
}

     