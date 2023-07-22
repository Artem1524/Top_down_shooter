using OneLine;

using System.Collections.Generic;
using UnityEngine;

public class DropPickups : MonoBehaviour
{
    [SerializeField]
    [OneLineWithHeader(Header = LineHeader.None)]
    private List<DropPickupData> _dropPickups;

    public List<DropPickupData> GetDropPickups() => _dropPickups;
}
