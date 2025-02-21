using BattleUI;
using BattleUI.Typo;
using HarmonyLib;
using MainUI;
using TMPro;
using UnityEngine.UI;
//new add
using System.IO;
using UnityEngine.SceneManagement;
using Il2CppSystem.Threading;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;
using System;
using FMOD;
using System.Threading;
using BepInEx.Unity.IL2CPP.UnityEngine;
//anti replace 

namespace LimbusLocalize.LLC;
public class something : MonoBehaviour
{
    private static something instance;
    private readonly Queue<System.Action> actions = new Queue<System.Action>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static something Instance()
    {
        if (!instance)
        {
            throw new System.Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
        }
        return instance;
    }

    public void Enqueue(System.Action action)
    {
        lock (actions)
        {
            actions.Enqueue(action);
        }
    }

    public void Update()
    {
        while (actions.Count > 0)
        {
            actions.Dequeue().Invoke();
        }
    }
}


public static class UIImproved
{
    [HarmonyPatch(typeof(ParryingTypoUI), nameof(ParryingTypoUI.SetParryingTypoData))]
    [HarmonyPrefix]
    private static void ParryingTypoUI_SetParryingTypoData(ParryingTypoUI __instance)
    {
        __instance.img_parryingTypo.sprite = ReadmeManager.ReadmeSprites["LLC_Combo"];
    }
    static FMOD.Channel channel = new FMOD.Channel();
    [HarmonyPatch(typeof(ActBossBattleStartUI), nameof(ActBossBattleStartUI.Init))]
    [HarmonyPostfix]
    private static void BossBattleStartInit(ActBossBattleStartUI __instance)
    {
        // UnityEngine.Input.
        System.Collections.Generic.List<string> _loadingTexts;
        System.Collections.Generic.List<string> _loadingTextsTitles;
        _loadingTexts = [.. File.ReadAllLines(LLCMod.ModPath + "/Localize/Readme/BossBattleStartInitTexts.md")];
        _loadingTextsTitles = [.. File.ReadAllLines(LLCMod.ModPath + "/Localize/Readme/BossBattleStartInitTextsTitles.md")];
        var textGroup = __instance.transform.GetChild(2).GetChild(1);
        var tmp = textGroup.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        if (_loadingTexts.Count == 0 || _loadingTextsTitles.Count == 0)
        {
            LLCMod.LogWarning("nothing in BossBattleStartInitTextsTitles.md or BossBattleStartInitTextsTitles.md,using default.");
            return;
        }
        if (!tmp.text.Equals("Proelium Fatale"))
            return;
        {
            int i = UnityEngine.Random.RandomRangeInt(0, _loadingTexts.Count);
            tmp.font = ChineseFont.Tmpchinesefonts[0];
            if (i > _loadingTextsTitles.Count - 1)
            {
                tmp.text = "<b>" + SelectOne(_loadingTextsTitles) + "</b>";
            }
            else
            {
                tmp.text = "<b>" + SelectOne(_loadingTextsTitles, i) + "</b>";
            }
            tmp = textGroup.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            tmp.font = ChineseFont.Tmpchinesefonts[0];
            if (i > _loadingTexts.Count - 1)
            {
                tmp.text = "<b>" + SelectOne(_loadingTexts) + "</b>";
            }
            else
            {
                tmp.text = "<b>" + SelectOne(_loadingTexts, i) + "</b>";
            }
        }
    }
    public static T SelectOne<T>(System.Collections.Generic.List<T> list, int i = -1)
    {
        if (i != -1) return list[i];
        else
        {
            UnityEngine.Random.seed = (int)(Time.deltaTime + Time.timeSinceLevelLoad + DateTime.Today.Day + DateTime.Now.Minute);
            UnityEngine.Random.InitState((int)(Time.deltaTime + Time.timeSinceLevelLoad + DateTime.Today.Day + DateTime.Now.Minute));
            LLCMod.LogWarning((Time.deltaTime + Time.timeSinceLevelLoad + DateTime.Today.Day + DateTime.Now.Minute).ToString());
            return list.Count == 0 ? default : list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
    public class LyricLine
    {
        [JsonPropertyName("from")]
        public double from { get; set; }

        [JsonPropertyName("to")]
        public double to { get; set; }

        [JsonPropertyName("content")]
        public string content { get; set; }
    }//anti replace 
    public static string json = "";

    private static TextMeshProUGUI lyricText;
    private static List<LyricLine> lyrics;
    private static bool inLoginScene = false;
    private static something dwk;
    [HarmonyPatch(typeof(FMODUnity.RuntimeManager),
nameof(FMODUnity.RuntimeManager.PlayOneShot),
new[] { typeof(FMOD.GUID), typeof(Vector3) })]
    [HarmonyPrefix]
    static bool PlayOneShotPrefix(FMOD.GUID guid, Vector3 position)
    {
        LLCMod.LogInfo($"PlayOneShotPrefix guid");

        // RuntimeManager.PlayOneShot
        if (guid.IsNull) return true; // 继续执行原方法

        // 获取事件路径
        string eventPath;
        FMOD.Studio.EventDescription eventDescription;
        FMODUnity.RuntimeManager.StudioSystem.getEventByID(guid, out eventDescription);
        eventDescription.getPath(out eventPath);

        // 检测目标路径
        if (eventPath == "event:/BGM/TitleBgm")
        {
            // 替换为本地 MP3 文件
            PlayLocalMP3(position);
            return false; // 阻止原方法执行
        }
        LLCMod.LogInfo($"[FMOD] 播放: {eventPath}");
        return true; // 继续执行原方法

    }
    static void PlayLocalMP3(Vector3 position)
    {
        string filePath = Path.Combine(LLCMod.ModPath, "Localize/TitleBgm.mp3");

        try
        {
            FMOD.Sound sound;
            // FMOD.Channel channel;

            // 创建声音并播放
            FMOD.RESULT rs = FMODUnity.RuntimeManager.CoreSystem.createSound(filePath, FMOD.MODE.LOOP_NORMAL, out sound);
            if (rs != FMOD.RESULT.OK)
            {
                LLCMod.LogError($"创建声音失败: {rs}");
                return;
            }
            FMODUnity.RuntimeManager.CoreSystem.playSound(sound, default, false, out channel);
            // 设置 3D 位置（如果需要）
            FMOD.VECTOR pos = new FMOD.VECTOR
            {
                x = position.x,
                y = position.y,
                z = position.z
            };
            FMOD.VECTOR vel = new FMOD.VECTOR { x = 0, y = 0, z = 0 }; // 速度设置为 0
            channel.set3DAttributes(ref pos, ref vel);


            LLCMod.LogInfo($"[FMOD] enjoy it {filePath} !");
        }
        catch (Exception e)
        {
            LLCMod.LogError($"播放本地文件失败: {e.Message}");
        }
    }
    [HarmonyPatch(typeof(FMODUnity.RuntimeManager), nameof(FMODUnity.RuntimeManager.CreateInstance), new[] { typeof(string) })]
    [HarmonyPrefix]
    static bool CreateInstancePrefix(string path, ref FMOD.Studio.EventInstance __result)
    {
        if (path == "event:/BGM/TitleBgm")
        {
            PlayLocalMP3(Vector3.zero);
            __result = default; // 返回空实例
            return false; // 阻止原方法执行
        }

        // 其他事件正常记录
        LLCMod.LogInfo($"[FMOD] CreateInstance: {path}");
        return true;
    }
    [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
    [HarmonyPostfix]
    public static void Postfix(Scene scene, LoadSceneMode mode)
    {
        GameObject something = new GameObject("something");
        dwk = something.AddComponent<something>();
        try
        {
            if (scene.name == "LoginScene")
            {
                Canvas canvas = GameObject.FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    GameObject canvasObject = new GameObject("Canvas");
                    canvas = canvasObject.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasObject.AddComponent<CanvasScaler>();
                    canvasObject.AddComponent<GraphicRaycaster>();
                }

                // 创建一个新的TextMeshProUGUI对象
                GameObject textObject = new GameObject("LyricText");
                lyricText = textObject.AddComponent<TextMeshProUGUI>();
                // 设置父对象为Canvas
                textObject.transform.SetParent(canvas.transform, false);

                // 设置锚点和轴心点以确保文字始终居中
                lyricText.rectTransform.anchorMin = new Vector2(0.5f, 0.9f);
                lyricText.rectTransform.anchorMax = new Vector2(0.5f, 0.95f);
                lyricText.rectTransform.pivot = new Vector2(0.5f, 0);
                lyricText.rectTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                // 设置文本的位置
                lyricText.rectTransform.anchoredPosition = new Vector2(0, 10);
                lyricText.rectTransform.sizeDelta = new Vector2(10000, 65.5f);  // 这里高度可以根据需要调整

                lyricText.font = ChineseFont.Tmpchinesefonts[0];
                lyricText.fontStyle = FontStyles.Italic;
                lyricText.fontSize = 40f;

                // 设置文本对齐方式
                lyricText.alignment = TextAlignmentOptions.Center;
                if (lyrics == null)
                {
                    json = File.ReadAllText(Path.Combine(LLCMod.ModPath, "Localize/lyrics.json"), System.Text.Encoding.UTF8);
                    lyrics = System.Text.Json.JsonSerializer.Deserialize<List<LyricLine>>(json);
                }
                StartSinging();
            }
            else if (scene.name != "LogoScene")
            {
                StopSinging();
            }
        }
        catch (Exception ex)
        {
            LLCMod.LogError($"Error in BGMPostfix: {ex}");
        }
    }
    public static void StartSinging()
    {
        string json = File.ReadAllText(LLCMod.ModPath + "\\Localize\\lyrics.json");
        if (!inLoginScene)
        {
            inLoginScene = true;
            new System.Threading.Thread((System.Threading.ThreadStart)UpdateLyrics).Start();
        }
    }
    private static void UpdateLyrics()
    {
        while (inLoginScene)
        {
            uint timeMs;
            channel.getPosition(out timeMs, FMOD.TIMEUNIT.MS);
            double currentTime = (double)timeMs / 1000.0; // 转换为秒
            if (lyrics == null) return;

            foreach (var lyric in lyrics)
            {
                if (currentTime >= (double)lyric.from && currentTime < (double)lyric.to)
                {
                    // 使用RichText来支持颜色
                    dwk.Enqueue(() =>
                    {
                        lyricText.text = $"{lyric.content}";

                    });
                    break;
                } else {
                    dwk.Enqueue(() =>
                    {
                        lyricText.text = "";

                    });
                }
            }
            System.Threading.Thread.Sleep(25); // 控制刷新率
        }
    }
    public static void StopSinging()
    {
        inLoginScene = false;
        channel.stop();
        GameObject.Destroy(dwk);
        lyricText.text = "";
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