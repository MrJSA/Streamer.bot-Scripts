using System;
using System.IO;
using System.Text;

public class CPHInline
{
    public bool Execute()
    {
        // Get raw input of the Bad Word
        string rawInput = args["rawInput"].ToString();
        // Get User Name
        string user = args["userName"].ToString();
        // Get Path to .txt file set in Global Variable
        string filePath = args["BadWordListPath"].ToString();
        // Set a temporary Path to for a .txt file
        string tempFilePath = Path.GetTempFileName();
        bool lineDeleted = false;
        // Open the original file for reading
        using (StreamReader reader = new StreamReader(filePath))
        {
            // Open a temporary file for writing
            using (StreamWriter writer = new StreamWriter(tempFilePath))
            {
                string line;
                bool foundStringToDelete = false;
                // Read lines from the original file and write to the temporary file,
                // excluding lines that match the string to delete
                while ((line = reader.ReadLine()) != null)
                {
                    if (!foundStringToDelete && line.Equals(rawInput, StringComparison.OrdinalIgnoreCase))
                    {
                        foundStringToDelete = true;
                        lineDeleted = true;
                        continue;
                    }

                    if (line.Trim() != "") // Skip empty lines
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }

        // Replace the original file with the temporary file
        File.Delete(filePath);
        File.Move(tempFilePath, filePath);
        // Send confirmation message
        if (lineDeleted)
        {
            CPH.SendMessage(user + " deleted \"" + rawInput + "\" from the BadWordList", false);
        }
        else
        {
            CPH.SendMessage("Word or Phrase not found", false);
        }

        return true;
    }
}