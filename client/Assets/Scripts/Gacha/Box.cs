using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterDropRate
{
    public Character character;
    public int weight;
}

[CreateAssetMenu(fileName = "New Box", menuName = "Box")]
[System.Serializable]
public class Box : ScriptableObject
{
    public string description;
    public List<CharacterDropRate> characterDropRates;

    public CurrencyValue[] individualCosts;

    private Dictionary<Currency, int> cost = new Dictionary<Currency, int>();

    private void Awake() {
        foreach (CurrencyValue individualCost in individualCosts) {
            cost.Add(individualCost.name, individualCost.value);
        }
    }

    public Dictionary<Currency, int> GetCost() { return cost; }

    public Character RollChampion()
    {
        List<CharacterDropRate> sortedDropRates = characterDropRates;
        sortedDropRates.Sort((x, y) => y.weight.CompareTo(x.weight));

        int totalWeight = 0;
        foreach (var dropRate in sortedDropRates)
        {
            totalWeight += dropRate.weight;
        }

        int randomValue = UnityEngine.Random.Range(1, totalWeight + 1);

        Character character = null;
        foreach (var dropRate in sortedDropRates)
        {
            randomValue -= dropRate.weight;
            if (randomValue <= 0)
            {
                character = dropRate.character;
                break;
            }
        }

        return character;
    }
}
