using System.Collections.Generic;
using System.IO;
using OpenAI;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.UI;

namespace Samples.OpenAI_Unity._0._2._2.ChatGPT
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        private OpenAIApi openai;

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt;

        private string textPath = "C:\\Users\\zhuju\\Documents\\OpenAI\\auth.json";

        private OpenAIData _openAIData;

        private void Start()
        {
            string text = File.ReadAllText(textPath);
            var res = JsonUtility.FromJson<OpenAIData>(text);
            prompt = res.prompt;
            openai = new OpenAIApi(res.api_key);
            button.onClick.AddListener(SendReply);

            _openAIData = new OpenAIData
            {
                api_key = res.api_key,
                prompt = res.prompt,
                organization = res.organization,
                messages = new List<ChatMessage>()
            }; 
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);
            
            AddMessageToOpenAI(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-4o-mini",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                
                messages.Add(message);
                AppendMessage(message);
                
                AddMessageToOpenAI(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            SaveDataToLocal(_openAIData, true);
            
            button.enabled = true;
            inputField.enabled = true;
        }

        private void AddMessageToOpenAI(ChatMessage message)
        {
            _openAIData.messages.Add(message);
        }

        private async void SaveDataToLocal(object obj, [DefaultValue("false")]bool readability)
        {
            string jsonStr = JsonUtility.ToJson(obj, readability);
            await File.WriteAllTextAsync(textPath, jsonStr);
        }
    }
}
