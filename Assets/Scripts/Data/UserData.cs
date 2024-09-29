using System.Collections;
using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using ProjectBase.Date;
using UnityEngine;
using Utilities;

public class UserData
{
    public string userName;
    public string userQuestion;

    public static void SaveUserInformation(GPTNO1Response response)
    {
        UserData userData = new UserData()
        {
            userName = response.user_name,
            userQuestion = response.user_question
        };
        SaveSystem.SaveGameByJson(FileNames.UserData, userData, JsonType.JsonUtility);
    }
}
