using Microsoft.AspNetCore.Mvc;
using ProductionPlan.API.Models;
using ProductionPlan.API.Models.Enums;

namespace ProductionPlan.API.Controllers
{
    [Route("productionplan")]
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {

        /// <summary>
        /// This method returns the production plan of the required load
        /// </summary>
        /// <param name="productionInfo">Production information</param>
        /// <returns>the list of power plants to use with the produced amount per plant</returns>
        [HttpPost]
        public JsonResult ComputeProductionPlan(ProductionInfo productionInfo)
        {
            return new JsonResult(GetProductionPlan(productionInfo));
        }
        
        /// <summary>
        /// This method returns the production plan of the required load
        /// </summary>
        /// <returns>An IEnumerable of produced amount of power per power plant</returns>
        private IEnumerable<ProducedAmountPerPowerPlant> GetProductionPlan(ProductionInfo productionInfo)
        {
            if (productionInfo?.Powerplants == null || productionInfo?.Fuels == null)
                throw new Exception("Invalid production information");

            // Compute each power plant cost
            var powerPlantsWithCalculatedCosts = ComputePowerPlantCosts(productionInfo);

            // Order power plants
            var orderedPowerPlants = powerPlantsWithCalculatedCosts
                .OrderBy(p => p.Cost)
                .ThenBy(p => p.Pmin)
                .ThenByDescending(p => p.Pmax);

            var accumulatedProducedAmount = 0;
            foreach (var plant in orderedPowerPlants)
            {
                // The load is already covered
                if (accumulatedProducedAmount >= productionInfo.Load)
                    break;

                // set effective production capacity of wind turbine plants
                if (plant.Type == PowerPlantType.Windturbine)
                {
                    plant.Pmin = (int)productionInfo.Fuels.Wind * plant.Pmin / 100;
                    plant.Pmax = (int)productionInfo.Fuels.Wind * plant.Pmax / 100;
                }

                // Ignore the plant if it cannot produce any power
                if(plant.Pmax == 0)
                    continue; 
                
                // Pmin covers the remaining amount required so we produce Pmin
                if (accumulatedProducedAmount + plant.Pmin >= productionInfo.Load)
                {
                    accumulatedProducedAmount += plant.Pmin;
                    yield return new() { Name = plant.Name, Amount = (int)(plant.Pmin / 0.01) };
                    continue;
                }

                // Pmax is less than the remaining amount required so we produce Pmax
                if (accumulatedProducedAmount + plant.Pmax <= productionInfo.Load)
                {
                    accumulatedProducedAmount += plant.Pmax;
                    yield return new() { Name = plant.Name, Amount = (int)(plant.Pmax / 0.01) };
                    continue;
                }
                
                // The plant is able to produce the remaining amount required   
                var amountToProduce = productionInfo.Load - accumulatedProducedAmount;
                
                // The load is covered 
                accumulatedProducedAmount = productionInfo.Load;
                yield return new()
                {
                    Name = plant.Name,
                    Amount = (int)(amountToProduce / 0.01)
                };
            }
        } 

        /// <summary>
        /// This method computes the cost of producing 1Mwh of power for each power plant
        /// </summary>
        /// <param name="productionInfo">power production information</param>
        private IEnumerable<PowerPlant> ComputePowerPlantCosts(ProductionInfo productionInfo)
        {
            if (productionInfo?.Powerplants == null || productionInfo?.Fuels == null)
                throw new Exception("invalid production information");

            foreach(var inputPlant in productionInfo.Powerplants)
            {
                var plant = new PowerPlant(inputPlant);
                switch(plant.Type)
                {
                    case PowerPlantType.Turbojet:
                        plant.Cost = productionInfo.Fuels.Kerosine * (1 / plant.Efficiency);
                        break;
                    case PowerPlantType.Gasfired:
                        plant.Cost = productionInfo.Fuels.Gas * (1 / plant.Efficiency) + 0.3 * productionInfo.Fuels.Co2;
                        break;
                    default:
                        break;
                }
                yield return plant;
            }
        }
    }
}
