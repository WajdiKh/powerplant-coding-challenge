namespace ProductionPlan.API.Models
{
    public class PowerPlant : PowerPlantForInput
    {
        /// <summary>
        /// Cost of generating 1MWh of power
        /// </summary>
        public double Cost { get; set;}

        public PowerPlant(PowerPlantForInput powerPlantForInput)
        {
            Name = powerPlantForInput.Name;
            Type = powerPlantForInput.Type;
            Pmin = powerPlantForInput.Pmin;
            Pmax = powerPlantForInput.Pmax;
            Efficiency = powerPlantForInput.Efficiency;
        }
    }
}
