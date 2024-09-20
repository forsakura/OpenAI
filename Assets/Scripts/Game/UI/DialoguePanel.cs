using System;
using Data;
using OpenAI;
using ProjectBase.Event;
using ProjectBase.Res;
using ProjectBase.UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI
{
    public class DialoguePanel : BasePanel
    {
        private RectTransform sent;
        private RectTransform received;
        private float height;

        #region DATA

        private OpenAIApi openai;
        private OpenAIData data;
        private MessagesData messagesData;

        #endregion

        /*#region KEYPATH

        private const string AUTH_PATH = "C:\\Users\\zhuju\\Documents\\OpenAI\\myauth.json";

        #endregion*/
        

        private void OnEnable()
        {
            sent = ResManager.LoadResource<GameObject>("UI/Sent Message").GetComponent<RectTransform>();
            received = ResManager.LoadResource<GameObject>("UI/Received Message").GetComponent<RectTransform>();
        }

        private void Start()
        {
            messagesData = new MessagesData(0);
            data = DealMessages.LoadMessages<OpenAIData>("myauth");
            openai = new OpenAIApi(data.api_key);
            GetControl<Button>("SendButton").onClick.AddListener(() => SendReply(messagesData, data, openai));
        }

        private void OnDestroy()
        {
            GetControl<Button>("SendButton").onClick.RemoveListener(() => SendReply(messagesData, data, openai));
        }
        
        private void AppendMessage(ChatMessage message)
        {
            GetControl<ScrollRect>("ScrollView").content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, GetControl<ScrollRect>("ScrollView").content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            GetControl<ScrollRect>("ScrollView").content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            GetControl<ScrollRect>("ScrollView").verticalNormalizedPosition = 0;
        }
        
        private async void SendReply(MessagesData messagesData, OpenAIData data, OpenAIApi openai)
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = GetControl<InputField>("InputField").text
            };
            
            AppendMessage(newMessage);
            messagesData.messages.Add(newMessage);
            
            newMessage.Content = data.prompt + "\n" + GetControl<InputField>("InputField").text; 
            
            
            GetControl<Button>("SendButton").enabled = false;
            GetControl<InputField>("InputField").text = "";
            GetControl<InputField>("InputField").enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = data.model,
                Messages = messagesData.messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                
                messagesData.messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            GetControl<Button>("SendButton").enabled = true;
            GetControl<InputField>("InputField").enabled = true;
            DealMessages.SaveMessages(messagesData.messages, true, "messages");
        }
    }
}
