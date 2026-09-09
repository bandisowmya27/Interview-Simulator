using System.Collections.Generic;
using UnityEngine;

namespace Jing.UI
{
    public interface IUIManager
    {
        void ShowPage(string pageName, bool hideCurrent = true);

        void ClosePage();

        void OpenPopup(string popupName);

        void ClosePopup();

        void CloseAll();
    }
}