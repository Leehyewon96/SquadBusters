public interface ICharacterSpawnerInterface
{
    public bool GetIsDead();
    public CharacterType GetCharacterType();
    public void SetIsDead(bool value);
    public void Init();
}
