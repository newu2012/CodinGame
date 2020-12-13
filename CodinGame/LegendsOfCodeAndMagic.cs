using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class BotPlayer {
    public int playerHealth;
    public int playerMana;
    public int playerDeck;
    public int playerRune;
    public int playerDraw;

    public BotPlayer(int playerHealth = 30, int playerMana = 0, int playerDeck = 30, int playerRune = 3,
        int playerDraw = 1) {
        this.playerHealth = playerHealth;
        this.playerMana = playerMana;
        this.playerDeck = playerDeck;
        this.playerRune = playerRune;
        this.playerDraw = playerDraw;
    }
}

public class Card {
    public int    cardNumber;
    public int    instanceId;
    public int    location;
    public int    cardType;
    public int    cost;
    public int    attack;
    public int    defense;
    public string abilities;
    public int    myHealthChange;
    public int    opponentHealthChange;
    public int    cardDraw;

    public double cardValue;
    public Card(
        int cardNumber, int instanceId, int location, int cardType, int cost, int attack, int defense, string abilities,
        int myHealthChange, int opponentHealthChange, int cardDraw
    ) {
        this.cardNumber = cardNumber;
        this.instanceId = instanceId;
        this.location = location;
        this.cardType = cardType;
        this.cost = cost;
        this.attack = attack;
        this.defense = defense;
        this.abilities = abilities;
        this.myHealthChange = myHealthChange;
        this.opponentHealthChange = opponentHealthChange;
        this.cardDraw = cardDraw;

        this.cardValue = CalculateCardValue(cost, attack, defense, abilities, myHealthChange, opponentHealthChange, cardDraw);
    }

    private static double CalculateCardValue(double cost, double attack, double defense, string abilities, double myHealthChange,
    double opponentHealthChange, double cardDraw) {
        var value = 0.0;
        var breakthrough = abilities.Contains('B') ? 1 : 0;
        var charge = abilities.Contains('C') ? 1 : 0;
        var drain = abilities.Contains('D') ? 1 : 0;
        var guard = abilities.Contains('G') ? 1 : 0;
        var lethal = abilities.Contains('L') ? 1 : 0;
        var ward = abilities.Contains('W') ? 1 : 0;

        value = (
            (0.57 * attack) +
            (0.4 * defense) +
            (0.34 * myHealthChange) -
            (0.82 * opponentHealthChange) +
            (1.84 * cardDraw) +
            Math.Min(0.61 * breakthrough * attack, 1.5) +
            Math.Min(0.33 * charge * attack, 1) +
            Math.Min(0.34 * drain * attack, 1) + 
            Math.Min(0.51 * guard * defense, 1) +
            Math.Min(2.00 * lethal * defense, 1.5) +
            Math.Min(1.40 * ward * attack, 1.5)
        ) / (cost + 1);

        return value;
    }
}

internal class LegendsOfCodeAndMagicPlayer {
    private static void LegendsOfCodeAndMagicMain(string[] args) {
        var ourBot = new BotPlayer();
        var enemyBot = new BotPlayer();
        var knownCards = new List<Card>();

        while (true) {
            knownCards = InputParsing(ref ourBot, ref enemyBot);

            DebugPlayerStats(ourBot);

            var actionsOutput = ourBot.playerMana == 0
                ? DraftPhase(knownCards)
                : BattlePhase(knownCards, ourBot, enemyBot);

            if (actionsOutput.Length == 0)
                actionsOutput = "PASS";

            Console.Error.WriteLine(actionsOutput);
            Console.WriteLine(actionsOutput);
        }
    }

    

    private static void DebugPlayerStats(BotPlayer ourBot) {
        Console.Error.WriteLine("\nOur Stats:");
        Console.Error.WriteLine("Mana=" + ourBot.playerMana);
        Console.Error.WriteLine("OurHP=" + ourBot.playerHealth);
    }

    private static string BattlePhase(IEnumerable<Card> realCards, BotPlayer ourBot, BotPlayer enemyBot) {
        var ourCards = realCards.Where(x => x.location == 1);
        var enemyCards = realCards.Where(x => x.location == -1);
        var handCards = realCards.Where(x => x.location == 0);

        DebugCardsOnTable(ourCards, enemyCards);

        var actionsOutput = "";
        ourCards = BattleSummonCreatures(ourBot, enemyBot, ourCards, handCards, ref actionsOutput);

        // TODO Use Items

        var playStyle = SelectPlayStyle(ourBot, enemyBot, enemyCards, ourCards);

        actionsOutput += AttackEnemyBot(playStyle, ourCards, enemyCards);
        return actionsOutput;
    }

