using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableInstantiator : MonoBehaviour
{
    [SerializeField] AssetReferenceGameObject _mainSupercampaign;
    [SerializeField] AssetReferenceGameObject _dungeonSupercampaign;

    public async Task<GameObject> InstantiateMainSupercampaign()
    {
        AsyncOperationHandle<GameObject> handle = _mainSupercampaign.InstantiateAsync(Vector3.zero, Quaternion.identity);
        await handle.Task;
        return handle.Result;
    }

    public async Task<GameObject> InstantiateDungeonSupercampaign()
    {
        AsyncOperationHandle<GameObject> handle = _dungeonSupercampaign.InstantiateAsync(Vector3.zero, Quaternion.identity);
        await handle.Task;
        return handle.Result;
    }
}
