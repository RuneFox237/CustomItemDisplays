using UnityEngine;
using System.Collections.Generic;
using System;
using RoR2;
using BepInEx.Logging;


namespace RuneFoxMods.CustomItemDisplays
{
  public class CustomItemDisplayManager
  {
    public ManualLogSource InstanceLogger;

    internal Dictionary<string, Dictionary<string, ItemDisplayRule[]>> CustomDisplays = new Dictionary<string, Dictionary<string, ItemDisplayRule[]>>(); //Storage for CustomDisplays, Should only add to this in Init func

    Dictionary<BodyIndex, ItemDisplayRuleSet> BaseItemDisplayRuleSets = new Dictionary<BodyIndex, ItemDisplayRuleSet>();
    Dictionary<SkinDef, ItemDisplayRuleSet> CustomItemDisplayRuleSets = new Dictionary<SkinDef, ItemDisplayRuleSet>();

    internal void SkinDefApply(Action<SkinDef, GameObject> orig, SkinDef self, GameObject modelObject)
    {
      orig(self, modelObject);

      var characterModel = modelObject.GetComponent<CharacterModel>();

      //catch for survivor manequins not having a body assigned
      if (characterModel.body as object == null)
        return;

      //This should only fire as false on the first time selecting a skin on a specific body
      //which should be before any modification of IDRS
      if (BaseItemDisplayRuleSets.TryGetValue(characterModel.body.bodyIndex, out ItemDisplayRuleSet BaseIDRS) == false)
      {
        BaseIDRS = characterModel.itemDisplayRuleSet;
        BaseItemDisplayRuleSets.Add(characterModel.body.bodyIndex, BaseIDRS);
      }
      
      // reset IDRS to base
      characterModel.itemDisplayRuleSet = BaseIDRS;
      
      try
      {
        if (CustomDisplays.ContainsKey(self.nameToken))
        {
          //on first time loading skin with custom displays
          if (CustomItemDisplayRuleSets.TryGetValue(self, out ItemDisplayRuleSet customIDRS) == false)
          {
            customIDRS = UnityEngine.Object.Instantiate(BaseIDRS);
            CustomItemDisplayRuleSets.Add(self, customIDRS);
            PopulateIDRS(characterModel, self, customIDRS);
          }
          characterModel.itemDisplayRuleSet = customIDRS;
        }
      }
      catch (Exception e)
      {
        //error logging may need to be skin specific
        InstanceLogger.LogWarning("An error occured while generating Custom Item Displays for a custom skin");
        InstanceLogger.LogError(e);
      }
    }

    void PopulateIDRS(CharacterModel characterModel, SkinDef self, ItemDisplayRuleSet customIDRS)
    {
      //if it's one of our skins and it has Custom Displays
      if (CustomDisplays.TryGetValue(self.nameToken, out var customDisplays) == false)
        return;
          
      //loop through our custom item displays for this skin
      foreach (var display in customDisplays)
      {
        var IDRG = customIDRS.GetItemDisplayRuleGroup(ItemCatalog.FindItemIndex(display.Key));
        if (IDRG.rules != null)
        {
          IDRG.rules = ReplaceRules(IDRG.rules, display.Value);
          customIDRS.SetDisplayRuleGroup(ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(display.Key)), IDRG);
        }
      }

      customIDRS.GenerateRuntimeValues();
    }

    internal ItemDisplayRule[] ReplaceRules(ItemDisplayRule[] oldRules, ItemDisplayRule[] newRules)
    {
      //Assumption is that either there is only one prefab that is used for multiple rules
      //or that if there are multiple prefabs they are in the same order on both rule sets

      ItemDisplayRule[] modifiedRules = new ItemDisplayRule[newRules.Length];

      for (int i = 0; i < newRules.Length; i++)
      {
        modifiedRules[i].childName = newRules[i].childName == null ? oldRules[i % oldRules.Length].childName : newRules[i].childName;
        modifiedRules[i].followerPrefab = newRules[i].followerPrefab == null ? oldRules[i % oldRules.Length].followerPrefab : newRules[i].followerPrefab;
        modifiedRules[i].localPos = newRules[i].localPos == null ? oldRules[i % oldRules.Length].localPos : newRules[i].localPos;
        modifiedRules[i].localAngles = newRules[i].localAngles == null ? oldRules[i % oldRules.Length].localAngles : newRules[i].localAngles;
        modifiedRules[i].localScale = newRules[i].localScale == null ? oldRules[i % oldRules.Length].localScale : newRules[i].localScale;

        //modifiedRules[i].limbMask = newRules[i % oldRules.Length].limbMask == null?oldRules[i % oldRules.Length].limbMask : newRules[1 % newRules.Length].limbMask;
        modifiedRules[i].limbMask = oldRules[i % oldRules.Length].limbMask; //going with old rules atm since I don't have a way to set it yet

        //modifiedRules[i].ruleType = newRules[i%oldRules.Length].ruleType;
        modifiedRules[i].ruleType = oldRules[i % oldRules.Length].ruleType; //going with old rules atm since I don't have a way to set it yet
      }

      return modifiedRules;
    }

    void PrintDRG(DisplayRuleGroup drg)
    {
      foreach (var rule in drg.rules)
      {
        Debug.Log("childName: " + rule.childName);
        Debug.Log("followerPrefab: " + rule.followerPrefab);
        Debug.Log("limbMask: " + rule.limbMask);
        Debug.Log("localPos: " + rule.localPos);
        Debug.Log("localAngles: " + rule.localAngles);
        Debug.Log("localScale: " + rule.localScale);
        Debug.Log("ruleType: " + rule.ruleType);
      }
    }
  }

}