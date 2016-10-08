namespace MapApi
{
    public class MapPoint
    {
        public int Id { get; set; }

        public string MapId { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public string Description { get; set; }
    }
}