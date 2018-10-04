using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Components
{
    public class JsonFileReader
    {
        #region [ properties ]
        public JObject Root { get; private set; }
        #endregion

        #region [ constructor ]
        JsonFileReader(string file)
        {
            var nodeJson = JObject.Parse(File.ReadAllText(file));
        }
        #endregion

        #region [ public  methods]
        public T Get<T>(params string[] args)
        {
            JToken token = Root;
            foreach (var arg in args)
                token = token[arg];

            return JsonConvert.DeserializeObject<T>(token.ToString());
        }
        #endregion
    }
}
