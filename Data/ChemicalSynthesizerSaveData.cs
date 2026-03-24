using System;

namespace CompositeBuildables
{
    [Serializable]
    public class ChemicalSynthesizerSaveData
    {
        public float timeRemainingLubricant = -1f;
        public float timeRemainingBleach = -1f;
        public float timeRemainingBenzene = -1f;
        public float timeRemainingHydrochloricAcid = -1f;
    }
}