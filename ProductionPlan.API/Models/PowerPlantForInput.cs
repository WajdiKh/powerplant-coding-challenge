using ProductionPlan.API.Models.Enums;

namespace ProductionPlan.API.Models
{
    public class PowerPlantForInput
    {
        public string Name { get; set; } = string.Empty;

        public PowerPlantType Type { get; set; }

        public double Efficiency { get; set; } = 0;

        public int Pmin { get; set; } = 0;

        public int Pmax { get; set; } = 0;

    }
}
