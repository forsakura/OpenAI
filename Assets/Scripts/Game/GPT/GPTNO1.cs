using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using OpenAI;
using OpenAI.Chat;
using ProjectBase.Date;
using ProjectBase.Event;
using UnityEngine.Windows;
using Utilities;
using Utils;

namespace Game.GPT
{
    /*
     * 负责与玩家对话交流，收集玩家信息，占卜问题以及玩家昵称。
     * 对外提供获取对话记录，占卜问题和玩家昵称的方法。
     */
    public class GPTNO1
    {
        private static OpenAIClient _openAIClient;

        private static OpenAIData data;

        private static List<Message> CurrentMessages = new List<Message>();

        public GPTNO1()
        {
            data = DealMessages.LoadMessages<OpenAIData>(FileNames.GPT1Authentication);
            _openAIClient = new OpenAIClient(new OpenAIAuthentication(data.apiKey, data.organizationId, data.projectId));
            CurrentMessages.Add(new Message(Role.System, data.prompt));
        }

        /// <summary>
        /// 发送信息，并接收GPT返回的信息
        /// </summary>
        public static async Task<string> SendReplyToGPTNO1(string userMessage)
        {
            Message message = new Message(Role.User, userMessage);
            AddMessageToMessageList(message);
            var chatRequest = new ChatRequest(CurrentMessages, data.model);
            var chatResponse = await _openAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            var choice = chatResponse.FirstChoice;
            string gptResponse = DealResponseMessage(choice.Message);
            message = new Message(choice.Message.Role, gptResponse);
            AddMessageToMessageList(message);
            DealMessages.SaveMessages(CurrentMessages, true, FileNames.GPT1MessagesFileName);
            return gptResponse;
        }

        /// <summary>
        /// 添加对话信息到对话信息集合
        /// </summary>
        /// <param name="message"></param>
        static void AddMessageToMessageList(Message message)
        {
            CurrentMessages.Add(new Message(message.Role, message.Content.ToString()));
        }
        
        /// <summary>
        /// 处理GPT输出的结构化信息，提取其中的response部分。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        static string DealResponseMessage(Message message)
        {
            string[] strings = message.Content.ToString().Split(':', '\n');
            GPTNO1Response response = new GPTNO1Response()
            {
                respond = strings[1],
                user_name = strings[3],
                user_question = strings[5],
                finish_trigger = strings[7].Trim()
            };
            if (response.finish_trigger.Equals("1"))
            {
                UserData.SaveUserInformation(response);
                EventCenter.Instance.EventTrigger(EventName.GPTNO1Over);
            }

            return response.respond;
        }

        /// <summary>
        /// 获取玩家与1号GPT的聊天记录
        /// </summary>
        public static void GetChatMessage(out Message message)
        {
            message = DealMessages.LoadMessages<Message>(FileNames.GPT1MessagesFileName);
        }

        /// <summary>
        /// 获取玩家的占卜问题
        /// </summary>
        public static void GetQuestion(out string userQuestion)
        {
            userQuestion = SaveSystem.LoadGameFromJson<UserData>(FileNames.UserData, JsonType.JsonUtility).userQuestion;
        }

        /// <summary>
        /// 获取玩家的昵称
        /// </summary>
        /// <param name="userName"></param>
        public static void GetUserName(out string userName)
        {
            userName = SaveSystem.LoadGameFromJson<UserData>(FileNames.UserData, JsonType.JsonUtility).userName;
        }
    }
}
