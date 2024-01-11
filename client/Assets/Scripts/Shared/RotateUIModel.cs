using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateUIModel : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField]
    float rotationSpeed = 1f;

    [SerializeField]
    GameObject modelContainer;
    Transform model;
    Touch touch;
    Quaternion rotationX;
    Coroutine coroutine;

    void Start()
    {
        StartCoroutine(GetModel());
    }

    public IEnumerator GetModel()
    {
        yield return new WaitUntil(() => modelContainer.transform.childCount > 0);
        model = modelContainer.transform.GetChild(0).transform;
    }

    public void OnDrag(PointerEventData eventData)
    {
        touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Moved)
        {
            rotationX = Quaternion.Euler(0f, -touch.deltaPosition.x * rotationSpeed, 0f);
            model.rotation *= rotationX;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}
