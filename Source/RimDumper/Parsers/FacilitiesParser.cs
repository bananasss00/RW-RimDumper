using System;
using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;
using System.Collections.Generic;


namespace RimDumper.Parsers
{
    public class FacilitiesParser : Parser
    {
        public override string Name => "Facilities".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            var defs = from d in DefDatabase<ThingDef>.AllDefs
                       where d.GetCompProperties<CompProperties_Facility>() != null
                       select d;

            foreach (var d in defs)
            {
                var prop = d.GetCompProperties<CompProperties_Facility>();
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.DescriptionDetailed;
                row["MaxOnBuild".ParserTranslate()] = prop.maxSimultaneous;
                row["Buildings".ParserTranslate()] = String.Join(", ", GetAvailableBuildings(d));
                row.FillFrom(prop.statOffsets);
            }
            return table;
        }

        public static IEnumerable<string> GetAvailableBuildings(ThingDef facility)
        {
            var dataSources = from d in DefDatabase<ThingDef>.AllDefs
                              where d.GetCompProperties<CompProperties_AffectedByFacilities>() != null
                              select d;

            foreach (var building in dataSources)
            {
                var prop = building.GetCompProperties<CompProperties_AffectedByFacilities>();
                if (prop.linkableFacilities != null && prop.linkableFacilities.Contains(facility))
                {
                    yield return building.LabelCap;
                }
            }
        }
    }
}