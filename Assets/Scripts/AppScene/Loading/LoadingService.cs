using System;
using UnityEngine;

namespace Jing.Game.Loading
{
    public class LoadingService : ILoadingService
    {
        public event Action<bool> Action_IsShow;
        public void Show()
        {
            Action_IsShow?.Invoke(true);
        }

        public void Hide()
        {
            Action_IsShow?.Invoke(false);
        }
    }
}
