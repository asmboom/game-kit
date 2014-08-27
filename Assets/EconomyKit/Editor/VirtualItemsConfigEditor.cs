using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VirtualItemsConfig))]
public class VirtualItemsConfigEditor : Editor
{
    [MenuItem("Assets/Economy Kit/Import Virtual Items Config Spreadsheet", true)]
    private static bool ValidateImportVirtualItemsConfigSpreadSheet()
    {
        return Selection.objects.Length == 1 &&
               AssetDatabase.GetAssetPath(Selection.objects[0]).EndsWith("xls");
    }

    [MenuItem("Assets/Economy Kit/Import Virtual Items Config Spreadsheet")]
    private static void ImportVirtualItemsConfigSpreadSheet()
    {
        ImportXlsAndUpdateVirtualItems();
        UpdateVirtualItemsConfig();
    }

    public override void OnInspectorGUI()
    {
        VirtualItemsConfig config = target as VirtualItemsConfig;
        if (config != null)
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update"))
            {
                UpdateVirtualItemsConfig();
            }
        }
    }

    private static void ImportXlsAndUpdateVirtualItems()
    {
        string filePath = AssetDatabase.GetAssetPath(Selection.objects[0]);

        try
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new HSSFWorkbook(stream);

                if (workbook.NumberOfSheets == 0)
                {
                    Debug.LogError("Spreadsheet is empty!!!!");
                    return;
                }

                ImportVirtualItem<VirtualCurrency>(workbook, "Virtual Currency");
                ImportVirtualItem<SingleUseItem>(workbook, "Single Use", (dataRow, item) =>
                {
                    ParsePurchaseInfo(dataRow, 4, item.ID, ref item.PurchaseInfo);
                });
                ImportVirtualItem<LifeTimeItem>(workbook, "Life Time", (dataRow, item) =>
                {
                    item.IsEquippable = ParseIsEquippable(dataRow, item.ID);
                    ParsePurchaseInfo(dataRow, 5, item.ID, ref item.PurchaseInfo);
                });
                ImportVirtualItem<UpgradeItem>(workbook, "Upgrade Item", (dataRow, item) =>
                {
                    item.RelatedItemID = ParseRelatedItemID(dataRow, item.ID);
                    ParsePurchaseInfo(dataRow, 5, item.ID, ref item.PurchaseInfo);
                });
                ImportVirtualItem<VirtualItemPack>(workbook, "Pack", (dataRow, item) =>
                {
                    ParsePurchaseInfo(dataRow, 4, item.ID, ref item.PurchaseInfo);
                    ParsePackElements(dataRow, 10, item.ID, ref item.PackElements);
                });
            }
        }
        catch (IOException)
        {
            EditorUtility.DisplayDialog("ERROR", "The xls file is currently opened by another program, please close it first.", "OK");
        }
    }

    private static List<T> ImportVirtualItem<T>(IWorkbook workbook, string sheetName, Action<IRow, T> parseMore = null) where T : VirtualItem
    {
        List<T> items = new List<T>();

        ISheet sheet = workbook.GetSheet(sheetName);
        if (sheet == null)
        {
            Debug.LogError("Couldn't find sheet with name [" + sheetName + "] from xls file");
            return items;
        }

        _currentSheetName = sheetName;
        for (int i = 1; i <= sheet.LastRowNum; i++) // loop from 1 to skip title row
        {
            IRow dataRow = sheet.GetRow(i);
            try
            {
                string virtualItemID = ParseID(dataRow);
                if (!string.IsNullOrEmpty(virtualItemID))
                {
                    string filePath = VirtualItemDataPath + virtualItemID + ".asset";
                    T item = AssetDatabase.LoadAssetAtPath(filePath, typeof(T)) as T;
                    if (item == null)
                    {
                        item = CreateAsset<T>(filePath);
                    }

                    item.ID = virtualItemID;
                    item.Name = ParseName(dataRow, virtualItemID);
                    item.Description = ParseDescription(dataRow, virtualItemID);
                    item.SortIndex = dataRow.RowNum;
                    item.Category = ParseRelatedCategory(dataRow, virtualItemID);

                    if (parseMore != null)
                    {
                        parseMore(dataRow, item);
                    }

                    EditorUtility.SetDirty(item);

                    items.Add(item);
                }
            }
            catch (Exception) { }
        }

        return items;
    }

    private static void UpdateVirtualItemsConfig()
    {
        VirtualItemsConfig virtualItemsConfig = GetVirtualItemsConfigAndCreateIfNonExist();
        ClearVirtualItemConfigLists(virtualItemsConfig);

        // update virtual items list
        virtualItemsConfig.VirtualCurrencies.AddRange(Resources.FindObjectsOfTypeAll<VirtualCurrency>());
        virtualItemsConfig.VirtualCurrencies.Sort();
        virtualItemsConfig.SingleUseItems.AddRange(Resources.FindObjectsOfTypeAll<SingleUseItem>());
        virtualItemsConfig.SingleUseItems.Sort();
        virtualItemsConfig.LifeTimeItems.AddRange(Resources.FindObjectsOfTypeAll<LifeTimeItem>());
        virtualItemsConfig.LifeTimeItems.Sort();
        virtualItemsConfig.UpgradeItems.AddRange(Resources.FindObjectsOfTypeAll<UpgradeItem>());
        virtualItemsConfig.UpgradeItems.Sort();
        virtualItemsConfig.ItemPacks.AddRange(Resources.FindObjectsOfTypeAll<VirtualItemPack>());
        virtualItemsConfig.ItemPacks.Sort();

        virtualItemsConfig.UpdateIdToItemMap();

        // update categories
        virtualItemsConfig.Categories.Clear();
        virtualItemsConfig.Categories.AddRange(Resources.FindObjectsOfTypeAll<VirtualCategory>());

        foreach (var category in virtualItemsConfig.Categories)
        {
            category.Items.Clear();
        }
        foreach (var item in virtualItemsConfig.Items)
        {
            if (item.Category != null)
            {
                item.Category.Items.Add(item);
            }
        }
        foreach (var category in virtualItemsConfig.Categories)
        {
            EditorUtility.SetDirty(category);
        }

        CheckIfAnyInvalidRef(virtualItemsConfig);

        // update upgrades in virtual items
        foreach (var item in virtualItemsConfig.Items)
        {
            item.Upgrades.Clear();
        }
        foreach (var item in virtualItemsConfig.UpgradeItems)
        {
            VirtualItem relatedItem = item.RelatedItem;
            if (item.RelatedItem != null)
            {
                relatedItem.Upgrades.Add(item);
            }
            else
            {
                Debug.LogError("upgrade item [" + item.ID +
                    "]'s associated item is null");
            }
        }
        foreach (var item in virtualItemsConfig.Items)
        {
            EditorUtility.SetDirty(item);
        }

        EditorUtility.SetDirty(virtualItemsConfig);
    }

    private static VirtualItemsConfig GetVirtualItemsConfigAndCreateIfNonExist()
    {
        string configFilePath = VirtualItemDataPath + "VirtualItemsConfig.asset";
        VirtualItemsConfig virtualItemsConfig = AssetDatabase.LoadAssetAtPath(configFilePath, typeof(VirtualItemsConfig)) as VirtualItemsConfig;
        if (virtualItemsConfig == null)
        {
            virtualItemsConfig = CreateAsset<VirtualItemsConfig>(configFilePath);
        }
        return virtualItemsConfig;
    }

    private static void ClearVirtualItemConfigLists(VirtualItemsConfig virtualItemsConfig)
    {
        virtualItemsConfig.VirtualCurrencies.Clear();
        virtualItemsConfig.SingleUseItems.Clear();
        virtualItemsConfig.LifeTimeItems.Clear();
        virtualItemsConfig.UpgradeItems.Clear();
        virtualItemsConfig.ItemPacks.Clear();
        virtualItemsConfig.Categories.Clear();
    }

    private static void CheckIfAnyInvalidRef(VirtualItemsConfig config)
    {
        foreach(var item in config.UpgradeItems)
        {
            if (item.RelatedItem == null)
            {
                Debug.LogError("Upgrade item [" + item.ID + "]'s related item couldn't be found.");
            }
        }
        foreach (var pack in config.ItemPacks)
        {
            foreach (var element in pack.PackElements)
            {
                if (element.Item == null)
                {
                    Debug.LogError("Pack [" + pack.ID + "]'s element item [" + element.Item.ID + "] couldn't be found.");
                }
            }
        }
    }

    private static bool IsValidCell(ICell cell)
    {
        return cell != null && cell.CellType != CellType.Unknown && cell.CellType != CellType.BLANK;
    }

    private static bool TryParseStringCell(ICell cell, out string cellData)
    {
        cellData = string.Empty;
        if (IsValidCell(cell))
        {
            try
            {
                cellData = cell.StringCellValue;
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private static bool TryParseDoubleCell(ICell cell, out double cellData)
    {
        if (IsValidCell(cell))
        {
            cellData = cell.NumericCellValue;
            return true;
        }
        else
        {
            cellData = 0;
            return false;
        }
    }

    private static bool TryParseBoolCell(ICell cell, out bool cellData)
    {
        if (IsValidCell(cell))
        {
            cellData = cell.BooleanCellValue;
            return true;
        }
        else
        {
            cellData = false;
            return false;
        }
    }

    private static string ParseID(IRow row)
    {
        string id;
        if (TryParseStringCell(row.GetCell(0), out id))
        {
            return id;
        }
        else
        {
            Debug.Log("Sheet [" + _currentSheetName + "] Row [" + (row.RowNum + 1) + "]'s id is blank");
            return string.Empty;
        }
    }

    private static string ParseName(IRow row, string itemID)
    {
        string name;
        if (TryParseStringCell(row.GetCell(1), out name))
        {
            return name;
        }
        else
        {
            Debug.LogWarning("Item [" + itemID + "]'s name is blank");
            return string.Empty;
        }
    }

    private static string ParseDescription(IRow row, string itemID)
    {
        string description;
        if (TryParseStringCell(row.GetCell(2), out description))
        {
            return description;
        }
        else
        {
            Debug.LogWarning("Item [" + itemID + "]'s descrption is blank");
            return string.Empty;
        }
    }

    private static VirtualCategory ParseRelatedCategory(IRow row, string itemID)
    {
        string categoryId;
        if (TryParseStringCell(row.GetCell(3), out categoryId))
        {
            string filePath = VirtualItemDataPath + "Category" + categoryId + ".asset";
            VirtualCategory category = AssetDatabase.LoadAssetAtPath(filePath, typeof(VirtualCategory)) as VirtualCategory;
            if (category == null)
            {
                category = CreateAsset<VirtualCategory>(filePath);
            }
            category.ID = categoryId;
            return category;
        }
        else
        {
            return null;
        }
    }

    private static bool ParseIsEquippable(IRow row, string itemID)
    {
        bool isEuippable = false;
        if (TryParseBoolCell(row.GetCell(4), out isEuippable))
        {
            return isEuippable;
        }
        return false;
    }

    private static string ParseRelatedItemID(IRow row, string itemID)
    {
        string relatedItemID;
        if (TryParseStringCell(row.GetCell(4), out relatedItemID))
        {
            return relatedItemID;
        }
        else
        {
            Debug.LogWarning("Item [" + itemID + "]'s related item id is blank");
            return string.Empty;
        }
    }

    private static void ParsePurchaseInfo(IRow row, int cellIndex, string itemID, 
        ref List<Purchase> purchaseInfo)
    {
        purchaseInfo.Clear();

        int currentCellIndex = cellIndex;
        Purchase purchase = ParseOnePurchase(row, currentCellIndex, itemID);
        while (purchase != null)
        {
            purchase = ParseOnePurchase(row, currentCellIndex, itemID);
            purchaseInfo.Add(purchase);
            currentCellIndex += 3;
        }
    }

    private static Purchase ParseOnePurchase(IRow row, int currentCellIndex, string itemID)
    {
        string purchaseTypeStr;
        if (TryParseStringCell(row.GetCell(currentCellIndex), out purchaseTypeStr))
        {
            PurchaseType purchaseType = (PurchaseType)Enum.Parse(typeof(PurchaseType), purchaseTypeStr);
            Purchase purchase = new Purchase();
            purchase.Type = purchaseType;
            purchase.Price = ParsePurchasePrice(row, currentCellIndex + 1, itemID);
            purchase.AssociatedID = ParsePurchaseAssociatedID(row, currentCellIndex + 2, itemID);
            return purchase;
        }
        return null;
    }

    private static float ParsePurchasePrice(IRow row, int cellIndex, string itemID)
    {
        double priceDouble;
        if (TryParseDoubleCell(row.GetCell(cellIndex), out priceDouble))
        {
            return (float)priceDouble;
        }
        else
        {
            Debug.LogWarning("Item [" + itemID + "]'s purchase price at cell [" + (cellIndex + 1) + "] is blank");
            return 0;
        }
    }

    private static string ParsePurchaseAssociatedID(IRow row, int cellIndex, string itemID)
    {
        string purchaseAssociatedID;
        if (TryParseStringCell(row.GetCell(cellIndex), out purchaseAssociatedID))
        {
            return purchaseAssociatedID;
        }
        else
        {
            Debug.LogWarning("Item [" + itemID + "]'s purchase associated id at cell [" + (cellIndex + 1) + "] is blank");
            return string.Empty;
        }
    }

    private static void ParsePackElements(IRow row, int cellIndex, string packID, ref List<PackElement> packElements)
    {
        packElements.Clear();

        int currentCellIndex = cellIndex;
        PackElement element = ParseOnePackElement(row, currentCellIndex, packID);
        while (element != null)
        {
            packElements.Add(element);
            currentCellIndex += 2;
            element = ParseOnePackElement(row, currentCellIndex, packID);
        }
    }

    private static PackElement ParseOnePackElement(IRow row, int currentCellIndex, string packID)
    {
        string itemID;
        if (TryParseStringCell(row.GetCell(currentCellIndex), out itemID))
        {
            PackElement element = new PackElement()
            {
                ItemID = itemID,
                Amount = ParseItemAmount(row, currentCellIndex + 1, packID)
            };
            return element;
        }
        return null;
    }

    private static int ParseItemAmount(IRow row, int cellIndex, string packID)
    {
        double amountDouble;
        if (TryParseDoubleCell(row.GetCell(cellIndex), out amountDouble))
        {
            return (int)amountDouble;
        }
        else
        {
            Debug.LogWarning("Pack [" + packID + "]'s item amount at cell [" + (cellIndex + 1) + "] is blank");
            return 0;
        }
    }

    private static T CreateAsset<T>(string path) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        return asset;
    }

    private static readonly string VirtualItemDataPath = "Assets/EconomyKit/Resources/";
    private static string _currentSheetName;
}
