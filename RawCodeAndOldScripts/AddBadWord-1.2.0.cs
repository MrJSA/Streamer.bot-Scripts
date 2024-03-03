using System;
using System.IO;

public class CPHInline
{
    public bool Execute()
    {
        // Get raw input of the Bad Word
        string rawInput = args["rawInput"].ToString();
        // Get User Name
        string user = args["userName"].ToString();
        // Get .txt file Path specified here or in Arguments
        string filePath = args["wordsBlackList"].ToString();
        using (StreamWriter outputFile = new StreamWriter(filePath, true))
        {
            // Check if File is Empty if no
            if (new FileInfo(filePath).Length > 0)
            {
                // make new Line
                outputFile.Write("\n");
            }

            // Write new BadWord in File
            outputFile.Write(rawInput);
        }

        // Send confirmation message
        CPH.SendMessage(user + " added " + rawInput + " to the BadWordList", false);
        return true;
    }
}