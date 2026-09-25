using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Jing.Data;
using UnityEngine;

namespace Jing.UI
{
    public interface IUIManager
    {
        UniTask ShowPage(string pageName);
        UniTask ShowPageAndHideCurrent(string pageName);
        UniTask ShowPageAndReleaseCurrent(string pageName);
        void ClosePage();

        T OpenPopup<T>(string popupName) where T : UIBaseView;
        void ClosePopup();

        void CloseAll();
    }
}