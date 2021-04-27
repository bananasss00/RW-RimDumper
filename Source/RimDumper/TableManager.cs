//#define DEBUG

using System;
using System.Collections.Generic;
using Verse;
//using HarmonyLib;
using AutoTable;
using AutoTable.Xlsx;
using System.Threading.Tasks;
using RimDumper.Parsers;
using UnityEngine;
using OfficeOpenXml.Table;
using System.Linq;
using System.Reflection;


namespace RimDumper
{
    public static class TableManager
    {
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

        private static readonly Dictionary<Parser, bool> CustomParsers = new();

        public static readonly TableCollection Tables = new();
        public static readonly XlsxTableFormat XlsxFormat = new(new XlsxTableStyleColorizer("TableStyle"));
        public static readonly string SavePath = GenFilePaths.FolderUnderSaveData("RimDumper");

        public static bool IsCustomParser(this Parser parser)
        {
            return parser.GetType().Assembly != Assembly.GetExecutingAssembly();
        }

        public static Parser[] GetParsers()
        {
            return CustomParsers.Keys.Union(Parsers.Keys).ToArray();
        }

        public static bool IsEnabled(Parser parser)
        {
            if (Parsers.ContainsKey(parser))
                return Parsers[parser];
            if (CustomParsers.ContainsKey(parser))
                return CustomParsers[parser];
            return false;
        }

        public static void SetEnabled(bool value, Parser? parser = null)
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

        public static void AddParser(Parser parser)
        {
            CustomParsers.Add(parser, true);
        }

        public static void ClearParsers()
        {
            CustomParsers.Clear();
        }

        /// <summary>
        /// CAUSE CRASH WHEN CALLING:
        /// 	d.techLevel.ToStringHuman().CapitalizeFirst();
        /// ON MAIN MENU FROM ANOTHER THREAD
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        // public static async void UpdateAsync(Action? callback = null)
        // {
        //     Tables.Clear();
        //     try
        //     {
        //         foreach (var parser in Parsers)
        //         {
        //             if (parser.Value)
        //             {
        //                 await Task.Run(() =>
        //                 {
        //                     var table = TableBuilder.Create(parser.Key);
        //                     if (table != null)
        //                     {
        //                         Tables.Add(table);
        //                     }
        //                 });
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Error(ex.ToString());
        //     }
        //     callback?.Invoke();
        // }

        public static void Update(Action? callback = null)
        {
            Tables.Clear();
            try
            {
                var parsers = Parsers.Union(CustomParsers).ToList();
                foreach (var parser in parsers)
                {
                    if (parser.Value)
                    {
                        var table = TableBuilder.Create(parser.Key);
                        if (table != null)
                        {
                            Tables.Add(table);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            callback?.Invoke();
        }

        public static async void ExportAsync(Action? callback = null)
        {
            try
            {
                string fileName = $"{SavePath}\\Dump_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";
                await Task.Run(() => Tables.Export(fileName, XlsxFormat));
                OpenAction(fileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            callback?.Invoke();
        }

        public static void OpenAction(string fileName)
        {
            switch (Settings.SaveAndOpenAction)
            {
                case SaveAndOpenAction.Nothing:
                    break;
                case SaveAndOpenAction.OpenDirectory:
                    Application.OpenURL(SavePath);
                    break;
                case SaveAndOpenAction.OpenDocument:
                    Application.OpenURL(fileName);
                    break;
            }
        }

        // public static void Export(Action? callback = null)
        // {
        //     try
        //     {
        //         Tables.Export(
        //             $"{SavePath}\\Dump_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx",
        //             new XlsxTableFormat(new XlsxTableStyle("TableStyle")));
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Error(ex.ToString());
        //     }

        //     callback?.Invoke();
        // }
    }
}