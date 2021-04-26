using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;


namespace RimDumper.Parsers
{
    public class PlantParser : Parser
    {
        public override string Name => "Plant".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            var defs = from d in DefDatabase<ThingDef>.AllDefs
                       where d.plant != null
                       select d;

            foreach (var d in defs)
            {
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.DescriptionDetailed;

                float marketValue = d.plant.harvestedThingDef?.BaseMarketValue ?? 0f;
                float soilFertility = 1.8f;
                float growthSpeed180 = (soilFertility * d.plant.fertilitySensitivity) + (1 - d.plant.fertilitySensitivity);
                float growProgressIn10days = 10 * growthSpeed180;
                float mult = growProgressIn10days / d.plant.growDays;
                float growEff = mult * d.plant.harvestYield;
                float nutritionPer10Days = growEff * d.plant.harvestedThingDef?.ingestible?.CachedNutrition ?? 0f;
                float marketValuePer10Days = growEff * marketValue;

                row["NutritionPer10Days".ParserTranslate()] = nutritionPer10Days.Nullify().RoundTo2();
                row["MarketValuePer10Days".ParserTranslate()] = marketValuePer10Days.Nullify().RoundTo2();
                row["GrowDays".ParserTranslate()] = d.plant.growDays.Nullify().RoundTo2();
                row["GrowMinGlow".ParserTranslate()] = d.plant.growMinGlow.Nullify().ToPercent();
                row["FertilityMin".ParserTranslate()] = d.plant.fertilityMin.Nullify().ToPercent();
                row["FertilitySensitivity".ParserTranslate()] = d.plant.fertilitySensitivity.Nullify().ToPercent();
                row["HarvestYield".ParserTranslate()] = d.plant.harvestYield;
                row["LifespanDays".ParserTranslate()] = d.plant.LifespanDays.Nullify().RoundTo2();
                row["Sowable".ParserTranslate()] = d.plant.Sowable;
                row["IsTree".ParserTranslate()] = d.plant.IsTree;
                row["SowMinSkill".ParserTranslate()] = d.plant.sowMinSkill;
                row["HarvestedThingDef".ParserTranslate()] = d.plant.harvestedThingDef?.LabelCap;
                row["ProductNutrition".ParserTranslate()] = d.plant.harvestedThingDef?.ingestible?.CachedNutrition;
                row["ProductPreferability".ParserTranslate()] = d.plant.harvestedThingDef?.ingestible?.preferability.ToString();
                row["ProductDaysToRotStart".ParserTranslate()] = d.plant.harvestedThingDef?.GetCompProperties<CompProperties_Rottable>()?.daysToRotStart;
                row["ProductMarketValue".ParserTranslate()] = d.plant.harvestedThingDef?.BaseMarketValue;
            }
            return table;
        }
    }
}