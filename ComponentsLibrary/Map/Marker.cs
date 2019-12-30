namespace ComponentsLibrary.Map
{
    public class Marker
    {
        public string Description { get; set; }

        public double Lat { get; set; }

        public double Lon { get; set; }

        public string Color { get; set; }

        public bool ShowPopup { get; set; }
        public int SatNo { get; set; }
        public double Alt { get; set; }
        public double Quality { get; set; }
    }
}
