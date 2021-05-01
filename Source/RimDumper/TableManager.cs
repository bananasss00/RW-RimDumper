//#define DEBUG

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using HarmonyLib;
using AutoTable;
using AutoTable.Xlsx;
using OfficeOpenXml.Table;
using RimDumper.Parsers;
using UnityEngine;
using Verse;


namespace RimDumper
{
    public static class TableManager
    {
        public static readonly TableCollection Tables = new();
        public static readonly XlsxTableFormat XlsxFormat = new(new XlsxTableStyleExtended("TableStyle"));
        public static readonly string SavePath = GenFilePaths.FolderUnderSaveData("RimDumper");

        public static void Update(Action? callback = null)
        {
            Tables.Clear();
            try
            {
                var parsers = ParserStorage.Enabled();
                foreach (var parser in parsers)
                {
                    var table = TableBuilder.Create(parser);
                    if (table != null)
                    {
                        Tables.Add(table);
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
    }
}