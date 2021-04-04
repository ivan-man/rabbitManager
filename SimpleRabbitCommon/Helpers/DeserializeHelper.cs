using Newtonsoft.Json;
using System;

namespace SimpleRabbit.Common.Helpers
{
    public static class DeserializeHelper<T> where T : class
    {
        /// <summary>
        /// Deserialize string to object.
        /// </summary>
        /// <param name="stringToDeserialize">Serialized object.</param>
        /// <returns>Object of type T.</returns>
        public static T Deserialize(string stringToDeserialize)
        {
            if (string.IsNullOrWhiteSpace(stringToDeserialize))
            {
                throw new ArgumentException($"{nameof(stringToDeserialize)} is empty.");
            }

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error
            };

            try
            {
                var result = JsonConvert.DeserializeObject<T>(stringToDeserialize, settings) as T;
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }

    public static class DeserializeHelper
    {
        /// <summary>
        /// Deserialize string to object.
        /// </summary>
        /// <param name="stringToDeserialize">Serialized object.</param>
        public static object Deserialize(string stringToDeserialize)
        {
            try
            {
                var result = JsonConvert.DeserializeObject(stringToDeserialize);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
