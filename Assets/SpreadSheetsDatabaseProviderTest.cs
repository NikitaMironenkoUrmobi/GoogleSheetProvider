using Redpenguin.GoogleSheets.Core;
using Redpenguin.GoogleSheets.Examples;
using UnityEngine;

public class SpreadSheetsDatabaseProviderTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var provider = new SpreadSheetsDatabaseProvider();
        var database = provider.Load("ExampleProfile");
        Debug.Log(database.GetSpreadSheetData<ExampleData>()[0].myString);;
    }

}
