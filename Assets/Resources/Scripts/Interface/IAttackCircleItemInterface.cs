public interface IAttackCircleItemInterface
{
    public void GainTreasureBox();
    public int GetCoin();
    public void SetCoin(int newCoin);
    public void OnDetectedMoneyTree(MoneyTree tree);
    public void OnUnDetectedMoneyTree(MoneyTree tree);
}
