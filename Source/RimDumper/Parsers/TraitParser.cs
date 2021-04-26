using System;
using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;
using System.Text.RegularExpressions;


namespace RimDumper.Parsers
{
    public class TraitParser : Parser
    {
        public override string Name => "Traits".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            var defs = DefDatabase<TraitDef>.AllDefsListForReading;
            foreach (var d in defs)
            {
                var row = table.NewRow();

                string defName = d.defName, conflictingTraits = "", disabledWorkTypes = "", disabledWorkTags = "", requiredWorkTypes = "", requiredWorkTags = "";

                if (d.conflictingTraits?.Any() ?? false)
                {
                    conflictingTraits = String.Join(", ", d.conflictingTraits.Select(x => x.defName).OrderBy(x => x).ToArray());
                }

                if (d.disabledWorkTypes?.Any() ?? false)
                {
                    disabledWorkTypes = String.Join(", ", d.disabledWorkTypes.Select(x => x.label).OrderBy(x => x).ToArray());
                }

                if (d.disabledWorkTags != WorkTags.None)
                {
                    disabledWorkTags = d.disabledWorkTags.ToString();
                }

                if (d.requiredWorkTypes?.Any() ?? false)
                {
                    requiredWorkTypes = String.Join(", ", d.requiredWorkTypes.Select(x => x.label).OrderBy(x => x).ToArray());
                }

                if (d.requiredWorkTags != WorkTags.None)
                {
                    requiredWorkTags = d.requiredWorkTags.ToString();
                }

                if (d.degreeDatas?.Any() ?? false)
                {
                    foreach (var deg in d.degreeDatas)
                    {
                        row["Title".ParserTranslate()] = Regex.Replace(deg.label, "<.*?>", ""); // remove html tags
                        row["Description".ParserTranslate()] = deg.description;
                        row["defName"] = defName;
                        row["conflictingTraits".ParserTranslate()] = conflictingTraits;
                        row["DisabledWorkTypes".ParserTranslate()] = disabledWorkTypes;
                        row["disabledWorkTags".ParserTranslate()] = disabledWorkTags;
                        row["requiredWorkTypes".ParserTranslate()] = requiredWorkTypes;
                        row["requiredWorkTags".ParserTranslate()] = requiredWorkTags;
                        row.FillFrom(deg.skillGains);
                        row.FillFrom(deg.statFactors);
                        row.FillFrom(deg.statOffsets);
                    }
                }
            }
            return table;
        }
    }
}