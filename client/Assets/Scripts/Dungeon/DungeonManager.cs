using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    void Start()
    {
        ClaimRewards();
    }

    private void ClaimRewards()
    {
        SocketConnection.Instance.ClaimDungeonAfkRewards(GlobalUserData.Instance.User.id, (userReceived) =>
        {
            GlobalUserData userToUpdate = GlobalUserData.Instance;
            Dictionary<string, int> currenciesToAdd = new Dictionary<string, int>();

            userReceived.currencies.Select(c => c.Key).ToList().ForEach(c =>
            {
                if (!currenciesToAdd.ContainsKey(c))
                {
                    currenciesToAdd.Add(c, userReceived.currencies[c] - userToUpdate.GetCurrency(c).Value);
                }
            });
            userToUpdate.AddCurrencies(currenciesToAdd);
        });
    }
}










