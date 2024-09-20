using System.Collections.Generic;
using OpenAI;

namespace Data
{
    /// <summary>
    /// 对话信息数据类，记录玩家和gpt对话的所有信息的一个数据类。，暂时是这样的。
    /// </summary>
    public struct MessagesData
    {
        public List<ChatMessage> messages;

        public MessagesData(int capacity)
        {
            messages = new List<ChatMessage>(capacity);
        }
    }
}
