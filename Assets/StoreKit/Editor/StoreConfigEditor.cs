using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StoreConfig))]
public class StoreConfigEditor : Editor
{
    [MenuItem("Assets/Import Store Config Spreadsheet", true)]
    private static bool ValidateImportStoreConfigSpreadSheet()
    {
        return Selection.objects.Length == 1 &&
               AssetDatabase.GetAssetPath(Selection.objects[0]).EndsWith("xls");
    }

    [MenuItem("Assets/Import Store Config Spreadsheet")]
    private static void ImportStoreConfigSpreadSheet()
    {
        ImportXlsAndUpdateVirtualItems();
        UpdateStoreConfigAndVirtualItems();
    }

    public override void OnInspectorGUI()
    {
        StoreConfig config = target as StoreConfig;
        if (config != null)
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update"))
            {
                UpdateStoreConfigAndVirtualItems();
            }
        }
    }

    private static void ImportXlsAndUpdateVirtualItems()
    {
        string filePath = AssetDatabase.GetAssetPath(Selection.objects[0]);

        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = new HSSFWorkbook(stream);

            if (workbook.NumberOfSheets > 1)
            {
                Debug.LogWarning("Spreadsheet contains multiple sheets, we will only fetch data from 1st sheet");
            }
            else if (workbook.NumberOfSheets == 0)
            {
                Debug.LogError("Spreadsheet is empty!!!!");
                return;
            }

            ISheet sheet = workbook.GetSheetAt(0);

            for (int i = 1; i <= sheet.LastRowNum; i++) // loop from 1 to skip title row
            {
                IRow dataRow = sheet.GetRow(i);

                ICell cell = dataRow.GetCell(0);
                if (IsValidCell(cell))
                {
                    ParseVirtualItemByString(cell.StringCellValue, dataRow);
                }
                else
                {
                    Debug.LogWarning("row [" + (i + 1) + "] is not valid");
                }
            }
        }
    }

    private static void UpdateStoreConfigAndVirtualItems()
    {
        string storeConfigFilePath = VirtualItemDataPath + "StoreConfig.asset";
        StoreConfig storeConfig = AssetDatabase.LoadAssetAtPath(storeConfigFilePath, typeof(StoreConfig)) as StoreConfig;
        if (storeConfig == null)
        {
            storeConfig = CreateAsset<StoreConfig>(storeConfigFilePath);
        }

        // update store config
        storeConfig.Items.Clear();
        storeConfig.Items.AddRange(Resources.FindObjectsOfTypeAll<VirtualItem>());
        storeConfig.Items.Sort();

        storeConfig.Categories.Clear();
        storeConfig.Categories.AddRange(Resources.FindObjectsOfTypeAll<VirtualCategory>());

        // update categories
        foreach (var category in storeConfig.Categories)
        {
            category.Items.Clear();
        }
        foreach (var item in storeConfig.Items)
        {
            if (item.Category != null)
            {
                item.Category.Items.Add(item);
            }
        }
        foreach (var category in storeConfig.Categories)
        {
            EditorUtility.SetDirty(category);
        }

        CheckIdAndReferences(storeConfig);

        // update upgrades in virtual items
        foreach (var item in storeConfig.Items)
        {
            item.Upgrades.Clear();
        }
        foreach (var item in storeConfig.Items)
        {
            if (item.IsUpgradeType)
            {
                VirtualItem relatedItem = FindItemById(storeConfig, item.RelatedItemID);
                if (relatedItem != null)
                {
                    relatedItem.Upgrades.Add(item);
                }
                else
                {
                    Debug.LogError("upgrade item [" + item.ID + 
                        "]'s associated item id [" + item.RelatedItemID + "]'s item is null");
                }
            }
        } 
        foreach (var item in storeConfig.Items)
        {
            EditorUtility.SetDirty(item);
        }

        EditorUtility.SetDirty(storeConfig);
    }

    private static VirtualItem FindItemById(StoreConfig config, string id)
    {
        for (int i = 0; i < config.Items.Count; i++)
        {
            if (config.Items[i].ID.Equals(id))
            {
                return config.Items[i];
            }
        }
        return null;
    }

    private static void CheckIdAndReferences(StoreConfig storeConfig)
    {
        VirtualItem[] virtualItems = Resources.FindObjectsOfTypeAll<VirtualItem>();

        Dictionary<string, VirtualItem> idToItems = new Dictionary<string, VirtualItem>();
        foreach (var item in virtualItems)
        {
            if (idToItems.ContainsKey(item.ID))
            {
                Debug.LogError("Found duplicated key of item [" + item.ID + "]");
            }
            else
            {
                idToItems.Add(item.ID, item);
            }
        }

        foreach (var item in storeConfig.Items)
        {
            if (item.Type == VirtualItemType.ConsumablePack || 
                item.Type == VirtualItemType.VirtualCurrencyPack || 
                item.Type == VirtualItemType.UpgradeItem)
            {
                if (string.IsNullOrEmpty(item.RelatedItemID))
                {
                    Debug.LogError("Virtual item of type [" + item.Type + "], with id [" + item.ID +
                        "] must have an associated item");
                }
            }
            if (!string.IsNullOrEmpty(item.RelatedItemID) && !idToItems.ContainsKey(item.RelatedItemID))
            {
                Debug.LogError("Virtual item of type [" + item.Type + "], with id [" + item.ID +
                    "]'s associated item [" + item.RelatedItemID + "] doesn't exist");
            }
        }
    }

    private static void ParseVirtualItemByString(string typeString, IRow row)
    {
        try
        {
            VirtualItemType virtualItemType = (VirtualItemType)Enum.Parse(typeof(VirtualItemType), typeString);
            string virtualItemID = ParseID(row);
            if (!string.IsNullOrEmpty(virtualItemID))
            {
                Debug.Log("Parsing item type [" + virtualItemType + "] with id [" + virtualItemID + "]");
                string virtualItemName = ParseName(row, virtualItemID);
                string virtualItemDescription = ParseDescription(row, virtualItemID);
                ParseVirtualItem(virtualItemType, virtualItemID, virtualItemName, virtualItemDescription, row);
            }
            else
            {
                throw new Exception();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Parse virtual item type [" + typeString + "] failed: " + e.ToString());
        }
    }

    private static void ParseVirtualItem(VirtualItemType type, string id, string name, string description, IRow row)
    {
        string filePath = VirtualItemDataPath + id + ".asset";
        VirtualItem item = AssetDatabase.LoadAssetAtPath(filePath, typeof(VirtualItem)) as VirtualItem;
        if (item == null)
        {
            item = CreateAsset<VirtualItem>(filePath);
        }

        item.Type = type;
        item.ID = id;
        item.SortIndex = row.RowNum;
        item.Name = name;
        item.Description = description;
        item.Category = ParseRelatedCategory(row, id);

        switch (type)
        {
            case VirtualItemType.VirtualCurrency:
                break;
            case VirtualItemType.VirtualCurrencyPack:
            case VirtualItemType.ConsumablePack:
                item.RelatedItemID = ParseRelatedItemID(row, id);
                item.RelatedItemAmount = ParseRelatedAmount(row, id);
                ParsePurchaseInfo(row, id, ref item.PurchaseInfo);
                break;
            case VirtualItemType.ConsumableItem:
            case VirtualItemType.NonConsumableItem:
            case VirtualItemType.EquippableItem:
                ParsePurchaseInfo(row, id, ref item.PurchaseInfo);
                break; 
            case VirtualItemType.UpgradeItem:
                item.RelatedItemID = ParseRelatedItemID(row, id);
                ParsePurchaseInfo(row, id, ref item.PurchaseInfo);
                break;
            default:
                Debug.LogError("Unsupported virtual item type");
                break;
        }

        EditorUtility.SetDirty(item);
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

    private static string ParseID(IRow row)
    {
        string id;
        if (TryParseStringCell(row.GetCell(1), out id))
        {
            return id;
        }
        else
        {
            Debug.Log("Row [" + row.RowNum + "]'s id is blank");
            return string.Empty;
        }
    }

    private static string ParseName(IRow row, string itemID)
    {
        string name;
        if (TryParseStringCell(row.GetCell(2), out name))
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
        if (TryParseStringCell(row.GetCell(3), out description))
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
        if (TryParseStringCell(row.GetCell(4), out categoryId))
        {
            string filePath = VirtualItemDataPath + "category_" + categoryId + ".asset";
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

    private static string ParseRelatedItemID(IRow row, string itemID)
    {
        string relatedItemID;
        if (TryParseStringCell(row.GetCell(5), out relatedItemID))
        {
            return relatedItemID;
        }
        else
        {
            Debug.LogWarning("Item [" + itemID + "]'s related item id is blank");
            return string.Empty;
        }
    }

    private static int ParseRelatedAmount(IRow row, string itemID)
    {
        if (IsValidCell(row.GetCell(6)))
        {
            return (int)row.GetCell(6).NumericCellValue;
        }
        else
        {
            Debug.LogWarning("Item [" + itemID + "]'s related item amount is blank");
            return 0;
        }
    }

    private static void ParsePurchaseInfo(IRow row, string itemID, ref List<Purchase> purchaseInfo)
    {
        purchaseInfo.Clear();

        int currentCellIndex = 7;
        Purchase purchase = ParseOnePurchase(row, currentCellIndex, itemID);
        while (purchase != null)
        {
            purchaseInfo.Add(purchase);
            currentCellIndex += 3;
            purchase = ParseOnePurchase(row, currentCellIndex, itemID);
        }
    }

    private static void ParsePrimaryPurchase(IRow row, string itemID, ref Purchase primaryPurchase)
    {
        primaryPurchase = ParseOnePurchase(row, 7, itemID);
    }

    private static void ParseSecondaryPurchase(IRow row, string itemID, ref Purchase secondaryPurchase)
    {
        secondaryPurchase = ParseOnePurchase(row, 10, itemID);
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

    private static T CreateAsset<T>(string path) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        return asset;
    }

    private static readonly string VirtualItemDataPath = "Assets/StoreKit/Resources/";
}
