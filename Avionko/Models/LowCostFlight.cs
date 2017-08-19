using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avionko.Models
{
    /// <summary>
    /// class for low cost flights that we're returning upon search.
    /// </summary>
    [Serializable]
    public class LowCostFlight 
    {

        /// <summary>
        /// KING ICT's task specification document specifies in brackets only EUR, HRK and USD. 
        /// That's why we have only 3 currencies :)
        /// </summary>
        public enum FlightCurrency { EUR, HRK, USD }


        private string _airport1;

        private string _airport2;

        private string _dateDeparture;

        private string _dateReturn;

        private int _passengerNumber;

        private int _transferFlightsNumber;

        private FlightCurrency _currency;

        private float _cost;

        private readonly float _totalCost;





        /// <summary>
        /// an airport from which we're taking off
        /// </summary>
        public string Airport1 { get { return this._airport1; } set { this._airport1 = value; } }

        /// <summary>
        /// an airport to which we're flying to
        /// </summary>
        public string Airport2 { get { return this._airport2; } set { this._airport2 = value; } }


        /// <summary>
        /// when do we want to leave?
        /// </summary>
        public string DateDeparture { get { return this._dateDeparture; } set { this._dateDeparture = value; } }

        /// <summary>
        /// when are we coming back?
        /// </summary>
        public string DateReturn { get { return this._dateReturn; } set { this._dateReturn = value; } }


        /// <summary>
        /// are there any additional, transfer, flights needed in order to get from airport1 to aiport2 ? What is their count?
        /// zero if none.
        /// </summary>
        public int TransferFlightsNumber { get { return this._transferFlightsNumber; } set { this._transferFlightsNumber = value; } }

        

        /// <summary>
        /// how many passengers are flying on this flight?
        /// </summary>
        public int PassengerNumber { get { return this._passengerNumber; } set { this._passengerNumber = value; } }

        /// <summary>
        /// are we paying in euros, dollars or kunas ? 
        /// </summary>
        public FlightCurrency Currency { get { return this._currency; } set { this._currency = value; } }


        /// <summary>
        /// what is the price of 1 ticket?
        /// </summary>
        public float Cost { get { return this._cost; } set { this._cost = value; } }


        /// <summary>
        /// total cost = ticket price per one person * number of passengers.
        /// </summary>
        public float TotalCost { get { return this._cost * this._passengerNumber; } }




        public LowCostFlight(string airport1, string airport2, string dateDeparture, string DateReturn, int passengerNumber, int transferFlightsNumber, FlightCurrency currency, float costPerOnePerson)
        {
            this._airport1 = airport1;
            this._airport2 = airport2;
            this._dateDeparture = dateDeparture;
            this._dateReturn = DateReturn;
            this._transferFlightsNumber = transferFlightsNumber;
            this._passengerNumber = passengerNumber;
            this._currency = currency;
            this._cost = costPerOnePerson;
        }

    }
}