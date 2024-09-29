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
    public class GPTNO3 : MonoBehaviour
    {

        public OpenAIData data;

        public OpenAIClient OpenAIClient;

        public List<Message> currentMessages;
        
        // Start is called before the first frame update
        void Start()
        {
            data = DealMessages.LoadMessages<OpenAIData>(FileNames.GPT3Authentication);
            OpenAIClient = new OpenAIClient(new OpenAIAuthentication(data.apiKey, data.organizationId, data.projectId));
            currentMessages.Add(new Message(Role.System, data.prompt));
            GPTNO2.GetGPT2OverallMessage(out string overall);
            currentMessages.Add(new Message(Role.User, overall));
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
