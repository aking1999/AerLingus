using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AerLingus.Models;
//cao
namespace AerLingus.Models
{
    public class EIValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var fr = (Flight_Records)validationContext.ObjectInstance;

            if (fr.operatingAirline == "EI")
            {
                if (fr.targetCurrency == null || fr.ticketCurrency == null || fr.exchangeRate == null)
                {
                    return new ValidationResult("Field is required when operating airline is 'EI'");
                }
                else
                    return ValidationResult.Success;
            }
            else
                return ValidationResult.Success;



        }
    }
}