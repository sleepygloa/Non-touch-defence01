using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class StageTable_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Resources/Data/StageTable.xls";
	private static readonly string exportPath = "Assets/Resources/Data/StageTable.asset";
	private static readonly string[] sheetNames = { "sheet", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;

            StageTable data = (StageTable)AssetDatabase.LoadAssetAtPath (exportPath, typeof(StageTable));
			if (data == null) {
				data = ScriptableObject.CreateInstance<StageTable> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

                    StageTable.Sheet s = new StageTable.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;

                        StageTable.Param p = new StageTable.Param ();
						
					cell = row.GetCell(0); p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Level = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.EntityId = (cell == null ? "" : cell.StringCellValue);
                    cell = row.GetCell(3); p.EntityCnt =(int) (cell == null ? 0 : cell.NumericCellValue);
                    cell = row.GetCell(4); p.BossEntityId = (cell == null ? "" : cell.StringCellValue);
                    cell = row.GetCell(5); p.BossCnt = (int)(cell == null ? 0 : cell.NumericCellValue);
                        /*
					cell = row.GetCell(2); p.EntityType = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.HP = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Level = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.Prefab = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(6); p.SearchRange = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.AttackPower = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.AttackSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
                        */
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
