namespace MapApi
{
    public class Map
    {
        public string Id { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public int Elevation { get; set; }

        public int MaxZoom { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Attribution { get; set; }

        public string AccessToken { get ; set;}
    }
}