using System.Collections;
using System.Collections.Generic;
using Data;
using OpenAI;
using OpenAI.Chat;
using ProjectBase.Event;
using UnityEngine;
using Utilities;
using Utils;
using Choice = OpenAI.Chat.Choice;

public class GPTNO2 : MonoBehaviour
{
    private OpenAIData data;

    private OpenAIClient _openAIClient;

    private List<Message> curentMessages = new List<Message>();

    private string[] imageURLS;
    
    void Start()
    {
        data = DealMessages.LoadMessages<OpenAIData>(FileNames.GPT2Authentication);
        _openAIClient = new OpenAIClient(new OpenAIAuthentication(data.apiKey, data.organizationId, data.projectId));
        curentMessages.Add(new Message(Role.System, data.prompt));
    }

    async void SendReplyToGPTNO2()
    {
        var res = DealMessages.LoadMessages<List<Message>>(FileNames.GPT1MessagesFileName);
        foreach (var message in res)
        {
            curentMessages.Add(message);
        }
        
        AddImageMessage(imageURLS);

        var response = await _openAIClient.ChatEndpoint.GetCompletionAsync(new ChatRequest(curentMessages, data.model));
        Choice choice = response.FirstChoice;
        DealResponseMessage(choice.Message, out GPTNO2Response gptno2Response);
        DealMessages.SaveMessages(gptno2Response, true, FileNames.GPT2MessagesFileName);
        EventCenter.Instance.EventTrigger(EventName.GPTNO2Over);
    }

    void DealResponseMessage(Message message, out GPTNO2Response response)
    {
        string[] strings = message.Content.ToString().Split(':', '\n');
        response = new GPTNO2Response()
        {
            start = strings[1],
            card_1 = strings[3],
            card_2 = strings[5],
            card_3 = strings[7],
            card_4 = strings[9],
            overall_advice = strings[11]
        };
    }

    void AddImageMessage(string[] imageURL)
    {
        curentMessages.Add(new Message(Role.User, new List<Content>()
        {
            //玩家占卜问题，
            new ImageUrl(imageURLS[0]),
            new ImageUrl(imageURLS[1]),
            new ImageUrl(imageURLS[2]),
            new ImageUrl(imageURLS[3])
        }));
    }

    /// <summary>
    /// 获取2号GPT输出的解牌信息
    /// </summary>
    /// <param name="index">解牌下标，从0开始，3结束，共四牌解</param>
    /// <param name="cardMessage">输出的牌解信息</param>
    public static void GetGPT2CardMessage(int index, out string cardMessage)
    {
        GPTNO2Response response = DealMessages.LoadMessages<GPTNO2Response>(FileNames.GPT2MessagesFileName);
        List<string> messages = new List<string>()
        {
            response.card_1,
            response.card_2,
            response.card_3,
            response.card_4
        };
        cardMessage = messages[index];
    }

    /// <summary>
    /// 获取2号GPT输出的总体解释
    /// </summary>
    /// <param name="overall">输出的总体解释</param>
    public static void GetGPT2OverallMessage(out string overall)
    {
        GPTNO2Response response = DealMessages.LoadMessages<GPTNO2Response>(FileNames.GPT2MessagesFileName);
        overall = response.overall_advice;
    }
}
