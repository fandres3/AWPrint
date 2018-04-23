using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AWPrint.Services
{
    public class FTP
    {
        private static String mFTPServer = null;
        private static String mFTPUser = null;
        private static String mFTPPassword = null;
        private static String mFTPCarpeta = null;
        private static Boolean mFTPSSL = false;
        public Boolean estado = false;
        public String mensaje = "";

        public FTP(String FTPServer, String FTPUser, String FTPPassword, Boolean FTPSSL, String FTPCarpeta)
        {
            mFTPServer = FTPServer;
            mFTPUser = FTPUser;
            mFTPPassword = FTPPassword;
            mFTPCarpeta = FTPCarpeta;
            mFTPSSL = FTPSSL;
        }

        private Boolean FTPValida()
        {
            if (mFTPServer == "" || mFTPUser == "" || mFTPPassword == "") return false; 
            return true;
        }


        public Boolean FTPDescargaFichero(String carpetaOrigen, String ficheroOrigen, String carpetaDestino, String ficheroDestino)
        {
            estado = false;
            carpetaOrigen = carpetaOrigen == "" ? "/" : carpetaOrigen;
            carpetaOrigen = carpetaOrigen.EndsWith("/") ? carpetaOrigen : carpetaOrigen + "/";
            var uri = new Uri("ftp://" + mFTPServer + carpetaOrigen + ficheroOrigen);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.EnableSsl = mFTPSSL;
            try
            {
                request.Credentials = new NetworkCredential(mFTPUser, mFTPPassword);
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                return false;
            }

            FtpWebResponse response;
            try
            {
                response = (FtpWebResponse)request.GetResponse();

            }
            catch (System.Net.WebException e)
            {
                mensaje = e.Message;
                return false;
            }

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            int dataLength = (int)request.GetResponse().ContentLength;

            byte[] buffer = new byte[2048];

            if (!Directory.Exists(carpetaDestino))
            {
                mensaje = "No existe " + carpetaDestino;
                return false;
            }

            String fileName = Path.Combine(carpetaDestino, ficheroDestino);
            File.Delete(fileName);

            FileStream fs;
            try
            {
                fs = new FileStream(fileName, FileMode.Create);
            }
            catch (System.IO.IOException e)
            {
                mensaje = "Error creación " + ficheroDestino; ;
                return false;
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
            if (!File.Exists(fileName))
            {
                mensaje = "No descargado";
                return false;
            }

            estado = true;
            mensaje = "Correcto";
            return true;
        }

    }
}
