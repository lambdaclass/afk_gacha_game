using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
	public class DeadlineAvatar : TopDownMonoBehaviour
	{
		public Character NaomiPrefab;
		public Character JulesPrefab;
		public Sprite NaomiAvatar;
		public Sprite JulesAvatar;
		
		protected virtual void Start()
		{
			if (GameManager.Instance.StoredCharacter.name == JulesPrefab.name)
			{
				this.gameObject.GetComponent<Image>().sprite = JulesAvatar;
			}
			else
			{
				this.gameObject.GetComponent<Image>().sprite = NaomiAvatar;
			}
		}
	}	
}

