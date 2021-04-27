using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;

using System.Reflection;
using System.Reflection.Emit;
using RimDumper.Parsers;
using RimDumper.Extensions;
using AutoTable;

namespace RimDumper.Dynamic
{
    public class CustomParser1 : Parser
    {
        public override string Name => "CustomParser1";

        public override Table Create()
        {
            Table table = new Table(Name);
            foreach (var d in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                var row = table.NewRow();
                row["LabelCap"] = d.LabelCap;
                row["Description"] = d.DescriptionDetailed;
            }
            return table;
        }
    }

    public class CustomParser2 : Parser
    {
        public override string Name => "CustomParser2";

        public override Table Create()
        {
            Table table = new Table(Name);
            foreach (var d in DefDatabase<HediffDef>.AllDefsListForReading)
            {
                var row = table.NewRow();
                row["LabelCap"] = d.LabelCap;
                row["Description"] = d.description;
            }
            return table;
        }
    }
}