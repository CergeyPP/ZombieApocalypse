using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private RectTransform _unpurchasedStatus;
    [SerializeField] private RectTransform _purchasedStatus;
    [Header("Buttons")]
    [SerializeField] private Button _buyButton;
    [SerializeField] private Color _unableToBuyColor;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Button _adButton;
    [SerializeField] private RectTransform _maxPlaceholder;

    private Inventory _inventory;
    private WeaponView _view;

    private void Awake()
    {
        _inventory = FindAnyObjectByType<Inventory>();
        _view = GetComponent<WeaponView>();
        _view.OnWeaponViewUpdated += UpdateItem;
    }

    private void Start()
    {
        _adButton.onClick.AddListener(OnAdRewardButtonClicked);
        _buyButton.onClick.AddListener(OnBuyButtonClicked);
    }

    public void OnBuyButtonClicked()
    {
        if (_inventory.IsWeaponAlreadyBought(_view.Weapon))
            _inventory.UpgradeWeapon(_view.Weapon, _view.WeaponLevel);
        else
            _inventory.BuyWeapon(_view.Weapon);
    }
    public void OnAdRewardButtonClicked()
    {
        _inventory.ShowRewardAdForUpgradeWeapon(_view.Weapon);
    }


    private void UpdateItem()
    {
        if (_inventory.IsWeaponAlreadyBought(_view.Weapon))
        {
            _purchasedStatus.gameObject.SetActive(true);
            _unpurchasedStatus.gameObject.SetActive(false);
            if (_inventory.IsAbleToUpgrade(_view.Weapon, _view.WeaponLevel + 1))
            {
                if (_inventory.IsEnoughToUpgrade(_view.Weapon, _view.WeaponLevel + 1))
                {
                    _adButton.gameObject.SetActive(false);
                    _buyButton.gameObject.SetActive(true);
                    _maxPlaceholder.gameObject.SetActive(false);
                    _priceText.text = _view.Weapon.GetUpgradePrice(_view.WeaponLevel).ToString();
                }
                else
                {
                    _adButton.gameObject.SetActive(true);
                    _buyButton.gameObject.SetActive(false);
                    _maxPlaceholder.gameObject.SetActive(false);
                }
            }
            else
            {
                _adButton.gameObject.SetActive(false);
                _buyButton.gameObject.SetActive(false);
                _maxPlaceholder.gameObject.SetActive(true);
            }
        }
        else
        {
            _unpurchasedStatus.gameObject.SetActive(true);
            _purchasedStatus.gameObject.SetActive(false);
            _priceText.text = _view.Weapon.BuyPrice.ToString();
            _maxPlaceholder.gameObject.SetActive(false);
            _adButton.gameObject.SetActive(false);
            _buyButton.gameObject.SetActive(true);
            if (_inventory.IsEnoughToBuy(_view.Weapon))
            {
                _buyButton.image.color = Color.white;
                _buyButton.interactable = true;
            }
            else
            {
                _buyButton.image.color = _unableToBuyColor;
                _buyButton.interactable = false;
            }
        }
    }
}
