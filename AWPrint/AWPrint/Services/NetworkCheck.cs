using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWPrint.Services
{
    public class NetworkCheck
    {
        public bool IsNetworkConnected()
        {
            bool retVal = false;

            try
            {
                retVal = CrossConnectivity.Current.IsConnected;
                return retVal;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
