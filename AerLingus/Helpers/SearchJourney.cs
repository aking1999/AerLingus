using AerLingus.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AerLingus.Helpers
{
    public class SearchJourney
    {
        [MaxLength(30)]
        [SearchJourneyValidation]
        [DisplayName("Identifier Number")]
        public string identifierNo { get; set; }

        [MaxLength(30)]
        [DisplayName("First Name")]
        public string firstName { get; set; }

        [MaxLength(30)]
        [DisplayName("Last Name")]
        public string lastName { get; set; }

        [MaxLength(14)]
        [DisplayName("Ticket Number")]
        public string ticketNo { get; set; }
    }
}