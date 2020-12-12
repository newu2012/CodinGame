using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

public class BotPlayer {
    public int playerHealth;
    public int playerMana;
    public int playerDeck;
    public int playerRune;
    public int playerDraw;

    public BotPlayer(int playerHealth = 30, int playerMana = 0, int playerDeck = 30, int playerRune = 3, int playerDraw = 1) {
        this.playerHealth = playerHealth;
        this.playerMana = playerMana;
        this.playerDeck = playerDeck;
        this.playerRune = playerRune;
        this.playerDraw = playerDraw;
    }
}

public class Card {
    public int cardNumber;
    public int instanceId;
    public int location;
    public int cardType;
    public int cost;
    public int attack;
    public int defense;
    public string abilities;
    public int myHealthChange;
    public int opponentHealthChange;
    public int cardDraw;

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
        }
}

class LegendsOfCodeAndMagicPlayer
{
    static void LegendsOfCodeAndMagicMain(string[] args)
    {
        string[] inputs;

        BotPlayer ourBot = new BotPlayer();
        BotPlayer enemyBot = new BotPlayer();
        Card[] knownCards = new Card[60];

        // game loop
        while (true)
        {
            for (int i = 0; i < 2; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int playerHealth = int.Parse(inputs[0]);
                int playerMana = int.Parse(inputs[1]);
                int playerDeck = int.Parse(inputs[2]);
                int playerRune = int.Parse(inputs[3]);
                int playerDraw = int.Parse(inputs[4]);
                var newBot = new BotPlayer(playerHealth, playerMana, playerDeck, playerRune, playerDraw); 
                if (i == 0)
                    ourBot = newBot;
                else if (i == 1)
                    enemyBot = newBot;
            }

            inputs = Console.ReadLine().Split(' ');
            int opponentHand = int.Parse(inputs[0]);
            int opponentActions = int.Parse(inputs[1]);
            for (int i = 0; i < opponentActions; i++)
            {
                string cardNumberAndAction = Console.ReadLine();
            }

            int cardCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < cardCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int cardNumber = int.Parse(inputs[0]);
                int instanceId = int.Parse(inputs[1]);
                int location = int.Parse(inputs[2]);
                int cardType = int.Parse(inputs[3]);
                int cost = int.Parse(inputs[4]);
                int attack = int.Parse(inputs[5]);
                int defense = int.Parse(inputs[6]);
                string abilities = inputs[7];
                int myHealthChange = int.Parse(inputs[8]);
                int opponentHealthChange = int.Parse(inputs[9]);
                int cardDraw = int.Parse(inputs[10]);
                var newCard = new Card(cardNumber, instanceId, location, cardType, cost, attack, defense, abilities, myHealthChange, opponentHealthChange, cardDraw);
                knownCards[i] = newCard;
            }

            var actionsOutput = "";
            var realCards = knownCards.Where(x => x != null); 

            Console.Error.WriteLine("Mana=" + ourBot.playerMana);

            if (ourBot.playerMana == 0) {  // if Draft phase
                var bestId = 0;

                for (var i = 0; i < cardCount; i++)
                    if (knownCards[i].cardType == 0) {
                        if (knownCards[i].abilities.Contains('G') || knownCards[i].abilities.Contains('B')) {
                            if (knownCards[i].abilities.Length > 1) {
                                bestId = i;
                                break;
                            }

                            bestId = i;
                            break;
                        }
                        bestId = i;
                    }
                actionsOutput = "PICK " + bestId;
            }
            else {
                var currentMana = ourBot.playerMana;
                
                var ourCards = realCards.Where(x => x.location == 1);
                var enemyCards = realCards.Where(x => x.location == -1 && x.defense > 0);
                var handCards = realCards.Where(x => x.location == 0);

                handCards = handCards.OrderByDescending(card => card.abilities.Contains('G'));

                foreach(var card in enemyCards) {
                    Console.Error.WriteLine("EnemyID=" + card.instanceId);
                }

                foreach(var card in handCards) {
                    if (card.cardType == 0 && card.cost <= currentMana) {
                        actionsOutput += "SUMMON " + card.instanceId + ";";
                        currentMana -= card.cost;
                        if (card.abilities.Contains('C'))
                            ourCards = ourCards.Append(card);
                        if (card.opponentHealthChange != 0)
                            enemyBot.playerHealth += card.opponentHealthChange;
                    }
                }

                var guardEnemies = enemyCards.Where(card => card.abilities.Contains('G'));
                var goFace = enemyBot.playerHealth < CalculateTotalAttack(ourCards) && !guardEnemies.Any();

                actionsOutput += AttackEnemyBot(goFace, ourCards, enemyCards);
            }

            if (actionsOutput.Length == 0)
                actionsOutput += "PASS";

            Console.Error.WriteLine(actionsOutput);
            Console.WriteLine(actionsOutput);
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

        }
    }

    static string AttackEnemyBot(bool goFace, IEnumerable<Card> ourCards, IEnumerable<Card> enemyCards) {
        var actionsOutput = "";
        foreach(var card in ourCards) {
            var bestEnemyId = goFace ? -1 : FindBestEnemy(card, enemyCards);
            actionsOutput += "ATTACK " + card.instanceId + " " + bestEnemyId + ";";
        }
        return actionsOutput;
    }

    static int CalculateTotalAttack (IEnumerable<Card> cards) {
        int totalAttack = cards.Sum(card => card.attack);
        return totalAttack;
    }

    static int FindBestEnemy (Card attackingCard, IEnumerable<Card> enemyCards) {

        enemyCards = enemyCards.Where(card => card.defense > 0);

        //  enemyCards = enemyCards.OrderByDescending(card => card.defense); 
        var bestEnemyId = -1;

        var guardEnemies = enemyCards.Where(card => card.abilities.Contains('G'));

        Console.Error.Write("For" + attackingCard.instanceId + " EnemiesID=");
        foreach(var card in enemyCards) 
            Console.Error.Write(card.instanceId + ";");
        Console.Error.Write(" For" + attackingCard.instanceId + " GuardsID=");
        foreach(var card in guardEnemies) 
            Console.Error.Write(card.instanceId + ";");
        Console.Error.WriteLine();

        if (attackingCard.abilities.Contains('L')) {
            bestEnemyId = enemyCards
            .OrderByDescending(card => card.defense)
            .ThenByDescending(card => card.abilities.Contains('G'))
            .First()
            .instanceId;
            enemyCards.Where(card => card.instanceId == bestEnemyId).First().defense -= attackingCard.attack;
            return bestEnemyId;
        }

        if (attackingCard.attack == 0 && guardEnemies.Any()) {
            bestEnemyId = -1;
            return bestEnemyId;
        }

        if (guardEnemies.Any() && attackingCard.attack > guardEnemies.Min(card => card.defense)) {
            bestEnemyId = guardEnemies
            .Where(card => card.defense - attackingCard.attack <= 0)
            .OrderByDescending(card => card.defense)
            .First()
            .instanceId;
            enemyCards.Where(card => card.instanceId == bestEnemyId).First().defense -= attackingCard.attack;
            return bestEnemyId;
        }
        

        foreach(var card in enemyCards) {
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