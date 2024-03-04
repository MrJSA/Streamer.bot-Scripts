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
        // Get Path to .txt file set in Global Variable
        string filePath = args["global_BadWordListPath"].ToString();
        using (StreamWriter outputFile = new StreamWriter(filePath, true))
        {
            // Write new BadWord in File
            outputFile.WriteLine(rawInput);
        }

        // Send confirmation message
        CPH.SendMessage(user + " added " + rawInput + " to the BadWordList", false);
        return true;
    }
}