using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ARS.Assembly
{
    public class AssemblyAlgorithm : IDisposable
    {

        private readonly string fileConfig;
        private readonly string GUID;
        private readonly Dictionary<string, string> dictionaryArray;

        public AssemblyAlgorithm(string GUID)
        {
            dictionaryArray = new Dictionary<string, string>();
            this.GUID = GUID;
            fileConfig = Path.Combine(Environment.CurrentDirectory, GUID + ".loe");
            if (File.Exists(fileConfig))
                using (var readStreamData = new StreamReader(File.OpenRead(fileConfig)))
                {
                    string currentLine;
                    int currentLineNumber = 1;
                    while ((currentLine = readStreamData.ReadLine()) != null)
                    {
                        int j = currentLine.IndexOf(":");
                        if (j == -1)
                            throw new ArgumentException("Invalid settings.");
                        string currentValue = currentLine.Substring(j + 1);

                        dictionaryArray.Add(currentLine.Substring(0, j),
                            currentValue.Equals("null", StringComparison.InvariantCultureIgnoreCase) ? null : currentValue);
                        currentLineNumber++;
                    }
                }
            else
                throw new ArgumentException("Invalid token.");
        }

        public void Dispose()
        {
            try
            {
                using (var writer = new StreamWriter(File.OpenWrite(fileConfig)))
                    foreach (var i in dictionaryArray)
                        writer.WriteLine("{0}:{1}", i.Key, i.Value == null ? "null" : i.Value);
            }
            catch { }
        }

        public string getDataValue(string uniqueKey, string uniqueDefinition = null)
        {
            string returnValue;
            if (!dictionaryArray.TryGetValue(uniqueKey, out returnValue))
            {
                if (uniqueDefinition == null)
                    throw new ArgumentException(string.Format("'{0}' does not exist.", uniqueKey));
                returnValue = dictionaryArray[uniqueKey] = uniqueDefinition;
            }
            return returnValue;
        }

        public T getDataTask<T>(string uniqueKey, string uniqueDefinition = null)
        {
            string returnTask;
            if (!dictionaryArray.TryGetValue(uniqueKey, out returnTask))
            {
                if (uniqueDefinition == null)
                    throw new ArgumentException(string.Format("'{0}' does not exist.", uniqueKey));
                returnTask = dictionaryArray[uniqueKey] = uniqueDefinition;
            }
            return (T) Convert.ChangeType(returnTask, typeof (T));
        }

        public void setDataValue(string uniqueKey, string currentValue)
        {
            dictionaryArray[uniqueKey] = currentValue;
        }
    }
}