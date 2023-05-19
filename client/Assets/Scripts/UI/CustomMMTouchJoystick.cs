using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomMMTouchJoystick : MoreMountains.Tools.MMTouchJoystick
{
    public UnityEvent<Vector2> AoeAttackOnPointerUpEvent;
    GenericAoeAttack attack;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        attack = new GenericAoeAttack();
    }
    public override void OnPointerUp(PointerEventData data)
    {
        AoeAttackOnPointerUpEvent.Invoke(RawValue);
        //print("last value: " + RawValue);
        attack.ExecuteAoeAttack(RawValue);
        ResetJoystick();
    }
}
