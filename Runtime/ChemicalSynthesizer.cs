using System;
using Nautilus.Json;
using UnityEngine;

namespace CompositeBuildables
{
    public class ChemicalSynthesizer : HandTarget, IHandTarget
    {
        private const float LubricantTime = 540f;
        private const float BleachTime = 570f;
        private const float BenzeneTime = 600f;
        private const float HydrochloricAcidTime = 630f;

        private ChemicalSynthesizerSaveData _saveData = new ChemicalSynthesizerSaveData();

        public StorageContainer storageContainer;
        public float speedFactor = 1f;

        public int maxLubricant = 2;
        public int maxBleach = 2;
        public int maxBenzene = 2;
        public int maxHydrochloricAcid = 2;

        private float timeRemainingLubricant = -1f;
        private float timeRemainingBleach = -1f;
        private float timeRemainingBenzene = -1f;
        private float timeRemainingHydrochloricAcid = -1f;

        private bool _saveHooked;
        private string _persistentId;

        private static float _chemicalStorageCacheTime = -999f;
        private static ChemicalStorage[] _chemicalStorageCache;

        public ChemicalSynthesizerSaveData SaveData
        {
            get { return _saveData; }
        }

        private void Start()
        {
            string id = GetPersistentId();

            ChemicalSynthesizerSaveData saveData;
            if (Plugin.SaveCache.chemicalSynthesizerSaves.TryGetValue(id, out saveData))
            {
                _saveData = saveData;
            }

            timeRemainingLubricant = SanitizeTimer(_saveData.timeRemainingLubricant);
            timeRemainingBleach = SanitizeTimer(_saveData.timeRemainingBleach);
            timeRemainingBenzene = SanitizeTimer(_saveData.timeRemainingBenzene);
            timeRemainingHydrochloricAcid = SanitizeTimer(_saveData.timeRemainingHydrochloricAcid);

            StartSynthesisIfIdle(ref timeRemainingLubricant, LubricantTime, TechType.Lubricant);
            StartSynthesisIfIdle(ref timeRemainingBleach, BleachTime, TechType.Bleach);
            StartSynthesisIfIdle(ref timeRemainingBenzene, BenzeneTime, TechType.Benzene);
            StartSynthesisIfIdle(ref timeRemainingHydrochloricAcid, HydrochloricAcidTime, TechType.HydrochloricAcid);

            float initialDelay = UnityEngine.Random.Range(0.8f, 1.2f);
            InvokeRepeating(nameof(UpdateSynthesis), initialDelay, 1f);

            Invoke(nameof(TryHookContainer), 0.1f);
        }

        private void TryHookContainer()
        {
            if (storageContainer == null || storageContainer.container == null)
            {
                Invoke(nameof(TryHookContainer), 0.1f);
                return;
            }

            storageContainer.container.onRemoveItem -= RemoveItem;
            storageContainer.container.onRemoveItem += RemoveItem;
            storageContainer.container.isAllowedToAdd = new IsAllowedToAdd(IsAllowedToAdd);
        }

        public void OnEnable()
        {
            if (!_saveHooked)
            {
                Plugin.SaveCache.OnStartedSaving += OnBeforeSave;
                _saveHooked = true;
            }

            if (storageContainer == null)
                return;

            storageContainer.enabled = true;
            TryHookContainer();

            if (storageContainer.container == null)
                return;

            storageContainer.container.onRemoveItem -= RemoveItem;
            storageContainer.container.onRemoveItem += RemoveItem;
            storageContainer.container.isAllowedToAdd = new IsAllowedToAdd(IsAllowedToAdd);
        }

        private void OnDisable()
        {
            if (_saveHooked)
            {
                Plugin.SaveCache.OnStartedSaving -= OnBeforeSave;
                _saveHooked = false;
            }

            if (storageContainer == null)
                return;

            if (storageContainer.container != null)
            {
                storageContainer.container.onRemoveItem -= RemoveItem;
                storageContainer.container.isAllowedToAdd = null;
            }

            storageContainer.enabled = false;
        }

