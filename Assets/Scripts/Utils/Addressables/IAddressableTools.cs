using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Jing.Tools
{
    public interface IAddressableTools
    {
        UniTask<T> LoadAsset<T>(string key) where T : Object;
        public UniTask<IList<T>> LoadAssets<T>(string layer) where T : Object;
        void Release(string key);
        void ReleaseAll();
    }
}