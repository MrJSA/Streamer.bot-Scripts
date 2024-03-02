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
        // Get Message Id
        string messageId = args["msgId"].ToString();
        // Get the User that is permitted to post Links
        string permitUser = CPH.GetGlobalVar<string>("permitUser", false);
        // Get Path to .txt file set in Arguments or here
        string pathToFile = args["wordsBlackList"].ToString();
        // get Username that wrote the Message
        string chatterName = args["user"].ToString();
        // Check if user is allowed to post Badwords
        if (permitUser == null || permitUser == "" || chatterName != permitUser)
        {
            RegexOptions regexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            // Get Message that needs to be checked
            string message = args["message"].ToString();
            // Get the number of Entrys of .txt file
            // string array may not work for really big text files
            string[] readBadWordList = File.ReadAllLines(pathToFile);
            // get line count faster
            int countEntrys = 0;
            using (var reader = File.OpenText(pathToFile))
            {
                while (reader.ReadLine() != null)
                {
                    countEntrys++;
                }
            }
            // I dont know if neccessary, but check in pairs of 1000
            // May be deleted
            // Gets the Lines with x mod 1000
            int countSum = 1000;
            int countCombined = countEntrys / countSum;
            int countRest = countEntrys % countSum;
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