        private void OnDestroy()
        {
            string id = GetPersistentId();
            if (Plugin.SaveCache.chemicalSynthesizerSaves.ContainsKey(id))
                Plugin.SaveCache.chemicalSynthesizerSaves.Remove(id);
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!enabled)
                return;

            Constructable constructable = gameObject.GetComponent<Constructable>();
            if (constructable != null && !constructable.constructed)
                return;

            string progressText;

            if (!IsSynthesisEnabled())
            {
                progressText = "Synthesis disabled";
            }
            else
            {
                float lubricantProgress = GetProgress01(timeRemainingLubricant, LubricantTime);
                float bleachProgress = GetProgress01(timeRemainingBleach, BleachTime);
                float benzeneProgress = GetProgress01(timeRemainingBenzene, BenzeneTime);
                float hydrochloricAcidProgress = GetProgress01(timeRemainingHydrochloricAcid, HydrochloricAcidTime);

                progressText =
                    "Lubricant " + (lubricantProgress * 100f).ToString("0\\%") +
                    ", Bleach " + (bleachProgress * 100f).ToString("0\\%") +
                    ", Benzene " + (benzeneProgress * 100f).ToString("0\\%") +
                    ", Hydrochloric Acid " + (hydrochloricAcidProgress * 100f).ToString("0\\%");
            }

            HandReticle.main.SetText(HandReticle.TextType.Hand, "Use Chemical Shelf", false, GameInput.Button.LeftHand);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Synthesis Progress: " + progressText, false, GameInput.Button.None);
            HandReticle.main.SetIcon(HandReticle.IconType.Interact, 1f);
        }

        public void OnHandClick(GUIHand hand)
        {
            Constructable constructable = gameObject.GetComponent<Constructable>();
            if (constructable != null && !constructable.constructed)
                return;

            if (storageContainer != null)
                storageContainer.Open();
        }

        private void UpdateSynthesis()
        {
            if (DayNightCycle.main == null)
                return;

            if (storageContainer == null || storageContainer.container == null)
                return;

            if (!IsSynthesisEnabled())
                return;

            float delta = 1f * DayNightCycle.main.dayNightSpeed;
            if (delta <= 0f)
                return;

            ChemicalStorage[] storages = GetChemicalStorages();

            UpdateSingleSynthesis(
                ref timeRemainingLubricant,
                delta,
                LubricantTime,
                TechType.Lubricant,
                "96b1b863-2ff7-451b-aa38-8b3a06e72d63",
                1,
                1,
                storages);

            UpdateSingleSynthesis(
                ref timeRemainingBleach,
                delta,
                BleachTime,
                TechType.Bleach,
                "fbfacd7b-32a8-4065-8c25-b0a703f2683b",
                1,
                1,
                storages);

            UpdateSingleSynthesis(
                ref timeRemainingBenzene,
                delta,
                BenzeneTime,
                TechType.Benzene,
                "986b31ea-3c9d-498c-9f38-2af8ffe86ed7",
                1,
                1,
                storages);

            UpdateSingleSynthesis(
                ref timeRemainingHydrochloricAcid,
                delta,
                HydrochloricAcidTime,
                TechType.HydrochloricAcid,
                "74912c22-a383-48c7-8e9e-34b515c6aebb",
                1,
                1,
                storages);
        }

        private void UpdateSingleSynthesis(
            ref float timer,
            float delta,
            float baseTime,
            TechType techType,
            string prefabGuid,
            int width,
            int height,
            ChemicalStorage[] storages)
        {
            bool needsGlobal = NeedsMoreGlobal(techType, storages);
            bool needsLocal = NeedsMoreLocal(techType);

            if (!needsGlobal)
            {
                timer = -1f;
                return;
            }

            bool canStoreLocally = needsLocal && CanStoreOutput(width, height);
            bool canStoreExternally = !needsLocal && CanAnyChemicalStorageAccept(techType, width, height, storages);

            if (!canStoreLocally && !canStoreExternally)
            {
                return;
            }

            if (timer < 0f)
                timer = GetScaledSynthesisTime(baseTime);

            if (timer > 0f)
                timer = Mathf.Max(0f, timer - delta);

            if (timer != 0f)
                return;

            if (needsLocal)
            {
                if (TryAddToOwnContainer(prefabGuid, width, height))
                    timer = -1f;

                return;
            }

            if (TrySendToChemicalStorage(techType, prefabGuid, width, height, storages))
                timer = -1f;
        }

