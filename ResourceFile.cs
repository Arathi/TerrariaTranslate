using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using NLog;

namespace TerrariaTranslate
{
    public class ResourceFile
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public string Name { get; set; }

        public IList<TranslateReference> References { get; set; }

        // public IDictionary<string, IDictionary<string, string>> Cache { get; set; }

        public ResourceFile(string name)
        {
            Name = name;
            References = new List<TranslateReference>();
        }

        public IDictionary<string, IDictionary<string, string>> buildDictionary()
        {
            IDictionary<string, IDictionary<string, string>> dict = new Dictionary<string, IDictionary<string, string>>();
            
            foreach (var transref in References)
            {
                string translatedText = transref.GetText();
                if (translatedText != null)
                {
                    if (!dict.ContainsKey(transref.Parent))
                    {
                        dict[transref.Parent] = new Dictionary<string, string>();
                    }
                    dict[transref.Parent][transref.Key] = translatedText;
                }
            }

            return dict;
        }

        public bool Save(string basePath = null)
        {
            try
            {
                if (basePath == null)
                {
                    basePath = System.IO.Directory.GetCurrentDirectory();
                }

                string fileName = "Terraria.Localization.Content.zh-Hans";
                if (Name != "Main")
                {
                    fileName += "." + Name;
                }
                fileName += ".json";

                var dict = buildDictionary();
                string json = JsonConvert.SerializeObject(dict, Formatting.Indented);

                string filePath = basePath + "/" + fileName;
                File.WriteAllText(filePath, json);

                return true;
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }
            return false;
        }
    }
}
