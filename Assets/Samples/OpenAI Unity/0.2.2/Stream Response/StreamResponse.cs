using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenAI;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.OpenAI_Unity._0._2._2.Stream_Response
{
    public class StreamResponse : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Text text;
        
        private OpenAIApi openai = new OpenAIApi("sk-jWRna4VvgbQJojli7274646613824dEeB4A7435aF7B2Fc4b");
        private CancellationTokenSource token = new CancellationTokenSource();
        
        private void Start()
        {
            button.onClick.AddListener(SendMessage);
        }
        
        private void SendMessage()
        {
            button.enabled = false;

            var message = new List<ChatMessage>
            {
                new ChatMessage()
                {
                    Role = "user",
                    Content = "Write a 100 word long short story in La Fontaine style."
                }
            };
            
            openai.CreateChatCompletionAsync(new CreateChatCompletionRequest()
            {
                Model = "gpt-4o-mini",
                Messages = message,
                Stream = true
            }, HandleResponse, null, token);

            button.enabled = true;
        }

        private void HandleResponse(List<CreateChatCompletionResponse> responses)
        {
            text.text = string.Join("", responses.Select(r => r.Choices[0].Delta.Content));
        }

        private void OnDestroy()
        {
            token.Cancel();
        }
    }
}
