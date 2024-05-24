using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableInstantiator : MonoBehaviour
{
    [SerializeField] AssetReferenceGameObject _mainSupercampaign;

    public async Task<GameObject> InstantiateMainSupercampaign()
    {
        AsyncOperationHandle<GameObject> handle = _mainSupercampaign.InstantiateAsync(Vector3.zero, Quaternion.identity);
        await handle.Task;
        return handle.Result;
    }
}
