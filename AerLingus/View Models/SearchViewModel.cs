using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AerLingus.Helpers;
using AerLingus.Models;

namespace AerLingus.View_Models
{
    public class SearchViewModel
    {
        public SearchFlightRecord Search { get; set; }
        public List<Flight_Records> FlightRecords { get; set; }
    }
}