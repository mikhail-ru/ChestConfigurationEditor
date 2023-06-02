using System;
using UnityEngine;

namespace SomeGame.Configuration
{
    [CreateAssetMenu(menuName = "Inventory/Chest")]
    public class TreeChestConfig : ScriptableObject
    {
        public enum RewardType
        {
            Soft,
            Hard,
            Chest,
        }

        [Serializable]
        public class RewardInfo
        {
            [Range(0, 1)] public float randomWeight = 1;
            public RewardType type;
            public int softMin;
            public int softMax;
            public float hardMin;
            public float hardMax;
            public TreeChestConfig chest;
            public int chestCount;
        }

        [SerializeField] private RewardInfo[] _rewards;
        public RewardInfo[] Rewards => _rewards;
    }
}