    private static int SelectPlayStyle(BotPlayer ourBot, BotPlayer enemyBot, IEnumerable<Card> enemyCards, IEnumerable<Card> ourCards) {
        var enemyGuards = enemyCards.Where(card => card.abilities.Contains('G'));
        var ourGuards = ourCards.Where(card => card.abilities.Contains('G'));

        int playStyle;
        var ourTotalAttack = CalculateTotalAttack(ourCards);
        var ourHPWithGuards = ourBot.playerHealth + ourGuards.Sum(card => card.defense);
        var enemyTotalAttack = CalculateTotalAttack(enemyCards);
        var enemyHPWithGuards = enemyBot.playerHealth + enemyGuards.Sum(card => card.defense);

        if (CheckForFullAttack(ourBot, enemyBot, enemyCards, ourCards, enemyGuards, ourGuards, ourTotalAttack, ourHPWithGuards, enemyTotalAttack, enemyHPWithGuards)) {
            playStyle = 1; // Full Attack
            Console.Error.WriteLine("GOING FULL ATTACK MODE");
        }
        else if (ourBot.playerHealth <= enemyTotalAttack && !ourGuards.Any()) {
            playStyle = -1; // Full Defense
            Console.Error.WriteLine("GOING FULL DEFENSE MODE");
        }
        else {
            playStyle = 0; // Normal Play
            Console.Error.WriteLine("GOING NORMAL MODE");
        }

        return playStyle;
    }

    private static bool CheckForFullAttack(BotPlayer ourBot, BotPlayer enemyBot, IEnumerable<Card> enemyCards, 
    IEnumerable<Card> ourCards, IEnumerable<Card> enemyGuards,  IEnumerable<Card> ourGuards, int ourTotalAttack, 
    int ourHPWithGuards, int enemyTotalAttack, int enemyHPWithGuards) {
        var goFullAttack = false;
        if (!enemyCards.Any())
            goFullAttack = true;
        else if (!enemyGuards.Any() && enemyBot.playerHealth <= ourTotalAttack)
            goFullAttack = true;
        else if (enemyHPWithGuards <= ourTotalAttack)
            goFullAttack = false;

        return goFullAttack;
    }

    private static IEnumerable<Card> BattleSummonCreatures(BotPlayer ourBot, BotPlayer enemyBot, IEnumerable<Card> ourCards,
        IEnumerable<Card> handCards, ref string actionsOutput) {
        Console.Error.WriteLine("\nThere was " + ourCards.Count() + " cards");

        handCards = handCards.OrderByDescending(card => card.cardValue);

        foreach (var card in handCards)
            if (card.cardType == 0 && card.cost <= ourBot.playerMana) {
                actionsOutput += "SUMMON " + card.instanceId + ";";
                ourBot.playerMana -= card.cost;

                if (card.opponentHealthChange != 0)
                    enemyBot.playerHealth += card.opponentHealthChange;

                ourCards = ourCards.Append(card);
                Console.Error.WriteLine("Now there is " + ourCards.Count() + " cards");
            }

        return ourCards;
    }

    private static void DebugCardsOnTable(IEnumerable<Card> ourCards, IEnumerable<Card> enemyCards) {
        Console.Error.WriteLine("\nAllies alive = " + ourCards.Count());
        foreach (var card in ourCards)
            Console.Error.WriteLine("Id=" + card.instanceId + " ATK=" + card.attack + " DEF=" + card.defense);

        Console.Error.WriteLine("\nEnemies alive = " + enemyCards.Count());
        foreach (var card in enemyCards)
            Console.Error.WriteLine("Id=" + card.instanceId + " ATK=" + card.attack + " DEF=" + card.defense);
    }

    private static string DraftPhase(List<Card> knownCards) {

        var bestCard = knownCards.OrderBy(card => card.cardType).ThenByDescending(card => card.cardValue).First();
        foreach (var card in knownCards)
            Console.Error.WriteLine("CardValue=" + card.cardValue);
        Console.Error.WriteLine();
        return "PICK " + knownCards.IndexOf(bestCard);
    }

