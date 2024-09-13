using System;
using System.IO;
using System.Linq;

namespace MountandShardblade.Util
{
    /*
     * Reading a config file in a submodule
     */
    public class Config
    {
        public Config()
        {
            filePath = "../../Modules/config.txt";
        }

        public Config(String configFilePath)
        {
            filePath = configFilePath;
        }

        public Config(String configFilePath, Logger logger)
        {
            filePath = configFilePath;
            this.logger = logger;
        }

        public String GetField(String fieldName)
        {
            String value = null;
            String errorMessage = null;
            if (File.Exists(filePath))
            {
                String[] lines = File.ReadAllLines(filePath);
                foreach (String line in lines)
                {
                    // Does the field exist in the config file
                    if (line.StartsWith(fieldName))
                    {
                        // If this condition fails it means the config file is malformed
                        if (line.Contains(delimiter) && line.Split(delimiter).Length == 2)
                        {
                            value = line.Split(delimiter).Last();
                            break;
                        }
                        else
                        {
                            if (!line.Contains(delimiter))
                            {
                                errorMessage = "Invalid delimiter in line - " + line + " - should be " + delimiter;
                            }
                            else
                            {
                                errorMessage = "Invalid delimiter count in line - " + line + " - should only be one instance of " + delimiter;
                            }
                        }
                    }

                }

                if (value == null)
                {
                    errorMessage = fieldName + " not found in " + filePath;
                }
            }
            else
            {
                errorMessage = filePath + " not found";
            }

            if (logger != null && errorMessage != null)
            {
                logger.Log(errorMessage, LogSeverity.Error);
            }

            return value;
        }

        private Logger logger = null;
        private readonly char delimiter = '=';
        private readonly String filePath;
    }
}
