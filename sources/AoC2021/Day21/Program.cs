var data = (await File.ReadAllLinesAsync("input.txt")).ToList();

/*

--- Day 21: Dirac Dice ---
There's not much to do as you slowly descend to the bottom of the ocean. The submarine computer challenges you to a nice game of Dirac Dice.

This game consists of a single die, two pawns, and a game board with a circular track containing ten spaces marked 1 through 10 clockwise. Each player's starting space is chosen randomly (your puzzle input). Player 1 goes first.

Players take turns moving. On each player's turn, the player rolls the die three times and adds up the results. Then, the player moves their pawn that many times forward around the track (that is, moving clockwise on spaces in order of increasing value, wrapping back around to 1 after 10). So, if a player is on space 7 and they roll 2, 2, and 1, they would move forward 5 times, to spaces 8, 9, 10, 1, and finally stopping on 2.

After each player moves, they increase their score by the value of the space their pawn stopped on. Players' scores start at 0. So, if the first player starts on space 7 and rolls a total of 5, they would stop on space 2 and add 2 to their score (for a total score of 2). The game immediately ends as a win for any player whose score reaches at least 1000.

Since the first game is a practice game, the submarine opens a compartment labeled deterministic dice and a 100-sided die falls out. This die always rolls 1 first, then 2, then 3, and so on up to 100, after which it starts over at 1 again. Play using this die.

For example, given these starting positions:

Player 1 starting position: 4
Player 2 starting position: 8
This is how the game would go:

Player 1 rolls 1+2+3 and moves to space 10 for a total score of 10.
Player 2 rolls 4+5+6 and moves to space 3 for a total score of 3.
Player 1 rolls 7+8+9 and moves to space 4 for a total score of 14.
Player 2 rolls 10+11+12 and moves to space 6 for a total score of 9.
Player 1 rolls 13+14+15 and moves to space 6 for a total score of 20.
Player 2 rolls 16+17+18 and moves to space 7 for a total score of 16.
Player 1 rolls 19+20+21 and moves to space 6 for a total score of 26.
Player 2 rolls 22+23+24 and moves to space 6 for a total score of 22.
...after many turns...

Player 2 rolls 82+83+84 and moves to space 6 for a total score of 742.
Player 1 rolls 85+86+87 and moves to space 4 for a total score of 990.
Player 2 rolls 88+89+90 and moves to space 3 for a total score of 745.
Player 1 rolls 91+92+93 and moves to space 10 for a final score, 1000.
Since player 1 has at least 1000 points, player 1 wins and the game ends. At this point, the losing player had 745 points and the die had been rolled a total of 993 times; 745 * 993 = 739785.

Play a practice game using the deterministic 100-sided die. The moment either player wins, what do you get if you multiply the score of the losing player by the number of times the die was rolled during the game?

*/

var die = new DeterministicD100();

var p1 = new Player { Position = StartFor(data, 0) };
var p2 = new Player { Position = StartFor(data, 1) };

var players = new List<Player> { p1, p2 };

while (true)
{
    var p1Rolls = die.Roll() + die.Roll() + die.Roll();
    p1.Position = StepBoard(p1.Position, p1Rolls);
    p1.Score += p1.Position;
    if(p1.Score >= 1000) break;

    var p2Rolls = die.Roll() + die.Roll() + die.Roll();
    p2.Position = StepBoard(p2.Position, p2Rolls);
    p2.Score += p2.Position;
    if (p2.Score >= 1000) break;
}

var result = players.Min(x => x.Score) * die.Rolled;

Console.WriteLine(result);

/*

PartTwo

*/

var multiverse = new Dictionary<(int p1Step, long p1Score, int p2Step, long p2Score), long>
{
    [(StartFor(data, 0), 0, StartFor(data, 1), 0)] = 1
};

var p1Won = 0L;
var p2Won = 0L;

var possibleRolls = new Dictionary<int, int>();
for (var a = 1; a < 4; a++)
{
    for (var b = 1; b < 4; b++)
    {
        for (var c = 1; c < 4; c++)
        {
            if (!possibleRolls.ContainsKey(a + b + c)) possibleRolls[a + b + c] = 0;
            possibleRolls[a + b + c] += 1;
        }
    }
}

while (true)
{
    var currentMultiverse = multiverse.ToDictionary(x => x.Key, y => y.Value);

    multiverse.Clear();

    foreach (var similarUniverses in currentMultiverse)
    {
        foreach (var possibleRollP1 in possibleRolls)
        {
            var p1PreRoll = similarUniverses.Key;
            var p1NewStep = StepBoard(p1PreRoll.p1Step, possibleRollP1.Key);
            var p1NewScore = p1PreRoll.p1Score + p1NewStep;

            if (p1NewScore >= 21)
            {
                p1Won += similarUniverses.Value * possibleRollP1.Value;
                continue;
            }

            foreach (var possibleRollP2 in possibleRolls)
            {
                var p2NewStep = StepBoard(p1PreRoll.p2Step, possibleRollP2.Key);
                var p2NewScore = p1PreRoll.p2Score + p2NewStep;
                var addVal = similarUniverses.Value * possibleRollP1.Value * possibleRollP2.Value;

                if (p2NewScore >= 21)
                {
                    p2Won += addVal;
                    continue;
                }

                var p2PostRoll = (p1Step: p1NewStep, p1Score: p1NewScore, p2Step: p2NewStep, p2Score: p2NewScore);
                if (!multiverse.ContainsKey(p2PostRoll))
                {
                    multiverse[p2PostRoll] = addVal;
                }
                else
                {
                    multiverse[p2PostRoll] += addVal;
                }
            }
        }
    }

    if(currentMultiverse.Count == 0) break;
}

Console.WriteLine(Math.Max(p1Won, p2Won));

/* methods */

int StartFor(List<string> src, int playerId) => int.Parse(src[playerId].Split(':', StringSplitOptions.TrimEntries)[1]);

int StepBoard(int current, int step) => (current + step - 1) % 10 + 1;

/* classes */

internal class DeterministicD100
{
    private int _lastRoll;

    private long _rollCount;

    public long Rolled => _rollCount;

    public int Roll()
    {
        _rollCount += 1;
        if (_lastRoll == 100) _lastRoll = 0;
        return ++_lastRoll;
    }
}

internal class Player
{
    public long Score { get; set; }

    public int Position { get; set; }
}