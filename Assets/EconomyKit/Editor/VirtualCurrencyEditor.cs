using UnityEngine;
using UnityEditor;

namespace Beetle23
{
    [CustomEditor(typeof(VirtualCurrency))]
    public class VirtualCurrencyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            VirtualItem item = target as VirtualItem;
            EditorGUILayout.LabelField("ID", item.ID);
            EditorGUILayout.LabelField("Name", item.Name);
            EditorGUILayout.LabelField("Description", item.Description);
            if (item.Category != null)
            {
                EditorGUILayout.LabelField("Category", item.Category.ID);
            }
        }

        public static void DrawPurchaseInspector(PurchasableItem item)
        {
            if (item == null) return;

            string final = string.Empty;
            if (item.PurchaseInfo.Count > 0)
            {
                for (var i = 0; i < item.PurchaseInfo.Count; i++)
                {
                    Purchase purchase = item.PurchaseInfo[i];
                    if (purchase != null)
                    {
                        if (i > 0)
                        {
                            final += "\nor ";
                        }
                        final += purchase.IsMarketPurchase ?
                            string.Format("Real money {0} ({1})", purchase.Price, purchase.MarketID) :
                            string.Format("{0}x{1}",
                                purchase.VirtualCurrency != null ? purchase.VirtualCurrency.Name : "null", purchase.Price);
                    }
                }
            }
            else
            {
                final = "Free";
            }

            EditorGUILayout.LabelField("Purchase info", final);
        }
    }
}