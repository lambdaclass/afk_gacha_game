using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicators : MonoBehaviour
{
	Dictionary<string, int> statusCount = new Dictionary<string, int>();

	[SerializeField]
	Dictionary<string, Image> icons = new Dictionary<string, Image>();

	[SerializeField]
	GameObject statusIconPrefab;

	public void SetStatus(Status status) {
		if (!statusCount.ContainsKey(status.name))
		{
			Image icon = Instantiate(statusIconPrefab, transform).GetComponent<Image>();
			icon.sprite = status.sprite;
			icons.Add(status.name, icon);
			statusCount.Add(status.name, 1);
		}
		else
		{
			statusCount[status.name]++;
		}
	}

	public void RemoveStatus(Status status)
	{
		if (statusCount.ContainsKey(status.name))
		{
			statusCount[status.name]--;
			if (statusCount[status.name] == 0)
			{
				Image icon = icons[status.name];
				Destroy(icon.gameObject);
				icons.Remove(status.name);
				statusCount.Remove(status.name);
			}
		}
	}
}
