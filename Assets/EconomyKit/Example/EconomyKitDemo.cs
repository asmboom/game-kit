using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class EconomyKitDemo : MonoBehaviour
{
    private void Awake()
    {
        EconomyKit.Init(new EconomyKitDemoFactory());

        _coin = EconomyKit.Config.GetItemByID("Coin");
        _gem = EconomyKit.Config.GetItemByID("Gem");
        _primaryCharacterItem = EconomyKit.Config.GetItemByID("Wuzheng") as LifeTimeItem;

        _items = new List<PurchasableItem>();
        foreach(var item in EconomyKit.Config.Items)
        {
            if (item is PurchasableItem &&
                !(item is UpgradeItem))
            {
                _items.Add(item as PurchasableItem);
            }
        }

        if (!IsLaunched())
        {
            FirstTimeLaunch();
        }

        Market.Instance.StartProductListRequest((list) =>
        {
            Debug.Log("Get product list succeeded");
        }, () =>
        {
            Debug.LogError("Get product list failed");
        });
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _touchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!_isDragging)
            {
                if (Mathf.Abs(_touchPosition.y - Input.mousePosition.y) > 10f)
                {
                    _isDragging = true;
                }
            }
            else
            {
                _pageScrollPosition.y -= _touchPosition.y - Input.mousePosition.y;
                _touchPosition = Input.mousePosition;
            }
        }
    }

    private void FirstTimeLaunch()
    {
        Debug.Log("First time launch");

        _primaryCharacterItem.Give();
        _primaryCharacterItem.Equip();
        _coin.Give(4000);
        _gem.Give(5);

        SetLaunched();
    }

    private void OnGUI()
    {
        DrawVirtualCurrencyIcon(_coin.ID, 0, 0);
        GUI.Label(new Rect(25, 0, 100, 20), string.Format("{0}", _coin.Balance));
        DrawVirtualCurrencyIcon(_gem.ID, 125, 0);
        GUI.Label(new Rect(150, 0, 100, 20), string.Format("{0}", _gem.Balance));

        if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 20), "Reset"))
        {
            PlayerPrefs.DeleteAll();
            _primaryCharacterItem.Give();
            _primaryCharacterItem.Equip();
        }
        if (GUI.Button(new Rect(Screen.width - 100, 25, 100, 20), "Gimme Coin"))
        {
            _coin.Give(4000);
        }
        if (GUI.Button(new Rect(Screen.width - 100, 50, 100, 20), "Gimme Gem"))
        {
            _gem.Give(5);
        }

        DrawItems();
    }

    private void DrawVirtualCurrencyIcon(string id, float x, float y)
    {
        GUI.DrawTexture(new Rect(x, y, 20, 20),
                       Resources.Load<Texture2D>(id));
    }

    private void DrawItems()
    {
        float productSize = Screen.width * 0.30f;
        float totalHeight = _items.Count * productSize;
        float y = 0;

        _pageScrollPosition = GUI.BeginScrollView(new Rect(0,
            Screen.height * 2f / 8f, Screen.width, Screen.height * 5f / 8f),
            _pageScrollPosition, new Rect(0, 0, Screen.width - 50, totalHeight));

        for (int i = 0; i < _items.Count; i++)
        {
            PurchasableItem item = _items[i];

            Color oriColor = GUI.color;

            GUI.DrawTexture(new Rect(0 + productSize / 8f, y + productSize / 8f, productSize * 6f / 8f, productSize * 6f / 8f),
                Resources.Load<Texture2D>(item.ID));

            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            GUI.Label(new Rect(productSize, y, Screen.width, productSize / 3f), 
                item.Name + GetGradeString(item));
            GUI.Label(new Rect(productSize + 10f, y + productSize / 3f, Screen.width - productSize - 15f, productSize / 3f), item.Description);

            if (item is SingleUseItem)
            {
                GUI.Label(new Rect(Screen.width * 3 / 4f, y + productSize * 2 / 3f, Screen.width, productSize / 3f), "Balance:" + item.Balance);
            }

            if (item.CanPurchaseNow())
            {
                DrawPrice(item.PrimaryPurchase, productSize, y);

                GUI.skin.label.alignment = TextAnchor.UpperRight;
                if (GUI.Button(new Rect(Screen.width - 120, y, 100, 50), "Click to buy") && !_isDragging)
                {
                    try
                    {
                        item.Purchase();
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
            else
            {
                LifeTimeItem lifetimeItem = item as LifeTimeItem;
                if (lifetimeItem != null && lifetimeItem.IsEquippable)
                {
                    if (lifetimeItem.IsEquipped())
                    {
                        if (lifetimeItem.CanUpgrade)
                        {
                            DrawPrice(item.NextUpgradeItem.PrimaryPurchase, productSize, y);

                            GUI.skin.label.alignment = TextAnchor.UpperRight;
                            if (GUI.Button(new Rect(Screen.width - 120, y, 100, 50), "Upgrade") && !_isDragging)
                            {
                                lifetimeItem.Upgrade();
                            }
                        }
                    }
                    else
                    {
                        GUI.skin.label.alignment = TextAnchor.UpperRight;
                        if (GUI.Button(new Rect(Screen.width - 120, y, 100, 50), "Equip") && !_isDragging)
                        {
                            lifetimeItem.Equip();
                        }
                    }
                }
            }

            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            GUI.color = oriColor;
            y += productSize;
        }

        GUI.EndScrollView();
    }

    private string GetGradeString(VirtualItem item)
    {
        return item.HasUpgrades ? 
            item.CurrentLevel == item.MaxLevel ? "(max level)" : string.Format("(level {0})", item.CurrentLevel) 
            : "";
    }

    private void DrawPrice(Purchase purchase, float productSize, float y)
    {
        if (purchase.IsMarketPurchase)
        {
            MarketProduct marketProduct = Market.Instance.ProductList[purchase.AssociatedID];
            GUI.Label(new Rect(Screen.width / 2f, y + productSize * 2 / 3f, Screen.width, productSize / 3f), string.Format("{0}", marketProduct.FormattedPrice));
        }
        else
        {
            DrawVirtualCurrencyIcon(purchase.AssociatedID, Screen.width / 2f - 25, y + productSize * 2 / 3f);
            GUI.Label(new Rect(Screen.width / 2f, y + productSize * 2 / 3f, Screen.width, productSize / 3f), string.Format("{0}", purchase.Price));
        }
    }

    private bool IsLaunched()
    {
        return PlayerPrefsEconomyStorage.GetBool("already_launched", false);
    }

    private void SetLaunched()
    {
        PlayerPrefsEconomyStorage.SetBool("already_launched", true);
    }

    private List<PurchasableItem> _items;
    private VirtualItem _coin;
    private VirtualItem _gem;
    private LifeTimeItem _primaryCharacterItem;
    private Vector2 _pageScrollPosition;
    private Vector3 _touchPosition;
    private bool _isDragging;
}
