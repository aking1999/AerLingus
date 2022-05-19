using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AerLingus.Models;

namespace AerLingus.Controllers.Api
{
    public class JourneySegmentsApiController : ApiController
    {
        private AerLingus_databaseEntities entities;

        public JourneySegmentsApiController()
        {
            entities = new AerLingus_databaseEntities();
        }

        [HttpGet]
        public IEnumerable<JourneySegment> GetJourneySegments(string ticketNo) //ovo ID je ticketNo
        {
            if (!entities.JourneySegments.Any())
                return default(IEnumerable<JourneySegment>);

            return entities.JourneySegments.Where(js => js.TicketNo == ticketNo).AsEnumerable();
        }

        [HttpPost]
        [Route("api/JourneySegmentsApi/AddJourneySegment")]
        public async Task<HttpResponseMessage> AddJourneySegmentAsync([FromBody] JourneySegment j)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (entities.JourneySegments.Any(b => b.TicketNo == j.TicketNo && b.couponNo ==j.couponNo))
                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    else
                    {
                        entities.JourneySegments.Add(j);
                        await entities.SaveChangesAsync();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }

                }
                else return Request.CreateResponse(HttpStatusCode.Conflict);
            }



            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        public HttpResponseMessage EditJourneySegment(int id, JourneySegment journeySegment)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                var journeySegmentInDatabase = entities.JourneySegments.SingleOrDefault(j => j.ID == id);

                if (journeySegmentInDatabase == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

              //  journeyInDatabase.TicketNo = journeySegment.TicketNo;
                journeySegmentInDatabase.couponNo = journeySegment.couponNo;
                journeySegmentInDatabase.destination = journeySegment.destination;
                journeySegmentInDatabase.origin = journeySegment.origin;
                journeySegmentInDatabase.departureDate = journeySegment.departureDate;


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
        public HttpResponseMessage DeleteJourneySegment(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                var journeySegmentInDatabase = entities.JourneySegments.SingleOrDefault(j => j.ID == id);

                if (journeySegmentInDatabase == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                entities.JourneySegments.Remove(journeySegmentInDatabase);
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
