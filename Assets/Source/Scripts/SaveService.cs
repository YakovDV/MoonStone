public class SaveService
{
    private SaveData _saveData = new();

    public void SaveWalletValue(Wallet wallet)
    {
        _saveData.WalletValue = wallet.Value;
    }

    public void LoadWalletValue(Wallet wallet)
    {
        wallet.RestoreValue(_saveData.WalletValue);
    }
}