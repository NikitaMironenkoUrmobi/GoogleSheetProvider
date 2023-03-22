namespace Redpenguin.GoogleSheets.Core
{
    public static class SpreadSheetsDatabaseExtensions
    {
        public static void Merge(this SpreadSheetsDatabase targetDatabase,
            SpreadSheetsDatabase database)
        {
            foreach (var sheetDataContainer in database.Containers)
            {
                targetDatabase.InsertContainer(sheetDataContainer);
            }
        }
    }
}