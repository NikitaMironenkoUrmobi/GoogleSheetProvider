using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Redpenguin.GoogleSheets.Core
{
    public interface ISheetDataProvider<T> : ISheetDataContainer where T : ISheetData
    {
        public List<T> Data { get; set; }
    }

    public interface ISheetDataContainer
    {
        public Type SheetDataType { get; }
        void SetListCount(int count);
        void InsertData(ISheetDataContainer insertContainer);
    }

    [Serializable]
    public class SpreadSheetDataContainer<T> : ISheetDataProvider<T> where T : ISheetData, new()
    {
        public List<T> data = new();

        [JsonIgnore]
        public List<T> Data
        {
            get => data;
            set => data = value;
        }

        [JsonIgnore] public Type SheetDataType => typeof(T);
        [JsonProperty] public Type ContainerType => GetType();

        public SpreadSheetDataContainer(List<T> data)
        {
            this.data = data;
        }

        public SpreadSheetDataContainer()
        {
        }

        public void SetListCount(int count)
        {
            var result = count - Data.Count;
            for (var i = 0; i < result; i++)
            {
                Data.Add(new T());
            }
        }

        public void InsertData(ISheetDataContainer insertContainer)
        {
            if (insertContainer is not SpreadSheetDataContainer<T> inputContainer) return;
            foreach (var containerData in inputContainer.Data)
            {
                data.Add(containerData);
            }
        }
    }
}