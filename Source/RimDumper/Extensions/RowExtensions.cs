using System.Collections.Generic;
using AutoTable;
using RimWorld;
using Verse;

namespace RimDumper.Extensions
{
    public static class RowExtensions
    {
        public static void FillFrom(this Row row, IEnumerable<StatModifier>? stats, string prefix = "")
        {
            if (stats == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(prefix))
            {
                prefix = prefix + "\r\n";
            }

            foreach (var sm in stats)
            {
                row[prefix + sm.stat.LabelCap] = sm.value.Nullify().ByStyle(sm.stat.toStringStyle);
            }
        }

        public static void FillFrom(this Row row, IEnumerable<PawnCapacityModifier>? mods)
        {
            if (mods == null)
            {
                return;
            }

            foreach (var cap in mods)
            {
                row[cap.capacity.LabelCap] = cap.offset.Nullify().ToPercent();
            }
        }

        public static void FillFrom(this Row row, Dictionary<SkillDef, int>? skills)
        {
            if (skills == null)
            {
                return;
            }

            foreach (var skill in skills)
            {
                float? value = skill.Value;
                row[skill.Key.LabelCap] = value.Nullify();
            }
        }
    }
}