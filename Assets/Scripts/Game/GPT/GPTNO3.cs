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
    /*
     * 三号GPT负责接收2号GPT输出的总体解释，根据该总体解释输出供comfyUI绘图的prompt
     * 对外提供获取三号GPTprompt的方法。
     */
    public class GPTNO3
    {
        private OpenAIData data;

        private OpenAIClient OpenAIClient;

        private List<Message> currentMessages = new List<Message>();

        public GPTNO3()
        {
            data = DealMessages.LoadMessages<OpenAIData>(FileNames.GPT3Authentication);
            OpenAIClient = new OpenAIClient(new OpenAIAuthentication(data.apiKey, data.organizationId, data.projectId));
            currentMessages.Add(new Message(Role.System, data.prompt));
            EventCenter.Instance.AddEventListener(EventName.GPTNO2Over, Function);
        }

        void Function()
        {
            GPTNO2.GetGPT2OverallMessage(out string overall);
            currentMessages.Add(new Message(Role.User, overall));
            SendReply(currentMessages);
        }
        
        /// <summary>
        /// 将2号GPT的总体解释输入给3号GPT并保存3号GPT输出的prompt
        /// </summary>
        /// <param name="messages"></param>
        void SendReply(List<Message> messages)
        {
            var response = OpenAIClient.ChatEndpoint.GetCompletionAsync(new ChatRequest(currentMessages, data.model));
            DealMessages.SaveMessages(response.Result.FirstChoice.Message, true, FileNames.GPT3MessagesFileName);
        }

        /// <summary>
        /// 获取三号GPT输出的prompt
        /// </summary>
        /// <param name="prompt"></param>
        public static void GetGPT3Prompt(out string prompt)
        {
            prompt = DealMessages.LoadMessages<GPTNO3Response>(FileNames.GPT3MessagesFileName).prompt;
        }
    }
}
