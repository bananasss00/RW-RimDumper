using System;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;


namespace RimDumper.Parsers
{
    public class DebuffParser : Parser
    {
        public override string Name => "Debuffs".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            var defs = DefDatabase<ThoughtDef>.AllDefs;
            foreach (var d in defs)
            {
                for (int i = 0; i < d.stages.Count; ++i)
                {
                    MakeRow(table, d, i);
                }
            }
            return table;
        }

        private static float MoodOffsetOfGroup(float baseMood, float kef, int count)
        {
            float num1 = 0.0f;
            float num2 = 1f;
            float num3 = 0.0f;
            for (int index = 0; index < count; ++index)
            {
                num1 += baseMood;
                num3 += num2;
                num2 *= kef;
            }
            float num4 = num1 / count;
            return (float)Math.Round(num4 * num3, 0);
        }

        private void MakeRow(Table table, ThoughtDef t, int stage)
        {
            if (t.stages[stage] == null || String.IsNullOrEmpty(t.stages[stage].label))
            {
                return;
            }

            var row = table.NewRow();
            row["Title".ParserTranslate()] = t.stages[stage].label;
            row["Description".ParserTranslate()] = t.stages[stage].description;
            row["StackLimit".ParserTranslate()] = t.stackLimit;
            row["DurationDays".ParserTranslate()] = t.durationDays;
            row["StackedEffectMultiplier".ParserTranslate()] = t.stackedEffectMultiplier;
            for (int i = 1; i <= t.stackLimit; ++i)
            {
                row[$"X{i}"] = MoodOffsetOfGroup(t.stages[stage].baseMoodEffect, t.stackedEffectMultiplier, i);
            }
        }
    }
}