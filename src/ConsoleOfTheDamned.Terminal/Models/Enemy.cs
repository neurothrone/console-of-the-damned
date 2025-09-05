namespace ConsoleOfTheDamned.Terminal.Models;

public class Enemy
{
    public string Name { get; }
    public int HealthPoints { get; private set; }
    public int Damage { get; }
    public int GoldReward { get; }

    public bool IsAlive => HealthPoints > 0;

    public Enemy(
        string name,
        int healthPoints,
        int damage,
        int goldReward)
    {
        Name = name;
        HealthPoints = healthPoints;
        Damage = damage;
        GoldReward = goldReward;
    }

    public int AttackRoll(Random rng) => Damage + rng.Next(0, 3);
    public void TakeDamage(int amount) => HealthPoints = Math.Max(0, HealthPoints - amount);
}