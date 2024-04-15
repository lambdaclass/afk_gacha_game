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

    List<(string casterUnitId, string targetUnitId, LineRenderer lineRendererComponent)> lineRenderersList = new List<(string casterUnitId, string targetUnitId, LineRenderer lineRendererComponent)>();

	int start, hit, dissapear = 0;

	void Start() {
		foreach(LineRenderer lineRenderer in initialLineRenderers) {
			lineRenderersList.Add((null, null, lineRenderer));
		}
	}

	public void StartProjectile(BattleUnit casterUnit, BattleUnit targetUnit, Color porjectileColor) {
		int availableIndex = lineRenderersList.FindIndex(linerenderer => linerenderer.casterUnitId == null && linerenderer.targetUnitId == null);
        (string casterUnitId, string targetUnitId, LineRenderer lineRendererComponent) lineRenderer;

        if(availableIndex != -1) {
			lineRenderer = lineRenderersList.Find(linerenderer => linerenderer.casterUnitId == null && linerenderer.targetUnitId == null);
        } else {
			GameObject lineRendererGO = Instantiate(lineRendererPrefab, transform);
			LineRenderer newLineRenderer = lineRendererGO.GetComponent<LineRenderer>();
			lineRenderersList.Add((null, null, newLineRenderer));
			lineRenderer = lineRenderersList[lineRenderersList.Count - 1];
			availableIndex = lineRenderersList.Count - 1;
		}
        
        lineRenderer.lineRendererComponent.SetPosition(0, new Vector3(casterUnit.transform.localPosition.x, casterUnit.transform.localPosition.y, -100));
        lineRenderer.lineRendererComponent.SetPosition(1, new Vector3(targetUnit.transform.localPosition.x, targetUnit.transform.localPosition.y, -100));
		lineRenderer.lineRendererComponent.startColor = porjectileColor;
		lineRenderer.lineRendererComponent.endColor = porjectileColor;
        lineRenderer.lineRendererComponent.gameObject.SetActive(true);
        lineRenderersList[availableIndex] = (casterUnit.SelectedUnit.id, targetUnit.SelectedUnit.id, lineRenderer.lineRendererComponent);

		start++;
		Debug.LogWarning($"start {start}");
	}

	public void ProjectileHit(BattleUnit casterUnit, BattleUnit targetUnit) {
		int availableIndex = lineRenderersList.FindIndex(linerenderer => linerenderer.casterUnitId == casterUnit.SelectedUnit.id && linerenderer.targetUnitId == targetUnit.SelectedUnit.id);
		
		if(availableIndex != -1) {
			var lineRenderer = lineRenderersList[availableIndex];
			lineRenderer.lineRendererComponent.startColor = Color.white;
			lineRenderer.lineRendererComponent.endColor = Color.white;
			StartCoroutine(DisappearAfterDelay(availableIndex, 0.4f));
		} else {
			Debug.LogError("Couldn't find line renderer for caster unit ID: " + casterUnit.SelectedUnit.id);
		}

		hit++;
		Debug.LogWarning($"hit {hit}");
	}

    IEnumerator DisappearAfterDelay(int projectileIndex, float delay) {
        yield return new WaitForSeconds(delay);
        lineRenderersList[projectileIndex].lineRendererComponent.gameObject.SetActive(false);
        lineRenderersList[projectileIndex] = (null, null, lineRenderersList[projectileIndex].lineRendererComponent);

		dissapear++;
		Debug.LogWarning($"dissapear {dissapear}");
    }
}
