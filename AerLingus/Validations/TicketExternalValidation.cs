using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AerLingus.Models;

namespace AerLingus.Validations
{
    public class TicketExternalValidation: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var fr = (Flight_Records)validationContext.ObjectInstance;

            if(fr.externalPaxID == null && fr.ticketNo == null )
            {
                return new ValidationResult("Either the ticket number or externalPaxID has to be provided!");
            }
            return ValidationResult.Success;
        }
    }
}