using System.Collections.Generic;
using System.IO;

namespace Walterlv.CloudTyping
{
    public class FileConfigurationRepo
    {
        public static Dictionary<string, string> Deserialize(string fileName)
        {
            var keyValue = new Dictionary<string, string>();
            var lines = File.ReadAllLines(fileName);

            string currentKey = null;
            string currentValue = null;
            foreach (var line in lines)
            {
                if (line.StartsWith(">"))
                {
                    if (currentKey != null)
                    {
                        keyValue[currentKey] = currentValue ?? "";
                    }

                    currentKey = null;
                    currentValue = null;
                    continue;
                }

                if (currentKey == null)
                {
                    currentKey = line.Trim();
                }
                else
                {
                    currentValue = currentValue == null
                        ? line.Trim()
                        : $@"{currentValue}
{line.Trim()}";
                }
            }

            if (currentKey != null)
            {
                keyValue[currentKey] = currentValue ?? "";
            }

            return keyValue;
        }
    }
}
