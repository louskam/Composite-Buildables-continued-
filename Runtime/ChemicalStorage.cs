using System;
using UnityEngine;

namespace CompositeBuildables
{
    public class ChemicalStorage : HandTarget, IHandTarget
    {
        public StorageContainer storageContainer;

        public int maxLubricant = 4;
        public int maxBleach = 4;
        public int maxBenzene = 4;
        public int maxHydrochloricAcid = 4;

        private string _persistentId;

        private void Start()
        {
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

        private void OnEnable()
        {
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
            if (storageContainer == null)
                return;

            if (storageContainer.container != null)
            {
                storageContainer.container.onRemoveItem -= RemoveItem;
                storageContainer.container.isAllowedToAdd = null;
            }

            storageContainer.enabled = false;
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!enabled)
                return;

            Constructable constructable = gameObject.GetComponent<Constructable>();
            if (constructable != null && !constructable.constructed)
                return;

            HandReticle.main.SetText(HandReticle.TextType.Hand, "Use Chemical Storage", false, GameInput.Button.LeftHand);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Stores synthesized chemicals", false, GameInput.Button.None);
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

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            if (pickupable == null)
                return false;

            return CanAccept(pickupable.GetTechType(), 1, 1);
        }

        private void RemoveItem(InventoryItem item)
        {
            // úmyslně prázdné
        }

        public int GetMaxCount(TechType techType)
        {
            if (techType == TechType.Lubricant) return maxLubricant;
            if (techType == TechType.Bleach) return maxBleach;
            if (techType == TechType.Benzene) return maxBenzene;
            if (techType == TechType.HydrochloricAcid) return maxHydrochloricAcid;
            return 0;
        }

        public int GetStoredCount(TechType techType)
        {
            if (storageContainer == null || storageContainer.container == null)
                return 0;

            return storageContainer.container.GetCount(techType);
        }

        public bool CanAccept(TechType techType, int width, int height)
        {
            if (storageContainer == null || storageContainer.container == null)
                return false;

            if (techType != TechType.Lubricant &&
                techType != TechType.Bleach &&
                techType != TechType.Benzene &&
                techType != TechType.HydrochloricAcid)
                return false;

            if (storageContainer.container.GetCount(techType) >= GetMaxCount(techType))
                return false;

            return storageContainer.container.HasRoomFor(width, height);
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