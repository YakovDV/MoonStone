using UnityEngine;
using Zenject;

public class ShopSFXPlayer : MonoBehaviour
{
    [SerializeField] private UpgradeShop _shop;
    [Inject] private SFXPlayer _sfxPlayer;

    private void OnEnable()
    {
        _shop.PurchaseFailed += OnPurchaseFailed;
        _shop.SuccsessfulPurchase += OnSuccsessfulPurchase;
    }

    private void OnDisable()
    {
        _shop.PurchaseFailed -= OnPurchaseFailed;
        _shop.SuccsessfulPurchase -= OnSuccsessfulPurchase;
    }

    private void OnPurchaseFailed()
    {
        _sfxPlayer.Play(SFXID.Error);
    }

    private void OnSuccsessfulPurchase()
    {
        _sfxPlayer.Play(SFXID.Purchase);
    }
}