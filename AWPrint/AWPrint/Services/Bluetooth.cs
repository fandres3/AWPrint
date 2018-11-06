using Android.Bluetooth;
using Android.OS;
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
            if (mAdapter == null)
            {
                mensaje = "No encontrado adaptador Bluetooth";
                estado = false;
                return null;
            }

            if (!mAdapter.IsEnabled)
            {
                mensaje = "Adaptador Bluetooth no conectado";
                estado = false;
                return null;
            }
            estado = true;
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
            estado = true;
            return mDevice;
        }

        public BluetoothSocket BluetoothPaquete()
        {
            try
            {
                mSocket = null;
                //mSocket = mDevice.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                // 00001101-0000-1000-8000-00805f9b34fb Original
                // 0000111f-0000-1000-8000-00805f9b34fb     
                // 00001132-0000-1000-8000-00805f9b34fb
                // 00000000-deca-fade-deca-deafdecacafe

                //mSocket = mDevice.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));

                ParcelUuid[] uuids = null;
                if (mDevice.FetchUuidsWithSdp())
                {
                    
                    uuids = mDevice.GetUuids();
                }
                if ((uuids != null) && (uuids.Length > 0))
                {
                    foreach (var uuid in uuids)
                    {
                        try
                        {
                            mSocket = mDevice.CreateRfcommSocketToServiceRecord(uuid.Uuid);
                            // mSocket = mDevice.CreateInsecureRfcommSocketToServiceRecord(uuid.Uuid);
                            mSocket.Connect();
                            estado = true;
                            mensaje = "Correcto";
                            return mSocket;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ex: " + ex.Message);
                            mensaje = ex.Message;
                            estado = false;
                            return null;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                mensaje = e.Message;
                estado = false;
                return null;
            }
            return null;
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
                if (BluetoothPaquete() == null) return mensaje;
           }
           
            return "";
        }

        public String BluetoothEnviarFichero(String carpetaAFichero, String fichero, String nombreDispositivo )
        {
            // Si no hay conexión previa vuelve a conectar
            if (mSocket == null) {
                if (BluetoothConecta(nombreDispositivo) != "")
                {
                    estado = false;
                    return mensaje;
                }
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

            if (final>-1)
            {
                message = message.Substring(0, final);
            }

            message = message + "\r\n";
            // ----
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
