using System;
using UnityEngine;

namespace Jing.Game.Loading
{
    public interface ILoadingService
    {
        event Action<bool> Action_IsShow;
        void Show();
        void Hide();
    }
}