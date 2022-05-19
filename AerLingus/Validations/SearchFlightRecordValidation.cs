using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AerLingus.Models;
using AerLingus.Helpers;

namespace AerLingus.Validations
{
    public class SearchFlightRecordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var fr = (SearchFlightRecord)validationContext.ObjectInstance;

            if (
                fr.S_identifierNo == null && fr.S_Origin == null && fr.S_destination == null &&
                fr.S_bookingClass == null && fr.S_externalPaxID == null && fr.S_ticketNo == null &&
                fr.S_pnrNo == null && fr.S_firstName == null && fr.S_lastName == null &&
                fr.S_departureDate == null && fr.S_operatingAirline == null && fr.S_otherFFPNo == null
                )
            {
                return new ValidationResult("At least one field is required");
            }

            return ValidationResult.Success;
        }
    }
}