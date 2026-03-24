using System;
using System.Collections.Generic;
using Nautilus.Json;
using Nautilus.Json.Attributes;

namespace CompositeBuildables
{
    [FileName("CompositeBuildablesSaveData")]
    internal class SaveCache : SaveDataCache
    {
        public Dictionary<string, MushroomGrowerSaveData> mushroomGrowerSaves =
            new Dictionary<string, MushroomGrowerSaveData>();

        public Dictionary<string, FruitPlantSaveData> fruitPlantSaves =
            new Dictionary<string, FruitPlantSaveData>();

        public Dictionary<string, ChemicalSynthesizerSaveData> chemicalSynthesizerSaves = 
            new Dictionary<string, ChemicalSynthesizerSaveData>();

        public SaveCache()
        {
            // pokud chceš debug logy, dej to pod config flag
            // OnFinishedLoading += OnFinishedLoading_DebugDump;

            OnStartedSaving += RefreshSaves;
        }

        private void RefreshSaves(object sender, JsonFileEventArgs e)
        {
            mushroomGrowerSaves.Clear();
            fruitPlantSaves.Clear();
        }

        // Volitelné – jen pro debug
        /*
        private void OnFinishedLoading_DebugDump(object sender, JsonFileEventArgs e)
        {
            foreach (var kv in mushroomGrowerSaves)
                Plugin.Log.LogMessage($"mushroom key {kv.Key}, value {kv.Value}");

            foreach (var kv in fruitPlantSaves)
                Plugin.Log.LogMessage($"fruit key {kv.Key}, value {kv.Value}");
        }
        */
    }
}