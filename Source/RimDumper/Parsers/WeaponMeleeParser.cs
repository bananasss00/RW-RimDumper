using System;
using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;


namespace RimDumper.Parsers
{
    public class WeaponMeleeParser : Parser
    {
        public override string Name => "WeaponsMelee".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            var defs = from d in DefDatabase<ThingDef>.AllDefs
                       where d.IsWeapon && d.IsMeleeWeapon/* && d.tradeability.TraderCanSell()*/
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
                    float tmpCldwn = 1f;
                    float tmpDmg = 0f;
                    bool usethis = false;
                    float cooldown = 0f, damage = 0f;
                    string damageType = "";

                    foreach (Tool tl in d.tools ?? Enumerable.Empty<Tool>())
                    {
                        usethis = false;
                        if (tmpDmg / tmpCldwn < tl.power / tl.cooldownTime)
                        {
                            cooldown = tl.cooldownTime;
                            damage = tl.power;
                            usethis = true;
                        }
                        if (usethis)
                        {
                            foreach (ToolCapacityDef tcd in tl.capacities)
                            {
                                damageType = tcd.label + " (" + tl.label + ")";
                            }
                        }
                    }

                    // In HSK GetStatValue cause exception for MeleeWeapon_Shocker, MeleeWeapon_ElectricBaton
                    row["CanCraft".ParserTranslate()] = d.CanCraft();
                    row["Dps".ParserTranslate()] = (float)Math.Round(d.GetStatValueAbstract(StatDefOf.MeleeWeapon_AverageDPS), 2);
                    row["Damage".ParserTranslate()] = damage;
                    row["CooldownTime".ParserTranslate()] = cooldown;
                    row["CE_OneHanded".ParserTranslate()] = d.weaponTags?.Contains("CE_OneHandedWeapon") ?? false;

                    if (d.weaponTags != null)
                    {
                        row["WeaponTags".ParserTranslate()] = string.Join("; ", d.weaponTags.OrderBy(x => x).ToArray());
                    }

                    row.FillFrom(d.equippedStatOffsets);
                }
                catch (Exception e)
                {
                    Log.Error($"{d.LabelCap} - {e.Message} - {e.StackTrace}");
                }
            }
            return table;
        }
    }
}