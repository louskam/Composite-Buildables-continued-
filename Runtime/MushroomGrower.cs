using System;
using System.Collections.Generic;
using Nautilus.Json;
using UnityEngine;

namespace CompositeBuildables
{
    public class MushroomGrower : HandTarget, IHandTarget
    {
        private MushroomGrowerSaveData _saveData = new MushroomGrowerSaveData();

        public StorageContainer storageContainer;
        public float speedFactor = 1f;

        private const float PinkTime = 336f;
        private const float RattlerTime = 448f;
        private const float JaffaTime = 672f;

        private float timeRemainingPink = -1f;
        private float timeRemainingRattler = -1f;
        private float timeRemainingJaffa = -1f;

        private bool _saveHooked;
        private string _persistentId;

        private float _lastNonZeroProductionFactor = 0.25f;
        private bool _productionWasPaused;

        private static float _bioReactorCacheTime = -999f;
        private static SubRoot _bioReactorCacheSubRoot;
        private static BaseBioReactor[] _bioReactorCache;

        public MushroomGrowerSaveData SaveData
        {
            get { return _saveData; }
        }

        private void Start()
        {
            string id = GetPersistentId();

            MushroomGrowerSaveData saveData;
            if (Plugin.SaveCache.mushroomGrowerSaves.TryGetValue(id, out saveData))
            {
                _saveData = saveData;
            }

            timeRemainingPink = SanitizeTimer(_saveData.timeRemainingPink);
            timeRemainingRattler = SanitizeTimer(_saveData.timeRemainingRattler);
            timeRemainingJaffa = SanitizeTimer(_saveData.timeRemainingJaffa);

            float currentFactor = Plugin.config != null ? Plugin.config.MushroomProductionSpeedFactor : 0.25f;
            if (currentFactor > 0f)
                _lastNonZeroProductionFactor = currentFactor;

            StartGrowthIfIdle(ref timeRemainingPink, PinkTime);
            StartGrowthIfIdle(ref timeRemainingRattler, RattlerTime);
            StartGrowthIfIdle(ref timeRemainingJaffa, JaffaTime);

            float initialDelay = UnityEngine.Random.Range(0.8f, 1.2f);
            InvokeRepeating(nameof(UpdateGrowth), initialDelay, 1f);

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
            if (Plugin.SaveCache.mushroomGrowerSaves.ContainsKey(id))
                Plugin.SaveCache.mushroomGrowerSaves.Remove(id);
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!enabled)
                return;

            Constructable constructable = gameObject.GetComponent<Constructable>();
            if (constructable != null && !constructable.constructed)
                return;

            string progressText;

            if (IsProductionPaused())
            {
                progressText = "Production paused";
            }
            else
            {
                progressText = "Stores Filled";

                if (timeRemainingPink >= 0f || timeRemainingRattler >= 0f || timeRemainingJaffa >= 0f)
                {
                    float pinkProgress = GetProgress01(timeRemainingPink, PinkTime);
                    float rattlerProgress = GetProgress01(timeRemainingRattler, RattlerTime);
                    float jaffaProgress = GetProgress01(timeRemainingJaffa, JaffaTime);

                    progressText =
                        "Pink Cap " + (pinkProgress * 100f).ToString("0\\%") +
                        ", Speckled Rattler " + (rattlerProgress * 100f).ToString("0\\%") +
                        ", Jaffa Cup " + (jaffaProgress * 100f).ToString("0\\%");
                }
            }

            HandReticle.main.SetText(HandReticle.TextType.Hand, "Use Mushroom Terrarium", false, GameInput.Button.LeftHand);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Production Progress: " + progressText, false, GameInput.Button.None);
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

        public void UpdateGrowth()
        {
            if (DayNightCycle.main == null)
                return;

            if (storageContainer == null || storageContainer.container == null)
                return;

            HandleProductionFactorChange();

            if (IsProductionPaused())
                return;

            float delta = 1f * DayNightCycle.main.dayNightSpeed;
            if (delta <= 0f)
                return;

            BaseBioReactor[] reactors = null;
            if (Plugin.config != null && Plugin.config.MushroomsToReactors)
                reactors = GetBioReactorsInCurrentSubRoot();

            UpdateSingleGrowth(
                ref timeRemainingPink,
                delta,
                PinkTime,
                "7f9a765d-0b4e-4b3f-81b9-38b38beedf55",
                1,
                1,
                reactors);

            UpdateSingleGrowth(
                ref timeRemainingRattler,
                delta,
                RattlerTime,
                "28818d8a-5e50-41f0-8e14-44cb89a0b611",
                1,
                1,
                reactors);

            UpdateSingleGrowth(
                ref timeRemainingJaffa,
                delta,
                JaffaTime,
                "ff727b98-8d85-416a-9ee7-4beda86d2ba2",
                2,
                2,
                reactors);
        }

