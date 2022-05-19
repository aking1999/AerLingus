using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AerLingus.Models;
using System.IO;
using System.Web;
using Microsoft.VisualBasic;
using System.Text;
using AerLingus.Validations;
using System.Threading.Tasks;
using AerLingus.Helpers;

namespace AerLingus.Controllers.Api
{
    public class FlightRecordsApiController : ApiController
    {
        public string FailedRecords { get; set; }
        private AerLingus_databaseEntities entities;

        public FlightRecordsApiController()
        {
            FailedRecords = string.Empty;
            entities = new AerLingus_databaseEntities();
        }

        [HttpGet]
        public IEnumerable<Flight_Records> GetFlightRecords()
        {
            if (!entities.Flight_Records.Any())
                return default(IEnumerable<Flight_Records>);

            return entities.Flight_Records;
        }

        [HttpGet]
        public IHttpActionResult GetFlightRecord(int id)
        {
            var flightRecord = entities.Flight_Records.SingleOrDefault(fr => fr.ID == id);

            if (flightRecord == null)
                return NotFound();

            return Ok(flightRecord);
        }

        [HttpGet]
        [Route("api/FlightRecordsApi/SearchApi")]
        public List<Flight_Records> GetSearchedFlightRecordsApi()
        {
            DateTime departureDate = default(DateTime);
            bool departureDateConversionSuccessful = true;

            if (HttpContext.Current.Request.QueryString["departureDate"] != null)
                departureDateConversionSuccessful = DateTime.TryParse(HttpContext.Current.Request.QueryString["departureDate"], out departureDate);

            string identifierNo = HttpContext.Current.Request.QueryString["identifierNo"] != null ? HttpContext.Current.Request.QueryString["identifierNo"] : null;
            string otherFFPNo = HttpContext.Current.Request.QueryString["otherFFPNo"] != null ? HttpContext.Current.Request.QueryString["otherFFPNo"] : null;
            string firstName = HttpContext.Current.Request.QueryString["firstName"] != null ? HttpContext.Current.Request.QueryString["firstName"] : null;
            string lastName = HttpContext.Current.Request.QueryString["lastName"] != null ? HttpContext.Current.Request.QueryString["lastName"] : null;
            string origin = HttpContext.Current.Request.QueryString["origin"] != null ? HttpContext.Current.Request.QueryString["origin"] : null;
            string destination = HttpContext.Current.Request.QueryString["destination"] != null ? HttpContext.Current.Request.QueryString["destination"] : null;
            string bookingClass = HttpContext.Current.Request.QueryString["bookingClass"] != null ? HttpContext.Current.Request.QueryString["bookingClass"] : null;
            string operatingAirline = HttpContext.Current.Request.QueryString["operatingAirline"] != null ? HttpContext.Current.Request.QueryString["operatingAirline"] : null;
            string ticketNo = HttpContext.Current.Request.QueryString["ticketNo"] != null ? HttpContext.Current.Request.QueryString["ticketNo"] : null;
            string externalPaxID = HttpContext.Current.Request.QueryString["externalPaxID"] != null ? HttpContext.Current.Request.QueryString["externalPaxID"] : null;
            string pnrNo = HttpContext.Current.Request.QueryString["pnrNo"] != null ? HttpContext.Current.Request.QueryString["pnrNo"] : null;

            if (HttpContext.Current.Request.QueryString["departureDate"] != null && departureDateConversionSuccessful)
            {
                var searchedRecords = entities.Flight_Records.Where(fr =>
                                                        (identifierNo != null ? fr.identifierNo.StartsWith(identifierNo) : fr.identifierNo == fr.identifierNo) &&
                                                        (otherFFPNo != null ? fr.otherFFPNo.StartsWith(otherFFPNo) : fr.otherFFPNo == fr.otherFFPNo) &&
                                                        (pnrNo != null ? fr.pnrNo.StartsWith(pnrNo) : fr.pnrNo == fr.pnrNo) &&
                                                        (firstName != null ? fr.firstName.StartsWith(firstName) : fr.firstName == fr.firstName) &&
                                                        (lastName != null ? fr.lastName.StartsWith(lastName) : fr.lastName == fr.lastName) &&
                                                        (operatingAirline != null ? fr.operatingAirline.StartsWith(fr.operatingAirline) : fr.operatingAirline == fr.operatingAirline) &&
                                                        (externalPaxID != null ? fr.externalPaxID.StartsWith(externalPaxID) : fr.externalPaxID == fr.externalPaxID) &&
                                                        (ticketNo != null ? fr.ticketNo.StartsWith(ticketNo) : fr.ticketNo == fr.ticketNo) &&
                                                        (bookingClass != null ? fr.bookingClass.StartsWith(bookingClass) : fr.bookingClass == fr.bookingClass) &&
                                                        (departureDate != default(DateTime) ? fr.departureDate == departureDate : fr.departureDate == fr.departureDate) &&
                                                        (destination != null ? fr.destination.StartsWith(destination) : fr.destination == fr.destination) &&
                                                        (origin != null ? fr.origin.StartsWith(origin) : fr.origin == fr.origin)
                                                        ).ToList();

                return searchedRecords;
            }
            else if (HttpContext.Current.Request.QueryString["departureDate"] != null && !departureDateConversionSuccessful)
            {
                return default(List<Flight_Records>);
            }
            else
            {
                var searchedRecords = entities.Flight_Records.Where(fr =>
                                                        (identifierNo != null ? fr.identifierNo.StartsWith(identifierNo) : fr.identifierNo == fr.identifierNo) &&
                                                        (otherFFPNo != null ? fr.otherFFPNo.StartsWith(otherFFPNo) : fr.otherFFPNo == fr.otherFFPNo) &&
                                                        (pnrNo != null ? fr.pnrNo.StartsWith(pnrNo) : fr.pnrNo == fr.pnrNo) &&
                                                        (firstName != null ? fr.firstName.StartsWith(firstName) : fr.firstName == fr.firstName) &&
                                                        (lastName != null ? fr.lastName.StartsWith(lastName) : fr.lastName == fr.lastName) &&
                                                        (operatingAirline != null ? fr.operatingAirline.StartsWith(fr.operatingAirline) : fr.operatingAirline == fr.operatingAirline) &&
                                                        (externalPaxID != null ? fr.externalPaxID.StartsWith(externalPaxID) : fr.externalPaxID == fr.externalPaxID) &&
                                                        (ticketNo != null ? fr.ticketNo.StartsWith(ticketNo) : fr.ticketNo == fr.ticketNo) &&
                                                        (bookingClass != null ? fr.bookingClass.StartsWith(bookingClass) : fr.bookingClass == fr.bookingClass) &&
                                                        (destination != null ? fr.destination.StartsWith(destination) : fr.destination == fr.destination) &&
                                                        (origin != null ? fr.origin.StartsWith(origin) : fr.origin == fr.origin)
                                                        ).ToList();

                return searchedRecords;
            }
        }

