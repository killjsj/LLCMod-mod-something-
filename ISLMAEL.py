# -*- coding: UTF-8 -*-
oldtext = """private static void BossBattleStartInit(ActBossBattleStartUI __instance)
    {
        if (!IsUseChinese.Value)
            return;
        var textGroup = __instance.transform.GetChild(2).GetChild(1);
        var tmp = textGroup.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        if (!tmp.text.Equals("Proelium Fatale"))
            return;
        tmp.font = ChineseFont.Tmpchinesefonts[0];
        tmp.text = "<b>命定之战</b>";
        tmp = textGroup.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
        tmp.font = ChineseFont.Tmpchinesefonts[0];
        tmp.text = "凡跨入此门之人，当放弃一切希望";
    }"""
newtext = """private static void BossBattleStartInit(ActBossBattleStartUI __instance)
    {
        if (!IsUseChinese.Value)
            return;
        List<string> _loadingTexts;
        List<string> _loadingTextsTitles;
        _loadingTexts = [.. File.ReadAllLines(LLCMod.ModPath + "/Localize/Readme/BossBattleStartInitTexts.md")];
        _loadingTextsTitles = [.. File.ReadAllLines(LLCMod.ModPath + "/Localize/Readme/BossBattleStartInitTextsTitles.md")];
        var textGroup = __instance.transform.GetChild(2).GetChild(1);
        var tmp = textGroup.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        if (_loadingTexts.Count == 0|| _loadingTextsTitles.Count == 0){
            LLCMod.LogWarning("nothing in BossBattleStartInitTextsTitles.md or BossBattleStartInitTextsTitles.md,using default.");
            return;
        }
        if (!tmp.text.Equals("Proelium Fatale"))
            return;
        
        if (_loadingTexts.Count != _loadingTextsTitles.Count){ //不等于就随机
            tmp.font = ChineseFont.Tmpchinesefonts[0];
            tmp.text = "<b>"+SelectOne(_loadingTextsTitles)+"</b>";
            tmp = textGroup.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            tmp.font = ChineseFont.Tmpchinesefonts[0];
            tmp.text = SelectOne(_loadingTexts);
        } else {
            int i = UnityEngine.Random.RandomRangeInt(0,_loadingTexts.Count);
            tmp.font = ChineseFont.Tmpchinesefonts[0];
            tmp.text = "<b>"+SelectOne(_loadingTextsTitles,i)+"</b>";
            tmp = textGroup.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            tmp.font = ChineseFont.Tmpchinesefonts[0];
            tmp.text = SelectOne(_loadingTexts,i);
        }
    }
    public static T SelectOne<T>(List<T> list,int i = -1){
        if (i != -1) return list[i]; else {
            UnityEngine.Random.seed = (int)(Time.deltaTime+Time.realtimeSinceStartup + DateTime.Today.Millisecond);
            return list.Count == 0 ? default : list[UnityEngine.Random.Range(0, list.Count)];
            }
        }"""
oldusingtext = """using System;
using BattleUI.Dialog;
using BattleUI.Typo;
using BepInEx.Configuration;
using HarmonyLib;
using LocalSave;
using MainUI;
using StorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voice;
using Object = UnityEngine.Object;"""
newusingtext = """using System;
using BattleUI.Dialog;
using BattleUI.Typo;
using BepInEx.Configuration;
using HarmonyLib;
using LocalSave;
using MainUI;
using StorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voice;
using Object = UnityEngine.Object;
//new add
using System.IO;
using System.Collections.Generic;
"""
texts = []
text = ''
filePath = "./src/LLC/ChineseSetting.cs"
with open(filePath,"r+",encoding='utf-8') as file:
    texts = file.readlines()
    text = ''
    for n in texts:
        text += n
    text = text.replace(oldtext,newtext)
    text = text.replace(oldusingtext,newusingtext)
with open(filePath,"w",encoding='utf-8') as file:
    file.write(text)