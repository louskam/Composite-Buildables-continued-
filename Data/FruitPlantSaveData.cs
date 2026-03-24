using System;
using System.Collections.Generic;

namespace CompositeBuildables
{
    [Serializable]
    public class FruitPlantSaveData
    {
        public int version = 1;

        public float timeLastFruit = -1f;

        public List<bool> pickedStates = new List<bool>();
    }
}
