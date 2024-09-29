using Data;
using Game.GPT;
using UnityEngine;

public class Test : MonoBehaviour
{
    private string str =
        "respond: Understood, 11. You’re seeking clarity about whether danger is present in your path. I have gathered all the details I need to proceed.\nuser_name: 11 \nuser_question: Is 11 in danger? \nfinish_trigger: 1";
    private async void Start()
    {
        /*string[] strings = str.Split(':', '\n');
        GPTNO1Response response = new GPTNO1Response()
        {
            respond = strings[1],
            user_name = strings[3],
            user_question = strings[5],
            finish_trigger = strings[7].Trim()
        };
        if (response.finish_trigger == "1")
        {
            UserData.SaveUserInformation(response);
        }*/
        GPTNO0.GetCardPrompts(out string res);
        Debug.Log(res);
    }
}
