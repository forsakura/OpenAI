using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Internal;

namespace Utils
{
    /*
     * 对对话信息的存储和读取，以及删除操作。所有信息数据文件都存储在streamingAssets的ChatMessages目录下，
     * 要更改目录就改messagesPath字段。
     * （尚未做测试，只是照着思路写了）
     */
    public static class DealMessages
    {
        private static string messagesPath = Application.streamingAssetsPath + "/ChatMessages";
        
        /// <summary>
        /// 将对话信息保存到文件中，使用前将玩家和GPT的每条信息添加到MessagesData实例对象中，最后将该实例全部存储
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="readability"></param>
        /// <param name="fileName"></param>
        public static void SaveMessages(object obj, [DefaultValue("false")]bool readability, string fileName)
        {
            string path = messagesPath + "/" + fileName + ".json";
            string jsonStr = JsonConvert.SerializeObject(obj);
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            File.WriteAllText(path, jsonStr);
        }

        /// <summary>
        /// 将对话信息从文件中读取出来，读取出来后直接就是MessagesData类实例，数据都在该实例中。
        /// </summary>
        /// <param name="fileName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadMessages<T>(string fileName)
        {
            string path = messagesPath + "/" + fileName + ".json";
            string text = File.ReadAllText(path);
            T res = JsonConvert.DeserializeObject<T>(text);
            return res;
        }

        /// <summary>
        /// 删除某文件中的对话信息
        /// </summary>
        /// <param name="fileName"></param>
        public static void ClearAnyMessages(string fileName)
        {
            string path = messagesPath + "/" + fileName + ".json";
            File.WriteAllText(path, string.Empty);
        }

        /// <summary>
        /// 清空所有对话信息
        /// </summary>
        public static void ClearAllMessages()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(messagesPath);
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (var fileInfo in files)
            {
                if (fileInfo.Extension.Equals(".json"))
                {
                    File.WriteAllText(fileInfo.FullName, string.Empty);
                }
            }
        }
    }
}
