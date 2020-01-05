using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

public class EntityTableImporter : AssetPostprocessor {
	private static readonly string filePath = "Assets/95.RTS/9.ResourcesData/Resources/Data/EntityTable.xls";
	private static readonly string exportPath = "Assets/95.RTS/9.ResourcesData/Resources/Data/EntityTable.asset";
	private static readonly string[] sheetNames = { "sheet", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			EntityTable data = (EntityTable)AssetDatabase.LoadAssetAtPath (exportPath, typeof(EntityTable));
			if (data == null) {
				data = ScriptableObject.CreateInstance<EntityTable> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read)) {
				IWorkbook book = new HSSFWorkbook (stream);
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[Data] sheet not found:" + sheetName);
						continue;
					}

					EntityTable.Sheet s = new EntityTable.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i< sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						EntityTable.Param p = new EntityTable.Param ();
						
					cell = row.GetCell(0); p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.EntityCategory = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.EntityType = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.HP = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Level = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.Prefab = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(6); p.SearchRange = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.AttackPower = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.AttackSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
