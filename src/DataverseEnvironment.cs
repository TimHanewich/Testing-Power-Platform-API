using System;

namespace TestPowerAPI
{
    public class DataverseEnvironment
    {
        public string TenantID {get; set;} //ID of the tenant it is in
        public string ID {get; set;}
        public string DisplayName {get; set;}
        public string URL {get; set;}

        public DataverseEnvironment()
        {
            TenantID = "";
            ID = "";
            DisplayName = "";
            URL = "";
        }

        public string ToSelectionString()
        {
            return DisplayName + " (" + URL + ")";
        }


    }
}