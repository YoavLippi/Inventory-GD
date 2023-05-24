using System;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public struct ItemCounter
    {
        [SerializeField] private item item;
        [SerializeField] private int count;

        public item Item
        {
            get => item;
            set => item = value;
        }

        public int Count
        {
            get => count;
            set => count = value;
        }
    }
}