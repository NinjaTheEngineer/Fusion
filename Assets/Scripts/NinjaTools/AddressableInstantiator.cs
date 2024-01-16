using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
namespace NinjaTools {
    [System.Serializable]
    public class AssetReferenceAudioClip : AssetReferenceT<AudioClip> {
        public AssetReferenceAudioClip(string guid) : base(guid) { }
    }
    public class AddressableInstantiator : NinjaMonoBehaviour {
        [SerializeField] AssetReferenceGameObject assetGameObject; 
        GameObject _instanceReference;
        public GameObject InstanceReference => _instanceReference;
        private void Update() {
            //assetGameObject.InstantiateAsync();
            
            if(Input.GetKeyDown(KeyCode.I)) {
                assetGameObject.LoadAssetAsync().Completed += OnAddressableLoaded;
            } else if(Input.GetKeyDown(KeyCode.J)) {
                assetGameObject.ReleaseInstance(_instanceReference);
            }
        }
        Vector3 instantiatePosition;
        public void InstantiateAssetReference(Vector3 position = default) {
            instantiatePosition = position;
            if(_instanceReference) {
                Instantiate(_instanceReference, instantiatePosition, Quaternion.identity);
                return;
            }
            assetGameObject.LoadAssetAsync().Completed += OnAddressableLoaded;
        }
        private void OnAddressableLoaded(AsyncOperationHandle<GameObject> handle) {
            var logId = "OnAddressableLoaded";
            if(handle.Status==AsyncOperationStatus.Succeeded) {
                _instanceReference = handle.Result;
                Instantiate(_instanceReference, instantiatePosition, Quaternion.identity);
            } else {
                loge(logId, "Loading Asset Failed");
            }
        }
    }
}
*/