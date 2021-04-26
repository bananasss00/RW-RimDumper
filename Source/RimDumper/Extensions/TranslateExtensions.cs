//#define DEBUG

using System.Linq;
using Verse;
//using HarmonyLib;

namespace RimDumper.Extensions
{
    public static class TranslateExtensions
    {
        private const string ModName = "RimDumper";
        public static TaggedString ParserTranslate(this string str, params NamedArgument[] args)
        {
            const string prefix = "Parser";
            return args.Any() ? $"{ModName}.{prefix}.{str}".Translate(args) : $"{ModName}.{prefix}.{str}".Translate();
        }
        public static TaggedString Table(this string str, params NamedArgument[] args)
        {
            const string prefix = "Table";
            return args.Any() ? $"{ModName}.{prefix}.{str}".Translate(args) : $"{ModName}.{prefix}.{str}".Translate();
        }
        public static TaggedString UiTranslate(this string str, params NamedArgument[] args)
        {
            const string prefix = "UI";
            return args.Any() ? $"{ModName}.{prefix}.{str}".Translate(args) : $"{ModName}.{prefix}.{str}".Translate();
        }

    }
}