        private void HandleProductionFactorChange()
        {
            float currentFactor = Plugin.config != null ? Plugin.config.MushroomProductionSpeedFactor : 0.25f;

            if (currentFactor <= 0f)
            {
                _productionWasPaused = true;
                return;
            }

            if (_productionWasPaused)
            {
                RescaleActiveTimers(_lastNonZeroProductionFactor, currentFactor);
                _productionWasPaused = false;
                _lastNonZeroProductionFactor = currentFactor;
                return;
            }

            if (!Mathf.Approximately(currentFactor, _lastNonZeroProductionFactor))
            {
                RescaleActiveTimers(_lastNonZeroProductionFactor, currentFactor);
                _lastNonZeroProductionFactor = currentFactor;
            }
        }

        private void RescaleActiveTimers(float oldProductionFactor, float newProductionFactor)
        {
            if (oldProductionFactor <= 0f || newProductionFactor <= 0f)
                return;

            RescaleSingleTimer(ref timeRemainingPink, PinkTime, oldProductionFactor, newProductionFactor);
            RescaleSingleTimer(ref timeRemainingRattler, RattlerTime, oldProductionFactor, newProductionFactor);
            RescaleSingleTimer(ref timeRemainingJaffa, JaffaTime, oldProductionFactor, newProductionFactor);
        }

        private void RescaleSingleTimer(ref float timer, float baseGrowTime, float oldProductionFactor, float newProductionFactor)
        {
            if (timer <= 0f)
                return;

            float oldTotal = GetScaledGrowTime(baseGrowTime, oldProductionFactor);
            float newTotal = GetScaledGrowTime(baseGrowTime, newProductionFactor);

            if (oldTotal <= 0f || newTotal <= 0f)
                return;

            float progress = 1f - Mathf.Clamp01(timer / oldTotal);
            timer = Mathf.Max(0f, newTotal * (1f - progress));
        }

