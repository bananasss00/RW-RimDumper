using System;
using System.Linq;
using Verse;
using AutoTable;
using RimDumper.Extensions;


namespace RimDumper.Parsers
{
    public class MaterialParser : Parser
    {
        public override string Name => "Materials".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            var defs = from d in DefDatabase<ThingDef>.AllDefs
                       where d.IsStuff
                       orderby d.BaseMarketValue
                       select d;

            foreach (var d in defs)
            {
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.DescriptionDetailed;
                row["OnMapCount".ParserTranslate()] = d.CountOnMap();
                row["Category".ParserTranslate()] = GetCategory(d);
                row.FillFrom(d.stuffProps?.statFactors);
                row.FillFrom(d.stuffProps?.statOffsets);
            }
            return table;
        }

        public static string? GetCategory(ThingDef d)
        {
            var cats = d.thingCategories;
            if (cats == null)
            {
                return null;
            }

            static string getCatPath(ThingCategoryDef categoryDef)
            {
                var path = categoryDef.Parents
                    .Where(x => !x.defName.Equals("Root"))
                    .Select(x => x.label)
                    .Reverse()
                    .ToList();
                path.Add(categoryDef.label);
                return String.Join("/", path.ToArray());
            }

            return String.Join("; ", cats.Select(getCatPath).OrderBy(x => x).ToArray());
        }
    }
}