        private void StartSynthesisIfIdle(ref float timer, float baseTime, TechType techType)
        {
            if (timer >= 0f)
                return;

            if (!IsSynthesisEnabled())
                return;

            ChemicalStorage[] storages = GetChemicalStorages();

            if (!NeedsMoreGlobal(techType, storages))
                return;

            if (NeedsMoreLocal(techType))
            {
                if (!CanStoreOutput(1, 1))
                    return;
            }
            else
            {
                if (!CanAnyChemicalStorageAccept(techType, 1, 1, storages))
                    return;
            }

            timer = GetScaledSynthesisTime(baseTime);
        }

        private bool CanStoreOutput(int width, int height)
        {
            if (storageContainer == null || storageContainer.container == null)
                return false;

            return storageContainer.container.HasRoomFor(width, height);
        }

        private bool TryAddToOwnContainer(string prefabGuid, int width, int height)
        {
            if (storageContainer == null || storageContainer.container == null)
                return false;

            if (!storageContainer.container.HasRoomFor(width, height))
                return false;

            Pickupable pickupable = CreatePickupable(prefabGuid);
            if (pickupable == null)
                return false;

            storageContainer.container.UnsafeAdd(new InventoryItem(pickupable));
            return true;
        }

        private bool CanAnyChemicalStorageAccept(TechType techType, int width, int height, ChemicalStorage[] storages)
        {
            if (storages == null || storages.Length == 0)
                return false;

            for (int i = 0; i < storages.Length; i++)
            {
                ChemicalStorage storage = storages[i];
                if (storage == null)
                    continue;

                if (storage.CanAccept(techType, width, height))
                    return true;
            }

            return false;
        }

        private bool TrySendToChemicalStorage(TechType techType, string prefabGuid, int width, int height, ChemicalStorage[] storages)
        {
            if (storages == null || storages.Length == 0)
                return false;

            for (int i = 0; i < storages.Length; i++)
            {
                ChemicalStorage storage = storages[i];
                if (storage == null)
                    continue;

                if (!storage.CanAccept(techType, width, height))
                    continue;

                Pickupable pickupable = CreatePickupable(prefabGuid);
                if (pickupable == null)
                    return false;

                storage.storageContainer.container.AddItem(pickupable);
                return true;
            }

            return false;
        }

        private Pickupable CreatePickupable(string prefabGuid)
        {
            GameObject prefab = PrefabFactory.GetPrefabGameObject(prefabGuid);
            if (prefab == null)
                return null;

            GameObject instance = UnityEngine.Object.Instantiate(prefab);
            Pickupable pickupable = instance.GetComponent<Pickupable>();
            if (pickupable == null)
            {
                UnityEngine.Object.Destroy(instance);
                return null;
            }

            pickupable.Pickup(false);
            return pickupable;
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            return false;
        }

        private bool IsSynthesisEnabled()
        {
            return Plugin.config != null && Plugin.config.ChemicalShelvesSynthesize;
        }

        private float GetScaledSynthesisTime(float baseTime)
        {
            float safeSpeedFactor = Mathf.Max(0.01f, speedFactor);
            return baseTime / safeSpeedFactor;
        }

        private float GetProgress01(float remainingTime, float baseTime)
        {
            if (remainingTime < 0f)
                return 0f;

            if (remainingTime == 0f)
                return 1f;

            float totalTime = GetScaledSynthesisTime(baseTime);
            if (totalTime <= 0f || float.IsNaN(totalTime) || float.IsInfinity(totalTime))
                return 0f;

            float ratio = remainingTime / totalTime;
            if (float.IsNaN(ratio) || float.IsInfinity(ratio))
                return 0f;

            return 1f - Mathf.Clamp01(ratio);
        }

