using Avionko.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Avionko.Controllers
{

    #region JSON to C# (I generated this using JSON to C# web app )
    public class Origin
    {
        public string airport { get; set; }
        public string terminal { get; set; }
    }

    public class Destination
    {
        public string airport { get; set; }
        public string terminal { get; set; }
    }

    public class BookingInfo
    {
        public string travel_class { get; set; }
        public string booking_code { get; set; }
        public int seats_remaining { get; set; }
    }

    public class Flight
    {
        public string departs_at { get; set; }
        public string arrives_at { get; set; }
        public Origin origin { get; set; }
        public Destination destination { get; set; }
        public string marketing_airline { get; set; }
        public string operating_airline { get; set; }
        public string flight_number { get; set; }
        public string aircraft { get; set; }
        public BookingInfo booking_info { get; set; }
    }

    public class Outbound
    {
        public List<Flight> flights { get; set; }
    }

    public class Origin2
    {
        public string airport { get; set; }
        public string terminal { get; set; }
    }

    public class Destination2
    {
        public string airport { get; set; }
        public string terminal { get; set; }
    }

    public class BookingInfo2
    {
        public string travel_class { get; set; }
        public string booking_code { get; set; }
        public int seats_remaining { get; set; }
    }

    public class Flight2
    {
        public string departs_at { get; set; }
        public string arrives_at { get; set; }
        public Origin2 origin { get; set; }
        public Destination2 destination { get; set; }
        public string marketing_airline { get; set; }
        public string operating_airline { get; set; }
        public string flight_number { get; set; }
        public string aircraft { get; set; }
        public BookingInfo2 booking_info { get; set; }
    }

    public class Inbound
    {
        public List<Flight2> flights { get; set; }
    }

    public class Itinerary
    {
        public Outbound outbound { get; set; }
        public Inbound inbound { get; set; }
    }

    public class PricePerAdult
    {
        public string total_fare { get; set; }
        public string tax { get; set; }
    }

    public class Restrictions
    {
        public bool refundable { get; set; }
        public bool change_penalties { get; set; }
    }

    public class Fare
    {
        public string total_price { get; set; }
        public PricePerAdult price_per_adult { get; set; }
        public Restrictions restrictions { get; set; }
    }

    public class Result
    {
        public List<Itinerary> itineraries { get; set; }
        public Fare fare { get; set; }
    }

    public class RootObject
    {
        public string currency { get; set; }
        public List<Result> results { get; set; }
    }

    #endregion


    public class HomeController : Controller
    {

        /// <summary>
        /// our http client to which we'll refer over and over again. Query, after a query.
        /// </summary>
        //public static HttpClient client = new HttpClient();

        


        private void DisplaySearchResultsDiv()
        {
            Session["ShouldDisplaySearchResults"] = "true";

            List<LowCostFlight> lowCostFlights = (List<LowCostFlight>)Session["fetchedLowCostData"];
            Session["LowCostAirFlights"] = lowCostFlights;
        }

        private void HideSearchResultsDiv()
        {
            Session["ShouldDisplaySearchResults"] = "false";
            Session["LowCostAirFlights"] = null;
        }




        /// <summary>
        /// Index page. 
        /// </summary>
        public ActionResult Index()
        {
            if ((string)Session["ShouldDisplaySearchResults"] == null)
                HideSearchResultsDiv();
            ViewBag.shouldDisplaySearchResultsDiv = (string)Session["ShouldDisplaySearchResults"];

            // if we haven't fetched in low cost air flights so far, return just view, else pack list in it so we can display data in table.
            if ((List<LowCostFlight>)Session["LowCostAirFlights"] == null)
                return View(new List<LowCostFlight>().ToList());
            else
                return View(((List<LowCostFlight>)Session["LowCostAirFlights"]).ToList());


        }



        /// <summary>
        /// Index page with displayed search results div.
        /// </summary>
        public ActionResult IndexWithSearchResult(List<LowCostFlight> lowCostFlightsToDisplayInTable)
        {
            
            DisplaySearchResultsDiv();
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// gets called when "pretraži" button is clicked !
        /// </summary>
        [HttpPost]
        public ActionResult SearchLowCostFlights(string airport1, string airport2, string daterange, int passengerNumber, string currency) 
        {

            string dateDeparture = AdjustDateFormat(daterange.Split('-')[0]);
            string dateReturn = AdjustDateFormat(daterange.Split('-')[1]);
            LowCostFlight.FlightCurrency desiredCurrency = (LowCostFlight.FlightCurrency)Enum.Parse(typeof(LowCostFlight.FlightCurrency), currency, true);


            //waiting for data that is being fetched from API :
            List<Result> rawLowCostFlightsResults = FetchLowCostFlights(airport1, airport2, dateDeparture, dateReturn, passengerNumber, desiredCurrency);

            //pulling cost per person and transitional flights data from session ("JSONdeserializedObjectList") 
            //and storing them in lowCostFlights as LowCostFlight data
            List<LowCostFlight> lowCostFlights = ConvertDeserializedJSONToLowCostFlights(rawLowCostFlightsResults, airport1, airport2, dateDeparture, dateReturn, passengerNumber, desiredCurrency);
            Session["fetchedLowCostData"] = lowCostFlights;

            return RedirectToAction("IndexWithSearchResult", "Home", lowCostFlights);
        }


        /// <summary>
        /// returns list of low cost flights that is created by iterating through every instance stored
        /// in JSONdeserializedObjectList session (type RootObject which contains list of results and used currency).
        /// </summary>
        private List<LowCostFlight> ConvertDeserializedJSONToLowCostFlights(List<Result> rawLowCostFlightsResults,  string airport1, string airport2, string dateDeparture, string dateReturn, int passengerNumber, LowCostFlight.FlightCurrency currency)
        {
            List<LowCostFlight> lowCostFlights = new List<LowCostFlight>();


            for (int i = 0; i < rawLowCostFlightsResults.Count; i++)
            {
                float pricePerAdult = float.Parse(rawLowCostFlightsResults[i].fare.price_per_adult.total_fare);
                int transferFlightsNumberDeparture = rawLowCostFlightsResults[i].itineraries[0].outbound.flights.Count; //Outbound.flights.count = number of flights needed to get there . inbound.fligts.count = nuber of flights needed to come back

                LowCostFlight instanceFromDeserializedJSON = new LowCostFlight(airport1, airport2, AdjustDateFormatFinalDisplay(dateDeparture), AdjustDateFormatFinalDisplay(dateReturn), passengerNumber, transferFlightsNumberDeparture, currency, pricePerAdult);
                lowCostFlights.Add(instanceFromDeserializedJSON);
            }

            return lowCostFlights;
        }




        /// <summary>
        /// based on the passed needed parameters,
        /// fetches all low cost flights (their prices and stuff) from Amadeus API using GET method, adds them in list and returns them.
        /// </summary>
        [HttpGet]
        private List<Result> FetchLowCostFlights(string airport1, string airport2, string dateDeparture, string dateReturn, int passengerNumber, LowCostFlight.FlightCurrency currency)
        {

            string amadeusAPIurl = "https://api.sandbox.amadeus.com/v1.2/flights/low-fare-search";
            string apiKey = "c0vp8TikeMSu8U9iGrzU2nkeAgTuxSr4";
            string origin = airport1;
            string destination = airport2;
            string departure_date = dateDeparture;
            string return_date = dateReturn;
            string adults = passengerNumber.ToString(); //in this case, for simplicity, number of adults == passenger number.

            string requestURI = amadeusAPIurl + "?apikey=" + apiKey + "&origin=" + origin + "&destination=" + destination + "&departure_date=" + departure_date + "&return_date=" + return_date + "&adults=" + adults + "&nonstop=false" + "&currency=" + currency.ToString() + "&number_of_results=10 HTTP/1.1";


            using (HttpClient httpClient = new HttpClient())
            {
                Task<string> response = httpClient.GetStringAsync(requestURI);
                try
                {
                    return JsonConvert.DeserializeObject<RootObject>(response.Result).results;
                }
                //if user entered some weird data (like "sudhsusd" for airport name) ,then just redirect to original index
                catch
                {
                    HideSearchResultsDiv();
                    RedirectToAction("Index", "Home");
                    return new List<Result>();
                }
            }


        }



        /// <summary>
        /// adjusts passed date (which has to be in format : 6/24/2017)
        /// so it matches the format : 2017-06-28 
        /// </summary>
        private string AdjustDateFormat(string dateToAdjust)
        {
            
            string day = Regex.Replace(dateToAdjust.Split('/')[1], @"\s+", "") ;
            string month = Regex.Replace(dateToAdjust.Split('/')[0], @"\s+", "");
            string year = Regex.Replace(dateToAdjust.Split('/')[2], @"\s+", "");

            string adjustedDate = year + "-" + month + "-" + day;

            return adjustedDate;
        }


        /// <summary>
        /// writes month names in croatian. Use this for final date display (just
        /// before calling a view ). input format must be -> 2017-06-25
        /// </summary>
        private string AdjustDateFormatFinalDisplay(string dateToAdjust)
        {
            Dictionary<string, string> croatianMonthsNames = new Dictionary<string, string>();
            croatianMonthsNames.Add("01","siječnja");
            croatianMonthsNames.Add("02", "veljače");
            croatianMonthsNames.Add("03", "ožujka");
            croatianMonthsNames.Add("04", "travnja");
            croatianMonthsNames.Add("05", "svibnja");
            croatianMonthsNames.Add("06", "lipnja");
            croatianMonthsNames.Add("07", "srpnja");
            croatianMonthsNames.Add("08", "kolovoza");
            croatianMonthsNames.Add("09", "rujna");
            croatianMonthsNames.Add("10", "listopada");
            croatianMonthsNames.Add("11", "studenog");
            croatianMonthsNames.Add("12", "prosinca");



            string day = Regex.Replace(dateToAdjust.Split('-')[2], @"\s+", "");
            string month = Regex.Replace(dateToAdjust.Split('-')[1], @"\s+", "");
            string year = Regex.Replace(dateToAdjust.Split('-')[0], @"\s+", "");

            string adjustedDate = day+". "+ croatianMonthsNames[month]+ " "+ year+".";

            return adjustedDate;
        }



    }
}