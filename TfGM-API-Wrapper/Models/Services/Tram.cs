namespace TfGM_API_Wrapper.Models.Services
{
    /// <summary>
    /// Stores service information about a single tram.
    /// </summary>
    public class Tram
    {
        public string Destination { get;}
        
        // The carriages could be a good candidate for an enum, but given there are only
        // two possible values, this may be unnecessary. 
        public string Carriages { get; }
        public string Status { get; }
        public string Wait { get; }


        public Tram(string destination, string carriages, string status, string wait)
        {
            this.Destination = destination;
            this.Carriages = carriages;
            this.Status = status;
            this.Wait = wait;
        }
            
        //TODO Add Equals Method
    }
}