        private void UpdateSingleGrowth(
    ref float timer,
    float delta,
    float baseGrowTime,
    string prefabGuid,
    int width,
    int height,
    BaseBioReactor[] reactors)
        {
            bool canStoreLocally = CanStoreOutput(width, height);
            bool canStoreInReactor = Plugin.config != null &&
                                     Plugin.config.MushroomsToReactors &&
                                     CanAnyBioReactorAccept(width, height, reactors);

            if (!canStoreLocally && !canStoreInReactor)
            {
                return;
            }

            if (timer < 0f)
                timer = GetScaledGrowTime(baseGrowTime);

            if (timer > 0f)
                timer = Mathf.Max(0f, timer - delta);

            if (timer != 0f)
                return;

            if (TryAddToOwnContainer(prefabGuid, width, height))
            {
                timer = -1f;
                return;
            }

            if (Plugin.config != null &&
                Plugin.config.MushroomsToReactors &&
                TrySendToBioReactor(prefabGuid, width, height, reactors))
            {
                timer = -1f;
            }
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

        private BaseBioReactor[] GetBioReactorsInCurrentSubRoot()
        {
            SubRoot sub = GetComponentInParent<SubRoot>();
            if (sub == null)
                return null;

            if (_bioReactorCache != null &&
                _bioReactorCacheSubRoot == sub &&
                Time.time - _bioReactorCacheTime < 1f)
            {
                return _bioReactorCache;
            }

            _bioReactorCache = sub.gameObject.GetComponentsInChildren<BaseBioReactor>(true);
            _bioReactorCacheSubRoot = sub;
            _bioReactorCacheTime = Time.time;

            return _bioReactorCache;
        }

        private bool TrySendToBioReactor(string prefabGuid, int width, int height, BaseBioReactor[] reactors)
        {
            if (reactors == null || reactors.Length == 0)
                return false;

            List<StorageContainer> candidates = new List<StorageContainer>();

            for (int i = 0; i < reactors.Length; i++)
            {
                BaseBioReactor reactor = reactors[i];
                if (reactor == null)
                    continue;

                StorageContainer reactorStorage = reactor.GetComponentInChildren<StorageContainer>(true);
                if (reactorStorage == null || reactorStorage.container == null)
                    continue;

                if (reactorStorage.container.HasRoomFor(width, height))
                    candidates.Add(reactorStorage);
            }

            if (candidates.Count == 0)
                return false;

            int idx = UnityEngine.Random.Range(0, candidates.Count);
            StorageContainer selectedStorage = candidates[idx];

            Pickupable pickupable = CreatePickupable(prefabGuid);
            if (pickupable == null)
                return false;

            selectedStorage.container.AddItem(pickupable);
            return true;
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

        private void StartGrowthIfIdle(ref float timer, float baseGrowTime)
        {
            if (timer >= 0f)
                return;

            if (IsProductionPaused())
                return;

            timer = GetScaledGrowTime(baseGrowTime);
        }

        private bool IsProductionPaused()
        {
            return Plugin.config == null || Plugin.config.MushroomProductionSpeedFactor <= 0f;
        }

        private float GetScaledGrowTime(float baseGrowTime)
        {
            float productionFactor = Plugin.config != null ? Plugin.config.MushroomProductionSpeedFactor : 0.25f;
            return GetScaledGrowTime(baseGrowTime, productionFactor);
        }

        private float GetScaledGrowTime(float baseGrowTime, float productionFactor)
        {
            float safeSpeedFactor = Mathf.Max(0.01f, speedFactor);

            if (productionFactor <= 0f)
                return float.PositiveInfinity;

            return baseGrowTime / productionFactor / safeSpeedFactor;
        }

        private float GetProgress01(float remainingTime, float baseGrowTime)
        {
            if (remainingTime < 0f)
                return 1f;

            if (remainingTime == 0f)
                return 1f;

            float factorForUi = Plugin.config != null ? Plugin.config.MushroomProductionSpeedFactor : _lastNonZeroProductionFactor;
            if (factorForUi <= 0f)
                factorForUi = Mathf.Max(0.01f, _lastNonZeroProductionFactor);

            float totalTime = GetScaledGrowTime(baseGrowTime, factorForUi);

            if (totalTime <= 0f || float.IsNaN(totalTime) || float.IsInfinity(totalTime))
                return 0f;

            float ratio = remainingTime / totalTime;
            if (float.IsNaN(ratio) || float.IsInfinity(ratio))
                return 0f;

            return 1f - Mathf.Clamp01(ratio);
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            if (storageContainer == null || storageContainer.container == null || pickupable == null)
                return false;

            TechType techType = pickupable.GetTechType();

            if (techType == TechType.PinkMushroom)
                return storageContainer.container.HasRoomFor(1, 1);

            if (techType == TechType.PurpleRattle)
                return storageContainer.container.HasRoomFor(1, 1);

            if (techType == TechType.OrangeMushroomSpore)
                return storageContainer.container.HasRoomFor(2, 2);

            return false;
        }

        private void OnBeforeSave(object sender, JsonFileEventArgs e)
        {
            _saveData.timeRemainingPink = timeRemainingPink;
            _saveData.timeRemainingRattler = timeRemainingRattler;
            _saveData.timeRemainingJaffa = timeRemainingJaffa;

            string id = GetPersistentId();
            Plugin.SaveCache.mushroomGrowerSaves[id] = _saveData;
        }

        private void RemoveItem(InventoryItem item)
        {
            StartGrowthIfIdle(ref timeRemainingPink, PinkTime);
            StartGrowthIfIdle(ref timeRemainingRattler, RattlerTime);
            StartGrowthIfIdle(ref timeRemainingJaffa, JaffaTime);
        }

        private float SanitizeTimer(float timer)
        {
            if (float.IsNaN(timer) || float.IsInfinity(timer))
                return -1f;

            return timer;
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
        private bool CanStoreOutput(int width, int height)
        {
            if (storageContainer == null || storageContainer.container == null)
                return false;

            return storageContainer.container.HasRoomFor(width, height);
        }

        private bool CanAnyBioReactorAccept(int width, int height, BaseBioReactor[] reactors)
        {
            if (reactors == null || reactors.Length == 0)
                return false;

            for (int i = 0; i < reactors.Length; i++)
            {
                BaseBioReactor reactor = reactors[i];
                if (reactor == null)
                    continue;

                StorageContainer reactorStorage = reactor.GetComponentInChildren<StorageContainer>(true);
                if (reactorStorage == null || reactorStorage.container == null)
                    continue;

                if (reactorStorage.container.HasRoomFor(width, height))
                    return true;
            }

            return false;
        }
    }
}