using AerLingus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AerLingus.Controllers
{
    public class JourneySegmentsController : Controller
    {
        private HttpClient client;
        private HttpClient client1;
        private List<JourneySegment> journeySegments;
        private List<JourneySegment> listaSearch;
        private AerLingus_databaseEntities entities;
        private Dictionary<int, string> errors;

        public JourneySegmentsController()
        {
            client = new HttpClient();
            client1 = new HttpClient();
            journeySegments = new List<JourneySegment>();
            listaSearch = new List<JourneySegment>();
            entities = new AerLingus_databaseEntities();
            errors = new Dictionary<int, string>();
            fillErrors(errors);
        }

        // GET: JourneySegments
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddJourneySegment (JourneySegment j)
        {
            client.BaseAddress = new Uri(@"http://localhost:54789/api/JourneySegmentsApi/AddJourneySegment");
            var insertRecord = client.PostAsJsonAsync<JourneySegment>("", j);
            insertRecord.Wait();
            var recorddisplay = insertRecord.Result;
            if (recorddisplay.IsSuccessStatusCode)
            {
                //redirekcija u listu svih flight rekorda
                return View("UploadSuccessful");
            }
            else
            {
                return View("JourneySegmentForm");
            }
        }


        public ActionResult Delete(int id)
        {
            try
            {
                var searchedJourney = entities.JourneySegments.SingleOrDefault(j => j.ID == id);

                if (searchedJourney == null)
                    return View("Error", (object)(errors[404]));

                return View("Delete",searchedJourney);
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var searchedJourney = entities.JourneySegments.SingleOrDefault(j => j.ID == id);

                if (searchedJourney == null)
                    return View("Error", (object)(errors[404]));

                return View("Edit",searchedJourney);
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        [System.Web.Http.HttpPut]
        public async Task<ActionResult> EditJourneySegment(JourneySegment journeySegment)
        {
            try
            {
                var response = await client.PutAsJsonAsync(@"http://localhost:54789/api/JourneySegmentsApi/" + journeySegment.ID.ToString(), journeySegment);

                object errorMessage = null;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return View("UploadSuccessful");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    errorMessage = errors[400];
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    errorMessage = errors[404];
                else errorMessage = errors[500];

                return View("Error", errorMessage);
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        [System.Web.Http.HttpDelete]
        public async Task<ActionResult> DeleteJourneySegment(int id)
        {
            try
            {
                var response = await client.DeleteAsync(@"http://localhost:54789/api/JourneySegmentsApi/" + id.ToString());

                object errorMessage = null;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return View("UploadSuccessful");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    errorMessage = errors[400];
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    errorMessage = errors[404];
                else errorMessage = errors[500];

                return View("Error", errorMessage);
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public ActionResult JourneySegmentForm()
        {
            return View("JourneySegmentForm");
        }

        private void fillErrors(Dictionary<int, string> errors)
        {
            errors.Add(204, "ERROR 204: No content");
            errors.Add(400, "ERROR 400: Bad request");
            errors.Add(404, "ERROR 404: Not found");
            errors.Add(406, "ERROR 406: Not acceptable");
            errors.Add(409, "ERROR 409: Conflict");
            errors.Add(412, "ERROR 412: Precondition failed");
            errors.Add(500, "ERROR 500: Internal server error");
        }
    }
}