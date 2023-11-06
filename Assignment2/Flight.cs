using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    public class Flight
    {
        public string Code {  get; set; }
        public string Airline { get; set; }
        public string CityFrom { get; set; }
        public string CityTo { get; set; }
        public string Day {  get; set; }
        public string Time { get; set; }
        public int Seats { get; set; }
        public float Cost { get; set; }

        public Flight() { }

        public Flight(string code, string airline, string cityFrom, string cityTo, string day, string time, int seats, float cost)
        {
            Code = code;
            Airline = airline;
            CityFrom = cityFrom;
            CityTo = cityTo;
            Day = day;
            Time = time;
            Seats = seats;
            Cost = cost;
        }

        public override string ToString()
        {
            return $"{Code}, {Airline}, {CityFrom}, {CityTo}, {Day}, {Time}, {Cost}";
        }
    }
}
