using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System;
using System.IO;
using Excel;
using System.Data;

namespace Codeplay
{
    public abstract class ItemTreeExplorer
    {
        public Action<IItem> OnSelectionChange = delegate { };
        public IItem CurrentSelectedItem { get; protected set; }

        public ItemTreeExplorer()
        {
            _searchText = string.Empty;
        }

        public void SelectItem(IItem item)
        {
            if (item != CurrentSelectedItem)
            {
                CurrentSelectedItem = item;
                DoOnSelectItem(item);
                OnSelectionChange(item);
            }
        }

        public void Draw(Rect position)
        {
            GUILayout.BeginArea(position, string.Empty, "Box");

			GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check References", GUILayout.Width(150)))
            {
                GameKitConfigEditor.CheckIfAnyInvalidRef(GameKit.Config);
            }
			if (GUILayout.Button("Upload Data", GUILayout.Width(150)))
			{
				UploadDataFromExcel();
			}
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search", GUILayout.Width(position.width * 0.17f));
            _searchText = GUILayout.TextField(_searchText, 50, GUILayout.Width(position.width * 0.7f));
            if (GUILayout.Button("x", GUILayout.Height(15), GUILayout.Width(20)))
            {
                _searchText = string.Empty;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            /*
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+Expand All", GUILayout.Width(90)))
            {
                DoExpandAll();
            }
            if (GUILayout.Button("-Collapse All", GUILayout.Width(90)))
            {
                DoCollapseAll();
            }
            GUILayout.EndHorizontal();
            */

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
            DoDraw(new Rect(0, 0, position.width - 15, position.height - 50), _searchText);
            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        protected GUIStyle GetItemCenterStyle(IItem item)
        {
            return !string.IsNullOrEmpty(item.ID) && item == CurrentSelectedItem ?
                        GameKitEditorDrawUtil.ItemSelectedCenterStyle : GameKitEditorDrawUtil.ItemCenterLabelStyle;
        }

        protected GUIStyle GetItemLeftStyle(IItem item)
        {
            return !string.IsNullOrEmpty(item.ID) && item == CurrentSelectedItem ?
                        GameKitEditorDrawUtil.ItemSelectedLeftStyle : GameKitEditorDrawUtil.ItemLeftLabelStyle;
        }

        protected void DrawItemIfMathSearch(string searchText, IItem item, float width)
        {
            if (item.ID.Contains(searchText) && GUILayout.Button(" " + item.ID, GetItemLeftStyle(item),
                    GUILayout.Height(22), GUILayout.Width(width)))
            {
                SelectItem(item);
            }
        }

        protected abstract void DoOnSelectItem(IItem item);
        protected abstract void DoExpandAll();
        protected abstract void DoCollapseAll();
        protected abstract void DoDraw(Rect position, string search);

		private void UploadDataFromExcel()
		{
			GameKit.Config.ClearVirtualItems();

			FileStream stream = File.Open(Application.dataPath + "/ShopContents.xlsx", FileMode.Open, FileAccess.Read);
			IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
			DataTable mainTable = excelReader.AsDataSet().Tables["商品主表"];
			DataTable mainChineseToEnglishTable = excelReader.AsDataSet().Tables["Chinese-English"];
			int categoryColumn = GetIndexByColumnName(mainTable.Rows[0].ItemArray, "一级分类");
			int idColumn = GetIndexByColumnName(mainTable.Rows[0].ItemArray, "ID");
			for (int i = 1; i < mainTable.Rows.Count; i++)
			{
				LifeTimeItem item = new LifeTimeItem();
				item.ID = "LiftTimeItem" + mainTable.Rows[i][idColumn].ToString();
				string category = mainTable.Rows[i][categoryColumn].ToString();
				if (category.Equals("角色"))
				{
					stream = File.Open(Application.dataPath + "/Characters.xlsx", FileMode.Open, FileAccess.Read);
					excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
					DataTable characterPorpertyTable = excelReader.AsDataSet().Tables["角色属性表"];
					int idInMainTableColumn = GetIndexByColumnName(characterPorpertyTable.Rows[1].ItemArray, "商品ID");
					int nameColumn = GetIndexByColumnName(characterPorpertyTable.Rows[1].ItemArray, "角色名");
					for (int k = 0; k < characterPorpertyTable.Rows.Count; k++)
					{
						if (characterPorpertyTable.Rows[k][idInMainTableColumn].ToString().Equals(mainTable.Rows[i][idColumn].ToString()))
						{
							item.Name = characterPorpertyTable.Rows[k][nameColumn].ToString();
							break;
						}
					}
				}

				VirtualCategory virtualCategory;
				if (!GameKit.Config.TryGetCategoryByCategoryID(ChineseToEnglish(mainChineseToEnglishTable, category), out virtualCategory))
				{
					virtualCategory = new VirtualCategory();
					virtualCategory.ID = ChineseToEnglish(mainChineseToEnglishTable, category);
					virtualCategory.Name = category;
					GameKit.Config.Categories.Add(virtualCategory);
				}
				virtualCategory.ItemIDs.Add(item.ID);
				GameKit.Config.LifeTimeItems.Add(item);
				GameKit.Config.UpdateMapsAndTree();
			}
		}
			
		private string ChineseToEnglish(DataTable table, string chineseText)
		{
			for (int i = 0; i < table.Rows.Count; i++)
			{
				if (table.Rows[i][0].Equals(chineseText))
				{
					return table.Rows[i][1].ToString();
				}
			}
			return chineseText;
		}

		private int GetIndexByColumnName(object[] titleRow, string columnName)
		{
			int index = 0;
			for (int i = 0; i < titleRow.Length; i++)
			{
				if (titleRow[i].ToString().Equals(columnName))
				{
					index = i;
					break;
				}
			}
			return index;
		}

        private Vector2 _scrollPosition;
        private string _searchText;
    }
}