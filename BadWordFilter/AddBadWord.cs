using System;
using System.IO;

public class CPHInline
{
    public bool Execute()
    {
        string rawInput = args["rawInput"].ToString();
        CPH.SetArgument("newBannedWord", rawInput);
        string user = args["userName"].ToString();
        string filePath = args["wordsBlackList"].ToString();
        using (StreamWriter outputFile = new StreamWriter(filePath, true))
        {
            if (new FileInfo(filePath).Length > 0)
            {
                outputFile.Write("\n");
            }

            outputFile.Write(rawInput);
        }

        return true;
    }
}
