using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using OpenAI;
using OpenAI.Chat;
using ProjectBase.Date;
using ProjectBase.Event;
using ProjectBase.Res;
using ProjectBase.UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utils;

namespace Game.GPT
{
    public class GPTNO1 : BasePanel
    {
        /*private RectTransform sent;
        private RectTransform received;*/
        
        private static OpenAIClient _openAIClient;

        private static OpenAIData data;

        private static List<Message> CurrentMessages = new List<Message>();

        //private float height;
        // Start is called before the first frame update
        void Start()
        {
            /*sent = ResManager.LoadResource<GameObject>("UI/Sent Message").GetComponent<RectTransform>();
            received = ResManager.LoadResource<GameObject>("UI/Received Message").GetComponent<RectTransform>();*/
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
            //ShowMessage(message);
            var chatRequest = new ChatRequest(CurrentMessages, data.model);
            var chatResponse = await _openAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            var choice = chatResponse.FirstChoice;
            //处理GPT返回的信息，提取GPT结构化输出的response部分，将该部分返回到调用端
            string gptResponse = DealResponseMessage(choice.Message);
            message = new Message(choice.Message.Role, gptResponse);
            AddMessageToMessageList(message);
            //ShowMessage(message);
            DealMessages.SaveMessages(CurrentMessages, true, FileNames.GPT1MessagesFileName);
            return gptResponse;
        }

        /*void ShowMessage(Message message)
        {
            GetControl<ScrollRect>("ScrollView").content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            RectTransform rt = Instantiate(message.Role == Role.User ? sent : received, GetControl<ScrollRect>("ScrollView").content);
            rt.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Role == Role.User
                ? message.Content.ToString()
                : DealResponseMessage(message);
            rt.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            height += rt.sizeDelta.y;
            GetControl<ScrollRect>("ScrollView").content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            GetControl<ScrollRect>("ScrollView").verticalNormalizedPosition = 0;
        }*/

        /// <summary>
        /// 添加对话信息到对话信息集合
        /// </summary>
        /// <param name="message"></param>
        static void AddMessageToMessageList(Message message)
        {
            CurrentMessages.Add(new Message(message.Role,
                message.Role == Role.User ? message.Content.ToString() : DealResponseMessage(message)));
        }
        
        /// <summary>
        /// 处理GPT输出的结构化信息，
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
