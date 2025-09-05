using ConsoleOfTheDamned.Terminal.Models;
using ConsoleOfTheDamned.Terminal.Utils;

namespace ConsoleOfTheDamned.Terminal.Core;

public class Game
{
    private static readonly string[] EnemyNames =
    [
        "Graveling",
        "Crypt Mote",
        "Fetid Ghoul",
        "Bone Knight",
        "Woe Maggot",
        "Cultist Adept",
        "Hollow Priest"
    ];

    private static readonly string[] LootLines =
    [
        "You pry a blinking coin from the dust—ominous… but valid tender.",
        "The pockets hold mostly lint and a little gold. Lint discarded.",
        "A ring from a cold finger; the console approves your bling.",
        "A cracked sigil—useless, but the scattered gold is not.",
        "A rat scurries away, leaving behind a single shiny tooth. Valuable? Maybe.",
        "You scoop up a handful of ash. Somehow it counts as currency.",
        "A scroll written in an unknown language… but it folds into a decent wallet.",
        "You find a mug labeled '#1 Cultist'. Inside: two coins and a lot of despair.",
        "The corpse drops a half-eaten sandwich. You take the coins stuck in the bread."
    ];

    private static readonly string[] BattleActions =
    [
        "1) Attack",
        "2) Drink blood vial (heal)",
        "3) Run"
    ];

    private static readonly Random Rng = new();

    public static void Run()
    {
        PrintTitle();

        Console.Write(Colors.Highlight("State your name, lost soul: "));
        var name = NonEmptyInput();

        var player = CreatePlayerViaClassChoice(name);

        bool running = true;
        while (running && player.IsAlive)
        {
            Console.WriteLine();
            Console.WriteLine(Colors.Arcane("=== Console of the Damned ==="));
            Console.WriteLine(Colors.Highlight("1) Venture forth"));
            Console.WriteLine(Colors.Highlight("2) Rest"));
            Console.WriteLine(Colors.Highlight("3) Status"));
            Console.WriteLine(Colors.Highlight("4) Quit"));
            Prompt("Damned");

            string choice = Console.ReadLine()?.Trim() ?? "";
            switch (choice)
            {
                case "1":
                    Adventure(player);
                    break;
                case "2":
                    player.Rest();
                    break;
                case "3":
                    player.PrintStatus();
                    break;
                case "4":
                    running = false;
                    Console.WriteLine(Colors.Flavor("The console fades… the whispers do not."));
                    break;
                default:
                    Console.WriteLine(Colors.Warning("Unrecognized command. The terminal chuckles without a voice."));
                    break;
            }
        }

        if (!player.IsAlive)
            Console.WriteLine("\n" + Colors.Warning("💀 GAME OVER: The console etches a new curse in the log."));

        Console.WriteLine("\n" + Colors.Flavor("(Press Enter to sever the connection…)"));
        Console.ReadLine();
    }

    private static void Adventure(Player player)
    {
        var enemy = CreateRandomEnemy(player);
        Console.WriteLine("\n" + Colors.Flavor("👁️ The screen flickers. A ") + Colors.Warning(enemy.Name) +
                          Colors.Flavor(" materializes!"));

        // Combat loop
        while (player.IsAlive && enemy.IsAlive)
        {
            Console.WriteLine();
            Console.WriteLine(Colors.Highlight(string.Join("  |  ", BattleActions)));
            Prompt("Damned/Battle");
            var pick = Console.ReadLine()?.Trim();

            if (pick == "1")
            {
                PlayerAttack(player, enemy);
                if (!enemy.IsAlive) break;
                EnemyAttack(player, enemy);
            }
            else
            {
                switch (pick)
                {
                    case "2":
                    {
                        player.HealSmall();
                        if (enemy.IsAlive) EnemyAttack(player, enemy);
                        break;
                    }
                    case "3" when TryRun():
                        Console.WriteLine(Colors.Flavor("🏃 You slip between lines of code and vanish."));
                        return;
                    case "3":
                        Console.WriteLine(Colors.Warning("The shadows hang. Escape failed!"));
                        EnemyAttack(player, enemy);
                        break;
                    default:
                        Console.WriteLine(Colors.Warning("Hesitation invites pain. The foe strikes!"));
                        EnemyAttack(player, enemy);
                        break;
                }
            }
        }

        if (!player.IsAlive || enemy.IsAlive)
            return;

        Console.WriteLine("\n" + Colors.Highlight("✔ ") + Colors.Warning(enemy.Name) +
                          Colors.Flavor(" collapses into static and dust."));
        int reward = enemy.GoldReward + Rng.Next(0, 3);
        player.AddGold(reward);
        Console.WriteLine(Colors.Flavor("— ") + Colors.Flavor(LootLines[Rng.Next(LootLines.Length)]));
    }

