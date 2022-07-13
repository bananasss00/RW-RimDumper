using System;
using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;
using System.Text;
using UnityEngine;
using CombatExtended;

namespace RimDumper.Parsers
{
    public class WeaponRangedParser : Parser
    {
        public override string Name => "WeaponsRanged".Table();

        private const int RNG_TOUCH = 4;
        private const int RNG_SHORT = 15;
        private const int RNG_MEDIUM = 30;
        private const int RNG_LONG = 50;
        private const int TPS = 60;

        public override Table? Create()
        {
            Table table = new(Name);
            var defs = from d in DefDatabase<ThingDef>.AllDefs
                           //where d.IsWeapon && d.IsRangedWeapon && (d.tradeability.TraderCanSell() || (d.weaponTags != null && d.weaponTags.Contains("TurretGun")))
                       where (d.IsWeapon && d.IsRangedWeapon) || (d.weaponTags != null && d.weaponTags.Contains("TurretGun")) // this show bows
                       orderby d.BaseMarketValue
                       select d;

            foreach (var d in defs)
            {
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.DescriptionDetailed;
                row["MarketValue".ParserTranslate()] = d.BaseMarketValue;
                row["OnMapCount".ParserTranslate()] = d.CountOnMap();
                row["TechLevel".ParserTranslate()] = d.techLevel.ToStringHuman().CapitalizeFirst();
                try
                {
                    var accuracyTouch = d.GetStatValueAbstract(StatDefOf.AccuracyTouch).ToPercent();
                    var accuracyShort = d.GetStatValueAbstract(StatDefOf.AccuracyShort).ToPercent();
                    var accuracyMedium = d.GetStatValueAbstract(StatDefOf.AccuracyMedium).ToPercent();
                    var accuracyLong = d.GetStatValueAbstract(StatDefOf.AccuracyLong).ToPercent();
                    var cooldown = d.GetStatValueAbstract(StatDefOf.RangedWeapon_Cooldown);
                    var mass = d.BaseMass;

                    var verb = d.Verbs.OfType<VerbProperties>().FirstOrDefault();
                    int damage = GetDamageAmount(d, verb.defaultProjectile.projectile);
                    int burstShotCount = verb.burstShotCount > 0 ? verb.burstShotCount : 1;
                    int ticksBetweenBurstShots = verb.ticksBetweenBurstShots > 0 ? verb.ticksBetweenBurstShots : 10;
                    float warmup = verb.warmupTime;
                    float maxRange = verb.range;
                    float minRange = verb.minRange;
                    float burstShotFireRate = (float)Math.Round(60f / verb.ticksBetweenBurstShots.TicksToSeconds());

                    row["CanCraft".ParserTranslate()] = d.CanCraft();
                    row["Dps".ParserTranslate()] = GetDps(damage, burstShotCount, cooldown, warmup, ticksBetweenBurstShots);
                    row["Rpm".ParserTranslate()] = burstShotFireRate;
                    row["Damage".ParserTranslate()] = damage;
                    row["BurstShotCount".ParserTranslate()] = burstShotCount;
                    row["Range".ParserTranslate()] = maxRange;
                    row["CooldownTime".ParserTranslate()] = cooldown;
                    row["WarmupTime".ParserTranslate()] = warmup;
                    row["Accuracy".ParserTranslate()] = GetAccuracyStr(minRange, maxRange, accuracyTouch, accuracyShort, accuracyMedium, accuracyLong);

                    if (ModActive.CombatExtended)
                    {
                        FillRowCombatExtended(row, d, verb);
                    }
                    else
                    {
                        row["DamageType".ParserTranslate()] = verb.defaultProjectile.projectile.damageDef.label;
                    }

                    row["TurretGun".ParserTranslate()] = d.weaponTags?.Contains("TurretGun");
                    row["Artillery".ParserTranslate()] = d.weaponTags?.Contains("Artillery");

                    if (d.weaponTags != null)
                    {
                        row["WeaponTags".ParserTranslate()] = string.Join("; ", d.weaponTags.OrderBy(x => x).ToArray());
                    }
                }
                catch (Exception)
                {
                    //Log.Error($"{d.LabelCap} - {e.Message} - {e.StackTrace}");
                }
            }
            return table;
        }

        private static void FillRowCombatExtended(Row row, ThingDef d, VerbProperties verb)
        {
            var ceAmmo = d.GetCompProperties<CompProperties_AmmoUser>();
            row["DamageType".ParserTranslate()] = ceAmmo != null ? ceAmmo.ammoSet.LabelCap : verb.defaultProjectile.projectile.damageDef.label;
            // TODO: check full stat dump
            row["CE_SightsEfficiency".ParserTranslate()] = d.GetStatValueAbstract(StatDef.Named("SightsEfficiency")).ToPercent();
            row["CE_ShotSpread".ParserTranslate()] = d.GetStatValueAbstract(StatDef.Named("ShotSpread"));
            row["CE_SwayFactor".ParserTranslate()] = d.GetStatValueAbstract(StatDef.Named("SwayFactor"));
            row["CE_OneHanded".ParserTranslate()] = d.weaponTags?.Contains("CE_OneHandedWeapon") ?? false;
        }

        private static float? GetDps(float damage, int burstShotCount, float cooldown, float warmup, int ticksBetweenBurstShots)
        {
            float burstDamage = damage * burstShotCount;
            float warmupTicks = (cooldown + warmup) * TPS;
            float burstTicks = burstShotCount * ticksBetweenBurstShots;
            float totalTime = (warmupTicks + burstTicks) / TPS;

            return (float?)Math.Round(burstDamage / totalTime, 2);
        }

        private static int GetDamageAmount(ThingDef weapon, ProjectileProperties pp)
        {
            var damageAmountBase = pp.damageAmountBase;

            float? wdm = weapon.GetStatValueAbstract(StatDefOf.RangedWeapon_DamageMultiplier);
            float weaponDamageMultiplier = wdm == null ? 1f : (float)wdm;

            int num;
            if (damageAmountBase != -1)
            {
                num = damageAmountBase;
            }
            else
            {
                if (pp.damageDef == null)
                {
                    return 1;
                }
                num = pp.damageDef.defaultDamage;
            }

            return Mathf.RoundToInt(num * weaponDamageMultiplier);
        }

        private static string GetAccuracyStr(float minRange, float maxRange, float accuracyTouch, float accuracyShort, float accuracyMedium, float accuracyLong)
        {
            StringBuilder sb = new();
            _ = minRange > RNG_TOUCH || maxRange < RNG_TOUCH
                ? sb.Append(" - /")
                : sb.Append(" ").Append(Math.Round(accuracyTouch, 1).ToString()).Append(" /");
            _ = minRange > RNG_SHORT || maxRange < RNG_SHORT
                ? sb.Append(" - /")
                : sb.Append(" ").Append(Math.Round(accuracyShort, 1).ToString()).Append(" /");
            _ = minRange > RNG_MEDIUM || maxRange < RNG_MEDIUM
                ? sb.Append(" - /")
                : sb.Append(" ").Append(Math.Round(accuracyMedium, 1).ToString()).Append(" /");
            _ = minRange > RNG_LONG || maxRange < RNG_LONG ? sb.Append(" -") : sb.Append(" ").Append(Math.Round(accuracyLong, 1).ToString());
            return sb.ToString();
        }
    }
}