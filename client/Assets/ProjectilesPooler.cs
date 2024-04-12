using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesPooler : MonoBehaviour
{
    [SerializeField]
	LineRenderer[] lineRenderersPool;

	public void ShootProjectile(Transform startTransform, Transform targetTransform) {
		Vector3 targetInsidePosition = targetTransform.position;
        Vector3 startObjectPosition = startTransform.position;
        Vector3 relativePosition = startTransform.InverseTransformPoint(targetInsidePosition);

		lineRenderersPool[0].SetPosition(0, startObjectPosition);
		lineRenderersPool[0].SetPosition(1, relativePosition);
		lineRenderersPool[0].gameObject.SetActive(true);
	}
}
