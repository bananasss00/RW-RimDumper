//#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
//using HarmonyLib;
using AutoTable;
using RimDumper.Extensions;
using RimWorld;
using Verse;

namespace RimDumper
{
    public class ApparelFilter
    {
        private List<Row> _rows;
        private readonly Dictionary<string, HashSet<string>> _allBodysOnLayers;
        private readonly ColumnNames cn = new();

        public class ColumnNames
        {
            public string target = "", layer = "", body = "", canCraft = "", techLvl = "";
        };

        public ApparelFilter(Table table, string columnName, TechLevel? maxTechLevel)
        {
            table.Editor.StartEdit();
            _rows = table.Editor.Rows;

            cn.target = columnName;
            cn.techLvl = "TechLevel".ParserTranslate();
            cn.canCraft = "CanCraft".ParserTranslate();
            cn.layer = "Layer".ParserTranslate();
            cn.body = "Body".ParserTranslate();

            if (maxTechLevel != null)
            {
                _rows = _rows.Where(x => TechLvl(x) <= maxTechLevel).ToList();
            }

            foreach (var ap in _rows)
            {
                ap[cn.layer] = SortJoinedString(ap.GetValue<string>(cn.layer));
                ap[cn.body] = SortJoinedString(ap.GetValue<string>(cn.body));
            }

            // Get unique layers
            var allLayers = new HashSet<string>(_rows
                    .Select(x => x.GetValue<string>(cn.layer))
                    .Where(x => x != null)
                    .Cast<string>()
                    .Distinct()
                );

            // Get unique bodys per layer
            _allBodysOnLayers = new Dictionary<string, HashSet<string>>();
            foreach (var layer in allLayers)
            {
                if (!_allBodysOnLayers.ContainsKey(layer))
                {
                    var uniqueBodys = new HashSet<string>(_rows
                        .Where(x => x.GetValue<string>(cn.layer) == layer)
                        .Select(x => x.GetValue<string>(cn.body))
                        .Where(x => x != null)
                        .Cast<string>()
                        .Distinct());

                    _allBodysOnLayers.Add(layer, uniqueBodys);
                }
            }
        }

        public TechLevel TechLvl(Row row)
        {
            // tl.ToStringHuman().CapitalizeFirst()
            var techLevels = ((TechLevel[])Enum.GetValues(typeof(TechLevel))).ToDictionary(x => x.ToStringHuman().CapitalizeFirst(), x => x);

            string? str = row.GetValue<string>(cn.techLvl);
            if (str == null || !techLevels.TryGetValue(str, out var result))
            {
                result = TechLevel.Undefined;
            }

            return result;
        }

        private string? SortJoinedString(string? str)
        {
            return str == null ? null : String.Join(",", str.Split(',').OrderBy(x => x).ToArray());
        }

        internal void SortApparelsByDescending()
        {
            _rows = _rows.OrderByDescending(x => x.GetValue<float?>(cn.target) ?? 0f)
                    .ThenBy(x => x.GetValue<string>(cn.layer))
                    .ThenBy(x => x.GetValue<string>(cn.body))
                    .ToList();
        }

        internal void SortApparels()
        {
            _rows = _rows.OrderBy(x => x.GetValue<float?>(cn.target) ?? 0f)
                    .ThenBy(x => x.GetValue<string>(cn.layer))
                    .ThenBy(x => x.GetValue<string>(cn.body))
                    .ToList();
        }

        internal List<Row> Filter(bool onlyCraftable, int bodyResultsCount)
        {
            List<Row> filtered = new();

            foreach (var allBodysOnLayer in _allBodysOnLayers)
            {
                string layer = allBodysOnLayer.Key;
                foreach (var bodyOnLayer in allBodysOnLayer.Value)
                {
                    var apparelsWithCurrentBody = _rows
                        .Where(x => x.GetValue<string>(cn.layer) == layer
                                    && x.GetValue<string>(cn.body) == bodyOnLayer
                                    && (!onlyCraftable || x.GetValue<bool>(cn.canCraft))
                                    && x.GetValue<float?>(cn.target) != null
                                    //&& String.IsNullOrWhiteSpace(_techLevel) || (x.GetValue<string>(_techLevelIdx) == _techLevel)
                                    )
                        .ToList();

                    int end = Math.Min(bodyResultsCount, apparelsWithCurrentBody.Count);
                    for (int i = 0; i < end; ++i)
                    {
                        filtered.Add(apparelsWithCurrentBody[i]);
                    }
                }
            }

            return filtered;
        }

        public static Table FilterApparels(Table table, string fieldName, bool orderByDescending, bool onlyCraftable, int bodyResultsCount, TechLevel? maxTechLevel)
        {
            ApparelFilter filter = new(table, fieldName, maxTechLevel);

            if (orderByDescending)
            {
                filter.SortApparelsByDescending();
            }
            else
            {
                filter.SortApparels();
            }

            var filtered = filter.Filter(onlyCraftable, bodyResultsCount);
            table.Editor.Rows = filtered;
            return table.Editor.FinalizeTable();
        }
    }
}