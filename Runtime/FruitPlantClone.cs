using System;
using System.Collections.Generic;
using Nautilus.Json;
using UnityEngine;

namespace CompositeBuildables
{
    public class FruitPlantClone : MonoBehaviour
    {
        private FruitPlantSaveData _saveData = new FruitPlantSaveData();

        public PickPrefab[] fruits;

        private const int currentVersion = 1;
        private const float defaultTimeLastFruit = -1f;

        public int version = currentVersion;
        public float timeLastFruit = defaultTimeLastFruit;

        private readonly List<PickPrefab> inactiveFruits = new List<PickPrefab>();
        private bool initialized;
        private bool _saveHooked;
        private string _persistentId;

        public FruitPlantSaveData SaveData
        {
            get { return _saveData; }
        }

        private void Start()
        {
            string id = GetPersistentId();

            FruitPlantSaveData saveData;
            if (!Plugin.SaveCache.fruitPlantSaves.TryGetValue(id, out saveData))
            {
                if (!Plugin.config.LanternTreesSpawnFruited)
                {
                    for (int i = 0; i < fruits.Length; i++)
                    {
                        fruits[i].SetPickedState(true);
                        inactiveFruits.Add(fruits[i]);
                    }

                    if (DayNightCycle.main != null)
                        timeLastFruit = DayNightCycle.main.timePassedAsFloat;
                }

                _saveData = new FruitPlantSaveData();
                // Plugin.Log.LogDebug("FruitPlantClone " + id + " was placed");
            }
            else
            {
                _saveData = saveData;

                for (int i = 0; i < fruits.Length; i++)
                {
                    bool picked = i < _saveData.pickedStates.Count && _saveData.pickedStates[i];
                    fruits[i].SetPickedState(picked);

                    if (picked)
                        inactiveFruits.Add(fruits[i]);
                }

                timeLastFruit = _saveData.timeLastFruit;
                // Plugin.Log.LogDebug("FruitPlantClone " + id + " was loaded");
            }

            Initialize();
        }

        private void OnEnable()
        {
            if (!_saveHooked)
            {
                Plugin.SaveCache.OnStartedSaving += OnBeforeSave;
                _saveHooked = true;
            }
        }

        private void OnDisable()
        {
            if (_saveHooked)
            {
                Plugin.SaveCache.OnStartedSaving -= OnBeforeSave;
                _saveHooked = false;
            }
        }

        private void OnDestroy()
        {
            string id = GetPersistentId();
            if (Plugin.SaveCache.fruitPlantSaves.ContainsKey(id))
                Plugin.SaveCache.fruitPlantSaves.Remove(id);
        }

        private void Initialize()
        {
            if (initialized)
                return;

            inactiveFruits.Clear();

            if (fruits != null)
            {
                for (int i = 0; i < fruits.Length; i++)
                {
                    if (fruits[i] == null)
                        continue;

                    fruits[i].pickedEvent.AddHandler(this, OnFruitHarvest);

                    if (fruits[i].GetPickedState())
                        inactiveFruits.Add(fruits[i]);
                }
            }

            initialized = true;
        }

        private void Update()
        {
            if (DayNightCycle.main == null)
                return;

            while (inactiveFruits.Count != 0 &&
                   DayNightCycle.main.timePassed >= timeLastFruit + Plugin.config.LanternFruitSpawnTime)
            {
                PickPrefab random = inactiveFruits.GetRandom<PickPrefab>();
                random.SetPickedState(false);
                inactiveFruits.Remove(random);
                timeLastFruit += Plugin.config.LanternFruitSpawnTime;
            }
        }

        private void OnFruitHarvest(PickPrefab fruit)
        {
            if (inactiveFruits.Count == 0 && DayNightCycle.main != null)
            {
                timeLastFruit = DayNightCycle.main.timePassedAsFloat;
            }

            if (!inactiveFruits.Contains(fruit))
                inactiveFruits.Add(fruit);
        }

        private void OnBeforeSave(object sender, JsonFileEventArgs e)
        {
            _saveData.pickedStates = new List<bool>();

            if (fruits != null)
            {
                for (int i = 0; i < fruits.Length; i++)
                {
                    bool picked = fruits[i] != null && fruits[i].pickedState;
                    _saveData.pickedStates.Add(picked);
                }
            }

            _saveData.timeLastFruit = timeLastFruit;

            string id = GetPersistentId();
            Plugin.SaveCache.fruitPlantSaves[id] = _saveData;
        }

        private string GetPersistentId()
        {
            if (!string.IsNullOrEmpty(_persistentId))
                return _persistentId;

            _persistentId = TryGetIdFromComponentByName(gameObject, "PrefabIdentifier", "Id", "id");
            if (!string.IsNullOrEmpty(_persistentId))
                return _persistentId;

            _persistentId = TryGetIdFromComponentByName(gameObject, "UniqueIdentifier", "Id", "id");
            if (!string.IsNullOrEmpty(_persistentId))
                return _persistentId;

            _persistentId = gameObject.GetInstanceID().ToString();
            return _persistentId;
        }

        private static string TryGetIdFromComponentByName(GameObject go, string componentTypeName, params string[] memberNames)
        {
            Component[] comps = go.GetComponents<Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                Component c = comps[i];
                if (c == null)
                    continue;

                Type t = c.GetType();
                if (!string.Equals(t.Name, componentTypeName, StringComparison.Ordinal))
                    continue;

                for (int m = 0; m < memberNames.Length; m++)
                {
                    string name = memberNames[m];

                    var prop = t.GetProperty(name);
                    if (prop != null && prop.PropertyType == typeof(string))
                    {
                        object v = prop.GetValue(c, null);
                        return v as string;
                    }

                    var field = t.GetField(name);
                    if (field != null && field.FieldType == typeof(string))
                    {
                        object v = field.GetValue(c);
                        return v as string;
                    }
                }
            }

            return null;
        }
    }
}