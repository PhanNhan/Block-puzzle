using Newtonsoft.Json;

namespace ZUserData
{
    public static class JsonService
    {
        public static string ToJson<T>(object obj, bool isPrettyPrint = false)
        {
            return JsonConvert.SerializeObject(obj, isPrettyPrint ? Formatting.Indented : Formatting.None);
        }

        public static T ToObject<T>(string str)
            where T : class
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
