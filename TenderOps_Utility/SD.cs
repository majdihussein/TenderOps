namespace TenderOps_Utility
{
    public static class SD
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public static string AccessToken = "JWTToken";
        public static string RefreshToken = "RefreshToken";

        public static string CurrentAPIVersion = "v2";

        public const string superadmin = "superadmin";
        public const string Admin = "admin";
        public const string Customer = "customer";
        public const string TestRole = "testrole";



        public enum ContentType
        {
            Json,
            MultipartFormData,
        }

        public enum InvoiceStatus
        {
            Pending,
            Paid,  
            UnPaid,
            Cancelled
        }

        public enum TenderLocation
        {
            Ramallah,
            Nablus,
            Hebron,
            Gaza,
            Jenin,
            Tulkarm,
            Jericho,
            Bethlehem
        }

    }
}
