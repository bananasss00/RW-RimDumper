using System;
using System.Linq;
using Verse;
using AutoTable;
using RimDumper.Extensions;
using CombatExtended;

namespace RimDumper.Parsers
{
    public class AmmoParser : Parser
    {
        public override string Name => "CEAmmos".Table();

        public override Table? Create()
        {
            if (!ModActive.CombatExtended)
            {
                return null;
            }

            Table table = new(Name);
            Parse(table);
            return table;
        }

        private static void Parse(Table table)
        {
            var defs = DefDatabase<AmmoSetDef>.AllDefs;
            foreach (var d in defs)
            {
                foreach (var ammoType in d.ammoTypes)
                {
                    // ammo users
                    string users = ammoType.ammo.users != null
                        ? String.Join(Environment.NewLine, ammoType.ammo.users
                            .Select(x => x.label)
                            .ToArray())
                        : "";

                    var row = table.NewRow();
                    row["Title".ParserTranslate()] = ammoType.ammo.LabelCap;
                    row["Description".ParserTranslate()] = ammoType.ammo.DescriptionDetailed + $"\n{users}";


                    // ammo stats
                    ProjectilePropertiesCE? projectile = ammoType.projectile.projectile as ProjectilePropertiesCE;
                    if (projectile == null)
                    {
                        continue;
                    }

                    row["DamageTypeArgs".ParserTranslate()] = projectile.damageDef.LabelCap;
                    row["Damage".ParserTranslate()] = projectile.damageAmountBase;
                    row["ArmorPenetration".ParserTranslate()] = projectile.armorPenetrationBase.Nullify(); // OUTDATED
                    row["ArmorPenetrationSharp".ParserTranslate()] = projectile.armorPenetrationSharp.Nullify();
                    row["ArmorPenetrationBlunt".ParserTranslate()] = projectile.armorPenetrationBlunt.Nullify();
                    row["Speed".ParserTranslate()] = projectile.speed;

                    int dmgNum = 2;
                    foreach (var secondaryDamage in projectile.secondaryDamage)
                    {
                        var dmgDef = secondaryDamage.def;
                        var amount = secondaryDamage.amount;
                        row["DamageTypeArgs".ParserTranslate(dmgNum)] = secondaryDamage.def.LabelCap;
                        if (amount != 0)
                        {
                            row["DamageArgs".ParserTranslate(dmgNum)] = secondaryDamage.amount;
                        }
                    }
                }
            }
        }
    }
}