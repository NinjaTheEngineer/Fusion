using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NinjaTools {
    public class NinjaMonoBehaviour : MonoBehaviour {
        public void logd(string id, string message, bool ignoreDuplicates = false, float delay = 0, GameObject highlightGO = null) {
            Utils.logd(id, message, ignoreDuplicates, delay, highlightGO);
        }

        public void logw(string id, string message, bool ignoreDuplicates = false, float delay = 0, GameObject highlightGO = null) {
            Utils.logw(id, message, ignoreDuplicates, delay, highlightGO);
        }

        public void loge(string id = null, string message = null, bool ignoreDuplicates = false, float delay = 0, GameObject highlightGO = null) {
            Utils.loge(id, message, ignoreDuplicates, delay, highlightGO);
        }
        public void logt(string id = null, string message = null, bool ignoreDuplicates = false, float delay = 0, GameObject highlightGO = null) {
            return;
        }
    }
}