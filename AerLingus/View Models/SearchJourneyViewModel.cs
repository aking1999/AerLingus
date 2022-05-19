using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AerLingus.Helpers;
using AerLingus.Models;

namespace AerLingus.View_Models
{
    public class SearchJourneyViewModel
    {
        public SearchJourney SearchJourney { get; set; }
        public List<Journey> Journeys { get; set; }
    }
}