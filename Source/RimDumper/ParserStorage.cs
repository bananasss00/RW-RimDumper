//#define DEBUG

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
//using HarmonyLib;
using RimDumper.Parsers;


namespace RimDumper
{
    public static class ParserStorage
    {
        private static object _locker = new();

        private static readonly Dictionary<Parser, bool> CustomParsers = new();

        private static readonly Dictionary<Parser, bool> Parsers = new()
        {
            { new MaterialParser(), false },
            { new ApparelParser(true), false },
            { new WeaponMeleeParser(), false },
            { new WeaponRangedParser(), false },
            { new AmmoParser(), false },
            { new ToolParser(), false },
            { new FacilitiesParser(), false },
            { new PlantParser(), false },
            { new FoodParser(), false },
            { new BodypartParser(), false },
            { new AnimalParser(), false },
            { new BackstoryParser(), false },
            { new TraitParser(), false },
            { new DebuffParser(), false },
            { new DrugParser(), false },
        };

        private static bool _isCompiling;

        public static bool IsCustomParser(this Parser parser)
        {
            return parser.GetType().Assembly != Assembly.GetExecutingAssembly();
        }

        public static Parser[] All()
        {
            lock (_locker)
            {
                return CustomParsers.Keys.Union(Parsers.Keys).ToArray();
            }
        }

        public static T[] OfType<T>()
        {
            lock (_locker)
            {
                return All().OfType<T>().ToArray();
            }
        }

        public static Parser[] Enabled()
        {
            lock (_locker)
            {
                var parsers = CustomParsers.Union(Parsers)
                    .Where(parser => parser.Value)
                    .Select(parser => parser.Key);
                return parsers.ToArray();
            }
        }

        public static async void LoadCustomFromSource()
        {
            if (_isCompiling)
                return;

            _isCompiling = true;
            
            ClearCustom();
            var parsers = await Task.Run(() => RimDumper.CustomParsers.GenerateCustomParsers());
            foreach (var parser in parsers)
            {
                AddCustom(parser);
            }

            _isCompiling = false;
        }

        public static bool IsEnabled(Parser parser)
        {
            lock (_locker)
            {
                if (Parsers.ContainsKey(parser))
                    return Parsers[parser];
                if (CustomParsers.ContainsKey(parser))
                    return CustomParsers[parser];
            }
            return false;
        }

        public static void SetEnabled(bool value, Parser? parser = null)
        {
            lock (_locker)
            {
                if (parser != null)
                {
                    if (Parsers.ContainsKey(parser))
                        Parsers[parser] = value;
                    else if (CustomParsers.ContainsKey(parser))
                        CustomParsers[parser] = value;
                    return;
                }

                foreach (var key in Parsers.Keys.ToList())
                {
                    Parsers[key] = value;
                }
                foreach (var key in CustomParsers.Keys.ToList())
                {
                    CustomParsers[key] = value;
                }
            }
        }

        public static void AddCustom(Parser parser)
        {
            lock (_locker)
            {
                CustomParsers.Add(parser, true);
            }
        }

        public static void ClearCustom()
        {
            lock (_locker)
            {
                CustomParsers.Clear();
            }
        }
    }
}