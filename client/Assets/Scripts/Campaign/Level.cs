using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    List<Character> characters;
    
    [SerializeField]
    List<int> levels;

    public void SetUnits() {
        OpponentData opponentData = OpponentData.Instance;
        List<Unit> units = new List<Unit>();
        for(int i = 0; i < characters.Count; i++) {
            units.Add(new Unit { id = "op-" + i.ToString(), level = levels[i], character = characters[i], slot = i, selected = true });
        }
        opponentData.Units = units;
    }
}
