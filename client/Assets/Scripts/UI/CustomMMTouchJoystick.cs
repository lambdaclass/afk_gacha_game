using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomMMTouchJoystick : MoreMountains.Tools.MMTouchJoystick
{
    public UnityEvent<Vector2> AoeAttackOnPointerUpEvent;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
    public override void OnPointerUp(PointerEventData data)
    {
        AoeAttackOnPointerUpEvent.Invoke(RawValue);
        print("last value: " + RawValue);
        ResetJoystick();
    }
}