        private void OnBeforeSave(object sender, JsonFileEventArgs e)
        {
            _saveData.timeRemainingLubricant = timeRemainingLubricant;
            _saveData.timeRemainingBleach = timeRemainingBleach;
            _saveData.timeRemainingBenzene = timeRemainingBenzene;
            _saveData.timeRemainingHydrochloricAcid = timeRemainingHydrochloricAcid;

            string id = GetPersistentId();
            Plugin.SaveCache.chemicalSynthesizerSaves[id] = _saveData;
        }

        private void RemoveItem(InventoryItem item)
        {
            StartSynthesisIfIdle(ref timeRemainingLubricant, LubricantTime, TechType.Lubricant);
            StartSynthesisIfIdle(ref timeRemainingBleach, BleachTime, TechType.Bleach);
            StartSynthesisIfIdle(ref timeRemainingBenzene, BenzeneTime, TechType.Benzene);
            StartSynthesisIfIdle(ref timeRemainingHydrochloricAcid, HydrochloricAcidTime, TechType.HydrochloricAcid);
        }

        private float SanitizeTimer(float timer)
        {
            if (float.IsNaN(timer) || float.IsInfinity(timer))
                return -1f;

            return timer;
        }

        private int GetLocalTargetCount(TechType techType)
        {
            if (techType == TechType.Lubricant) return maxLubricant;
            if (techType == TechType.Bleach) return maxBleach;
            if (techType == TechType.Benzene) return maxBenzene;
            if (techType == TechType.HydrochloricAcid) return maxHydrochloricAcid;
            return 0;
        }

        private int GetLocalCount(TechType techType)
        {
            if (storageContainer == null || storageContainer.container == null)
                return 0;

            return storageContainer.container.GetCount(techType);
        }

        private bool NeedsMoreLocal(TechType techType)
        {
            return GetLocalCount(techType) < GetLocalTargetCount(techType);
        }

        private ChemicalStorage[] GetChemicalStorages()
        {
            if (_chemicalStorageCache != null && Time.time - _chemicalStorageCacheTime < 1f)
                return _chemicalStorageCache;

            _chemicalStorageCache = UnityEngine.Object.FindObjectsOfType<ChemicalStorage>();
            _chemicalStorageCacheTime = Time.time;
            return _chemicalStorageCache;
        }

        private int GetGlobalStoredCount(TechType techType, ChemicalStorage[] storages)
        {
            int total = GetLocalCount(techType);

            if (storages == null)
                return total;

            for (int i = 0; i < storages.Length; i++)
            {
                ChemicalStorage storage = storages[i];
                if (storage == null)
                    continue;

                total += storage.GetStoredCount(techType);
            }

            return total;
        }

        private int GetGlobalTargetCount(TechType techType, ChemicalStorage[] storages)
        {
            int total = GetLocalTargetCount(techType);

            if (storages == null)
                return total;

            for (int i = 0; i < storages.Length; i++)
            {
                ChemicalStorage storage = storages[i];
                if (storage == null)
                    continue;

                total += storage.GetMaxCount(techType);
            }

            return total;
        }

        private bool NeedsMoreGlobal(TechType techType, ChemicalStorage[] storages)
        {
            return GetGlobalStoredCount(techType, storages) < GetGlobalTargetCount(techType, storages);
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
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null)
                    continue;

                Type type = component.GetType();
                if (!string.Equals(type.Name, componentTypeName, StringComparison.Ordinal))
                    continue;

                for (int j = 0; j < memberNames.Length; j++)
                {
                    string memberName = memberNames[j];

                    var property = type.GetProperty(memberName);
                    if (property != null && property.PropertyType == typeof(string))
                    {
                        object value = property.GetValue(component, null);
                        return value as string;
                    }

                    var field = type.GetField(memberName);
                    if (field != null && field.FieldType == typeof(string))
                    {
                        object value = field.GetValue(component);
                        return value as string;
                    }
                }
            }

            return null;
        }
    }
}