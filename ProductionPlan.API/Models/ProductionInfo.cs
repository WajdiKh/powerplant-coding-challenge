namespace ProductionPlan.API.Models
{
    public class ProductionInfo
    {
        public int Load { get; set; }

        public FuelsInfo? Fuels { get; set; }

        public IEnumerable<PowerPlantForInput>? Powerplants { get; set; }
    }

}
