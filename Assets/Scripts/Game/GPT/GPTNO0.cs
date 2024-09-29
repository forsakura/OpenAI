using System;
using System.Collections.Generic;
using Data;
using OpenAI;
using OpenAI.Chat;
using ProjectBase.Event;
using UnityEngine;
using Utilities;
using Utils;

namespace Game.GPT
{
    public class GPTNO0 : MonoBehaviour
    {
        private OpenAIData data;
        private OpenAIClient openAi;
        private List<Message> currentMessages = new List<Message>();
        private KeyWordLibrary KeyWordLibrary1;
        private KeyWordLibrary KeyWordLibrary2;
        private KeyWordLibrary KeyWordLibrary3;
        private static Message cardPromptMessage;

        private void Start()
        {
            data = DealMessages.LoadMessages<OpenAIData>(FileNames.GPT0Authentication);
            openAi = new OpenAIClient(new OpenAIAuthentication(data.apiKey, data.organizationId, data.projectId));
            currentMessages.Add(new Message(Role.System, data.prompt));
            KeyWordLibrary1 = DealMessages.LoadMessages<KeyWordLibrary>(FileNames.KeyWordLibrary1);
            KeyWordLibrary2 = DealMessages.LoadMessages<KeyWordLibrary>(FileNames.KeyWordLibrary2);
            KeyWordLibrary3 = DealMessages.LoadMessages<KeyWordLibrary>(FileNames.KeyWordLibrary3);
            SendReply();
        }

        private async void SendReply()
        {
            GetCombineKeyWord(out string prompts);
            currentMessages.Add(new Message(Role.User, prompts));
            var response = await openAi.ChatEndpoint.GetCompletionAsync(new ChatRequest(currentMessages, data.model));
            cardPromptMessage = response.FirstChoice.Message;
            EventCenter.Instance.EventTrigger(EventName.GPTNO0Over);
        }

        /// <summary>
        /// 获取三个词条库中关键词组合的prompt，四个prompt组合起来的集合prompt。
        /// </summary>
        /// <param name="prompts"></param>
        void GetCombineKeyWord(out string prompts)
        {
            prompts = "";
            for (int i = 0; i < 4; i++)
            {
                string partKeyWord = String.Concat(KeyWordLibrary1.GetPartKeyWord(), KeyWordLibrary2.GetPartKeyWord(),
                    KeyWordLibrary3.GetPartKeyWord());
                prompts += partKeyWord;
            }
        }

        /// <summary>
        /// 为GPT0生成的四张卡牌的prompt添加颜色prompt并获取他们的最终prompt
        /// </summary>
        /// <param name="prompts"></param>
        public static void GetCardPrompts(out string prompts)
        {
            string res = cardPromptMessage.ToString();
            string[] strings = res.Split('\n');
            Debug.Log(strings.Length);
            prompts = "";
            for (int i = 1, j = 1; i < strings.Length ; i += 2,j++)
            {
                strings[i] = String.Concat(strings[i], $"\ncard_{j}_color:" + "black\n");
                prompts = prompts + strings[i - 1] + "\n" + strings[i];
            }
        }
    }
}
