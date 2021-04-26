//#define DEBUG

using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
//using HarmonyLib;

namespace RimDumper.Extensions
{
    public static class AnimalExtensions
    {
        public static float AnimalMeleeDps(this ThingDef def)
        {
            return def.tools == null ? 0f : def.AnimalMeleeDmg() * def.AnimalMeleeHitChance() / def.AnimalMeleeCooldown();
        }

        public static float AnimalArmorPenetration(this ThingDef def)
        {
            if (def.tools == null)
            {
                return 0f;
            }

            float ArmorPenetration(Tool tool)
            {
                return tool.armorPenetration < 0f ? tool.power * 0.015f : tool.armorPenetration;
            }

            float Weight(Tool tool)
            {
                return tool.power >= 0.001f ? tool.power * tool.power * tool.chanceFactor * 0.3f : 1f;
            }

            float toolsWeigth = 0f;
            foreach (var tool in def.tools)
            {
                toolsWeigth += Weight(tool) * tool.capacities.Count;
            }

            if (toolsWeigth == 0f)
            {
                return 0f;
            }

            float ap = 0f;
            foreach (var tool in def.tools)
            {
                ap += Weight(tool) * tool.capacities.Count / toolsWeigth * ArmorPenetration(tool);
            }

            return ap;
        }

        public static float AnimalMeleeHitChance(this ThingDef def)
        {
            var kindDef = DefDatabase<PawnKindDef>.AllDefs.FirstOrDefault(k => k.defName == def.defName);
            //Pawn_AgeTracker_Patch.SkipNextPawnKindDef = kindDef;
            Pawn pawn = new() { def = def, kindDef = kindDef };
            pawn.ageTracker = new Pawn_AgeTracker(pawn);
            pawn.health = new Pawn_HealthTracker(pawn);
            pawn.mindState = new Pawn_MindState(pawn);
            return pawn.GetStatValue(StatDefOf.MeleeHitChance);
        }

        public static float AnimalMeleeDmg(this ThingDef def)
        {
            static float Weight(Tool tool)
            {
                return tool.power >= 0.001f ? tool.power * tool.power * tool.chanceFactor * 0.3f : 1f;
            }

            var tools = def.tools;
            float toolsWeigth = 0f;
            foreach (var tool in tools)
            {
                toolsWeigth += Weight(tool) * tool.capacities.Count;
            }

            if (toolsWeigth == 0f)
            {
                return 0f;
            }

            float dmg = 0f;
            foreach (var tool in tools)
            {
                dmg += Weight(tool) * tool.capacities.Count / toolsWeigth * tool.power;
            }

            return dmg;
        }

        public static float AnimalMeleeCooldown(this ThingDef def)
        {
            static float Weight(Tool tool)
            {
                return tool.power >= 0.001f ? tool.power * tool.power * tool.chanceFactor * 0.3f : 1f;
            }

            var tools = def.tools;
            float toolsWeigth = 0f;
            foreach (var tool in tools)
            {
                toolsWeigth += Weight(tool) * tool.capacities.Count;
            }

            if (toolsWeigth == 0f)
            {
                return 1f;
            }

            float cooldown = 0f;
            foreach (var tool in tools)
            {
                cooldown += Weight(tool) * tool.capacities.Count / toolsWeigth * tool.cooldownTime.SecondsToTicks();
            }

            return cooldown / 60f;
        }
    }
}