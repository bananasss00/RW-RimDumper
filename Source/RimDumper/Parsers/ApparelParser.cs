using System;
using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;

namespace RimDumper.Parsers
{
    public class ApparelParser : Parser
    {
        public override string Name => "Apparel".Table();

        public bool UseDefStuff { get; set; }

        //private List<StatDef> _statDefs;

        public ApparelParser(bool useDefStuff)
        {
            UseDefStuff = true;//useDefStuff;
        }

        public override Table? Create()
        {
            //_statDefs = DefDatabase<StatDef>.AllDefsListForReading;

            Table table = new(Name);
            var defs = from d in DefDatabase<ThingDef>.AllDefs
                       where d.IsApparel
                       orderby d.BaseMarketValue
                       select d;

            foreach (var d in defs)
            {
                if (!UseDefStuff && d.MadeFromStuff)
                {
                    ThingDef defStuff = GenStuff.DefaultStuffFor(d);
                    foreach (var stuff in GenStuff.AllowedStuffsFor(d))
                    {
                        MakeRow(table, d, defStuff, stuff);
                    }
                }
                else
                {
                    MakeRow(table, d);
                }
            }

            // удаление мусора после брута StatDef. все колонки где значение одинаковые для всех строк
            // ClearEqualsColumns(table);
            return table;
        }

        // private static void ClearEqualsColumns(Table table)
        // {

        //     var cellsByColumn = table.SelectMany(x => /*row*/x.Select(x => /*cell*/x))
        //         .GroupBy(x => x.column);
        //     List<Cell> toRemove = new();
        //     foreach (var group in cellsByColumn)
        //     {
        //         Cell first = group.First();
        //         if (group.All(x => first.value?.Equals(x.value) ?? x.value == null))
        //         {
        //             // Если все элементы колоки равны
        //             toRemove.Add(first);
        //         }
        //     }
        //     foreach (var row in table)
        //     {
        //         foreach (var cell in toRemove)
        //         {
        //             row.Cells.RemoveWithColumn(cell.column.name);
        //             Log.Warning($"remove: {cell.column.name}");
        //         }
        //     }
        //     Log.Warning($"Removed: {toRemove.Count}");
        // }

        private void MakeRow(Table table, ThingDef d, ThingDef? defStuff = null, ThingDef? stuff = null)
        {
            var row = table.NewRow();
            row["Title".ParserTranslate()] = d.LabelCap;
            row["Description".ParserTranslate()] = d.DescriptionDetailed;
            row["OnMapCount".ParserTranslate()] = d.CountOnMap();
            row["MarketValue".ParserTranslate()] = d.BaseMarketValue;
            row["CanCraft".ParserTranslate()] = d.CanCraft(); //d.recipeMaker != null;
            row["DefMaterial".ParserTranslate()] = defStuff?.label;
            row["TechLevel".ParserTranslate()] = d.techLevel.ToStringHuman().CapitalizeFirst();

            try
            {
                // used values from def without stuff bonuses
                /*
                if (d.MadeFromStuff)
                {
                    defStuff = stuff ?? GenStuff.DefaultStuffFor(d);
                }

                if (defStuff != null)
                {
                    row["Title".ParserTranslate()] += $"({defStuff.label})";
                }

                
                row["Material".ParserTranslate()] = defStuff?.label;
                */
                row["CanCraft".ParserTranslate()] = d.CanCraft();
                row["Body".ParserTranslate()] = String.Join(",", d.apparel.bodyPartGroups.Select(x => x.label).OrderBy(x => x).ToArray());
                row["Layer".ParserTranslate()] = String.Join(",", d.apparel.layers.Select(x => x.label).OrderBy(x => x).ToArray());

                // это значения из дефов, не учитывается влияение материала
                row.FillFrom(d.statBases);
                row.FillFrom(d.equippedStatOffsets);

                // брут всех статов в игре на одежду с влиянием материала
                // foreach (var s in _statDefs)
                // {
                //     try {
                //         if (row[s.LabelCap] == null)
                //             row[s.LabelCap] = d.GetStatValueAbstract(s, defStuff).Nullify();//.ByStyle(s.toStringStyle);
                //     } catch {}
                // }
            }
            catch (Exception e)
            {
                Log.Error($"{d.LabelCap} - {e.Message} - {e.StackTrace}");
            }
        }
    }
}