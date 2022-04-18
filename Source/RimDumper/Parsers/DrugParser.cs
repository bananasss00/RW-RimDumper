using System.Linq;
using Verse;
using RimWorld;
using AutoTable;
using RimDumper.Extensions;
using System.Collections.Generic;


namespace RimDumper.Parsers
{
    public class DrugParser : Parser
    {
        public override string Name => "Drugs".Table();

        public override Table? Create()
        {
            Table table = new(Name);
            var defs = from d in DefDatabase<ThingDef>.AllDefs where d.IsDrug select d;

            foreach (var d in defs)
            {
                var row = table.NewRow();
                row["Title".ParserTranslate()] = d.LabelCap;
                row["DrugName".ParserTranslate()] = d.LabelCap;
                row["Description".ParserTranslate()] = d.DescriptionDetailed;

                var compDrug = d.GetCompProperties<CompProperties_Drug>();
                row["Addictiveness".ParserTranslate()] = compDrug?.addictiveness.ToPercent();
                row["BaseSeverity".ParserTranslate()] = "+" + d.ingestible?.outcomeDoers?.OfType<IngestionOutcomeDoer_GiveHediff>().FirstOrDefault()?.severity.ToString("F2");

                var doers = GetValidNarcoDoers(d);
                if (doers == null)
                {
                    continue;
                }

                // get BEFORE tolerance heddif for detect he in 'doers' iterate
                var toleranceHeddif = GetToleranceDef(doers);
                IngestionOutcomeDoer_GiveHediff? doerForAddiction = null;
                foreach (var doer in doers)
                {
                    bool isToleranceHeddif = toleranceHeddif != null && doer.hediffDef == toleranceHeddif;
                    if (isToleranceHeddif)
                    {
                        GetFromHeddifTolerance(table, d, doer.hediffDef, doer.severity, d.GetCompProperties<CompProperties_Drug>()?.minToleranceToAddict);
                    }
                    else
                    {
                        var thought = DefDatabase<ThoughtDef>.AllDefs.FirstOrDefault(t => t.hediff == doer.hediffDef);
                        GetFromHeddifStages(table, d, doer.hediffDef, doer.severity, thought);
                        doerForAddiction = doer;
                    }
                }

                //get addictionHeddif
                var addictionHeddif = GetAddictionDef(doers);
                if (addictionHeddif != null)
                {
                    var addictionThought = DefDatabase<ThoughtDef>.AllDefs.FirstOrDefault(t => t.hediff == addictionHeddif);
                    GetFromHeddifAddiction(table, d, addictionHeddif, doerForAddiction?.hediffDef.initialSeverity ?? 0f, addictionThought);
                }
            }
            return table;
        }

        private static void GetFromHeddifTolerance(Table table, ThingDef def, HediffDef hediff, float doerSeverity, float? minToleranceToAddict = null)
        {
            var row = table.NewRow();
            row["DrugName".ParserTranslate()] = def.LabelCap;

            float? severityPerDay = hediff.CompProps<HediffCompProperties_SeverityPerDay>()?.severityPerDay;
            float severity = 0f;
            int count = 0;

            if (minToleranceToAddict == null)
            {
                Log.Error($"[AddRowFromHeddifTolerance] minToleranceToAddict = null");
            }
            else
            {
                severity = CalcSeverityLevelByMin(doerSeverity, (float)minToleranceToAddict, out count);
            }

            row["Title".ParserTranslate()] = $"   {hediff.label}";
            row["Description".ParserTranslate()] = "MinimumToleranceForAddiction".ParserTranslate(minToleranceToAddict == null ? 0f : (float)minToleranceToAddict) + "\r\n" + "TolerancePerDose".ParserTranslate(doerSeverity) + "\r\n" + "TheСhanceOfAddictionWillAppearAfterTheConsumedDoses".ParserTranslate(count, severity);
            row["BaseSeverity".ParserTranslate()] = "+" + doerSeverity.ToString("F2");

            if (minToleranceToAddict != null)
            {
                row["MinToleranceToAddict".ParserTranslate()] = $"{(float)minToleranceToAddict:F2}(x{count})";
            }

            if (severityPerDay != null)
            {
                row["SeverityPerDay".ParserTranslate()] = ((float)severityPerDay).ToString("F2");
                row["SeverityDays".ParserTranslate()] = GetSeverityDays(severity, (float)severityPerDay).ToString("F2");
            }
        }

