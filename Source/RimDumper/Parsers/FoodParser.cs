using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;
using System.Collections.Generic;


namespace RimDumper.Parsers
{
    public class FoodParser : Parser
    {
        public override string Name => "Foods".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            var defs = from d in DefDatabase<ThingDef>.AllDefs where d.IsIngestible select d;

            foreach (var d in defs)
            {
                var prop = d.GetCompProperties<CompProperties_Facility>();
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.DescriptionDetailed;

                var ip = d.ingestible;
                row["Nutrition".ParserTranslate()] = ip.CachedNutrition.RoundTo2();
                row["JoyKind".ParserTranslate()] = ip.JoyKind?.label;
                row["Joy".ParserTranslate()] = ip.joy;
                row["IsCorpse".ParserTranslate()] = d.IsCorpse;
                row["IsMeat".ParserTranslate()] = d.IsMeat;
                row["IsMeal".ParserTranslate()] = ip.IsMeal;
                row["DrugCategory".ParserTranslate()] = ip.drugCategory.ToString();
                row["FoodType".ParserTranslate()] = ip.foodType.ToString();
                row["IngestEffect".ParserTranslate()] = ip.ingestEffect?.LabelCap.ToString();
                row["Preferability".ParserTranslate()] = ip.preferability.ToString();

                var doers = DoersGiveHediff(ip.outcomeDoers);
                if (doers != null)
                {
                    row["Description".ParserTranslate()] += $"\nHediffs:\n";
                    var toleranceDef = ToleranceDef(doers);
                    foreach (var doer in doers)
                    {
                        var hediff = doer.hediffDef;
                        if (hediff == null || hediff == toleranceDef || hediff.stages == null)
                        {
                            continue;
                        }

                        HediffStage? stage = null;
                        int i = 0;
                        for (; i < hediff.stages.Count; i++)
                        {
                            if (doer.severity > hediff.stages[i].minSeverity)
                            {
                                stage = hediff.stages[i];
                                break;
                            }
                        }
                        if (stage == null)
                        {
                            continue; // недостаточно 1го употребления
                        }

                        row["Description".ParserTranslate()] += $"{hediff.LabelCap}({stage.label})\n";
                        row["PainFactor".ParserTranslate()] = stage.painFactor.ToPercent();

                        var severityPerDay = hediff.CompProps<HediffCompProperties_SeverityPerDay>()?.severityPerDay;
                        if (severityPerDay != null)
                        {
                            row["SeverityDays".ParserTranslate()] = SeverityDays(doer.severity, (float)severityPerDay).ToString("F2");
                        }

                        var thought = DefDatabase<ThoughtDef>.AllDefsListForReading.FirstOrDefault(t => t.hediff == doer.hediffDef);
                        if (thought != null && thought.stages != null)
                        {
                            row["BaseMoodEffect".ParserTranslate()] = i < thought.stages.Count
                                    ? thought.stages[i].baseMoodEffect
                                    : thought.stages.Last().baseMoodEffect;
                        }

                        row.FillFrom(stage.capMods);
                    }
                }
            }
            return table;
        }

        private static IEnumerable<IngestionOutcomeDoer_GiveHediff>? DoersGiveHediff(IEnumerable<IngestionOutcomeDoer> d)
        {
            return d?.OfType<IngestionOutcomeDoer_GiveHediff>()/*.Where(d => d.hediffDef?.stages != null)*/;
        }

        private static HediffDef? ToleranceDef(IEnumerable<IngestionOutcomeDoer_GiveHediff> doers)
        {
            return doers?.Select(doer => doer.toleranceChemical?.toleranceHediff).FirstOrDefault();
        }

        private static float SeverityDays(float severity, float severityPerDay)
        {
            //float severity = this.averageSeverityPerDayBeforeGeneration * daysPassed * Rand.Range(0.5f, 1.4f) + hediff.def.initialSeverity;
            if (severityPerDay >= 0)
            {
                return float.PositiveInfinity;
            }

            float days = 0;
            while (true)
            {
                days += 0.1f;
                if (severity + (days * severityPerDay) <= 0)
                {
                    break;
                }
            }

            return days;
        }
    }
}