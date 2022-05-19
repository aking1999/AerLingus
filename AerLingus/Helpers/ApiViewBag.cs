using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AerLingus.Models;

namespace AerLingus.Helpers
{
    public class ApiViewBag
    {
        public static class UploadRequest
        {
            public static bool RequestIsComingFromController { get; set; } = false;
            public static HttpPostedFileBase RequestedFile { get; set; }
        }
        
        public static class SearchRequest
        {
            public static bool RequestIsComingFromController { get; set; } = false;
            public static string identifierNo { get; set; }
            public static string otherFFPNo { get; set; }
            public static string firstName { get; set; }
            public static string lastName { get; set; }
            public static Nullable<DateTime> departureDate { get; set; }
            public static string origin { get; set; }
            public static string destination { get; set; }
            public static string bookingClass { get; set; }
            public static string operatingAirline { get; set; }
            public static string ticketNo { get; set; }
            public static string externalPaxID { get; set; }
            public static string pnrNo { get; set; }
        }

        public static class ExportRequest
        {
            public static List<Flight_Records> SearchedItems { get; set; }
        }
    }
}