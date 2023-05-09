using System.Text.Json.Serialization;

namespace ProductionPlan.API.Models
{
    public class ProducedAmountPerPowerPlant
    {
        public string Name { get; set; }

        [JsonPropertyName("p")]
        public int Amount { get; set; }
    }
}
