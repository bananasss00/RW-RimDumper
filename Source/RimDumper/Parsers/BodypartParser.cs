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

            foreach (var d in defs)
            {
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.description;
                row["Efficiency".ParserTranslate()] = d.addedPartProps?.partEfficiency.ToPercent();
                if (d.stages != null)
                {
                    var stage = d.stages.FirstOrDefault();
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