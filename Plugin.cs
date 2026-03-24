using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace CompositeBuildables
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.louskam.CompositeBuildables";
        public const string PLUGIN_NAME = "CompositeBuildables";
        public const string PLUGIN_VERSION = "1.0.0";

        internal static ManualLogSource Log { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        internal static SaveCache SaveCache { get; } =
            SaveDataHandler.RegisterSaveDataCache<SaveCache>();

        public static Config config;

        private void Awake()
        {
            Log = base.Logger;

            config = OptionsPanelHandler.RegisterModOptions<Config>();

            RecipeJsonHelper.Initialize();

            InitializeBuildables();

            Harmony.CreateAndPatchAll(Assembly, PLUGIN_GUID);

            Log.LogInfo($"{PLUGIN_NAME} {PLUGIN_VERSION} loaded");
        }

        private void InitializeBuildables()
        {
            DegasiPlanter.Register();
            DegasiPlanter2.Register();
            DegasiPlanterRound.Register();

            ScienceBench1.Register();
            ScienceBench2.Register();

            LabShelving.Register();
            TubularShelfSmall.Register();

            MushroomTerrariumSmall.Register();
            MushroomTerrariumLarge.Register();

            FlowerTerrariumSmall.Register();
            //ACU.Register();
        }
    }
}