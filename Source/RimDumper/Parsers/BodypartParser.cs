using System;
using System.Linq;
using Verse;
using AutoTable;
using RimDumper.Extensions;


namespace RimDumper.Parsers
{
    public class BodypartParser : Parser
    {
        public override string Name => "BodyParts".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            var defs = from d in DefDatabase<HediffDef>.AllDefs
                       where d.addedPartProps != null || d.spawnThingOnRemoved != null || d.countsAsAddedPartOrImplant
                       select d;

            var surgeryRecipes = DefDatabase<RecipeDef>.AllDefs.Where(x => x.IsSurgery && x.targetsBodyPart && !x.appliedOnFixedBodyParts.NullOrEmpty()).ToList();

            foreach (var d in defs)
            {
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.spawnThingOnRemoved?.description ?? d.description;
                row["ThingDef"] = d.spawnThingOnRemoved?.LabelCap;

                var recipeDef = surgeryRecipes.FirstOrDefault(x => x.addsHediff == d);
                if (recipeDef != null)
                {
                    row["BodyPart".ParserTranslate()] = string.Join(", ", recipeDef.appliedOnFixedBodyParts.Select(x => x.LabelCap.RawText ?? "empty").OrderBy(x => x));
                    if (recipeDef.incompatibleWithHediffTags?.Any() ?? false)
                    {
                        row["IncompatibleWithTags".ParserTranslate()] = string.Join(", ", recipeDef.incompatibleWithHediffTags.OrderBy(x => x));
                    }

                    row["AllRecipeUsers".ParserTranslate()] = string.Join(", ", recipeDef.AllRecipeUsers.Where(x => x.label != null).Select(x => x.LabelCap.RawText).OrderBy(x => x));
                }

                if (d.tags != null) row["Tags".ParserTranslate()] = string.Join(", ", d.tags.OrderBy(x => x));

                row["Solid"] = d.addedPartProps?.solid;
                row["Efficiency".ParserTranslate()] = d.addedPartProps?.partEfficiency.ToPercent();
                if (d.stages?.Any() ?? false)
                {
                    var stage = d.stages.First();
                    // print statOffsets if exists
                    var statOffsets = stage.statOffsets;
                    if (statOffsets != null)
                    {
                        row["Description".ParserTranslate()] += "\r\n";
                        row["Description".ParserTranslate()] += String.Join("\r\n", stage
                            .statOffsets
                            .Select(x => $" {x.stat.LabelCap} - {x.value}")
                            .ToArray());
                    }
                    row.FillFrom(stage.capMods);
                }

            }
            return table;
        }
    }
}