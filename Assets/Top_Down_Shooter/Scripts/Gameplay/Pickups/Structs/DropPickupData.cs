using OneLine;

using System;
using UnityEngine;

[Serializable]
public struct DropPickupData
{
    [Width(50), Range(0f, 1f)]
    public float DropRate;

    public Pickup DropPickup;
}
