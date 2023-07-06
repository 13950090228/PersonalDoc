using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;

namespace Campfire.Sound {
    /// <summary>
    /// SoundbanksInfo 读取类
    /// </summary>
    public static class SoundbanksInfoXmlHelper {
        static XmlDocument xmlDoc = new XmlDocument();
        static Dictionary<string, uint> temporaryBnkInfoDic = new Dictionary<string, uint>();  // 临时的 bnk 信息会被经常加载和卸载
        static Dictionary<string, uint> baseBnkInfoDic = new Dictionary<string, uint>();  // 常驻的 bnk 信息，用于全局相关的 bnk 信息存放
        static string xmlPath;

        /// <summary>
        /// 设置文件路径
        /// </summary>
        /// <param name="path"></param>
        public static void SetXmlPath(string path) {
            xmlPath = path;
        }

        /// <summary>
        /// 读取 Xml 文件进内存
        /// </summary>
        public static void ReadXml() {
            if (xmlPath == null) {
                Debug.LogError("请先设置文件路径: SetXmlPath");
                return;
            }
            xmlDoc.Load(xmlPath);
        }

        static bool CheckPathIsNull() {
            if (xmlPath == null) {
                Debug.LogError("请先设置文件路径: SetXmlPath");
                return true;
            }

            return false;
        }


        /// <summary>
        /// 加载全局基础 bnk 信息
        /// </summary>
        /// <param name="bnks">bnk 字符串数组</param>
        public static void LoadBaseBnkInfo(string[] bnks) {
            if (CheckPathIsNull()) {
                return;
            }

            XmlNode xmlRoot = xmlDoc.DocumentElement;
            foreach (XmlNode node1 in xmlRoot.SelectNodes("SoundBanks/SoundBank")) {
                string bnkSign = node1.SelectSingleNode("Path").InnerText.Replace(".bnk", "");
                if (Array.IndexOf(bnks, bnkSign) != -1) {
                    foreach (XmlNode node2 in node1.SelectNodes("IncludedEvents/Event")) {
                        baseBnkInfoDic[node2.Attributes["Name"].Value] = Convert.ToUInt32(node2.Attributes["Id"].Value);
                    }
                }
            }
        }

        /// <summary>
        /// 全量加载所有 bnk 信息
        /// </summary>
        /// <param name="path"></param>
        public static void LoadAllBnkInfo() {
            if (CheckPathIsNull()) {
                return;
            }

            ClearTemporaryBnkInfo();
            XmlNode xmlRoot = xmlDoc.DocumentElement;
            foreach (XmlNode node1 in xmlRoot.SelectNodes("SoundBanks/SoundBank/IncludedEvents")) {
                foreach (XmlNode node2 in node1.SelectNodes("Event")) {
                    temporaryBnkInfoDic[node2.Attributes["Name"].Value] = Convert.ToUInt32(node2.Attributes["Id"].Value);
                }
            }
        }

        /// <summary>
        /// 加载临时 bnk 信息
        /// </summary>
        /// <param name="bnks">bnk 字符串数组</param>
        public static void LoadTemporaryBnkInfo(string[] bnks) {
            if (CheckPathIsNull()) {
                return;
            }

            XmlNode xmlRoot = xmlDoc.DocumentElement;
            foreach (XmlNode node1 in xmlRoot.SelectNodes("SoundBanks/SoundBank")) {
                string bnkSign = node1.SelectSingleNode("Path").InnerText.Replace(".bnk", "");
                if (Array.IndexOf(bnks, bnkSign) != -1) {
                    foreach (XmlNode node2 in node1.SelectNodes("IncludedEvents/Event")) {
                        temporaryBnkInfoDic[node2.Attributes["Name"].Value] = Convert.ToUInt32(node2.Attributes["Id"].Value);
                    }
                }
            }
        }

        /// <summary>
        /// 获取事件 ID
        /// </summary>
        /// <param name="eventSign"></param>
        /// <returns></returns>
        public static uint GetEventId(string eventSign) {
            return baseBnkInfoDic[eventSign];
        }

        /// <summary>
        /// 获取事件 Sign
        /// </summary>
        /// <param name="eventSign"></param>
        /// <returns></returns>
        public static string GetEventSign(uint eventid) {
            foreach (var item in baseBnkInfoDic) {
                if (item.Value == eventid) {
                    return item.Key;
                }
            }

            return null;
        }
        /// <summary>
        /// 从 Temporary 获取事件 ID
        /// </summary>
        /// <param name="eventSign"></param>
        /// <returns></returns>
        public static uint GetEventIdFormTemporary(string eventSign) {
            return temporaryBnkInfoDic[eventSign];
        }

        /// <summary>
        /// 从 Temporary 获取事件 Sign
        /// </summary>
        /// <param name="eventSign"></param>
        /// <returns></returns>
        public static string GetEventSignFormTemporary(uint eventid) {
            foreach (var item in temporaryBnkInfoDic) {
                if (item.Value == eventid) {
                    return item.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// 清理 TemporaryBnkInfoDic 
        /// </summary>
        public static void ClearTemporaryBnkInfo() {
            temporaryBnkInfoDic.Clear();
        }

        /// <summary>
        /// 清理所有缓存
        /// </summary>
        /// <param name="clearPath">是否清理路径信息</param>
        public static void TearDown(bool clearPath = false) {
            ClearTemporaryBnkInfo();
            baseBnkInfoDic.Clear();
            xmlDoc = new XmlDocument();
            if (clearPath) {
                xmlPath = null;
            }
        }
    }
}
