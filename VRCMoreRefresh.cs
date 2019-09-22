using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC.UI;
using VRC.Core;

using VRCModLoader;

using UnityEngine;

using VRCMenuUtils;
using VRChat.UI;

namespace VRCMoreRefresh
{
    [VRCModInfo("VRCMoreRefresh", "0.1.0", "AtiLion")]
    internal class VRCMoreRefresh : VRCMod
    {
        void OnApplicationStart()
        {
            ModManager.StartCoroutine(Setup());
        }

        #region VRCMoreRefresh Coroutines
        private IEnumerator Setup()
        {
            // Wait for load
            yield return VRCMenuUtilsAPI.WaitForInit();

            // Run UI loaders
            LoadSocialScreen();
            LoadUserInfoScreen();
        }
        #endregion

        #region UI Loaders
        private void LoadSocialScreen()
        {
            // Check
            if (VRCEUi.SocialScreen == null)
                return;

            // Get VRChat UI elements
            Transform currentStatus = VRCEUi.SocialScreen.transform.Find("Current Status");
            Transform btnStatus = currentStatus.Find("StatusButton");
            RectTransform rt_btnStatus = btnStatus.GetComponent<RectTransform>();
            Transform icnStatus = currentStatus.Find("StatusIcon");
            RectTransform rt_icnStatus = icnStatus.GetComponent<RectTransform>();
            Transform txtStatus = currentStatus.Find("StatusText");
            RectTransform rt_txtStatus = txtStatus.GetComponent<RectTransform>();

            // Setup refresh button
            VRCEUiButton btnRefresh = new VRCEUiButton("refresh", new Vector2(rt_btnStatus.localPosition.x - 20f, rt_btnStatus.localPosition.y), "Refresh", currentStatus);
            RectTransform rt_btnRefresh = btnRefresh.Control.GetComponent<RectTransform>();

            // Fix UI positions
            rt_btnStatus.localPosition += new Vector3(210f, 0f, 0f);
            rt_icnStatus.localPosition += new Vector3(210f, 0f, 0f);
            rt_txtStatus.localPosition += new Vector3(210f, 0f, 0f);
            rt_btnRefresh.sizeDelta -= new Vector2(5f, 10f);

            // Add click check
            btnRefresh.OnClick += () =>
            {
                UiUserList[] userLists = VRCEUi.SocialScreen.GetComponentsInChildren<UiUserList>(true);

                foreach (UiUserList userList in userLists)
                {
                    userList.ClearAll();
                    userList.Refresh();
                    userList.RefreshData();
                }
                VRCModLogger.Log("Refreshed social lists!");
            };
        }
        private void LoadUserInfoScreen()
        {
            // Refresh Button
            VRCEUiButton btnRefresh = new VRCEUiButton("refresh", new Vector2(0f, 0f), "Refresh");
            btnRefresh.OnClick += () =>
            {
                if (string.IsNullOrEmpty(PageUserInfo.userIdOfLastUserPageInfoViewed))
                    return;
                string id = PageUserInfo.userIdOfLastUserPageInfoViewed;

                ApiCache.Invalidate<APIUser>(id);
                APIUser.FetchUser(id, user =>
                {
                    PageUserInfo pageUserInfo = VRCEUi.UserInfoScreen.GetComponent<PageUserInfo>();

                    if (pageUserInfo != null)
                        pageUserInfo.SetupUserInfo(user);
                },
                error =>
                    VRCModLogger.LogError($"Failed to fetch user of id {id}: {error}"));
            };
            VRCMenuUtilsAPI.AddUserInfoButton(btnRefresh);
        }
        #endregion
    }
}
