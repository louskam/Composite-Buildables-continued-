using System;

namespace CompositeBuildables
{
    [Serializable]
    public class MushroomGrowerSaveData
    {
        public int version = 1;

        public float timeRemainingPink = -1f;

        public float timeRemainingRattler = -1f;

        public float timeRemainingJaffa = -1f;
    }
}
