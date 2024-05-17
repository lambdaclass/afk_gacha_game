using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonAnimations : Selectable
{
    [Header("Animation Scale values")]
    Vector3 initialScale;

    [Header("Final scale of the object to animate.")]
    Vector3 finalScale;

    [Header("Animation duration")]
    float duration = 0.25f;

    [Serializable]
    /// <summary>
    /// Function definition for a button click event.
    /// </summary>
    public class ButtonClickedEvent : UnityEvent { }

    // Event delegates triggered on click.
    [FormerlySerializedAs("onClick")]
    [SerializeField]
    public ButtonClickedEvent clickEvent = new ButtonClickedEvent();

    //Min difference of the touchStartPosition and the current touch
    private const float MIN_DIFFERENCE = 6.0f;
    private Vector2 touchStartPosition;
    private bool isInsideCard = false;
    public bool executeRelease = false;

    protected override void Start()
    {
        base.Start();
        initialScale = transform.localScale;
        finalScale = initialScale - new Vector3(0.05f, 0.05f, 0.05f);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        transform.DOScale(finalScale, duration).SetEase(Ease.OutQuad);
        touchStartPosition = eventData.position;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        transform.DOScale(initialScale, duration);
        CheckReleasePosition(eventData);
    }

    public void CheckReleasePosition(PointerEventData eventData)
    {
        var touchXDifference = Math.Abs(eventData.position.x - touchStartPosition.x);
        var touchYDifference = Math.Abs(eventData.position.y - touchStartPosition.y);
        if (isInsideCard && touchXDifference < MIN_DIFFERENCE && touchYDifference < MIN_DIFFERENCE)
        {
            clickEvent.Invoke();
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        isInsideCard = false;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        isInsideCard = true;
    }
}