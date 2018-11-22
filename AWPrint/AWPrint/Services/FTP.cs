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
        public static String cori = "";
        public static String cdes = "";
        public static String fdes = "";

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


        //private static void ResponseCallback(IAsyncResult ar)
        //{
        //    FtpWebRequest ftpreq = null;
        //    try
        //    {
        //        ftpreq = (FtpWebRequest)ar.AsyncState;
        //        // did not bother to finish as the error happen on the next line!
        //        FtpWebResponse ftpres = ftpreq.EndGetResponse(ar) as FtpWebResponse;
        //       // Stream responseStream = ftpres.GetResponseStream();
        //        //FileStream fileStream = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "test.txt"));
        //        //byte[] BufferRead = null;
        //        //responseStream.BeginRead(BufferRead, 0, 1284, new AsyncCallback(ReadCallback), state);

        //        Stream responseStream = ftpres.GetResponseStream();
        //        StreamReader reader = new StreamReader(responseStream);
        //        int dataLength = (int)ftpreq.GetResponse().ContentLength;

        //        byte[] buffer = new byte[2048];

        //        if (!Directory.Exists(cdes))
        //        {
        //            //mensaje = "No existe " + carpetaDestino;
        //            //return false;
        //        }

        //        String fileName = Path.Combine(cdes, fdes);

        //        try
        //        {
        //            File.Delete(fileName);
        //        }
        //        catch (Exception)
        //        {
        //            //mensaje = "No existe " + fileName;
        //            //return false;
        //        }

        //        FileStream fs;
        //        try
        //        {
        //            fs = new FileStream(fileName, FileMode.Create);
        //        }
        //        catch (System.UnauthorizedAccessException e)
        //        {
        //            //mensaje = e.Message;
        //            //return false;
        //        }
        //        catch (System.IO.IOException)
        //        {

        //        }

        //        int ReadCount = responseStream.Read(buffer, 0, buffer.Length);
        //        while (ReadCount > 0)
        //        {
        //            fs.Write(buffer, 0, ReadCount);
        //            ReadCount = responseStream.Read(buffer, 0, buffer.Length);
        //        }
        //        //      ResponseDescription = response.StatusDescription;
        //        fs.Close();
        //        reader.Close();
        //        ftpres.Close();
        //        if (!File.Exists(fileName))
        //        {

        //        }



        //    }
        //    catch (Exception e)
        //    {
        //        //handleError(e, state);
        //    }
        //}




        public Boolean FTPDescargaFichero(String carpetaOrigen, String ficheroOrigen, String carpetaDestino, String ficheroDestino)
        {
            estado = false;
            carpetaOrigen = carpetaOrigen == "" ? "/" : carpetaOrigen;
            carpetaOrigen = carpetaOrigen.EndsWith("/") ? carpetaOrigen : carpetaOrigen + "/";
            var uri = new Uri("ftp://" + mFTPServer + carpetaOrigen + ficheroOrigen );
            //cori = carpetaOrigen;
            //cdes = carpetaDestino;
            //fdes = ficheroDestino;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.EnableSsl = mFTPSSL;
            request.Timeout = 60000;
            request.ReadWriteTimeout = 60000;
            request.KeepAlive = true;
            request.UsePassive = false;
                    
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

            //request.BeginGetResponse(new AsyncCallback(ResponseCallback), request);


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

            try
            {
                File.Delete(fileName);
            }
            catch (Exception)
            {
                //mensaje = "No existe " + fileName;
                //return false;
            }

            FileStream fs;
            try
            {
                fs = new FileStream(fileName, FileMode.Create);
            }
            catch (System.UnauthorizedAccessException e)
            {
                mensaje = e.Message;
                return false;
            }
            catch (System.IO.IOException)
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
