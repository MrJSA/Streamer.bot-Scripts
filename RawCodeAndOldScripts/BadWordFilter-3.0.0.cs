using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

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
        // Get Message Id
        string messageId = args["msgId"].ToString();
        // Get the User that is permitted to post Links
        string permitUser = CPH.GetGlobalVar<string>("permitUser", false);
        // Get Path to .txt file set in Global Variable
        string pathToFile = args["global_BadWordListPath"].ToString();
        // get Username that wrote the Message
        string chatterName = args["user"].ToString();
        // Get Message that needs to be checked
        string message = args["message"].ToString();
        // Open the file for reading
        using (StreamReader reader = new StreamReader(pathToFile))
        {
            bool foundMatch = false;
            string line;
            // Read lines from the file and check if any line matches the entered string
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Equals(message, StringComparison.OrdinalIgnoreCase))
                {
                    foundMatch = true;
                    break;
                }
            }

            if (foundMatch)
            {
                if (timeoutBool)
                {
                    CPH.TwitchTimeoutUser(chatterName, timeoutTime, timeoutReason, false);
                    CPH.SendMessage(blockMessage1 + chatterName + blockMessage2, false);
                }
                else
                {
                    CPH.TwitchDeleteChatMessage(messageId, false);
                    CPH.SendMessage(blockMessage1 + chatterName + blockMessage2, false);
                }
            }
        }

        return true;
    }
}