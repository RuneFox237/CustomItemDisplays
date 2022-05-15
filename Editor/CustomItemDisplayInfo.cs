using System;
using UnityEngine;
using RoRSkinBuilder.Data;
using System.Collections.Generic;
using RuneFoxMods.RoRSkinBuilderExtension;

namespace RuneFoxMods.CustomItemDisplays
{

  [AddComponentMenu("RoR Skins/Custom Item Display")]
  public class CustomItemDisplayInfo : ExtensionBase
  {
    ////////////////////////////////////////////////
    /// Inherited Override
    private static readonly string _name = "CustomItemDisplay";
    public override string Name { get { return _name; } }
    ///
    ////////////////////////////////////////////////
    public SkinModInfo modInfo;
    public AssetsInfo assetInfo;

    //[SerializeField]
    //ItemDisplay test;
    //Mod Displays

    public List<CustomSkin> Skins = new List<CustomSkin>();

    [Serializable]
    public class CustomSkin
    {
      public SkinDefinition skinDef;
      public List<CustomItemDisplay> BaseItemDisplays;
      
      //Not able to use this yet since we don't have a way of setting up multiple dependencies per skin
      //public List<CustomModDisplay> ModItemDisplays = new List<CustomModDisplay>();
    }

    [Serializable]
    public class CustomItemDisplay
    {
      public string ItemName;
      public List<ItemDisplay> DisplayList = new List<ItemDisplay>();
    }

    [Serializable]
    public class CustomModDisplay
    {
      public string ModDependency;
      public List<CustomItemDisplay> DisplayList = new List<CustomItemDisplay>();
    }

    [Serializable]
    public class ItemDisplay
    {
      public string childName;
      public Vector3 localPos;
      public Vector3 localAngle;
      public Vector3 localScale;

      //used by the property drawer
      public string import; 
    }

    public CustomItemDisplayInfo(CustomItemDisplayInfo info)
    {
      modInfo = info.modInfo;
      InitializeAssetInfo();
    }

    public void InitializeAssetInfo()
    {
      assetInfo = new AssetsInfo(modInfo);
    }

  }

}
