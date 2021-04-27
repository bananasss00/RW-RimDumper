using Verse;
//using System.Drawing;
//using HarmonyLib;


namespace RimDumper
{
    public enum SaveAndOpenAction
    {
        Nothing,
        OpenDirectory,
        OpenDocument
    }

    public class Settings : ModSettings
    {
        private static bool _settingsChanged = false;

        private static bool colorizeValues = true;
        private static SaveAndOpenAction saveAndOpenAction = SaveAndOpenAction.OpenDocument;

        public static SaveAndOpenAction SaveAndOpenAction
        {
            get => saveAndOpenAction;
            set
            {
                saveAndOpenAction = value;
                _settingsChanged = true;
            }
        }

        public static bool ColorizeValues
        {
            get => colorizeValues;
            set
            {
                colorizeValues = value;
                _settingsChanged = true;
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref saveAndOpenAction, "SaveAndOpenAction", SaveAndOpenAction.OpenDocument);
            Scribe_Values.Look(ref colorizeValues, "ColorizeValues", true);
            _settingsChanged = false;
        }

        public static void WriteSettings()
        {
            if (_settingsChanged)
            {
                RimDumperMod.Instance.WriteSettings();
            }
        }
    }

    // public class HugsSub : ModBase
    // {
    //     public HugsSub()
    //     {
    //     }

    //     public override void DefsLoaded()
    //     {

    //     }
    // }

    public class RimDumperMod : Mod
    {
        public static RimDumperMod Instance { get; private set; } = null!;
        public static string RootDir = string.Empty;

        public RimDumperMod(ModContentPack content) : base(content)
        {
            Instance = this;
            GetSettings<Settings>();
            RootDir = content.RootDir;
        }
    }

    // [StaticConstructorOnStartup]
    // public static class Start
    // {
    //     static Start()
    //     {
    //         Log.Message("Mod template loaded successfully!");
    //     }
    // }

}
