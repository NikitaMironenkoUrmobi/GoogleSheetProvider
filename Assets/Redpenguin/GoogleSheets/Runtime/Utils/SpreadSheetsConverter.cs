using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redpenguin.GoogleSheets.Core;

namespace Redpenguin.GoogleSheets
{
  public class SpreadSheetsConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var jo = JObject.Load(reader);
      var type = Type.GetType(jo["Type"].Value<string>());
      if (type == null) return null;
      return jo.ToObject(type, serializer);
    }

    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(ISheetDataContainer);
    }
  }
}