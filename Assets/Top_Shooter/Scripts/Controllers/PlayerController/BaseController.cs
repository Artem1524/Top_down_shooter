using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 20f)]
    protected float _speed = 5f;

    abstract protected void Move(Vector3 direction);
}
