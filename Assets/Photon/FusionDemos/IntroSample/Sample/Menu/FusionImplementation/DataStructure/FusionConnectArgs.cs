using System;
using System.Linq;
using Photon.Menu;
using UnityEngine;

namespace Fusion.Menu {
  public class FusionConnectArgs : PhotonMenuConnectArgs
  {
    public virtual FusionGameMode GameMode
    {
      get
      {
        if (Enum.TryParse<FusionGameMode>(PlayerPrefs.GetString("Fusion.Menu.GameMode"), out var result))
        {
          return result;
        }
        return FusionGameMode.AuthoritativeServer;
      }

      set => PlayerPrefs.SetString("Fusion.Menu.GameMode", Enum.GetName(typeof(FusionGameMode), value));
    }

    public virtual string UnityScene
    {
      get => PlayerPrefs.GetString("Fusion.Menu.UnityScene");
      set => PlayerPrefs.SetString("Fusion.Menu.UnityScene", value);
    }

    public override void SetDefaults(IPhotonMenuConfig config) { 
      base.SetDefaults(config);

      if (string.IsNullOrEmpty(UnityScene) || config.AvailableScenes.Contains(UnityScene) == false)
      {
        UnityScene = config.AvailableScenes.FirstOrDefault();
      }
    }
  }
}
