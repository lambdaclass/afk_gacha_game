using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicators : MonoBehaviour
{
	[SerializeField]
	List<Status> statuses;

	[SerializeField]
	Image icon;

	public void SetStatus(string statusName) {
		icon.sprite = statuses.Single(status => status.name == statusName).sprite;
	}
}
