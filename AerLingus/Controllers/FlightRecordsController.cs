using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AerLingus.Models;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using AerLingus.Controllers.Api;
using System.Web.Http;
using System.Web.Hosting;
using AerLingus.Helpers;
using System.Web.UI.WebControls;
using System.Web.UI;
using AerLingus.View_Models;

namespace AerLingus.Controllers

{
    public class FlightRecordsController : Controller
    {
        private HttpClient client;
        private List<Flight_Records> listaSearch;
        private List<Flight_Records> flight_Records;
        private AerLingus_databaseEntities entities;
        private Dictionary<int, string> errors;

        public FlightRecordsController()
        {
            client = new HttpClient();
            entities = new AerLingus_databaseEntities();
            listaSearch = new List<Flight_Records>();
            flight_Records = new List<Flight_Records>();
            errors = new Dictionary<int, string>();
            fillErrors(errors);
        }

        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                ApiViewBag.UploadRequest.RequestIsComingFromController = true;
                ApiViewBag.UploadRequest.RequestedFile = file;

                FlightRecordsApiController api = new FlightRecordsApiController()
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };

                var returnedStatusCode = api.Upload().StatusCode;

                if (returnedStatusCode == System.Net.HttpStatusCode.OK)
                    return View("UploadSuccessful");
                else
                {
                    object errorMessage = null;

                    if (returnedStatusCode == System.Net.HttpStatusCode.NotFound)
                        errorMessage = errors[404];
                    else if (returnedStatusCode == System.Net.HttpStatusCode.NoContent)
                        errorMessage = errors[204];
                    else if (returnedStatusCode == System.Net.HttpStatusCode.NotAcceptable)
                        errorMessage = errors[406];
                    else if (returnedStatusCode == System.Net.HttpStatusCode.BadRequest)
                        errorMessage = errors[400];
                    else if (returnedStatusCode == System.Net.HttpStatusCode.PreconditionFailed)
                        errorMessage = errors[412];
                    else if (returnedStatusCode == System.Net.HttpStatusCode.Conflict)
                        errorMessage = errors[409];
                    else errorMessage = errors[500];
                    return View("Error", errorMessage);
                }
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public ActionResult FlightRecordForm(Flight_Records sfr)
        {
            try
            {
                if (!String.IsNullOrEmpty(sfr.firstName) && !String.IsNullOrEmpty(sfr.lastName))
                {
                    //Ako je sfr formular popunjen salju se podaci u API kako bi se sacuvali u bazu
                    HttpClient hc = new HttpClient();
                    hc.BaseAddress = new Uri(@"http://localhost:54789/api/FlightRecordsApi/AddFlightRecord");
                    var insertRecord = hc.PostAsJsonAsync("", sfr);
                    //insertRecord.Wait();
                    var recorddisplay = insertRecord.Result;
                    if (recorddisplay.IsSuccessStatusCode)
                    {
                        //redirekcija u listu svih flight rekorda
                        return View("UploadSuccessful");
                    }
                }

                //Prikazuje formular za dodavanje single flight rekorda ako zeli da doda novi sfr ili ako prethodno popunjavanje nije proslo kako treba
                List<SelectListItem> listItems = new List<SelectListItem>();
                listItems.Add(new SelectListItem
                {
                    Text = "AI",
                    Value = "AI"
                });
                listItems.Add(new SelectListItem
                {
                    Text = "AB",
                    Value = "AB",
                });

                List<SelectListItem> listItems2 = new List<SelectListItem>();
                listItems2.Add(new SelectListItem
                {
                    Text = "F",
                    Value = "F"
                });
                listItems2.Add(new SelectListItem
                {
                    Text = "J",
                    Value = "J",
                });
                listItems2.Add(new SelectListItem
                {
                    Text = "W",
                    Value = "W",
                });
                listItems2.Add(new SelectListItem
                {
                    Text = "Y",
                    Value = "Y",
                });

                List<SelectListItem> listItems3 = new List<SelectListItem>();
                listItems3.Add(new SelectListItem
                {
                    Text = "A",
                    Value = "A"
                });
                listItems3.Add(new SelectListItem
                {
                    Text = "C",
                    Value = "C",

                });
                listItems3.Add(new SelectListItem
                {
                    Text = "I",
                    Value = "I",
                });

                StreamReader sr = new StreamReader(HostingEnvironment.ApplicationPhysicalPath + "/Content/currencies.txt");

                List<string> listItems4 = new List<string>();

                while (!sr.EndOfStream)
                {
                    listItems4.Add(sr.ReadLine());
                }

                ViewBag.list1 = listItems;
                ViewBag.list2 = listItems2;
                ViewBag.list3 = listItems3;
                ViewBag.list4 = listItems4;

                return View();
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public ActionResult SearchFlightRecords()
        {
            try
            {
                ViewBag.A = false;
                return View(new SearchViewModel
                {
                    //FlightRecords = flight_Records,
                    FlightRecords = new List<Flight_Records>(),
                    Search = new SearchFlightRecord()
                });
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        [System.Web.Http.HttpGet]
        public ActionResult GetSearchedFlightRecords(SearchViewModel searchV)
        {
            var search = searchV.Search;

            try
            {
                FlightRecordsApiController api = new FlightRecordsApiController()
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };

                if (ModelState.IsValid)
                {
                    SearchViewModel viewModel = new SearchViewModel
                    {
                        FlightRecords = api.GetSearchedFlightRecords(search)
                    };

                    foreach (Flight_Records searchRecord in viewModel.FlightRecords)
                    {
                        listaSearch.Add(searchRecord);
                    }

                    ViewBag.A = true;

                    ModelState.Clear();

                    return View("_PartialViewList", viewModel);
                }
                else
                {
                    ViewBag.A = true;

                    SearchViewModel viewModel = new SearchViewModel
                    {
                        FlightRecords = new List<Flight_Records>()
                    };

                    //listaSearch = api.GetSearchedFlightRecords(search);

                    //ModelState.Clear();
                    return View("_PartialViewList", viewModel);
                }
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public void ExportToCSV(SearchFlightRecord search)
        {
            try
            {
                StringWriter sw = new StringWriter();
                Response.ClearContent();
                Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", "attachment;filename=FlightRecords" + DateTime.Now.ToShortDateString() + ".csv");


                FlightRecordsApiController api = new FlightRecordsApiController()
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };

                SearchViewModel viewModel = new SearchViewModel
                {
                    FlightRecords = api.GetSearchedFlightRecords(search)
                };


                foreach (Flight_Records searchRecord in viewModel.FlightRecords)
                {
                    listaSearch.Add(searchRecord);
                }


                foreach (var client in listaSearch)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\",\"{23}\",\"{24}\",\"{25}\",\"{26}\",\"{27}\",\"{28}\",\"{29}\"",
                                      client.identifierNo,
                                      client.transactionType,
                                      client.otherFFPNo,
                                      client.otherFFPScheme,
                                      client.firstName,
                                       client.lastName,
                                       client.partnerTransactionNo,
                                       client.bookingDate,
                                       client.departureDate,
                                       client.origin,
                                       client.destination,
                                       client.bookingClass,
                                       client.cabinClass,
                                       client.marketingFlightNo,
                                       client.marketingAirline,
                                       client.operatingFlightNo,
                                       client.operatingAirline,
                                       client.externalPaxID,
                                       client.couponNo,
                                       client.pnrNo,
                                       client.distance,
                                       client.baseFare,
                                       client.discountBase,
                                       client.exciseTax,
                                       client.customerType,
                                       client.promotionCode,
                                       client.ticketCurrency,
                                       client.targetCurrency,
                                       client.exchangeRate,
                                       client.fareBasis));
                }
                Response.Write(sw.ToString());
                Response.End();
            }
            catch(Exception ex)
            {
                View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public void ExportToExcel(SearchFlightRecord search)
        {
            try
            {
                var grid = new GridView();

                FlightRecordsApiController api = new FlightRecordsApiController()
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };

                SearchViewModel viewModel = new SearchViewModel
                {
                    FlightRecords = api.GetSearchedFlightRecords(search)
                };


                foreach (Flight_Records searchRecord in viewModel.FlightRecords)
                {
                    listaSearch.Add(searchRecord);
                }

                grid.DataSource = from client in listaSearch
                                  select new
                                  {
                                      identifierNo = client.identifierNo,
                                      transactionType = client.transactionType,
                                      otherFFPNo = client.otherFFPNo,
                                      otherFFPScheme = client.otherFFPScheme,
                                      firstName = client.firstName,
                                      lastName = client.lastName,
                                      partnerTransactionNo = client.partnerTransactionNo,
                                      bookingDate = client.bookingDate,
                                      departureDate = client.departureDate,
                                      origin = client.origin,
                                      destination = client.destination,
                                      bookingClass = client.bookingClass,
                                      cabinClass = client.cabinClass,
                                      marketingFlightNo = client.marketingFlightNo,
                                      marketingAirline = client.marketingAirline,
                                      operatingFlightNo = client.operatingFlightNo,
                                      operatingAirline = client.operatingAirline,
                                      ticketNo = client.ticketNo,
                                      externalPaxID = client.externalPaxID,
                                      couponNo = client.couponNo,
                                      pnrNo = client.pnrNo,
                                      distance = client.distance,
                                      baseFare = client.baseFare,
                                      discountBase = client.discountBase,
                                      exciseTax = client.exciseTax,
                                      customerType = client.customerType,
                                      promotionCode = client.promotionCode,
                                      ticketCurrency = client.ticketCurrency,
                                      targetCurrency = client.targetCurrency,
                                      exchangeRate = client.exchangeRate,
                                      fareBasis = client.fareBasis,

                                  };
                grid.DataBind();


                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=FlightRecords" + DateTime.Now.ToShortDateString() + ".xlsx");
                Response.ContentType = "application/excel";
                StringWriter sw = new StringWriter();

                HtmlTextWriter htmlTextWriter = new HtmlTextWriter(sw);

                grid.RenderControl(htmlTextWriter);
                Response.Write(sw.ToString());

                Response.End();
            }
            catch(Exception ex)
            {
                View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var response = await client.GetAsync(@"http://localhost:54789/api/FlightRecordsApi/" + id.ToString());

                if (response.IsSuccessStatusCode)
                    return View(await response.Content.ReadAsAsync<Flight_Records>());

                else return View("Error", (object)(errors[404]));
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
                var searchedFlightRecord = entities.Flight_Records.SingleOrDefault(f => f.ID == id);

                if (searchedFlightRecord == null)
                    return View("Error", (object)(errors[404]));

                List<SelectListItem> listItems = new List<SelectListItem>();
                listItems.Add(new SelectListItem
                {
                    Text = "AI",
                    Value = "AI"
                });
                listItems.Add(new SelectListItem
                {
                    Text = "AB",
                    Value = "AB",
                });

                List<SelectListItem> listItems2 = new List<SelectListItem>();
                listItems2.Add(new SelectListItem
                {
                    Text = "F",
                    Value = "F"
                });
                listItems2.Add(new SelectListItem
                {
                    Text = "J",
                    Value = "J",
                });
                listItems2.Add(new SelectListItem
                {
                    Text = "W",
                    Value = "W",
                });
                listItems2.Add(new SelectListItem
                {
                    Text = "Y",
                    Value = "Y",
                });

                List<SelectListItem> listItems3 = new List<SelectListItem>();
                listItems3.Add(new SelectListItem
                {
                    Text = "A",
                    Value = "A"
                });
                listItems3.Add(new SelectListItem
                {
                    Text = "C",
                    Value = "C",

                });
                listItems3.Add(new SelectListItem
                {
                    Text = "I",
                    Value = "I",
                });

                StreamReader sr = new StreamReader(HostingEnvironment.ApplicationPhysicalPath + "/Content/currencies.txt");

                List<string> listItems4 = new List<string>();

                while (!sr.EndOfStream)
                {
                    listItems4.Add(sr.ReadLine());
                }

                ViewBag.list1 = listItems;
                ViewBag.list2 = listItems2;
                ViewBag.list3 = listItems3;
                ViewBag.list4 = listItems4;

                return View(searchedFlightRecord);
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        [System.Web.Http.HttpPut]
        public async Task<ActionResult> EditFlightRecord(Flight_Records record)
        {
            try
            {
                var response = await client.PutAsJsonAsync(@"http://localhost:54789/api/FlightRecordsApi/" + record.ID.ToString(), record);

                object errorMessage = null;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return View("UploadSuccessful");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.Add(new SelectListItem
                    {
                        Text = "AI",
                        Value = "AI"
                    });
                    listItems.Add(new SelectListItem
                    {
                        Text = "AB",
                        Value = "AB",
                    });

                    List<SelectListItem> listItems2 = new List<SelectListItem>();
                    listItems2.Add(new SelectListItem
                    {
                        Text = "F",
                        Value = "F"
                    });
                    listItems2.Add(new SelectListItem
                    {
                        Text = "J",
                        Value = "J",
                    });
                    listItems2.Add(new SelectListItem
                    {
                        Text = "W",
                        Value = "W",
                    });
                    listItems2.Add(new SelectListItem
                    {
                        Text = "Y",
                        Value = "Y",
                    });

                    List<SelectListItem> listItems3 = new List<SelectListItem>();
                    listItems3.Add(new SelectListItem
                    {
                        Text = "A",
                        Value = "A"
                    });
                    listItems3.Add(new SelectListItem
                    {
                        Text = "C",
                        Value = "C",

                    });
                    listItems3.Add(new SelectListItem
                    {
                        Text = "I",
                        Value = "I",
                    });

                    StreamReader sr = new StreamReader(HostingEnvironment.ApplicationPhysicalPath + "/Content/currencies.txt");

                    List<string> listItems4 = new List<string>();

                    while (!sr.EndOfStream)
                    {
                        listItems4.Add(sr.ReadLine());
                    }

                    ViewBag.list1 = listItems;
                    ViewBag.list2 = listItems2;
                    ViewBag.list3 = listItems3;
                    ViewBag.list4 = listItems4;

                    return View("Edit", record);
                }
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

        public ActionResult Delete(int id)
        {
            try
            {
                var searchedFlightRecord = entities.Flight_Records.SingleOrDefault(f => f.ID == id);

                if (searchedFlightRecord == null)
                    return View("Error", (object)(errors[404]));

                return View(searchedFlightRecord);
            }
            catch (Exception ex)
            {
                return View("Error", (object)("ERROR 500: " + ex.Message));
            }
        }

        [System.Web.Http.HttpDelete]
        public async Task<ActionResult> DeleteFlightRecord(int id)
        {
            try
            {
                var response = await client.DeleteAsync(@"http://localhost:54789/api/FlightRecordsApi/" + id.ToString());

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
