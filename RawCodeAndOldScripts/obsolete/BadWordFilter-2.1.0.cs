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
        // Only stupid code after here, doesnt need to bother you
        // Get Message Id
        string messageId = args["msgId"].ToString();
        // Get the User that is permitted to post Links
        string permitUser = CPH.GetGlobalVar<string>("permitUser", false);
        // Get Path to .txt file set in Arguments or here
        string pathToFile = args["global_BadWordListPath"].ToString();
        // get Username that wrote the Message
        string chatterName = args["user"].ToString();
        // Check if user is allowed to post Badwords
        RegexOptions regexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase;
        // Get Message that needs to be checked
        string message = args["message"].ToString();
        // This creates the String for
        using (StreamReader reader = new StreamReader(pathToFile))
        {
            StringBuilder stringBuilder = new StringBuilder();
            string line;
            // Read lines from the file and append them to the StringBuilder
            while ((line = reader.ReadLine()) != null)
            {
                // Replace new line with | for regex
                stringBuilder.Append(line.Replace("\n", "|")).Append("|");
            }

            // Remove the last '|' character
            if (stringBuilder.Length > 0)
            {
                // Remove the last character so no | is at the end
                stringBuilder.Length--;
            }

            // Convert the StringBuilder to a string
            string combinedString = stringBuilder.ToString();
            // Add the new String to the Regex Expression
            string badWordDetect = @"(" + combinedString + @")";
            // Execute the Regex expression
            foreach (Match m in Regex.Matches(message, badWordDetect, regexOptions))
            {
                bool allowed = false;
                if (!allowed)
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
        }

        return true;
    }
}