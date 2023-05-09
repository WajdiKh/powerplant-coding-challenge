using System.Text.Json.Serialization;

namespace ProductionPlan.API.Models
{
    public class FuelsInfo
    {
        [JsonPropertyName("gas(euro/MWh)")]
        public double Gas { get; set; } = 0;


        [JsonPropertyName("kerosine(euro/MWh)")]
        public double Kerosine { get; set; } = 0;

        [JsonPropertyName("co2(euro/ton)")]
        public double Co2 { get; set; } = 0;

        [JsonPropertyName("wind(%)")]
        public double Wind { get; set; } = 0;
    }
}