        private static void GetFromHeddifAddiction(Table table, ThingDef def, HediffDef hediff, float doerSeverity, ThoughtDef thought)
        {
            var row = table.NewRow();
            row["DrugName".ParserTranslate()] = def.LabelCap;

            var severityPerDay = hediff.CompProps<HediffCompProperties_SeverityPerDay>()?.severityPerDay;
            if (hediff.stages.Count > 1)
            {
                var stage = hediff.stages[1];
                row["BaseSeverity".ParserTranslate()] = doerSeverity.ToString("F2");
                row["Title".ParserTranslate()] = $"   " + "Addiction".ParserTranslate();
                //row["Description".ParserTranslate()] = $"Это единственное состояние";
                if (thought != null)
                {
                    var thoughtStages = thought.stages;
                    row["BaseMoodEffect".ParserTranslate()] = 1 < thoughtStages.Count ? thoughtStages[1].baseMoodEffect : thoughtStages.Last().baseMoodEffect;
                }

                if (severityPerDay != null)
                {
                    row["SeverityPerDay".ParserTranslate()] = ((float)severityPerDay).ToString("F2");
                    row["SeverityDays".ParserTranslate()] = GetSeverityDays(doerSeverity, (float)severityPerDay).ToString("F2");
                }

                row["PainFactor".ParserTranslate()] = stage.painFactor.ToPercent();
                row.FillFrom(hediff.stages[0].capMods);
            }
            else
            {
                Log.Error($"[GetFromHeddifAddiction] hediff.stages.Count not standart {hediff.stages.Count}");
            }
        }

        private static void GetFromHeddifStages(Table table, ThingDef def, HediffDef hediff, float doerSeverity, ThoughtDef thought)
        {
            var severityPerDay = hediff.CompProps<HediffCompProperties_SeverityPerDay>()?.severityPerDay;
            for (int i = 0; i < hediff.stages.Count; ++i)
            {
                var stage = hediff.stages[i];

                var row = table.NewRow();
                row["DrugName".ParserTranslate()] = def.LabelCap;

                float severity;
                if (hediff.stages.Count == 1)
                {
                    severity = doerSeverity;
                    row["BaseSeverity".ParserTranslate()] = severity.ToString("F2");
                    row["Title".ParserTranslate()] = $"   {(string.IsNullOrEmpty(stage.label) ? hediff.label : stage.label)}(> {stage.minSeverity})";
                    row["Description".ParserTranslate()] = "ThisIsTheOnlyCondition".ParserTranslate();
                }
                else
                {
                    int count = 1;
                    severity = stage.minSeverity == 0f ? doerSeverity : CalcSeverityLevelByMin(doerSeverity, stage.minSeverity, out count);
                    row["BaseSeverity".ParserTranslate()] = severity.ToString("F2");
                    row["Title".ParserTranslate()] = $"   {(string.IsNullOrEmpty(stage.label) ? hediff.label : stage.label)}(> {stage.minSeverity}) x{count}";
                    row["Description".ParserTranslate()] = "ThisStateStartsAfter".ParserTranslate(stage.minSeverity) + "\r\n" + "ForThisStateYouNeedToTakeTimes".ParserTranslate(count);
                }

                if (thought != null)
                {
                    var thoughtStages = thought.stages;
                    row["BaseMoodEffect".ParserTranslate()] = i < thoughtStages.Count ? thoughtStages[i].baseMoodEffect : thoughtStages.Last().baseMoodEffect;
                }

                if (severityPerDay != null)
                {
                    row["SeverityPerDay".ParserTranslate()] = ((float)severityPerDay).ToString("F2");
                    row["SeverityDays".ParserTranslate()] = GetSeverityDays(severity, (float)severityPerDay).ToString("F2");
                }

                row["PainFactor".ParserTranslate()] = stage.painFactor.ToPercent();

                row.FillFrom(/*hediff.stages[0]*/stage.capMods); // why was [0] wtf?
            }
        }

        private static HediffDef? GetAddictionDef(IEnumerable<IngestionOutcomeDoer_GiveHediff> doers)
        {
            return doers.OrEmptyIfNull().Select(doer => doer.toleranceChemical?.addictionHediff).FirstOrDefault();
        }

        private static HediffDef? GetToleranceDef(IEnumerable<IngestionOutcomeDoer_GiveHediff> doers)
        {
            return doers.OrEmptyIfNull().Select(doer => doer.toleranceChemical?.toleranceHediff).FirstOrDefault();
        }

        private static IEnumerable<IngestionOutcomeDoer_GiveHediff>? GetValidNarcoDoers(ThingDef def)
        {
            return def.ingestible?.outcomeDoers?.OfType<IngestionOutcomeDoer_GiveHediff>().Where(d => d.hediffDef?.stages != null);
        }

        private static float GetSeverityDays(float severity, float severityPerDay)
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

        private static float CalcSeverityLevelByMin(float severity, float minSeverity, out int count)
        {
            count = 0;

            if (severity <= 0f)
                return 0f;

            float result = 0;
            while (result < minSeverity)
            {
                result += severity;
                count++;
            }

            return result;
        }
    }
}