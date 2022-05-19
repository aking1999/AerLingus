using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AerLingus.Models;
using AerLingus.Helpers;

namespace AerLingus.Validations
{
    public abstract class Validation
    {
        private static AerLingus_databaseEntities entities = new AerLingus_databaseEntities();

        public static Flight_Records TicketNoValidation(Flight_Records record)
        {
            return (from recordInDatabase in entities.Flight_Records
                    where recordInDatabase.ticketNo == record.ticketNo &&
                          recordInDatabase.departureDate == record.departureDate &&
                          recordInDatabase.origin == record.origin
                    select recordInDatabase).SingleOrDefault();
        }

        public static Flight_Records ExternalPaxIDValidation(Flight_Records record)
        {
            return (from recordInDatabase in entities.Flight_Records
                    where recordInDatabase.externalPaxID == record.externalPaxID &&
                          recordInDatabase.departureDate == record.departureDate &&
                          recordInDatabase.origin == record.origin
                    select recordInDatabase).SingleOrDefault();
        }

        public static void SetEmptyPropertiesToNull(Flight_Records record)
        {
            record.identifierNo = record.identifierNo == string.Empty ? null : record.identifierNo;
            record.otherFFPNo = record.otherFFPNo == string.Empty ? null : record.otherFFPNo;
            record.otherFFPScheme = record.otherFFPScheme == string.Empty ? null : record.otherFFPScheme;
            record.partnerTransactionNo = record.partnerTransactionNo == string.Empty ? null : record.partnerTransactionNo;
            record.bookingDate = record.bookingDate == default(DateTime) ? null : record.bookingDate;
            record.cabinClass = record.cabinClass == string.Empty ? null : record.cabinClass;
            record.ticketNo = record.ticketNo == string.Empty ? null : record.ticketNo;
            record.externalPaxID = record.externalPaxID == string.Empty ? null : record.externalPaxID;
            record.couponNo = record.couponNo == string.Empty ? null : record.couponNo;
            record.distance = record.distance == default(long) ? null : record.distance;
            record.baseFare = record.baseFare == default(double) ? null : record.baseFare;
            record.discountBase = record.discountBase == default(double) ? null : record.discountBase;
            record.exciseTax = record.exciseTax == default(double) ? null : record.exciseTax;
            record.customerType = record.customerType == string.Empty ? null : record.customerType;
            record.promotionCode = record.promotionCode == string.Empty ? null : record.promotionCode;
            record.ticketCurrency = record.ticketCurrency == string.Empty ? null : record.ticketCurrency;
            record.targetCurrency = record.targetCurrency == string.Empty ? null : record.targetCurrency;
            record.exchangeRate = record.exchangeRate == default(double) ? null : record.exchangeRate;
            record.fareBasis = record.fareBasis == string.Empty ? null : record.fareBasis;
        }

        public static bool Exists(Flight_Records record)
        {
            if (record.ticketNo != null)
            {
                if (TicketNoValidation(record) != null)
                {
                    return true;
                }
                else return false;
            }
            else if (record.externalPaxID != null)
            {
                if (ExternalPaxIDValidation(record) != null)
                {
                    return true;
                }
                else return false;
            }
            else throw new Exception("Record must either have Ticket Number or External Pax ID");
        }

        public static void SetEmptyPropertiesToNull(SearchFlightRecord search)
        {
            search.S_identifierNo = search.S_identifierNo == string.Empty ? null : search.S_identifierNo;
            search.S_otherFFPNo = search.S_otherFFPNo == string.Empty ? null : search.S_otherFFPNo;
            search.S_firstName = search.S_firstName == string.Empty ? null : search.S_firstName;
            search.S_lastName = search.S_lastName == string.Empty ? null : search.S_lastName;
            search.S_departureDate = search.S_departureDate == null ? null : search.S_departureDate;
            search.S_Origin = search.S_Origin == string.Empty ? null : search.S_Origin;
            search.S_destination = search.S_destination == string.Empty ? null : search.S_destination;
            search.S_bookingClass = search.S_bookingClass == string.Empty ? null : search.S_bookingClass;
            search.S_operatingAirline = search.S_operatingAirline == string.Empty ? null : search.S_operatingAirline;
            search.S_ticketNo = search.S_ticketNo == string.Empty ? null : search.S_ticketNo;
            search.S_externalPaxID = search.S_externalPaxID == string.Empty ? null : search.S_externalPaxID;
            search.S_pnrNo = search.S_pnrNo == string.Empty ? null : search.S_pnrNo;
        }

        public static void TrimBeginEnd(SearchFlightRecord search)
        {
            search.S_identifierNo = search.S_identifierNo.Trim();
            search.S_firstName = search.S_firstName.Trim();
            search.S_lastName = search.S_lastName.Trim();
            search.S_externalPaxID = search.S_externalPaxID.Trim();
            search.S_ticketNo = search.S_ticketNo.Trim();
            search.S_otherFFPNo = search.S_otherFFPNo.Trim();
            search.S_Origin = search.S_Origin.Trim();
            search.S_operatingAirline = search.S_operatingAirline.Trim();
            search.S_destination = search.S_destination.Trim();
            search.S_bookingClass = search.S_bookingClass.Trim();
            search.S_pnrNo = search.S_pnrNo.Trim();
        }
    }
}