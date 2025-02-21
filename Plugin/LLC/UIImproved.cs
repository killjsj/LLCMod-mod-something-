using BattleUI;
using BattleUI.Typo;
using HarmonyLib;
using MainUI;
using TMPro;
using UnityEngine.UI;

namespace LimbusLocalize.LLC;

public static class UIImproved
{
    [HarmonyPatch(typeof(ParryingTypoUI), nameof(ParryingTypoUI.SetParryingTypoData))]
    [HarmonyPrefix]
    private static void ParryingTypoUI_SetParryingTypoData(ParryingTypoUI __instance)
    {
        __instance.img_parryingTypo.sprite = ReadmeManager.ReadmeSprites["LLC_Combo"];
    }

    [HarmonyPatch(typeof(ActBossBattleStartUI), "Init")]
    [HarmonyPostfix]
    private static void BossBattleStartInit(ActBossBattleStartUI __instance)
    {
        var child = __instance.transform.GetChild(2).GetChild(1);
        var tmp = child.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        var img = child.GetChild(1).GetComponentInChildren<Image>();

        if (!tmp.text.Equals("Proelium Fatale")) return;

        img.sprite = ReadmeManager.ReadmeSprites["LLC_BossBattle"];
        tmp.m_fontAsset = ChineseFont.Tmpchinesefonts[1];
        tmp.text = "<b>命定之战</b>";

        tmp = child.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = "凡跨入此门之人，当放弃一切希望";
    }

    [HarmonyPatch(typeof(StageChapterAreaSlot), "Init")]
    [HarmonyPostfix]
    private static void AreaSlotInit(StageChapterAreaSlot __instance)
    {
        var tmp = __instance.tmpro_area;

        if (!tmp.text.StartsWith("DISTRICT ")) return;

        tmp.text = tmp.text.Replace("DISTRICT ", "") + "<size=25>区";
    }

    [HarmonyPatch(typeof(FormationPersonalityUI_Label), "Reload")]
    [HarmonyPostfix]
    private static void PersonalityUILabel(FormationPersonalityUI_Label __instance)
    {
        FormationPersonalityUI_LabelTypes status = __instance._model._status;
        if (status.Equals(1))
        {
            if (status.Equals(2))
            {
                __instance.tmp_text.text = "<size=45>已更改";
                return;
            }
        }
        else
        {
            __instance.img_label.sprite = ReadmeManager.ReadmeSprites["LLC_Selected"];
        }
    }
}