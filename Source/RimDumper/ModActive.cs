using Verse;

namespace RimDumper
{
    public class ModActive
    {
        private static bool? _ce, _survivaltoolsHsk;

        public static bool CombatExtended => _ce ??= ModLister.GetActiveModWithIdentifier("CETeam.CombatExtended") != null;
        public static bool SurvivalToolsHSK => _survivaltoolsHsk ??= ModLister.GetActiveModWithIdentifier("skyarkhangel.SurvivalToolsLite") != null;
    }
}