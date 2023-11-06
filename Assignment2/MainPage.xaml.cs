using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;

namespace Assignment2;

public partial class MainPage : ContentPage
{

	//public List<Airline> AllAirlines { get; set; }
	public List<Airport> AllAirports { get; set; }
    public ObservableCollection<string> AirportCodes { get; set; }
	public List<Flight> AllFlights { get; set; }
    public ObservableCollection<Flight> FoundFlights { get; set; }
    public ObservableCollection<string> Days { get; set; }
    public string ReservationCode {  get; set; }

    private Flight selectedFlight;
    public Flight SelectedFlight
    {
        get { return selectedFlight; }
        set
        {
            selectedFlight = value;
            OnPropertyChanged();
        }
    }

    public string CSVFilePathAirports
    {
        get
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(currentDir, "Data\\airports.csv");

            return filePath;
        }
    }
    public string JSONFilePathAirports
    {
        get
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(currentDir, "Data\\airports.json");

            return filePath;
        }
    }

    public string CSVFilePathFlights
    {
        get
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(currentDir, "Data\\flights.csv");

            return filePath;
        }
    }
    public string JSONFilePathFlights
    {
        get
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(currentDir, "Data\\flights.json");

            return filePath;
        }
    }

    public MainPage()
	{
		InitializeComponent();

        this.AllAirports = new List<Airport>();
        this.AirportCodes = new ObservableCollection<string>();
        this.FoundFlights = new ObservableCollection<Flight>();
        this.AllFlights = new List<Flight>();
        this.Days = new ObservableCollection<string>();

        SelectedFlight = new Flight();

        this.BindingContext = this;

        App.Current.MainPage.Window.Destroying += Window_Destroying;

        this.LoadAirportsFromFile();
        this.LoadFlightsFromFile();
        this.PopulateAirportsCode();
        this.PopulateDaysList();

        findAirportCodeFrom.SelectedIndex = 0;
        findAirportCodeTo.SelectedIndex = 1;
        findDay.SelectedIndex = 0;
    }

    private void Window_Destroying(object sender, EventArgs e)
    {
        this.SaveAirportsToFile();
    }

    private void LoadAirportsFromFile()
    {
        this.AllAirports.Clear();

        if(File.Exists(this.JSONFilePathAirports))
        {
            string contents = File.ReadAllText(this.JSONFilePathAirports);

            object airportsObj = JsonSerializer.Deserialize(contents, this.AllAirports.GetType());

            List<Airport> airports = airportsObj as List<Airport>;

            if( airports == null)
            {
                this.DisplayAlert("Error", "Unable to decode from JSON file.", "OK");
                return;
            }

            foreach(Airport airport in airports )
            {
                this.AllAirports.Add(airport);
            }
        }
        else if (File.Exists(this.CSVFilePathAirports))
        {
            // If not, load from CSV file.
//            string[] lines = File.ReadAllLines(this.CSVFilePathAirports);

            List<string> lines = new List<string>();

            var streamCsv = this.CSVFilePathAirports;

            using ( var reader = new StreamReader(streamCsv) )
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    lines.Add(line);
                }
            }

            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                string code = columns[0];
                string name = columns[1];

                Airport airport = new Airport(code, name);

                this.AllAirports.Add(airport);
            }
        }
    }

    private void LoadFlightsFromFile()
    {
        this.AllFlights.Clear();

        if (File.Exists(this.CSVFilePathFlights))
        {
            string[] lines = File.ReadAllLines(this.CSVFilePathFlights);

            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                string code = columns[0];
                string airline = columns[1];
                string cityFrom = columns[2];
                string cityTo = columns[3];
                string day = columns[4];
                string time = columns[5];
                int seats = int.Parse(columns[6]);
                float cost = float.Parse(columns[7]);

                Flight flight = new Flight(code, airline, cityFrom, cityTo, day, time, seats, cost);

                this.AllFlights.Add(flight);
            }
        }
    }

    private void SaveAirportsToFile()
    {
        string encoded = JsonSerializer.Serialize(this.AllAirports, this.AllAirports.GetType());

        File.WriteAllText(this.JSONFilePathAirports, encoded);
    }

    private void PopulateAirportsCode()
    {
        this.AirportCodes.Clear();

        foreach( Airport airport in this.AllAirports )
        {
            this.AirportCodes.Add(airport.Code);
        }
    }

    private void PopulateDaysList()
    {
        this.Days.Clear();

        Days.Add("Monday");
        Days.Add("Tuesday");
        Days.Add("Wednesday");
        Days.Add("Thursday");
        Days.Add("Friday");
        Days.Add("Saturday"); 
        Days.Add("Sunday");
    }

    private void FindFlightsClicked(object sender, EventArgs e)
    {
        string expectedFrom = (string)this.findAirportCodeFrom.SelectedItem;
        string expectedTo = (string)this.findAirportCodeTo.SelectedItem;
        string expectedDay = (string)this.findDay.SelectedItem;

        ObservableCollection<Flight> found = new ObservableCollection<Flight>();

        foreach( Flight flight in this.AllFlights)
        {
            string actualFrom = flight.CityFrom;
            string actualTo = flight.CityTo;
            string actualDay = flight.Day;

            if( expectedFrom == actualFrom && expectedTo == actualTo && expectedDay == actualDay)
            {

                found.Add(flight);
            }
        }

        this.FoundFlights.Clear();

        foreach ( Flight flight in found )
        {
            this.FoundFlights.Add(flight);
        }
        findFlight.SelectedIndex = 0;

        this.ReservationCode = null;
        OnPropertyChanged(nameof(ReservationCode));

        if (FoundFlights.Count == 0)
        {
            this.selectedFlight = null;
            OnPropertyChanged(nameof(SelectedFlight));
        }
    }

    private void ConfirmButtonClicked(object sender, EventArgs e)
    {
        string expectedName;
        string expectedCitizenship;

        if( string.IsNullOrEmpty(this.nameEntry.Text) == false)
        {
            expectedName = this.nameEntry.Text;
        }
        else
        {
            expectedName = "";
        }

        if( string.IsNullOrEmpty(this.citizenshipEntry.Text) == false)
        {
            expectedCitizenship = this.citizenshipEntry.Text;
        }
        else
        {
            expectedCitizenship = "";
        }

        bool nameEntered = false;
        bool citizenshipEntered = false;
        bool flightSelected = false;

        if( expectedName.Length > 0)
        {
            nameEntered = true;
        }

        if( expectedCitizenship.Length > 0 )
        {
            citizenshipEntered = true;
        }

        if ( selectedFlight != null )
        {
            flightSelected = true;
        }

        if( nameEntered && citizenshipEntered && flightSelected)
        {
            this.ReservationCode = RandomCode();
            OnPropertyChanged(nameof(ReservationCode));
        }
        else if( !flightSelected)
        {
            DisplayAlert("Ooops", "Please make sure that a flight is selected", "OK");
            this.ReservationCode = null;
            OnPropertyChanged(nameof(ReservationCode));
        }
        else if( !nameEntered && !citizenshipEntered )
        {
            DisplayAlert("Ooops", "The name and citizenship cannot be empty", "OK");
            this.ReservationCode = null; 
            OnPropertyChanged(nameof(ReservationCode));
        }
        else if( !nameEntered )
        {
            DisplayAlert("Ooops", "The name cannot be empty", "OK");
            this.ReservationCode = null;
            OnPropertyChanged(nameof(ReservationCode));
        }
        else if( !citizenshipEntered)
        {
            DisplayAlert("Ooops", "The citizenship cannot be empty", "OK");
            this.ReservationCode = null;
            OnPropertyChanged(nameof(ReservationCode));
        }
        else
        {
            DisplayAlert("Ooops", "Something went wrong please try again", "OK");
            this.ReservationCode = null;
            OnPropertyChanged(nameof(ReservationCode));
        }
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        Picker picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex >= 0 && selectedIndex < FoundFlights.Count)
        {
            SelectedFlight = FoundFlights[selectedIndex];
        }
    }

    private string RandomCode()
    {
        string[] letters = new string[26] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        Random random = new Random();

        string letter = letters[random.Next(letters.Length)];

        int x = random.Next(1000,9999);
        string nmb = x.ToString();

        return letter.ToUpper() + nmb;
    }
}