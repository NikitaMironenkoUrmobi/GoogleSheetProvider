
# Google Sheets Provider #
#### 1. Create Google Service Account ###

  [Video link](https://www.youtube.com/watch?v=fxGeppjO0Mg&ab_channel=AzzraelCode)
  
#### 2. Get service account key ####  
 [Video Link](https://youtu.be/fxGeppjO0Mg?t=91)  
 
  - Click on just created service account under **Service accounts** tab.
  - Go to **Keys** tab.
  - Click **Add key->Create New key**.
  - Select **Json** (default).
  - Click **Create**.
  - Place it in your project.  
 

#### 3. Add Service Account to Google Sheets ####  
  
  [Video link](https://youtu.be/fxGeppjO0Mg?t=174)
  - Copy service account mail under Service Accounts tab.  
  - Add created service account to your GoogleSheets table.  
  

#### 4. Import GoogleSheetsProvider package ####

  Just import package to unity.
  If you already using Newtonsoft in your project ignore dll "Redpenguin/Dependencies/Newtonsoft.Json.dll" before importing or remove it after.

#### 5. Create profile  
  - Select profiles  
    
    ![Menu](https://user-images.githubusercontent.com/80315052/223647601-fd5922a1-e78d-4e15-8318-4053c1128866.png)  
  - Add new profile  
    
    ![Profile](https://user-images.githubusercontent.com/80315052/223649250-b437ec3e-e982-47e2-8da4-77307b546907.png)
  - Choose name and color for profile
  - Get google sheet id from link
  https:/docs.google.com/spreadsheets/d/*************************/... and paste it as table id
  - Enter your credential from google service account at credential field
  - Enter save path for db (relative to asset folder)
  - Enter file name
  - Select serialization rule

#### Provider Window ####    
![Provider](https://user-images.githubusercontent.com/80315052/223651051-9f49a8b7-3998-4a5f-b36b-b169f28854c9.png)
1. Profiles dropdown list
2. Open profiles window
3. Serialization all sheets that profile contains
4. Use ScriptableObject containers as intermediate object for load and serialization data
5. Load data from google table before save (only available if use SO containers)
6. Clear data from cache and playerPrefs.    

Create minimum one table script using data from google sheet  
  
  ***Field name*** in your data class and ***column name*** in sheet should be the ***same***!

  ```C#
  [SpreadSheet("Example")] //Get values from Example sheet
  [Serializable]
  public class ExampleData : ISheetData
  {
    public string myString;         // string
    public int myInt;               // 1
    public bool myBool;             // true
    public List<int> myInts;        // [1,2,3]
    public List<string> myStrings;  // ["a","b","c"]
    public JsonExample jsonExample; // {"id":1,"myString":"string"}
    public ExampleEnum exampleEnum; // Example1
  }
  ```
>_Make sure that sheet exist in table, either data won't display. If sheet exist and still doesn't display just press "Clear Meta" button on your profile in profiles tab_

#### Create provider script

   To access google sheets data required to create provider script

 ```C#
  public class ExampleDatabaseProvider
  {
    public ExampleDatabaseProvider()
    {
      var databaseProvider = new SpreadSheetsDatabaseProvider();
      var database = databaseProvider.Load("ExampleProfile");       //Load all sheets that profile contains 
      var exampleData = database.GetSpreadSheetData<ExampleData>(); //Get list of ExampleData
    }
  }
  ```
#### Custom serialization rule  
If you want serialize or deserialize your data in other way, u can create your own serialization rule.   
For that you need inheritate from ***SerializationRule*** class, and select this rule for your profile in profiles tab.
  ```C#
  public abstract class SerializationRule
  {
    public abstract void Serialization(string filePath, string fileName, object objectToWrite);
    public abstract T Deserialization<T>(string text);
  } 
  ```