        [Route("api/FlightRecordsApi/Search")]
        public List<Flight_Records> GetSearchedFlightRecords(SearchFlightRecord search)
        {
            /*if (search.S_identifierNo == null && search.S_otherFFPNo == null && search.S_pnrNo == null &&
                search.S_firstName == null && search.S_lastName == null && search.S_operatingAirline == null &&
                search.S_externalPaxID == null && search.S_ticketNo == null && search.S_Origin == null &&
                search.S_bookingClass == null && search.S_departureDate == null && search.S_destination == null
                )
                return new List<Flight_Records>();*/

            var searchedRecords = entities.Flight_Records.Where(fr =>
                                                            (search.S_identifierNo != null ? fr.identifierNo.StartsWith(search.S_identifierNo) : fr.identifierNo == fr.identifierNo) &&
                                                            (search.S_otherFFPNo != null ? fr.otherFFPNo.StartsWith(search.S_otherFFPNo) : fr.otherFFPNo == fr.otherFFPNo) &&
                                                            (search.S_pnrNo != null ? fr.pnrNo.StartsWith(search.S_pnrNo) : fr.pnrNo == fr.pnrNo) &&
                                                            (search.S_firstName != null ? fr.firstName.StartsWith(search.S_firstName) : fr.firstName == fr.firstName) &&
                                                            (search.S_lastName != null ? fr.lastName.StartsWith(search.S_lastName) : fr.lastName == fr.lastName) &&
                                                            (search.S_operatingAirline != null ? fr.operatingAirline.StartsWith(search.S_operatingAirline) : fr.operatingAirline == fr.operatingAirline) &&
                                                            (search.S_externalPaxID != null ? fr.externalPaxID.StartsWith(search.S_externalPaxID) : fr.externalPaxID == fr.externalPaxID) &&
                                                            (search.S_ticketNo != null ? fr.ticketNo.StartsWith(search.S_ticketNo) : fr.ticketNo == fr.ticketNo) &&
                                                            (search.S_bookingClass != null ? fr.bookingClass.StartsWith(search.S_bookingClass) : fr.bookingClass == fr.bookingClass) &&
                                                            (search.S_departureDate != null ? fr.departureDate == search.S_departureDate : fr.departureDate == fr.departureDate) &&
                                                            (search.S_destination != null ? fr.destination.StartsWith(search.S_destination) : fr.destination == fr.destination) &&
                                                            (search.S_Origin != null ? fr.origin.StartsWith(search.S_Origin) : fr.origin == fr.origin)
                                                            ).ToList();

            return searchedRecords;
        }

