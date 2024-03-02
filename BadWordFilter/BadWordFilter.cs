using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        // Timeout Time set to wanted Value in arguments or here. Whatever floats your boat
        int timeoutTime = Convert.ToInt32(args["timeoutTime"].ToString());
        // Message before the Username, set here or in Arguments
        string blockMessage1 = args["blockMessage1"].ToString();
        // Message before the Username, set here or in Arguments
        string blockMessage2 = args["blockMessage2"].ToString();
        // Do you want Timeout? set here or in arguments "yes" or "no"
        bool timeoutBool = bool.Parse(args["timeout"].ToString());
        // Reason for Timeout for your Logs, set here or in arguments
        string timeoutReason = args["timeoutReason"].ToString();
        // Only stupid code after here, doesnt need to bother you
        string messageId = args["msgId"].ToString();
        string permitUser = CPH.GetGlobalVar<string>("permitUser", false);
        string pathToFile = args["wordsBlackList"].ToString();
        string chatterName = args["user"].ToString();
        if (permitUser == null || permitUser == "" || chatterName != permitUser)
        {
            RegexOptions regexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            string message = args["message"].ToString();
            string[] readBadWordList = File.ReadAllLines(pathToFile);
            int countEntrys = readBadWordList.Length;
            int countSum = 10;
            int countCombined = readBadWordList.Length / countSum;
            int countRest = readBadWordList.Length % countSum;
            for (int i = 0; i <= countCombined; i++)
            {
                string combinedLines = "";
                if (i < countCombined)
                {
                    for (int j = 0; j < countSum - 1; j++)
                    {
                        combinedLines += readBadWordList[i * 10 + j] + "|";
                    }

                    combinedLines += readBadWordList[i * 10 + countSum - 1];
                }
                else if (i == countCombined)
                {
                    for (int j = 0; j < countRest - 1; j++)
                    {
                        combinedLines += readBadWordList[i * 10 + j] + "|";
                    }

                    combinedLines += readBadWordList[i * 10 + countRest - 1];
                }

                string badWordDetect = @"(" + combinedLines + @")";
                foreach (Match m in Regex.Matches(message, badWordDetect, regexOptions))
                {
                    bool allowed = false;
                    if (!allowed)
                    {
                        if (timeoutBool)
                        {
                            CPH.TwitchTimeoutUser(chatterName, timeoutTime, timeoutReason);
                            CPH.SendMessage(blockMessage1 + chatterName + blockMessage2);
                        }
                        else
                        {
                            CPH.TwitchDeleteChatMessage(messageId, false);
                            CPH.SendMessage(blockMessage1 + chatterName + blockMessage2);
                        }
                    }
                }
            }
        }

        return true;
    }
}
