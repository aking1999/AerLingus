using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AerLingus.Validations;


namespace AerLingus.Helpers
{
    public class SearchFlightRecord
    {
        [SearchFlightRecordValidation]
		[Display(Name = "Identifier Number")]
        public string S_identifierNo { get; set; }

		[Display(Name = "Other FFP Number")]
        public string S_otherFFPNo { get; set; }

		[Display(Name = "First Name")]
        public string S_firstName { get; set; }

		[Display(Name = "Last Name")]
        public string S_lastName { get; set; }

		[Display(Name = "Departure Date")]
        public Nullable<DateTime> S_departureDate { get; set; }

		[Display(Name = "Origin")]
        public string S_Origin { get; set; }

		[Display(Name = "Destination")]       
        public string S_destination { get; set; }

		[Display(Name = "Booking Class")]
        public string S_bookingClass { get; set; }

		[Display(Name = "Operating Airline")]
        public string S_operatingAirline { get; set; }

		[Display(Name = "Ticket Number")]
        public string S_ticketNo { get; set; }

		[Display(Name = "External Pax ID")]
        public string S_externalPaxID { get; set; }

		[Display(Name = "PNR Number")]
        public string S_pnrNo { get; set; }
	}
}