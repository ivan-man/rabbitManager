using Newtonsoft.Json;
using System;
using System.Text;

namespace RabbitMqManager.Helpers
{
    public static class DeserializeHelper<T> where T : class
    {
        public static T Deserialize(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error
            };

            try
            {
                var body = JsonConvert.SerializeObject(obj);
                var message = JsonConvert.DeserializeObject<T>(body, settings) as T;
                return message;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }

    public static class DeserializeHelper
    {
        public static object  Deserialize(string obj)
        {
            try
            {
                var message = JsonConvert.DeserializeObject(obj);
                return message;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
