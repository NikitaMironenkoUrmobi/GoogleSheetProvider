using System;
using System.Collections.Generic;
using Redpenguin.GoogleSheets.Core;

namespace Redpenguin.GoogleSheets
{
    public abstract class SpreadSheetScriptableObject<T> : SpreadSheetSoWrapper
        where T : ISheetData, new()
    {
        public SpreadSheetDataContainer<T> container;
        public override ISheetDataContainer SheetDataContainer => container;
        public override Type SheetDataType => container.SheetDataType;
        public override void InsertData(ISheetDataContainer insertContainer) => container.InsertData(insertContainer);
        private List<T> Data
        {
            get => container.Data;
            set => container.Data = value;
        }

        public override void SetListCount(int count)
        {
            var result = count - Data.Count;
            for (var i = 0; i < result; i++)
            {
                Data.Add(new T());
            }
        }
    }
}