using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : BaseController
{
    protected override void Move(Vector3 direction)
    {
        transform.position += direction * _speed;
        // throw new System.NotImplementedException();
    }
}
