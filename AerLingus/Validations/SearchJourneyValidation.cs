using AerLingus.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AerLingus.Validations
{
    public class SearchJourneyValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var journey = (SearchJourney)validationContext.ObjectInstance;

            if(journey.identifierNo == null && journey.lastName == null &&
                journey.firstName == null && journey.ticketNo == null)
            {
                return new ValidationResult("At least one field is required");
            }

            return ValidationResult.Success;
        }
    }
}