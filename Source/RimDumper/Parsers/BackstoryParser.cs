using System;
using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;


namespace RimDumper.Parsers
{
    public class BackstoryParser : Parser
    {
        public override string Name => "Backstorys".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            foreach (var bs in BackstoryDatabase.allBackstories.Values)
            {
                if (bs.identifier.StartsWith("Skynet_off"))
                {
                    continue; // skip useless bs
                }

                var row = table.NewRow();
                row["Title".ParserTranslate()] = bs.title;
                row["Description".ParserTranslate()] = bs.baseDesc;
                row["BackstorySlot".ParserTranslate()] = bs.slot.ToString();

                if (bs.spawnCategories?.Any() ?? false)
                {
                    row["spawnCategories".ParserTranslate()] = String.Join(", ", bs.spawnCategories.OrderBy(x => x).ToArray());
                }

                if (bs.DisabledWorkGivers?.Any() ?? false)
                {
                    row["DisabledWorkGivers".ParserTranslate()] = String.Join(", ", bs.DisabledWorkGivers.Select(x => x.label).OrderBy(x => x).ToArray());
                }

                if (bs.DisabledWorkTypes?.Any() ?? false)
                {
                    row["DisabledWorkTypes".ParserTranslate()] = String.Join(", ", bs.DisabledWorkTypes.Select(x => x.label).OrderBy(x => x).ToArray());
                }

                if (bs.disallowedTraits?.Any() ?? false)
                {
                    row["disallowedTraits".ParserTranslate()] = String.Join(", ", bs.disallowedTraits.Select(x => x.def.defName).OrderBy(x => x).ToArray());
                }

                if (bs.forcedTraits?.Any() ?? false)
                {
                    row["forcedTraits".ParserTranslate()] = String.Join(", ", bs.forcedTraits.Select(x => x.def.defName).OrderBy(x => x).ToArray());
                }

                if (bs.workDisables != WorkTags.None)
                {
                    row["workDisables".ParserTranslate()] = bs.workDisables.ToString();
                }

                if (bs.skillGainsResolved?.Any() ?? false)
                {
                    foreach (var skillKv in bs.skillGainsResolved)
                    {
                        var def = skillKv.Key;
                        var value = (float?)skillKv.Value;
                        row[def.LabelCap] = value.Nullify();
                    }
                }
            }
            return table;
        }
    }
}