    private static void PlayerAttack(Player player, Enemy enemy)
    {
        int dmg = player.AttackRoll(Rng);
        enemy.TakeDamage(dmg);
        Console.WriteLine("🗡️" + Colors.Highlight(player.Name) + Colors.Flavor(" hits ") +
                          Colors.Warning(enemy.Name) + Colors.Flavor(" for ") + Colors.Warning($"{dmg}") +
                          Colors.Flavor(" damage (Enemy HP: ") + Colors.Highlight($"{enemy.HealthPoints}") +
                          Colors.Flavor(")."));
    }

    private static void EnemyAttack(Player player, Enemy enemy)
    {
        if (!enemy.IsAlive) return;
        int dmg = enemy.AttackRoll(Rng);
        player.TakeDamage(dmg);
        Console.WriteLine("👹 " + Colors.Warning(enemy.Name) + Colors.Flavor(" strikes you for ") +
                          Colors.Warning($"{dmg}") + Colors.Flavor(" damage! (") +
                          Colors.Highlight($"{player.HealthPoints}") + Colors.Flavor("/") +
                          Colors.Highlight($"{player.MaxHealthPoints}") + Colors.Flavor(" HP)"));
    }

    private static bool TryRun() => Rng.NextDouble() < 0.5;

    // Light enemy scaling by player's accumulated gold to add tension
    private static Enemy CreateRandomEnemy(Player player)
    {
        var name = EnemyNames[Rng.Next(EnemyNames.Length)];
        int baseHp = 8 + (player.Gold / 5);
        int baseDmg = 2 + (player.Gold / 10);
        int reward = 3 + Rng.Next(0, 4);

        baseHp = Math.Clamp(baseHp, 8, 30);
        baseDmg = Math.Clamp(baseDmg, 2, 8);

        return new Enemy(name, baseHp, baseDmg, reward);
    }

    private static Player CreatePlayerViaClassChoice(string name)
    {
        Console.WriteLine("\n" + Colors.Arcane("Choose your curse (class):"));
        Console.WriteLine(Colors.Highlight("1) Hellwalker") + Colors.Flavor(" — sturdy, consistent damage"));
        Console.WriteLine(Colors.Highlight("2) Bone Weaver") +
                          Colors.Flavor(" — fragile, higher damage, a touch of mana"));
        Console.WriteLine(Colors.Highlight("3) Shadow Bard") + Colors.Flavor(" — in-between, dangerously stylish"));

        while (true)
        {
            Prompt("Damned/Class");
            var pick = Console.ReadLine()?.Trim();

            switch (pick)
            {
                case "1":
                    Console.WriteLine("You don spiked sabatons. The console nods.");
                    return new Player(name, "Hellwalker", maxHealthPoints: 28, damage: 5, gold: 3, manaPoints: null,
                        bloodVials: 1);
                case "2":
                    Console.WriteLine("Runes and bone glimmer—power at the cost of softness.");
                    return new Player(name, "Bone Weaver", maxHealthPoints: 20, damage: 7, gold: 2, manaPoints: 5,
                        bloodVials: 1);
                case "3":
                    Console.WriteLine("A chord in shadow; even the cursor winces.");
                    return new Player(name, "Shadow Bard", maxHealthPoints: 24, damage: 6, gold: 3, manaPoints: 2,
                        bloodVials: 1);
                default:
                    Console.WriteLine("Invalid choice. The terminal hums a funeral march.");
                    break;
            }
        }
    }

    private static void Prompt(string path)
    {
        Console.Write(Colors.Arcane(path) + Colors.Warning(" › "));
    }

    private static string NonEmptyInput()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) return input.Trim();
            Console.Write("Nameless heroes are deleted first. Try a name: ");
        }
    }

    private static void PrintTitle()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Clear();

        // Frame (light gray / "flavor")
        Console.WriteLine(Colors.Flavor("╔══════════════════════════════════════════════════════════╗"));
        Console.WriteLine(Colors.Flavor("║                                                          ║"));

        // Title lines (keep exact spacing so the box stays aligned)
        Console.WriteLine(
            "║               " +
            Colors.Arcane("C O N S O L E   O F   T H E") +
            "                ║"
        );
        Console.WriteLine(
            "║                    " +
            Colors.Warning("D  A  M  N  E  D") +
            "                      ║"
        );

        Console.WriteLine(Colors.Flavor("║                                                          ║"));
        Console.WriteLine(Colors.Flavor("╚══════════════════════════════════════════════════════════╝"));

        // Subtitle: flavor text with a bright-white accent glyph
        Console.WriteLine(
            Colors.Flavor("A text-based crawler where the terminal stares back ") +
            Colors.Highlight("☠") + Colors.Flavor(".\n")
        );
    }
}