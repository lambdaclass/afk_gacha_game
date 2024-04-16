using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectilesPooler : MonoBehaviour
{
    [SerializeField]
	LineRenderer[] initialLineRenderers;

	[SerializeField]
	GameObject lineRendererPrefab;

    List<(bool isBeingUsed, LineRenderer lineRendererComponent)> lineRenderersList = new List<(bool isBeingUsed, LineRenderer lineRendererComponent)>();

	void Start() {
		foreach(LineRenderer lineRenderer in initialLineRenderers) {
			lineRenderersList.Add((false, lineRenderer));
		}
	}

    public void ShootProjectile(Transform startTransform, Transform targetTransform, Color porjectileColor) {
		int availableIndex = lineRenderersList.FindIndex(l => !l.isBeingUsed);
        (bool isBeingUsed, LineRenderer lineRendererComponent) lineRenderer;

        if(availableIndex != -1) {
			lineRenderer = lineRenderersList.Find(l => !l.isBeingUsed);
        } else {
			GameObject lineRendererGO = Instantiate(lineRendererPrefab, transform);
			LineRenderer newLineRenderer = lineRendererGO.GetComponent<LineRenderer>();
			lineRenderersList.Add((false, newLineRenderer));
			lineRenderer = lineRenderersList[lineRenderersList.Count - 1];
			availableIndex = lineRenderersList.Count - 1;
		}
        
        lineRenderer.lineRendererComponent.SetPosition(0, new Vector3(startTransform.localPosition.x, startTransform.localPosition.y, -100));
        lineRenderer.lineRendererComponent.SetPosition(1, new Vector3(targetTransform.localPosition.x, targetTransform.localPosition.y, -100));
		lineRenderer.lineRendererComponent.startColor = porjectileColor;
		lineRenderer.lineRendererComponent.endColor = porjectileColor;
        lineRenderer.lineRendererComponent.gameObject.SetActive(true);
        lineRenderersList[availableIndex] = (true, lineRenderer.lineRendererComponent);

        StartCoroutine(DisappearAfterDelay(availableIndex, 0.3f));
    }

    IEnumerator DisappearAfterDelay(int projectileIndex, float delay) {
        yield return new WaitForSeconds(delay);
        lineRenderersList[projectileIndex].lineRendererComponent.gameObject.SetActive(false);
        lineRenderersList[projectileIndex] = (false, lineRenderersList[projectileIndex].lineRendererComponent);
    }
}
