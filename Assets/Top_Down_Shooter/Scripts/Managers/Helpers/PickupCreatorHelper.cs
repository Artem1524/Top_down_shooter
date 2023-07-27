using UnityEngine;

public class PickupCreatorHelper : MonoBehaviour
{
    public static PickupCreatorHelper Self { get; private set; }

    [SerializeField]
    private Transform _pickupGroupParent;

    private void Awake()
    {
        //if (Self is null) закомментировать
        //{
            Self = this;
        //}
    }

    public static T InstantiatePickup<T>(T prefab, Vector3 position, Quaternion rotation) where T : Pickup
    {
        return Instantiate<T>(prefab, position, rotation, Self._pickupGroupParent);
    }
}
