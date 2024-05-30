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

    void Start()
    {
        foreach (LineRenderer lineRenderer in initialLineRenderers)
        {
            lineRenderersList.Add((null, null, lineRenderer));
        }
    }

    public void TriggerProjectile(BattleUnit casterUnit, BattleUnit targetUnit, Color projectileColor)
    {

        int availableIndex = lineRenderersList.FindIndex(linerenderer => linerenderer.casterUnitId == null && linerenderer.targetUnitId == null);
        (string casterUnitId, string targetUnitId, LineRenderer lineRendererComponent) lineRenderer;

        if (availableIndex != -1)
        {
            lineRenderer = lineRenderersList.Find(linerenderer => linerenderer.casterUnitId == null && linerenderer.targetUnitId == null);
        }
        else
        {
            GameObject lineRendererGO = Instantiate(lineRendererPrefab, transform);
            LineRenderer newLineRenderer = lineRendererGO.GetComponent<LineRenderer>();
            lineRenderersList.Add((null, null, newLineRenderer));
            lineRenderer = lineRenderersList[lineRenderersList.Count - 1];
            availableIndex = lineRenderersList.Count - 1;
        }

        projectileColor.a = .4f;
        lineRenderer.lineRendererComponent.startColor = projectileColor;
        lineRenderer.lineRendererComponent.endColor = projectileColor;
        lineRenderer.lineRendererComponent.SetPosition(0, new Vector3(casterUnit.transform.localPosition.x, casterUnit.transform.localPosition.y, -100));
        lineRenderer.lineRendererComponent.SetPosition(1, new Vector3(targetUnit.transform.localPosition.x, targetUnit.transform.localPosition.y, -100));
        lineRenderer.lineRendererComponent.gameObject.SetActive(true);
        lineRenderersList[availableIndex] = (casterUnit.SelectedUnit.id, targetUnit.SelectedUnit.id, lineRenderer.lineRendererComponent);
    }

    public void ProjectileHit(BattleUnit casterUnit, BattleUnit targetUnit, Color projectileColor)
    {

        int availableIndex = lineRenderersList.FindIndex(linerenderer => linerenderer.casterUnitId == casterUnit.SelectedUnit.id && linerenderer.targetUnitId == targetUnit.SelectedUnit.id);

        if (availableIndex != -1)
        {
            var lineRenderer = lineRenderersList[availableIndex];
            projectileColor.a = 1;
            lineRenderer.lineRendererComponent.startColor = projectileColor;
            lineRenderer.lineRendererComponent.endColor = projectileColor;
            StartCoroutine(DisappearAfterDelay(casterUnit.SelectedUnit.id, targetUnit.SelectedUnit.id, 0.2f));
        }
        else
        {
            Debug.LogError("Couldn't find line renderer for caster units: " + casterUnit.SelectedUnit.id + " and " + targetUnit.SelectedUnit.id);
        }
    }

    IEnumerator DisappearAfterDelay(string casterId, string targetId, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (lineRenderersList.Any(lr => lr.casterUnitId == casterId && lr.targetUnitId == targetId))
        {
            int availableIndex = lineRenderersList.FindIndex(linerenderer => linerenderer.casterUnitId == casterId && linerenderer.targetUnitId == targetId);
            lineRenderersList[availableIndex].lineRendererComponent.gameObject.SetActive(false);
            lineRenderersList[availableIndex] = (null, null, lineRenderersList[availableIndex].lineRendererComponent);
        }
    }

    public void ClearProjectiles()
    {
        foreach ((string casterUnitId, string targetUnitId, LineRenderer lineRendererComponent) linerenderer in lineRenderersList)
        {
            linerenderer.lineRendererComponent.gameObject.SetActive(false);
        }

        lineRenderersList = new List<(string casterUnitId, string targetUnitId, LineRenderer lineRendererComponent)>();
    }
}
