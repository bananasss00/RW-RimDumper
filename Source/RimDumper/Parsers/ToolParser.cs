using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;
using System.Collections.Generic;
using HarmonyLib;

namespace RimDumper.Parsers
{
    public class ToolParser : Parser
    {
        public override string Name => "Tools".Table();

        public override Table? Create()
        {
            if (!ModActive.SurvivalToolsHSK)
            {
                return null;
            }

            Table table = new(Name);
            var defs = from d in DefDatabase<ThingDef>.AllDefs
                       where (d.thingCategories != null && d.thingCategories.Exists(x => x.defName.StartsWith("SurvivalTools")))
                             || (d.modExtensions != null && (
                                 d.modExtensions.Find(x => x.ToString().StartsWith("SurvivalTools.SurvivalTool")) != null
                                 || d.modExtensions.Find(x => x.ToString().StartsWith("RightToolForJob")) != null
                                 ))
                       select d;

            foreach (var d in defs)
            {
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.DescriptionDetailed;
                row["OnMapCount".ParserTranslate()] = d.CountOnMap();
                row["IsStuff".ParserTranslate()] = false;

                // Select SurvivalTool statModifiers
                var statModifiers = d.modExtensions != null ? d.modExtensions
                    .Where(x => x.ToString().Contains("SurvivalToolProperties"))
                    .Where(x => GetSTBaseWorkStatFactors(x) != null)
                    .SelectMany(x => GetSTBaseWorkStatFactors(x)) : new List<StatModifier>();

                // Select equipped statModifiers
                if (d.equippedStatOffsets != null)
                {
                    statModifiers = statModifiers.Concat(d.equippedStatOffsets);
                }

                row.FillFrom(statModifiers);
            }

            // Materials
            defs = from d in DefDatabase<ThingDef>.AllDefs
                   where d.modExtensions != null && (d.modExtensions.Find(x => x.ToString().StartsWith("SurvivalToolsLite.StuffPropsTool")) != null)
                   select d;

            foreach (var d in defs)
            {
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.DescriptionDetailed;
                row["OnMapCount".ParserTranslate()] = d.CountOnMap();
                row["IsStuff".ParserTranslate()] = true;

                var statModifiers = d.modExtensions
                    .Where(x => x.ToString().Contains("SurvivalToolsLite.StuffPropsTool") && GetSTMaterialsStatFactors(x) != null)
                    .SelectMany(x => GetSTMaterialsStatFactors(x));

                row.FillFrom(statModifiers);
            }
            return table;
        }

        private static List<StatModifier> GetSTBaseWorkStatFactors(DefModExtension ext)
        {
            return Traverse.Create(ext).Field<List<StatModifier>>("baseWorkStatFactors").Value;
        }

        private static List<StatModifier> GetSTMaterialsStatFactors(DefModExtension ext)
        {
            return Traverse.Create(ext).Field<List<StatModifier>>("toolStatFactors").Value;
        }
    }
}