    private static List<Card> InputParsing(ref BotPlayer ourBot, ref BotPlayer enemyBot) {
        string[] inputs;
        for (var i = 0; i < 2; i++) {
            inputs = Console.ReadLine().Split(' ');
            var playerHealth = int.Parse(inputs[0]);
            var playerMana = int.Parse(inputs[1]);
            var playerDeck = int.Parse(inputs[2]);
            var playerRune = int.Parse(inputs[3]);
            var playerDraw = int.Parse(inputs[4]);
            var newBot = new BotPlayer(playerHealth, playerMana, playerDeck, playerRune, playerDraw);
            switch (i) {
                case 0:
                    ourBot = newBot;
                    break;
                case 1:
                    enemyBot = newBot;
                    break;
            }
        }

        inputs = Console.ReadLine().Split(' ');
        var opponentHand = int.Parse(inputs[0]);
        var opponentActions = int.Parse(inputs[1]);

        Console.Error.WriteLine("\nEnemy Actions");
        for (var i = 0; i < opponentActions; i++) {
            var cardNumberAndAction = Console.ReadLine();
            Console.Error.WriteLine(cardNumberAndAction);
        }

        var knownCards = new List<Card>();
        var cardCount = int.Parse(Console.ReadLine());
        for (var i = 0; i < cardCount; i++) {
            inputs = Console.ReadLine().Split(' ');
            var cardNumber = int.Parse(inputs[0]);
            var instanceId = int.Parse(inputs[1]);
            var location = int.Parse(inputs[2]);
            var cardType = int.Parse(inputs[3]);
            var cost = int.Parse(inputs[4]);
            var attack = int.Parse(inputs[5]);
            var defense = int.Parse(inputs[6]);
            var abilities = inputs[7];
            var myHealthChange = int.Parse(inputs[8]);
            var opponentHealthChange = int.Parse(inputs[9]);
            var cardDraw = int.Parse(inputs[10]);
            var newCard = new Card(cardNumber, instanceId, location, cardType, cost, attack, defense, abilities,
                myHealthChange, opponentHealthChange, cardDraw);
            knownCards.Add(newCard);
        }

        return knownCards;
    }

    private static string AttackEnemyBot(int goFace, IEnumerable<Card> ourCards, IEnumerable<Card> enemyCards) {
        var actionsOutput = "";
        foreach (var card in ourCards) { // TODO Full defense mode
            var bestEnemyId = goFace == 1 ? -1 : FindBestEnemy(card, enemyCards);
            actionsOutput += "ATTACK " + card.instanceId + " " + bestEnemyId + ";";
        }

        return actionsOutput;
    }

    private static int CalculateTotalAttack(IEnumerable<Card> cards) {
        var totalAttack = cards.Sum(card => card.attack);
        return totalAttack;
    }

    private static int FindBestEnemy(Card attackingCard, IEnumerable<Card> enemyCards) {
        enemyCards = enemyCards.Where(card => card.defense > 0);

        var bestEnemyId = -1; // Attack Face

        if (!enemyCards.Any())
            return bestEnemyId;
        var guardEnemies = enemyCards.Where(card => card.abilities.Contains('G'));

        if (!enemyCards.Any()) {
            Console.Error.Write("For" + attackingCard.instanceId + " EnemiesID=");
            foreach (var card in enemyCards)
                Console.Error.Write(card.instanceId + ";");
            Console.Error.Write(" GuardsID=");
            foreach (var card in guardEnemies)
                Console.Error.Write(card.instanceId + ";");
            Console.Error.WriteLine();
        }

        if (attackingCard.abilities.Contains('L')) {
            bestEnemyId = enemyCards
                .OrderByDescending(card => card.abilities.Contains('G'))
                .ThenByDescending(card => card.defense)
                .First()
                .instanceId;
            enemyCards.First(card => card.instanceId == bestEnemyId).defense -= attackingCard.attack;
            return bestEnemyId;
        }

        if (attackingCard.attack == 0 && guardEnemies.Any()) {
            bestEnemyId = -1;
            return bestEnemyId;
        }

        if (guardEnemies.Any()) {
            if (attackingCard.attack > guardEnemies.Min(card => card.defense))
                bestEnemyId = guardEnemies
                    .Where(card => card.defense - attackingCard.attack <= 0)
                    .OrderByDescending(card => card.defense)
                    .First()
                    .instanceId;
            else
                bestEnemyId = guardEnemies.First().instanceId;
            enemyCards.First(card => card.instanceId == bestEnemyId).defense -= attackingCard.attack;
            return bestEnemyId;
        }

        foreach (var card in enemyCards) {
            if (attackingCard.abilities.Contains('G') && attackingCard.defense <= card.attack)
                continue;
            if (card.abilities.IndexOf('G') != -1 || card.attack >= card.defense + 3) {
                card.defense -= attackingCard.attack;
                bestEnemyId = card.instanceId;
                break;
            }
        }

        return bestEnemyId;
    }
}