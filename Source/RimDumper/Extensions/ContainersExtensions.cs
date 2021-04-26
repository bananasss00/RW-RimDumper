//#define DEBUG

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using HarmonyLib;

namespace RimDumper.Extensions
{
    public static class ContainersExtensions
    {
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static void Dump<T>(this IList<T> rows, string outputFileName)
        {
            var props = typeof(T).GetProperties();
            if (props != null)
            {
                var file = new StringBuilder();

                foreach (var row in rows)
                {
                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(row, null);
                        if (value != null)
                        {
                            _ = file.AppendLine($"{prop.Name} = {value}");
                        }
                    }

                    _ = file.AppendLine("===new_row===");
                }

                File.WriteAllText(outputFileName, file.ToString());
            }
        }
    }
}