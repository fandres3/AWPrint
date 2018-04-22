using Android.Bluetooth;
using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AWPrint.Services
{


    public class Bluetooth
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

        public Bluetooth(String nombreDispositivo)
        {

        }

        public BluetoothAdapter BluetoothAdaptador()
        {
            mAdapter = BluetoothAdapter.DefaultAdapter;
            if (mAdapter == null) { mensaje = "No encontrado adaptador Bluetooth"; estado = false;  return null; }

            if (!mAdapter.IsEnabled)
            {
                mensaje = "Adaptador Bluetooth no conectado";
                estado = false;
                return null;
            }
            return mAdapter;
        }

        public BluetoothDevice BluetoothDispositivo(String nombreDispositivo)
        {
            mDevice = (from bd in mAdapter.BondedDevices
                        where bd.Name == nombreDispositivo
                        select bd).FirstOrDefault();
            if (mDevice == null)
            {
                mensaje = "Dispositivo " + nombreDispositivo + " no encontrado";
                estado = false;
                return null;
            }
            return mDevice;
        }

        public BluetoothSocket BluetoothPaquete()
        {
            try
            {
                mSocket = null;
                mSocket = mDevice.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                //_socket = device.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));

                mSocket.Connect();
                estado = true;
                mensaje = "Correcto";
                return mSocket;

            }
            catch (Exception e)
            {
                mensaje = e.Message;
                estado = false;
                return null;
            }
        }

        public String BluetoothConecta(String nombreDispositivo)
        {
            // Conecta con Bluetooth
            // Devuelve string "" si todo bien. Si error devuelve el mensaje de error.
            if (mSocket != null) return ""; // Ya conectado
            if (BluetoothAdaptador() == null) return mensaje;
            if (nombreDispositivo != null)
            {
                if (BluetoothDispositivo(nombreDispositivo) == null) return mensaje;
            }
           
            if (BluetoothPaquete() == null) return mensaje;
            return "";
        }

        public String BluetoothEnviarFichero(String carpetaAFichero, String fichero, String nombreDispositivo )
        {
            // Si no hay conexión previa vuelve a conectar
            if (mSocket == null) {
                if (BluetoothConecta(nombreDispositivo) == null) { estado = false; return mensaje; }
            }
            String fileName = Path.Combine(carpetaAFichero, fichero);

            String message = null;
            using (var streamReader = new StreamReader(fileName))
            {
                message = streamReader.ReadToEnd();
            }

            Encoding u8 = Encoding.UTF8;
            byte[] buffer = u8.GetBytes(message);
            // Read data from the device
            mSocket.InputStream.ReadAsync(buffer, 0, buffer.Length);

            // https://brianpeek.com/connect-to-a-bluetooth-device-with-xamarinandroid/
            // https://forums.xamarin.com/discussion/6576/how-to-send-data-to-printer
            // https://stackoverflow.com/questions/33775823/send-data-to-bluetooth-printer
            // https://www.androidcode.ninja/android-bluetooth-tutorial/

            buffer = u8.GetBytes(message);

            // Write data to the device
            mSocket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            mmOutputStream = mSocket.OutputStream;
            mmInputStream = mSocket.InputStream;

            mmOutputStream.Close();
            mmInputStream.Close();
            mSocket.Close();
            mSocket = null;
            mensaje = "Correcto";
            return "";

        }

    }
}
