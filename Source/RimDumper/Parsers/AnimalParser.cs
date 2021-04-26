using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;


namespace RimDumper.Parsers
{
    public class AnimalParser : Parser
    {
        public override string Name => "Animals".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            if (Current.ProgramState != ProgramState.Playing) // Create pawns/things not work from menu
            {
                var row = table.NewRow();
                row["Error".ParserTranslate()] = "Can't parse from MainMenu";
                return table;
            }

            var defs = from k in DefDatabase<ThingDef>.AllDefs
                       where k.race?.Animal ?? false
                       select k;

            foreach (var d in defs)
            {
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.DescriptionDetailed;
                row["MeleeDPS".ParserTranslate()] = d.AnimalMeleeDps().Nullify().RoundTo2();
                row["MeleeArmorPenetration".ParserTranslate()] = d.AnimalArmorPenetration().ToPercent();
                row["ManhunterOnTameFailChance".ParserTranslate()] = d.race!.manhunterOnTameFailChance.ToPercent();
                row["Predator".ParserTranslate()] = d.race.predator;
                row["Wildness".ParserTranslate()] = d.race.wildness.ToPercent();
                row["Petness".ParserTranslate()] = d.race.petness.ToPercent();
                row["PackAnimal".ParserTranslate()] = d.race.packAnimal;
                row["HerdAnimal".ParserTranslate()] = d.race.herdAnimal;
                row["Trainability".ParserTranslate()] = d.race.trainability?.LabelCap;

                var milkable = d.GetCompProperties<CompProperties_Milkable>();
                if (milkable != null)
                {
                    row["MilkDef".ParserTranslate()] = milkable.milkDef.LabelCap;
                    row["MilkIntervalDays".ParserTranslate()] = milkable.milkIntervalDays;
                    row["MilkAmount".ParserTranslate()] = milkable.milkAmount;
                }

                var shearable = d.GetCompProperties<CompProperties_Shearable>();
                if (shearable != null)
                {
                    row["WoolDef".ParserTranslate()] = shearable.woolDef.LabelCap;
                    row["ShearIntervalDays".ParserTranslate()] = shearable.shearIntervalDays;
                    row["WoolAmount".ParserTranslate()] = shearable.woolAmount;
                }

                var rescueDef = DefDatabase<TrainableDef>.AllDefs.FirstOrDefault(td => td.defName == "Rescue");
                var tr = d.race.trainability?.intelligenceOrder;
                if (tr != null && rescueDef != null)
                {
                    row["Train1".ParserTranslate()] = tr >= TrainabilityDefOf.None.intelligenceOrder;
                    row["Train2".ParserTranslate()] = tr >= TrainabilityDefOf.Simple.intelligenceOrder;
                    row["Train3".ParserTranslate()] = tr >= TrainabilityDefOf.Intermediate.intelligenceOrder &&
                            d.race.baseBodySize >= rescueDef.minBodySize;
                    row["Train4".ParserTranslate()] = tr >= TrainabilityDefOf.Advanced.intelligenceOrder;
                }

                row.FillFrom(d.statBases);
            }
            return table;
        }
    }
}