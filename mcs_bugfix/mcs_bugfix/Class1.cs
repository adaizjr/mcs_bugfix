﻿using BepInEx;
using HarmonyLib;
using KBEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace zjr_mcs
{
    [BepInPlugin("plugins.zjr.mcs_bugfix", "zjr修复BUG插件", "1.0.0.0")]
    public class bugfixBepInExMod : BaseUnityPlugin
    {// 在插件启动时会直接调用Awake()方法

        void Awake()
        {
            // 使用Debug.Log()方法来将文本输出到控制台
            Debug.Log("Hello,mcs_bugfix!");
            //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Harmony.CreateAndPatchAll(typeof(mcs_bugfix));
        }
    }

    internal class mcs_bugfix
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Avatar), "setMonstarDeath")]
        public static bool Avatar_setMonstarDeath_Prefix(Avatar __instance)
        {
            List<int> list = new List<int>();
            int num = 0;
            for (int i = 0; i < jsonData.instance.AvatarRandomJsonData.Count; i++)
            {
                if (num == 0)
                {
                    num++;
                }
                else
                {
                    string text = jsonData.instance.AvatarRandomJsonData.keys[i];
                    if (int.Parse(text) < 20000 && jsonData.instance.AvatarJsonData.HasField(text))
                    {
                        int num2 = (int)jsonData.instance.AvatarJsonData[text]["shouYuan"].n;
                        if (num2 > 5000)
                        {
                            num++;
                        }
                        else
                        {
                            try
                            {
                                if (DateTime.Parse(jsonData.instance.AvatarRandomJsonData[i]["BirthdayTime"].str).AddYears(num2) < __instance.worldTimeMag.getNowTime())
                                {
                                    int.Parse(text);
                                    list.Add(int.Parse(text));
                                }
                            }
                            catch (Exception)
                            {
                                UIPopTip.Inst.Pop("设置NPC死亡出现错误，重置NPC数据以解决问题。", PopTipIconType.叹号);
                                break;
                            }
                            num++;
                        }
                    }
                }
            }
            List<int> tmp_list = new List<int>() { 4101, 4102, 4103, 4104, 4105 };
            for (int i = 0; i < jsonData.instance.AvatarRandomJsonData.Count; i++)
            {
                int tmp_ke = int.Parse(jsonData.instance.AvatarRandomJsonData.keys[i]);
                if (tmp_ke < 20000 && tmp_list.Contains(tmp_ke) && !list.Contains(tmp_ke))
                {
                    if (DateTime.Parse(jsonData.instance.AvatarRandomJsonData[i]["BirthdayTime"].str).Year < __instance.worldTimeMag.getNowTime().Year)
                        list.Add(tmp_ke);
                }
            }
            for (int j = 0; j < list.Count; j++)
            {
                try
                {
                    jsonData.instance.setMonstarDeath(list[j], false);
                }
                catch (Exception)
                {
                    Debug.LogError("设置Npc" + list[j].ToString() + "死亡出错");
                }
            }
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NPCXiuLian), "NextNpcLunDao")]
        public static bool NPCXiuLian_NextNpcLunDao_Prefix(ref NPCXiuLian __instance)
        {
            my_NextNpcLunDao(__instance);
            return true;
        }
        static void my_NextNpcLunDao(NPCXiuLian __instance)
        {
            List<int> l_remove = new List<int>();
            for (int i = NpcJieSuanManager.inst.lunDaoNpcList.Count - 1; i >= 0; i--)
            {
                int num = NpcJieSuanManager.inst.lunDaoNpcList[i];
                if (NpcJieSuanManager.inst.npcDeath.npcDeathJson.HasField(num.ToString()))
                {
                    l_remove.Add(num);
                }
                if (!jsonData.instance.AvatarJsonData.HasField(num.ToString()))
                {
                    l_remove.Add(num);
                }
            }
            foreach (var num in l_remove)
            {
                NpcJieSuanManager.inst.lunDaoNpcList.Remove(num);
            }
        }
    }
}
