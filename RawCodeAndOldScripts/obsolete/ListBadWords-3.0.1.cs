using System;
using System.IO;
using System.Text;

public class CPHInline
{
    public bool Execute()
    {
        // Get User Name
        string user = args["userName"].ToString();
        // Get Path to .txt file set in Global Variable
        string filePath = args["BadWordListPath"].ToString();
        // Prepend String
        string prepend = "Here are the BadWords: ";
        // Open the original file for reading
        using (StreamReader reader = new StreamReader(filePath))
        {
            StringBuilder messageBuilder = new StringBuilder();
            string line;

            // Read lines from the file
            while ((line = reader.ReadLine()) != null)
            {
                // Remove newline characters from the line
                line = line.Replace("\n", "").Replace("\r", "");

                // Check if adding the line to the message would exceed 500 characters
                if (messageBuilder.Length + prepend.Length + line.Length + 1 > 500)
                {
                    // Output the current message
                    CPH.SendMessage(prepend + messageBuilder.ToString(), false);

                    // Start a new message with the current line
                    messageBuilder.Clear();
                    messageBuilder.Append(line);
                }
                else
                {
                    // Add the line to the current message
                    if (messageBuilder.Length > 0)
                    {
                        messageBuilder.Append(';');
                        messageBuilder.Append(' '); // Add separator if needed
                    }
                    messageBuilder.Append(line);
                }
            }

            // Output the last message if any
            if (messageBuilder.Length > 0)
            {
                CPH.SendMessage(prepend + messageBuilder.ToString(), false);
            }
            
        }

        return true;
    }
}
