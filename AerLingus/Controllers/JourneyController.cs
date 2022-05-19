using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using AerLingus.Helpers;
using AerLingus.Models;
using AerLingus.View_Models;
using AerLingus.Controllers.Api;
using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AerLingus.Controllers
{
    public class JourneyController : Controller
    {
        private HttpClient client;
        private HttpClient client1;
        private List<Journey> journeys;
        private List<Journey> listaSearch;
        private AerLingus_databaseEntities entities;
        private Dictionary<int, string> errors;

        public JourneyController()
        {
            client = new HttpClient();
            client1 = new HttpClient();
            journeys = new List<Journey>();
            listaSearch = new List<Journey>();         
            entities = new AerLingus_databaseEntities();
            errors = new Dictionary<int, string>();
            fillErrors(errors);
        }
        
        public ActionResult SearchJourney()
        {
            try
            {
                ViewBag.A = false;
                return View(new SearchJourneyViewModel
                {
                    Journeys = new List<Journey>(),
                    SearchJourney = new SearchJourney()
                });
            }
            catch(Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public ActionResult JourneyForm()
        {
            return View();    
        }

        public ActionResult AddJourney(Journey j)
        {
            client.BaseAddress = new Uri(@"http://localhost:54789/api/JourneyApi/AddJourney");
            var insertRecord = client.PostAsJsonAsync<Journey>("", j);
            insertRecord.Wait();
            
            var recorddisplay = insertRecord.Result;
            
            if (recorddisplay.IsSuccessStatusCode)
            {
                //redirekcija u listu svih flight rekorda
                return View("UploadSuccessful");
            }
            else if(recorddisplay.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return View("JourneyForm", j);
            }
            else
            {
                return View("Error", (object)errors[409]);
            }
        }

        [System.Web.Http.HttpGet]
        public ActionResult GetSearchedJourneys(SearchJourneyViewModel searchV)
        {
            var search = searchV.SearchJourney;

            try
            {
                JourneyApiController api = new JourneyApiController()
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };

                if (ModelState.IsValid)
                {
                    SearchJourneyViewModel viewModel = new SearchJourneyViewModel
                    {
                        Journeys = api.GetSearchedJourneys(search)
                    };

                    foreach (Journey searchJourney in viewModel.Journeys)
                    {
                        listaSearch.Add(searchJourney);
                    }

                    ViewBag.A = true;

                    ModelState.Clear();

                    return View("_SearchJourneyList", viewModel);
                }
                else
                {
                    ViewBag.A = true;

                    SearchJourneyViewModel viewModel = new SearchJourneyViewModel
                    {
                        Journeys = new List<Journey>()
                    };

                    return View("_SearchJourneyList", viewModel);
                }
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
                var searchedJourney = entities.Journeys.SingleOrDefault(j => j.ID == id);

                if (searchedJourney == null)
                    return View("Error", (object)(errors[404]));

                return View(searchedJourney);
            }
            catch(Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        [System.Web.Http.HttpPut]
        public async Task<ActionResult> EditJourney(Journey journey)
        {
            try
            {
                var response = await client.PutAsJsonAsync(@"http://localhost:54789/api/JourneyApi/" + journey.ID.ToString(), journey);

                object errorMessage = null;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return View("UploadSuccessful");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    return View("Edit", journey);
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    errorMessage = errors[404];
                else errorMessage = errors[500];

                return View("Error", errorMessage);
            }
            catch(Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var response = await client.GetAsync(@"http://localhost:54789/api/JourneyApi/" + id.ToString());

                if (response.IsSuccessStatusCode)
                    return View(await response.Content.ReadAsAsync<Journey>());

                else return View("Error", (object)(errors[404]));
            }
            catch(Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var searchedJourney = entities.Journeys.SingleOrDefault(j => j.ID == id);

                if (searchedJourney == null)
                    return View("Error", (object)(errors[404]));

                return View(searchedJourney);
            }
            catch(Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        [System.Web.Http.HttpDelete]
        public async Task<ActionResult> DeleteJourney(int id, string komentar)
        {
            //return Content("Ovo: " + komentar);
            try
            {
                //var response = await client.DeleteAsync(@"http://localhost:54789/api/JourneyApi/" + id.ToString());

                JourneyApiController api = new JourneyApiController()
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };

                var response = api.DeleteJourneyWithComment(id, komentar);

                object errorMessage = null;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Content(api.poruka);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    errorMessage = errors[400];
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    errorMessage = errors[404];
                else errorMessage = api.poruka;/*errors[500];*/

                return View("Error", errorMessage);
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public async Task<ActionResult> JourneySegmentDetails(int id)
        {
            try
            {
                var responseJourney = await client.GetAsync(@"http://localhost:54789/api/JourneyApi/" + id.ToString());

                if (responseJourney.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return View("Error", (object)(errors[404]));

                var journey = await responseJourney.Content.ReadAsAsync<JourneySegment>();

                JourneySegmentsApiController api = new JourneySegmentsApiController
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };

                var journeySegments = api.GetJourneySegments(journey.TicketNo);

                return View(journeySegments);
            }
            catch(Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
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