        [HttpPost]
        [Route("api/FlightRecordsApi/Upload")]
        public HttpResponseMessage Upload()
        {
            if (ApiViewBag.UploadRequest.RequestIsComingFromController)
            {
                ApiViewBag.UploadRequest.RequestIsComingFromController = false;

                HttpPostedFileBase file = ApiViewBag.UploadRequest.RequestedFile;

                Stream stream = file.InputStream;

                if (file == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                if (file.ContentLength == 0)
                    return Request.CreateResponse(HttpStatusCode.NoContent);

                var fileName = Path.GetFileName(file.FileName);

                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/UploadedFiles"), fileName);

                file.SaveAs(path);

                string failedToAddRecords = string.Empty;

                int recordsAdded = 0;
                int recordsNotAdded = 0;
                int numberOfFooterRecords = 0;
                int numberOfRecords = 0;

                try
                {
                    string tempRecord = string.Empty;

                    bool convertedFooterRecordsSuccessfully = false;

                    string header = string.Empty;
                    string[] headerArray = null;

                    string body = string.Empty;
                    string[] bodyArray = null;

                    string footer = string.Empty;
                    string[] footerArray = null;

                    char[] separator = new char[] { '|' };

                    Flight_Records record = new Flight_Records();

                    string content = string.Empty;

                    stream.Position = 0;

                    StreamReader streamReader1 = new StreamReader(stream);

                    header = streamReader1.ReadLine();

                    if (entities.FR_Batch_Files.Any(b => b.Header == header))
                        return Request.CreateResponse(HttpStatusCode.Conflict);

                    FR_Batch_Files batch = new FR_Batch_Files();

                    headerArray = header.Split(separator, StringSplitOptions.None);

                    if (headerArray[0].ToUpper() != "H")
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                    while (!streamReader1.EndOfStream)
                    {
                        footer = streamReader1.ReadLine();

                        footerArray = footer.Split(separator, StringSplitOptions.None);

                        numberOfRecords++;
                    }
                    numberOfRecords--;

                    if (footerArray[0] != "F")
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                    convertedFooterRecordsSuccessfully = int.TryParse(footerArray[1], out numberOfFooterRecords);

                    if (!convertedFooterRecordsSuccessfully)
                        return Request.CreateResponse(HttpStatusCode.BadRequest);

                    streamReader1.DiscardBufferedData();

                    stream.Position = 0;

                    System.IO.StreamReader streamReader = new System.IO.StreamReader(stream);

                    header = streamReader.ReadLine(); //H

                    headerArray = header.Split(separator, StringSplitOptions.None);

                    while (!streamReader.EndOfStream)
                    {
                        tempRecord = streamReader.ReadLine();

                        switch (tempRecord[0])
                        {

                            case 'R':
                                {
                                    bodyArray = tempRecord.Split(separator, StringSplitOptions.None);

                                    if (bodyArray[1] != string.Empty &&
                                        bodyArray[1].Length <= 16)
                                    {
                                        record.identifierNo = bodyArray[1];
                                    }
                                    else
                                    {
                                        record.identifierNo = null;
                                    }

                                    if (bodyArray[2] != string.Empty &&
                                        bodyArray[2].Length <= 2)
                                    {
                                        record.transactionType = bodyArray[2];
                                    }
                                    else
                                    {
                                        record.transactionType = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[3] != string.Empty &&
                                        bodyArray[3].Length <= 30)
                                    {
                                        record.otherFFPNo = bodyArray[3];
                                    }
                                    else
                                    {
                                        record.otherFFPNo = string.Empty;
                                    }

                                    if (bodyArray[4] != string.Empty &&
                                        bodyArray[4].Length <= 30)
                                    {
                                        record.otherFFPScheme = bodyArray[4];
                                    }
                                    else
                                    {
                                        record.otherFFPScheme = string.Empty;
                                    }

                                    if (bodyArray[5] != string.Empty &&
                                        bodyArray[5].Length <= 30)
                                    {
                                        record.firstName = bodyArray[5];
                                    }
                                    else
                                    {
                                        record.firstName = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[6] != string.Empty &&
                                        bodyArray[6].Length <= 30)
                                    {
                                        record.lastName = bodyArray[6];
                                    }
                                    else
                                    {
                                        record.lastName = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[7] != string.Empty &&
                                        bodyArray[7].Length <= 100)
                                    {
                                        record.partnerTransactionNo = bodyArray[7];
                                    }
                                    else
                                    {
                                        record.partnerTransactionNo = string.Empty;
                                    }

                                    if (bodyArray[8] != string.Empty)
                                    {
                                        record.bookingDate = Convert.ToDateTime(bodyArray[8]);
                                    }
                                    else
                                    {
                                        record.bookingDate = default(DateTime);
                                    }

                                    if (bodyArray[9] != string.Empty)
                                    {
                                        record.departureDate = Convert.ToDateTime(bodyArray[9]);
                                    }
                                    else
                                    {
                                        record.departureDate = default(DateTime);

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[10] != string.Empty &&
                                        bodyArray[10].Length <= 3)
                                    {
                                        record.origin = bodyArray[10];
                                    }
                                    else
                                    {
                                        record.origin = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[11] != string.Empty &&
                                        bodyArray[11].Length <= 3)
                                    {
                                        record.destination = bodyArray[11];
                                    }
                                    else
                                    {
                                        record.destination = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[12] != string.Empty &&
                                        bodyArray[12].Length <= 2)
                                    {
                                        record.bookingClass = bodyArray[12];
                                    }
                                    else
                                    {
                                        record.bookingClass = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[13] != string.Empty &&
                                        bodyArray[13].Length <= 1)
                                    {
                                        record.cabinClass = bodyArray[13];
                                    }
                                    else
                                    {
                                        record.cabinClass = string.Empty;
                                    }

                                    if (bodyArray[14] != string.Empty &&
                                        Information.IsNumeric(bodyArray[14]) &&
                                        bodyArray[14].Length <= 4)
                                    {
                                        record.marketingFlightNo = bodyArray[14];
                                    }
                                    else
                                    {
                                        record.marketingFlightNo = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[15] != string.Empty &&
                                        bodyArray[15].Length <= 2)
                                    {
                                        record.marketingAirline = bodyArray[15];
                                    }
                                    else
                                    {
                                        record.marketingAirline = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[16] != string.Empty &&
                                        Information.IsNumeric(bodyArray[16]) &&
                                        bodyArray[16].Length <= 4)
                                    {
                                        record.operatingFlightNo = bodyArray[16];
                                    }
                                    else
                                    {
                                        record.operatingFlightNo = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[17] != string.Empty &&
                                        bodyArray[17].Length <= 2)
                                    {
                                        record.operatingAirline = bodyArray[17];
                                    }
                                    else
                                    {
                                        record.operatingAirline = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[18] != string.Empty &&
                                        (bodyArray[18].Length == 13 || bodyArray[18].Length == 14) &&
                                        Information.IsNumeric(bodyArray[18]))
                                    {
                                        record.ticketNo = bodyArray[18];
                                    }
                                    else
                                    {
                                        record.ticketNo = string.Empty;
                                    }

                                    if (bodyArray[19] != string.Empty &&
                                        bodyArray[19].Length <= 25)
                                    {
                                        record.externalPaxID = bodyArray[19];
                                    }
                                    else
                                    {
                                        record.externalPaxID = string.Empty;
                                    }

                                    if (bodyArray[20] != string.Empty &&
                                        bodyArray[20].Length <= 2)
                                    {
                                        record.couponNo = bodyArray[20];
                                    }
                                    else
                                    {
                                        record.couponNo = string.Empty;
                                    }

                                    if (bodyArray[21] != string.Empty &&
                                        bodyArray[21].Length == 6 &&
                                        char.IsLetterOrDigit(bodyArray[21][0]) &&
                                        char.IsLetterOrDigit(bodyArray[21][1]) &&
                                        char.IsLetterOrDigit(bodyArray[21][2]) &&
                                        char.IsLetterOrDigit(bodyArray[21][3]) &&
                                        char.IsLetterOrDigit(bodyArray[21][4]) &&
                                        char.IsLetterOrDigit(bodyArray[21][5]))
                                    {
                                        record.pnrNo = bodyArray[21];
                                    }
                                    else
                                    {
                                        record.pnrNo = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[22] != string.Empty &&
                                        bodyArray[22].Length <= 5)
                                    {
                                        record.distance = Convert.ToInt64(bodyArray[22]);
                                    }
                                    else
                                    {
                                        record.distance = default(long);
                                    }

                                    if (bodyArray[23] != string.Empty &&
                                        bodyArray[23].Length <= 8)
                                    {
                                        record.baseFare = Convert.ToSingle(bodyArray[23]);
                                    }
                                    else
                                    {
                                        record.baseFare = default(double);
                                    }

                                    if (bodyArray[24] != string.Empty &&
                                        bodyArray[24].Length <= 8)
                                    {
                                        record.discountBase = Convert.ToSingle(bodyArray[24]);
                                    }
                                    else
                                    {
                                        record.discountBase = default(double);
                                    }

                                    if (bodyArray[25] != string.Empty &&
                                        bodyArray[25].Length <= 8)
                                    {
                                        record.exciseTax = Convert.ToSingle(bodyArray[25]);
                                    }
                                    else
                                    {
                                        record.exciseTax = default(double);
                                    }

                                    if (bodyArray[26] != string.Empty &&
                                        bodyArray[26].Length <= 1 &&
                                        (char.ToUpper(bodyArray[26][0]) == 'A' || char.ToUpper(bodyArray[26][0]) == 'C' || char.ToUpper(bodyArray[26][0]) == 'I'))
                                    {
                                        record.customerType = bodyArray[26];
                                    }
                                    else
                                    {
                                        record.customerType = string.Empty;
                                    }

                                    if (bodyArray[27] != string.Empty &&
                                        bodyArray[27].Length <= 100)
                                    {
                                        record.promotionCode = bodyArray[27];
                                    }
                                    else
                                    {
                                        record.promotionCode = string.Empty;
                                    }

                                    if (bodyArray[28] != string.Empty &&
                                        bodyArray[28].Length <= 3)
                                    {
                                        record.ticketCurrency = bodyArray[28];
                                    }
                                    else
                                    {
                                        record.ticketCurrency = string.Empty;
                                    }

                                    if (bodyArray[29] != string.Empty &&
                                        bodyArray[29].Length <= 3)
                                    {
                                        record.targetCurrency = bodyArray[29];
                                    }
                                    else
                                    {
                                        record.targetCurrency = string.Empty;
                                    }

                                    if (bodyArray[30] != string.Empty &&
                                        bodyArray[30].Length <= 10)
                                    {
                                        record.exchangeRate = Convert.ToSingle(bodyArray[30]);
                                    }
                                    else
                                    {
                                        record.exchangeRate = default(double);
                                    }

                                    if (bodyArray[31] != string.Empty &&
                                        bodyArray[31].Length <= 10)
                                    {
                                        record.fareBasis = bodyArray[31];
                                    }
                                    else
                                    {
                                        record.fareBasis = string.Empty;
                                    }

                                    if (bodyArray[17] == "EI")
                                    {
                                        if (bodyArray[28] != string.Empty &&
                                            bodyArray[29] != string.Empty &&
                                            bodyArray[30] != string.Empty)
                                        {
                                            if (numberOfRecords == numberOfFooterRecords)
                                            {
                                                if (record.ticketNo != string.Empty || record.externalPaxID != string.Empty)
                                                {
                                                    if (record.ticketNo != string.Empty)
                                                    {
                                                        if (Validation.TicketNoValidation(record) != null)
                                                        {
                                                            failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                            recordsNotAdded++;
                                                            continue;
                                                        }

                                                        Validation.SetEmptyPropertiesToNull(record);
                                                        recordsAdded++;
                                                        entities.Flight_Records.Add(record);
                                                        entities.SaveChanges();
                                                    }
                                                    else if (record.externalPaxID != string.Empty)
                                                    {
                                                        if (Validation.ExternalPaxIDValidation(record) != null)
                                                        {
                                                            failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                            recordsNotAdded++;
                                                            continue;
                                                        }

                                                        Validation.SetEmptyPropertiesToNull(record);
                                                        recordsAdded++;
                                                        entities.Flight_Records.Add(record);
                                                        entities.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                        recordsNotAdded++;
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                    recordsNotAdded++;
                                                    continue;
                                                }
                                            }
                                            else return Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                                        }
                                        else
                                        {
                                            recordsNotAdded++;
                                            failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                        }
                                    }
                                    else if (bodyArray[17] != "EI")
                                    {
                                        if (numberOfRecords == numberOfFooterRecords)
                                        {
                                            if (record.ticketNo != string.Empty || record.externalPaxID != string.Empty)
                                            {
                                                if (record.ticketNo != string.Empty)
                                                {
                                                    if (Validation.TicketNoValidation(record) != null)
                                                    {
                                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                        recordsNotAdded++;
                                                        continue;
                                                    }

                                                    Validation.SetEmptyPropertiesToNull(record);
                                                    recordsAdded++;
                                                    entities.Flight_Records.Add(record);
                                                    entities.SaveChanges();
                                                }
                                                else if (record.externalPaxID != string.Empty)
                                                {
                                                    if (Validation.ExternalPaxIDValidation(record) != null)
                                                    {
                                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                        recordsNotAdded++;
                                                        continue;
                                                    }

                                                    Validation.SetEmptyPropertiesToNull(record);
                                                    recordsAdded++;
                                                    entities.Flight_Records.Add(record);
                                                    entities.SaveChanges();
                                                }
                                                else
                                                {
                                                    recordsNotAdded++;
                                                    failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                recordsNotAdded++;
                                                continue;
                                            }
                                        }
                                        else return Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                                    }
                                    continue;
                                }

                            case 'F':
                                {
                                    break;
                                }
                            default:
                                {
                                    failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                    recordsNotAdded++;

                                    continue;
                                }
                        }
                    }

                    streamReader.DiscardBufferedData();

                    stream.Position = 0;

                    StreamReader streamReader2 = new StreamReader(stream);

                    batch.Header = header;
                    batch.Footer = footer;
                    batch.Content = streamReader2.ReadToEnd();

                    entities.FR_Batch_Files.Add(batch);
                    entities.SaveChanges();

                    FailedRecords = failedToAddRecords;

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                var currentRequest = HttpContext.Current;

                var file = currentRequest.Request.Files[0];

                Stream stream = file.InputStream;

                if (file == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                if (file.ContentLength == 0)
                    return Request.CreateResponse(HttpStatusCode.NoContent);

                string failedToAddRecords = string.Empty;

                int recordsAdded = 0;
                int recordsNotAdded = 0;
                int numberOfFooterRecords = 0;
                int numberOfRecords = 0;

                try
                {
                    string tempRecord = string.Empty;

                    bool convertedFooterRecordsSuccessfully = false;

                    string header = string.Empty;
                    string[] headerArray = null;

                    string body = string.Empty;
                    string[] bodyArray = null;

                    string footer = string.Empty;
                    string[] footerArray = null;

                    char[] separator = new char[] { '|' };

                    Flight_Records record = new Flight_Records();

                    string content = string.Empty;

                    stream.Position = 0;

                    StreamReader streamReader1 = new StreamReader(stream);

                    header = streamReader1.ReadLine();

                    if (entities.FR_Batch_Files.Any(b => b.Header == header))
                        return Request.CreateResponse(HttpStatusCode.Conflict);

                    FR_Batch_Files batch = new FR_Batch_Files();

                    headerArray = header.Split(separator, StringSplitOptions.None);

                    if (headerArray[0].ToUpper() != "H")
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                    while (!streamReader1.EndOfStream)
                    {
                        footer = streamReader1.ReadLine();

                        footerArray = footer.Split(separator, StringSplitOptions.None);

                        numberOfRecords++;
                    }
                    numberOfRecords--;

                    if (footerArray[0] != "F")
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                    convertedFooterRecordsSuccessfully = int.TryParse(footerArray[1], out numberOfFooterRecords);

                    if (!convertedFooterRecordsSuccessfully)
                        return Request.CreateResponse(HttpStatusCode.BadRequest);

                    streamReader1.DiscardBufferedData();

                    stream.Position = 0;

                    System.IO.StreamReader streamReader = new System.IO.StreamReader(stream);

                    header = streamReader.ReadLine(); //H

                    headerArray = header.Split(separator, StringSplitOptions.None);

                    while (!streamReader.EndOfStream)
                    {
                        tempRecord = streamReader.ReadLine();

                        switch (tempRecord[0])
                        {

                            case 'R':
                                {
                                    bodyArray = tempRecord.Split(separator, StringSplitOptions.None);

                                    if (bodyArray[1] != string.Empty &&
                                        bodyArray[1].Length <= 16)
                                    {
                                        record.identifierNo = bodyArray[1];
                                    }
                                    else
                                    {
                                        record.identifierNo = null;
                                    }

                                    if (bodyArray[2] != string.Empty &&
                                        bodyArray[2].Length <= 2)
                                    {
                                        record.transactionType = bodyArray[2];
                                    }
                                    else
                                    {
                                        record.transactionType = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[3] != string.Empty &&
                                        bodyArray[3].Length <= 30)
                                    {
                                        record.otherFFPNo = bodyArray[3];
                                    }
                                    else
                                    {
                                        record.otherFFPNo = string.Empty;
                                    }

                                    if (bodyArray[4] != string.Empty &&
                                        bodyArray[4].Length <= 30)
                                    {
                                        record.otherFFPScheme = bodyArray[4];
                                    }
                                    else
                                    {
                                        record.otherFFPScheme = string.Empty;
                                    }

                                    if (bodyArray[5] != string.Empty &&
                                        bodyArray[5].Length <= 30)
                                    {
                                        record.firstName = bodyArray[5];
                                    }
                                    else
                                    {
                                        record.firstName = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[6] != string.Empty &&
                                        bodyArray[6].Length <= 30)
                                    {
                                        record.lastName = bodyArray[6];
                                    }
                                    else
                                    {
                                        record.lastName = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[7] != string.Empty &&
                                        bodyArray[7].Length <= 100)
                                    {
                                        record.partnerTransactionNo = bodyArray[7];
                                    }
                                    else
                                    {
                                        record.partnerTransactionNo = string.Empty;
                                    }

                                    if (bodyArray[8] != string.Empty)
                                    {
                                        record.bookingDate = Convert.ToDateTime(bodyArray[8]);
                                    }
                                    else
                                    {
                                        record.bookingDate = default(DateTime);
                                    }

                                    if (bodyArray[9] != string.Empty)
                                    {
                                        record.departureDate = Convert.ToDateTime(bodyArray[9]);
                                    }
                                    else
                                    {
                                        record.departureDate = default(DateTime);

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[10] != string.Empty &&
                                        bodyArray[10].Length <= 3)
                                    {
                                        record.origin = bodyArray[10];
                                    }
                                    else
                                    {
                                        record.origin = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[11] != string.Empty &&
                                        bodyArray[11].Length <= 3)
                                    {
                                        record.destination = bodyArray[11];
                                    }
                                    else
                                    {
                                        record.destination = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[12] != string.Empty &&
                                        bodyArray[12].Length <= 2)
                                    {
                                        record.bookingClass = bodyArray[12];
                                    }
                                    else
                                    {
                                        record.bookingClass = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[13] != string.Empty &&
                                        bodyArray[13].Length <= 1)
                                    {
                                        record.cabinClass = bodyArray[13];
                                    }
                                    else
                                    {
                                        record.cabinClass = string.Empty;
                                    }

                                    if (bodyArray[14] != string.Empty &&
                                        Information.IsNumeric(bodyArray[14]) &&
                                        bodyArray[14].Length <= 4)
                                    {
                                        record.marketingFlightNo = bodyArray[14];
                                    }
                                    else
                                    {
                                        record.marketingFlightNo = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[15] != string.Empty &&
                                        bodyArray[15].Length <= 2)
                                    {
                                        record.marketingAirline = bodyArray[15];
                                    }
                                    else
                                    {
                                        record.marketingAirline = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[16] != string.Empty &&
                                        Information.IsNumeric(bodyArray[16]) &&
                                        bodyArray[16].Length <= 4)
                                    {
                                        record.operatingFlightNo = bodyArray[16];
                                    }
                                    else
                                    {
                                        record.operatingFlightNo = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[17] != string.Empty &&
                                        bodyArray[17].Length <= 2)
                                    {
                                        record.operatingAirline = bodyArray[17];
                                    }
                                    else
                                    {
                                        record.operatingAirline = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[18] != string.Empty &&
                                        (bodyArray[18].Length == 13 || bodyArray[18].Length == 14) &&
                                        Information.IsNumeric(bodyArray[18]))
                                    {
                                        record.ticketNo = bodyArray[18];
                                    }
                                    else
                                    {
                                        record.ticketNo = string.Empty;
                                    }

                                    if (bodyArray[19] != string.Empty &&
                                        bodyArray[19].Length <= 25)
                                    {
                                        record.externalPaxID = bodyArray[19];
                                    }
                                    else
                                    {
                                        record.externalPaxID = string.Empty;
                                    }

                                    if (bodyArray[20] != string.Empty &&
                                        bodyArray[20].Length <= 2)
                                    {
                                        record.couponNo = bodyArray[20];
                                    }
                                    else
                                    {
                                        record.couponNo = string.Empty;
                                    }

                                    if (bodyArray[21] != string.Empty &&
                                        bodyArray[21].Length == 6 &&
                                        char.IsLetterOrDigit(bodyArray[21][0]) &&
                                        char.IsLetterOrDigit(bodyArray[21][1]) &&
                                        char.IsLetterOrDigit(bodyArray[21][2]) &&
                                        char.IsLetterOrDigit(bodyArray[21][3]) &&
                                        char.IsLetterOrDigit(bodyArray[21][4]) &&
                                        char.IsLetterOrDigit(bodyArray[21][5]))
                                    {
                                        record.pnrNo = bodyArray[21];
                                    }
                                    else
                                    {
                                        record.pnrNo = string.Empty;

                                        recordsNotAdded++;

                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                        continue;
                                    }

                                    if (bodyArray[22] != string.Empty &&
                                        bodyArray[22].Length <= 5)
                                    {
                                        record.distance = Convert.ToInt64(bodyArray[22]);
                                    }
                                    else
                                    {
                                        record.distance = default(long);
                                    }

                                    if (bodyArray[23] != string.Empty &&
                                        bodyArray[23].Length <= 8)
                                    {
                                        record.baseFare = Convert.ToSingle(bodyArray[23]);
                                    }
                                    else
                                    {
                                        record.baseFare = default(double);
                                    }

                                    if (bodyArray[24] != string.Empty &&
                                        bodyArray[24].Length <= 8)
                                    {
                                        record.discountBase = Convert.ToSingle(bodyArray[24]);
                                    }
                                    else
                                    {
                                        record.discountBase = default(double);
                                    }

                                    if (bodyArray[25] != string.Empty &&
                                        bodyArray[25].Length <= 8)
                                    {
                                        record.exciseTax = Convert.ToSingle(bodyArray[25]);
                                    }
                                    else
                                    {
                                        record.exciseTax = default(double);
                                    }

                                    if (bodyArray[26] != string.Empty &&
                                        bodyArray[26].Length <= 1 &&
                                        (char.ToUpper(bodyArray[26][0]) == 'A' || char.ToUpper(bodyArray[26][0]) == 'C' || char.ToUpper(bodyArray[26][0]) == 'I'))
                                    {
                                        record.customerType = bodyArray[26];
                                    }
                                    else
                                    {
                                        record.customerType = string.Empty;
                                    }

                                    if (bodyArray[27] != string.Empty &&
                                        bodyArray[27].Length <= 100)
                                    {
                                        record.promotionCode = bodyArray[27];
                                    }
                                    else
                                    {
                                        record.promotionCode = string.Empty;
                                    }

                                    if (bodyArray[28] != string.Empty &&
                                        bodyArray[28].Length <= 3)
                                    {
                                        record.ticketCurrency = bodyArray[28];
                                    }
                                    else
                                    {
                                        record.ticketCurrency = string.Empty;
                                    }

                                    if (bodyArray[29] != string.Empty &&
                                        bodyArray[29].Length <= 3)
                                    {
                                        record.targetCurrency = bodyArray[29];
                                    }
                                    else
                                    {
                                        record.targetCurrency = string.Empty;
                                    }

                                    if (bodyArray[30] != string.Empty &&
                                        bodyArray[30].Length <= 10)
                                    {
                                        record.exchangeRate = Convert.ToSingle(bodyArray[30]);
                                    }
                                    else
                                    {
                                        record.exchangeRate = default(double);
                                    }

                                    if (bodyArray[31] != string.Empty &&
                                        bodyArray[31].Length <= 10)
                                    {
                                        record.fareBasis = bodyArray[31];
                                    }
                                    else
                                    {
                                        record.fareBasis = string.Empty;
                                    }

                                    if (bodyArray[17] == "EI")
                                    {
                                        if (bodyArray[28] != string.Empty &&
                                            bodyArray[29] != string.Empty &&
                                            bodyArray[30] != string.Empty)
                                        {
                                            if (numberOfRecords == numberOfFooterRecords)
                                            {
                                                if (record.ticketNo != string.Empty || record.externalPaxID != string.Empty)
                                                {
                                                    if (record.ticketNo != string.Empty)
                                                    {
                                                        if (Validation.TicketNoValidation(record) != null)
                                                        {
                                                            failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                            recordsNotAdded++;
                                                            continue;
                                                        }

                                                        Validation.SetEmptyPropertiesToNull(record);
                                                        recordsAdded++;
                                                        entities.Flight_Records.Add(record);
                                                        entities.SaveChanges();
                                                    }
                                                    else if (record.externalPaxID != string.Empty)
                                                    {
                                                        if (Validation.ExternalPaxIDValidation(record) != null)
                                                        {
                                                            failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                            recordsNotAdded++;
                                                            continue;
                                                        }

                                                        Validation.SetEmptyPropertiesToNull(record);
                                                        recordsAdded++;
                                                        entities.Flight_Records.Add(record);
                                                        entities.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                        recordsNotAdded++;
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                    recordsNotAdded++;
                                                    continue;
                                                }
                                            }
                                            else return Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                                        }
                                        else
                                        {
                                            recordsNotAdded++;
                                            failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                        }
                                    }
                                    else if (bodyArray[17] != "EI")
                                    {
                                        if (numberOfRecords == numberOfFooterRecords)
                                        {
                                            if (record.ticketNo != string.Empty || record.externalPaxID != string.Empty)
                                            {
                                                if (record.ticketNo != string.Empty)
                                                {
                                                    if (Validation.TicketNoValidation(record) != null)
                                                    {
                                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                        recordsNotAdded++;
                                                        continue;
                                                    }

                                                    Validation.SetEmptyPropertiesToNull(record);
                                                    recordsAdded++;
                                                    entities.Flight_Records.Add(record);
                                                    entities.SaveChanges();
                                                }
                                                else if (record.externalPaxID != string.Empty)
                                                {
                                                    if (Validation.ExternalPaxIDValidation(record) != null)
                                                    {
                                                        failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                        recordsNotAdded++;
                                                        continue;
                                                    }

                                                    Validation.SetEmptyPropertiesToNull(record);
                                                    recordsAdded++;
                                                    entities.Flight_Records.Add(record);
                                                    entities.SaveChanges();
                                                }
                                                else
                                                {
                                                    recordsNotAdded++;
                                                    failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                failedToAddRecords = failedToAddRecords + tempRecord + "\n";
                                                recordsNotAdded++;
                                                continue;
                                            }
                                        }
                                        else return Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                                    }
                                    continue;
                                }

                            case 'F':
                                {
                                    break;
                                }
                            default:
                                {
                                    failedToAddRecords = failedToAddRecords + tempRecord + "\n";

                                    recordsNotAdded++;

                                    continue;
                                }
                        }
                    }

                    streamReader.DiscardBufferedData();

                    stream.Position = 0;

                    StreamReader streamReader2 = new StreamReader(stream);

                    batch.Header = header;
                    batch.Footer = footer;
                    batch.Content = streamReader2.ReadToEnd();

                    entities.FR_Batch_Files.Add(batch);
                    entities.SaveChanges();

                    FailedRecords = failedToAddRecords;

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpPost]
        [Route("api/FlightRecordsApi/AddFlightRecord")]
        // POST: api/FlightRecordsApi/AddFlightRecord
        public async Task<HttpResponseMessage> AddFlightRecord([FromBody] Flight_Records sfr)
        {
            if(!string.IsNullOrEmpty(sfr.ticketNo) || !string.IsNullOrEmpty(sfr.externalPaxID))
            {
                if (!string.IsNullOrEmpty(sfr.ticketNo))
                {
                    if (Validation.TicketNoValidation(sfr) == null)
                    {
                        entities.Flight_Records.Add(sfr);
                        await entities.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                else if (!string.IsNullOrEmpty(sfr.externalPaxID))
                {
                    if (Validation.ExternalPaxIDValidation(sfr) == null)
                    {
                        entities.Flight_Records.Add(sfr);
                        await entities.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                else return Request.CreateResponse(HttpStatusCode.Conflict);
            }
            else return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpPut]
        public HttpResponseMessage EditFlightRecord(int id, Flight_Records record)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                var recordInDatabase = entities.Flight_Records.SingleOrDefault(f => f.ID == id);

                if (recordInDatabase == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                recordInDatabase.firstName = record.firstName;
                recordInDatabase.lastName = record.lastName;
                recordInDatabase.identifierNo = record.identifierNo;
                recordInDatabase.transactionType = record.transactionType;
                recordInDatabase.otherFFPNo = record.otherFFPNo;
                recordInDatabase.otherFFPScheme = record.otherFFPScheme;
                recordInDatabase.partnerTransactionNo = record.partnerTransactionNo;
                recordInDatabase.bookingDate = record.bookingDate;
                recordInDatabase.departureDate = record.departureDate;
                recordInDatabase.origin = record.origin;
                recordInDatabase.destination = record.destination;
                recordInDatabase.bookingClass = record.bookingClass;
                recordInDatabase.cabinClass = record.cabinClass;
                recordInDatabase.marketingFlightNo = record.marketingFlightNo;
                recordInDatabase.marketingAirline = record.marketingAirline;
                recordInDatabase.operatingFlightNo = record.operatingFlightNo;
                recordInDatabase.operatingAirline = record.operatingAirline;
                recordInDatabase.externalPaxID = record.externalPaxID;
                recordInDatabase.ticketNo = record.ticketNo;
                recordInDatabase.couponNo = record.couponNo;
                recordInDatabase.pnrNo = record.pnrNo;
                recordInDatabase.distance = record.distance;
                recordInDatabase.baseFare = record.baseFare;
                recordInDatabase.discountBase = record.discountBase;
                recordInDatabase.exciseTax = record.exciseTax;
                recordInDatabase.customerType = record.customerType;
                recordInDatabase.promotionCode = record.promotionCode;
                recordInDatabase.ticketCurrency = record.ticketCurrency;
                recordInDatabase.targetCurrency = record.targetCurrency;
                recordInDatabase.exchangeRate = record.exchangeRate;
                recordInDatabase.fareBasis = record.fareBasis;

                //entities.SaveChanges();

                //var journeyInDatabase = entities.Journeys.SingleOrDefault(j => j.TicketNo == record.ticketNo);

                //if(journeyInDatabase != null)
                //{
                //    journeyInDatabase.IdentifierNo = record.identifierNo;
                //    journeyInDatabase.FirstName = record.firstName;
                //    journeyInDatabase.LastName = record.lastName;
                //    journeyInDatabase.TicketNo = record.ticketNo;

                //    //entities.SaveChanges();

                //    var journeySegmentInDatabase = entities.JourneySegments.SingleOrDefault(js => js.IDFR == record.ID && js.couponNo == record.couponNo); //ako ne radi skloniti drugi deo

                //    if(journeySegmentInDatabase != null)
                //    {
                //        journeySegmentInDatabase.couponNo = record.couponNo;
                //        journeySegmentInDatabase.departureDate = record.departureDate;
                //        journeySegmentInDatabase.destination = record.destination;
                //        journeySegmentInDatabase.origin = record.origin;
                //        journeySegmentInDatabase.IDFR = record.ID;
                //        journeySegmentInDatabase.IDJourney = journeyInDatabase.ID;

                //        //entities.SaveChanges();
                //    }
                //}

                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                entities.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeleteFlightRecord(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                var recordInDatabase = entities.Flight_Records.SingleOrDefault(f => f.ID == id);

                if (recordInDatabase == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                entities.Flight_Records.Remove(recordInDatabase);
                entities.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }


        
    }
}
