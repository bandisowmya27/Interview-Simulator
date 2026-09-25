using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Jing.Tools
{
    public class AddressableTools : IAddressableTools
    {
        private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();

        public async UniTask<T> LoadAsset<T>(string key) where T : UnityEngine.Object
        {
            if (_handles.TryGetValue(key, out var existingHandle))
            {
                if (existingHandle.Status == AsyncOperationStatus.Failed)
                {
                    _handles.Remove(key);
                }
                else
                {
                    await existingHandle.ToUniTask();
                    return existingHandle.Result as T;
                }
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            _handles[key] = handle;

            try
            {
                var result = await handle.ToUniTask();
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AddressableTools] Loadiing Error. Key is {key}。Error Message: {ex.Message}");

                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
                _handles.Remove(key);

                return null;
            }
        }

        public async UniTask<IList<T>> LoadAssets<T>(string layer) where T : UnityEngine.Object
        {
            string handleKey = $"{typeof(T).FullName}:{layer}";

            if (_handles.TryGetValue(handleKey, out var existingHandle))
            {
                if (existingHandle.Status == AsyncOperationStatus.Failed)
                {
                    if (existingHandle.IsValid())
                    {
                        Addressables.Release(existingHandle);
                    }
                    _handles.Remove(handleKey);
                }
                else
                {
                    try
                    {
                        await existingHandle.ToUniTask();
                        return existingHandle.Result as IList<T>;
                    }
                    catch
                    {
                        if (existingHandle.IsValid())
                        {
                            Addressables.Release(existingHandle);
                        }
                        _handles.Remove(handleKey);
                        throw;
                    }
                }
            }

            var locations = await Addressables.LoadResourceLocationsAsync(layer, typeof(T)).ToUniTask();
            if (locations == null || locations.Count == 0)
            {
                Debug.LogWarning($"[AddressableTools] No assets found for layer: {layer}");
                return new List<T>();
            }

            var handle = Addressables.LoadAssetsAsync<T>(
                layer,
                _ => { }
            );

            _handles[handleKey] = handle;

            try
            {
                return await handle.ToUniTask();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AddressableTools] Batch loading failed. Layer = {layer}, Error Message: {ex.Message}");

                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }

                _handles.Remove(handleKey);
                return null;
            }
        }

        public void Release(string key)
        {
            if (_handles.TryGetValue(key, out var handle))
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
                _handles.Remove(key);
            }
        }

        public void ReleaseAll()
        {
            foreach (var handle in _handles.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _handles.Clear();
        }

    }
}