using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.Wrapper.JSonSerializer
{
    public class JSonSerializerNGS : IJsonSerializer
    {
        JsonSerializerOptions _jsonSerializerSettings { get; set; }

        public JSonSerializerNGS(IOptions<JsonSerializerOptions> jsonSettings) 
        { 
            _jsonSerializerSettings = jsonSettings.Value;
        }
        public T Deserialize<T>(string data)
            => JsonSerializer.Deserialize<T>(data, _jsonSerializerSettings);

        public string Serialize<T>(T data)
            => JsonSerializer.Serialize(data, _jsonSerializerSettings);
    }
    public interface IJsonSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string text);
    }
}
