<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
//	input = @"A Y
//B X
//C Z";
	
	var games = input
		.Split("\r\n")
		.Select(Game.Parse)
		.ToArray();
		
	games.Select(g => g.ResultValue).Sum().Dump();
}

public enum GameValue
{
	Rock = 1,
	Paper = 2,
	Scissors = 3,
}

public enum GameResult {
	Lose = 0,
	Draw = 3,
	Win = 6
}

public class Game
{
	public string Value {get; set; }
	public GameValue Elf { get; set; }
	public GameValue Me { get; set; }

	public GameResult Result =>
		Elf == Me ? GameResult.Draw :
		Elf == GameValue.Rock && Me == GameValue.Scissors ? GameResult.Lose :
		Elf == GameValue.Scissors && Me == GameValue.Rock ? GameResult.Win :
		(int)Elf < (int)Me ? GameResult.Win :
		GameResult.Lose;
		
	public int ResultValue => (int)Result + (int)Me;
		
	
	
	public static Game Parse(string value) {
		var elf = ParseGameValue(value[0]);
	
		return new Game
		{
			Value = value,
			Elf = elf,
			//Me = ParseGameValue(value[2]),
			Me = ParseWinValue(value[2], elf),
		};
	}

	private static GameValue ParseGameValue(char value) => value switch
	{
		'A' => GameValue.Rock,
		'B' => GameValue.Paper,
		'C' => GameValue.Scissors,

		'X' => GameValue.Rock,
		'Y' => GameValue.Paper,
		'Z' => GameValue.Scissors,
		
		_ => throw new InvalidOperationException(),
	};

	private static GameValue ParseWinValue(char value, GameValue other) => value switch
	{
		'X' => other == GameValue.Rock ? GameValue.Scissors : other - 1,
		'Y' => other,
		'Z' => other == GameValue.Scissors ? GameValue.Rock : other + 1,

		_ => throw new InvalidOperationException(),
	};
}


string input = @"A Y
B Z
C Y
B Y
A Y
A Y
A X
A Y
B Z
A Y
B Y
A Z
A Y
C Z
A Z
A Y
B Y
A Y
A Y
B Z
A Z
C Y
A X
B Z
B Z
B Y
A Y
A Y
B Z
B Z
B Z
B Z
A Y
C Z
A X
B Z
B Z
B Y
B Z
A Y
B Y
B Z
B Y
A Y
B Y
B Z
B Z
A Y
B Y
A Y
C Y
B Y
C X
A Y
A Y
B Z
A Y
B Y
A Y
B Z
A Z
B Y
A Y
A Z
A Y
B X
A Z
A Y
B Z
B X
A Y
B Y
A Y
B Z
A Y
B Z
A Z
B Z
A Y
A Y
A X
B Y
C Z
C Y
B Y
A X
A Z
C X
A Z
A Z
A Y
B Z
A Z
A Z
A Y
B Y
A Y
C X
B Z
A Y
C Z
A Y
A Z
A Y
A Y
A Z
A Y
C Z
A Y
A Y
B Z
C Y
B Z
C Z
C Z
B Z
B X
C Z
A Y
C X
A Y
A Z
B Z
A Y
A Z
A Y
B Z
B Z
B Y
A Z
A Z
A Y
A Y
A Z
B Y
A Y
B Y
A X
B Z
A Y
B X
B Z
B Y
A Z
C Y
C Y
A Z
A Z
A Y
A X
B Z
A Z
B Y
B Y
A Y
A Z
B Y
B Z
A Y
A Z
A Z
B Y
A Y
B Y
A Z
A Y
A Z
A Y
B Y
A Y
B Y
B Z
B Z
A Z
B Z
B Y
A Y
A Z
B Y
A Z
A Z
B Y
C Y
B Z
A Z
A Y
A Y
A Z
B Z
C X
A Z
A Y
A Y
B Y
A Y
B Z
A Z
B Z
B Z
A X
C Z
A Z
A Y
A Y
A Y
A Y
B Y
A Y
C Z
B Z
C Y
B Y
B Z
C Z
B Z
A Y
B Z
A Y
A Y
B Z
A Y
B Z
A Y
B Z
A Y
A X
A X
B Z
C Z
B Z
B Z
A Y
C Y
B Z
A Y
B Z
B Y
B Y
A Y
B Z
A Z
C Y
B Z
B Y
B Z
B Y
B Z
A Z
A Y
A Y
A Z
A Y
B Z
A Y
A Y
B Y
A Y
B Z
B Z
B Z
B Z
C X
A Y
A Z
C Z
A Z
A Z
A Z
A Y
B Z
A Z
A Y
B Z
A Y
A Y
A Z
B Z
A Y
A Y
A X
A Z
B Z
A Y
A Z
C Z
A Z
A Y
B Y
B Y
B Y
A Y
A Y
B Y
A Y
B Y
A X
B Y
B X
A Z
C Y
B Y
A Y
A Z
A Y
B Y
A Y
B Y
B Y
A Z
A Y
A Y
B Z
A X
B Z
B Z
A Z
A Z
A Z
A Z
B Y
B Z
B Y
B Z
B Y
B Z
A X
C Z
C Z
B Y
B Z
A Y
A Y
A Y
A Y
C Z
C Z
A Y
C Z
A Z
A Z
A Y
A Y
A Y
A Y
A Y
B Z
A Z
A Y
A Y
A Y
A Y
A Y
B Z
C X
B Z
A Y
B Z
A Y
B Z
A Z
B Z
A Y
C Z
B Y
A Y
B Z
A Z
C X
A Y
A Y
A Y
A Y
A Z
A Y
B Z
B Y
C Z
B X
A X
B Z
B Y
C Z
A Y
C X
A Y
A Z
B Y
B Y
C Z
A Y
A Y
B Y
A Y
A Y
A Y
B Z
B Y
A Y
A Z
B Y
A Y
A Z
A Z
B Y
A Z
B Y
A Z
B Z
B X
B Z
A X
B Z
B Z
A Y
B X
A Y
A Y
A Z
A X
A Y
A Y
A Y
B Y
C X
A Y
A Z
B Z
A Y
A Z
B Z
A Y
A Z
A Y
C X
A Y
B Z
B Y
B Y
B Y
B Y
A X
A Y
A X
B Y
A X
B Z
A Y
A Y
B Z
C Z
A Y
C Z
A Y
A X
A Y
A Z
B Z
B Z
C Y
B Z
B Y
A X
B Z
B Y
A Y
A Y
A Y
B Z
B Y
B Y
C Y
B Y
B Y
A Y
A Y
B Z
B Z
B Y
A Y
A Z
C Y
A Y
C Y
A Y
A Y
B Y
C Y
C Y
B Y
B Y
A Z
A Y
B Z
B Z
A Y
A Y
A Y
B Z
B Y
A Y
A Y
A Y
A Z
A Z
B Y
A Z
B Z
B Z
B Y
B Y
A Y
B Z
B Z
C X
B X
C Z
A Y
A X
B Z
A Z
B Y
B Y
B Z
B Z
A Y
A Y
B Y
B Z
A Y
A Z
A Z
B Y
C X
A Z
A X
B Z
A Y
A Y
B Y
B Z
B Z
B Y
C Z
C Z
A Y
A Z
A Z
B Y
A Y
A Z
C Y
A Y
A Y
A Z
C Y
B Z
A Z
A Y
A Z
A Y
B Y
A Z
B Z
A Z
A Z
C Z
A Y
A Z
B Z
B Z
A Z
B Z
B Y
B Y
B Y
A Y
A Y
A Z
A Y
A Y
A Y
B Z
A Z
B X
B Z
B Z
A Y
A Y
A Y
A Y
A Z
B Y
B Y
C Z
A Y
B Z
A Y
B Z
A Y
A Y
A Y
B Z
C Y
B Z
B Z
A Y
A Y
A Y
A Y
A Y
B Y
A Y
C X
B Y
A Y
A Y
B Z
A Z
A Y
B Y
A Y
B Y
A Y
B Y
B Z
B Z
B Y
A Z
C Z
C Z
A Y
A Y
B Y
A Y
C Z
A Y
B Y
B Z
A Y
A Y
B Z
B Y
C Z
B Z
B Y
A Z
B Y
A Y
B Y
A X
A Y
A Z
B Y
B Z
C Y
B Y
B Z
A Y
A Y
B Z
A Y
B Z
A Z
B Z
C Z
A Z
A Y
A Y
A Y
A Z
A Y
B Z
B Z
B Y
C Z
A Y
B Z
A Y
A Y
A Y
B Y
B Z
C Y
B Z
C Z
A Z
A X
B Y
B Z
B Y
B Y
B Z
A Y
A Y
C X
A Y
A Y
A Z
A Y
B Y
A Y
C Z
C Z
A Z
A Y
A Z
A Y
A Y
A Y
A Z
A Z
B Z
A Z
A Z
B Y
A Y
B Y
A Y
A Y
A X
B Z
C Z
C Z
A Y
A Z
C Z
A X
C X
B X
C Z
A Y
B Z
B Z
B Y
B Z
B Z
A Y
C X
B Y
B Y
B Y
C Y
A Y
A Y
B Y
A Z
C X
C X
A Y
A Z
B Y
B Y
A Y
B Y
A Y
C Y
B Z
A Y
B Z
A Z
A Y
B Y
B Z
B Y
A Z
A Z
A Z
A Z
A Y
A Z
B Y
A Y
B Y
A Y
B Y
A Y
B X
A Y
B Y
A Y
A Y
C Z
C Z
B Z
A Z
B Z
A Y
A Y
C Z
B Z
B Z
A Y
A Y
B Z
A Z
B Z
A Z
A Z
A Y
B Y
A Z
B Z
B Z
A Y
A Y
C Y
B Y
A Y
C Y
B Y
B X
A Y
B Y
A Y
B Y
B Y
C Y
B Z
A Y
B Y
B Y
B Y
A Y
A Y
B Z
B Y
A Y
B Y
B Y
A Y
B Y
A Y
A Z
C X
B Z
A Y
A Z
B Z
A Y
A Y
C Z
A Z
C Y
B Y
A Z
B X
A Z
B Y
B Z
C Z
A X
C Z
B Y
A Z
A Y
B Y
B Y
C Y
B Y
A Y
A Z
B Y
B Z
A Y
B Y
B Z
C Z
B Y
C Z
A X
A Y
A X
B Z
A Z
A Z
B X
B Y
C Y
B Y
A Z
B Y
C Z
A Z
A Y
A Y
C Z
B Y
A Y
C Z
A Y
B Z
A Y
B Z
B Y
A Y
B Z
C Y
C Z
C Z
A Y
C Z
A Z
A Y
B Y
B Z
C Z
A Y
A Y
B Y
B Y
A Y
B Z
A Y
B Y
B Z
B Z
A Y
A Y
B Z
C Z
C Y
B Y
C Z
A Y
B Y
A Y
C Z
A Y
B Z
B Y
A Y
B X
B Y
A Z
B Y
B Z
B Y
A Y
B Z
A Y
A Z
C X
A Y
B Z
A Y
B Y
B Y
B Y
A Y
A Y
C X
B Y
A Y
A Y
B Y
A Y
A Y
A Y
B Y
C Z
B Z
C Z
C Y
C Y
A Y
B Z
A X
B Y
B Y
B Y
A Y
B Z
B Y
B Y
C Z
A Y
B Y
A X
B X
B Y
C Z
B Y
B Y
B Z
B Y
B Z
A Z
A Y
A Y
C X
A Z
A Y
A Y
A Y
A Y
A Z
A Y
A Y
A Y
A X
A Y
B X
B Y
A Y
B Z
A X
C Z
A Y
A Z
A Y
B Z
B Y
A Y
A Y
A Z
A Y
A Y
A Y
A Y
A X
C Z
A Y
A Z
A Y
A Y
B Z
B Y
A Z
B Y
C Y
A Z
A Y
B Y
B Y
B Y
B Y
A Y
B Y
B Z
B Z
B X
B Z
A Z
B Z
B Y
A Z
A Y
A Y
A Y
B Y
B Z
A Y
A Y
A Y
B Y
A Z
A Y
B Y
A Z
A Y
C Y
A Y
B Z
C Z
B Y
C Z
A Y
A Z
B Z
B Y
A Z
A Y
B Y
B Y
A Y
C Z
A Z
A Y
A Y
B X
A Z
C X
C Z
B Z
C Y
B Y
C Z
A Z
B Z
B Y
A Y
B Z
B Y
A Z
B Y
A X
A Z
B Y
B Y
C Z
B Z
C Z
A Z
A Z
C Z
A Y
B Z
B Z
A Z
A X
B Y
A Z
A X
A X
A X
A Y
A Z
A Y
A Z
B Y
B Z
A Y
B Z
C Y
B Z
A Y
A Y
A Y
B Y
A Y
B Y
B Z
C X
B Y
B Y
B X
A Y
A Y
B Z
C Y
B Y
B Y
A Z
A Y
A Z
B Z
A Y
B Y
B Y
A Z
A Y
A X
A Y
C Z
A Y
A Y
C Z
B Z
A Y
A Z
A Z
C Z
A Y
A Z
A Z
A Y
B Y
A Z
B Z
A Y
B Y
A Z
A Y
B Y
A Z
A X
B Y
B X
A Y
B Y
A Z
A Z
B Z
A Z
B Z
A Y
B Y
A Y
B Z
C Z
A Y
A Y
A Y
B Y
A Y
A Z
A Z
B Y
B Y
A Y
B Z
B Y
B Z
A Y
C Y
C X
B Y
B Z
C Z
A Y
B Z
B Y
A Y
B Z
B Z
B Z
B Z
B Y
B Z
A Z
C Z
A Y
B Y
A Y
B Z
A Z
B Z
A Y
B Y
A Z
C Z
A Y
B Z
A Z
B Z
B Z
A Y
A Y
C Y
B Y
A Y
C X
A X
C Z
B Z
A Y
B Y
C Y
B Y
A X
B Y
C Y
B Y
A Y
A Y
B Z
A X
B Y
A Y
B Y
B Z
B Y
B Z
A Y
A Z
A Z
B Z
B Y
B Y
B Z
A X
B Y
A Z
A Y
A Y
A Y
A Y
A Z
A Y
A Z
A Y
A Y
C Y
B Y
A Y
C Z
B Y
A Y
A Y
C Y
A Y
A Y
B Y
B Y
A Y
A Y
B Y
B X
B Y
C Y
B Y
A Y
B X
C X
A Y
B Z
A Y
A Y
B Y
B Y
B Z
A Z
B Y
A Y
A X
A Y
A Z
A Y
B Z
C Z
A Y
A Y
A Y
A Y
A Y
A Y
B Y
A Y
A X
B Y
B Y
B Y
A Y
B Y
B Z
A Y
C Y
A Z
C Z
A Y
B Z
B X
B Z
B Y
A X
C Z
A Y
A Y
A Y
C Z
B Z
C Y
A Y
B Y
B X
A Y
B Y
A Y
A Z
A Y
A X
A Z
A X
C Z
B Y
B Y
B Y
A Y
A Y
C Z
A Y
B Y
A Y
A Y
A Z
C Z
C Z
C Y
C Z
B Y
A Z
B Y
A Y
A Z
A X
C Y
B Y
A Z
B Y
A Y
B Z
C Y
C Z
A Y
A Y
A Z
A Y
A Y
C Y
B Y
B Z
A Y
A Y
B Z
B Z
A Z
B Y
B Y
B Y
C X
B Z
C Y
A Z
B Z
A Y
A Y
A Y
B Z
B Y
A X
B X
A Y
A Y
B Z
C X
C Y
A Y
B Y
B Z
B Z
A Z
A Y
A Y
A Y
B Y
A Y
C Y
A Y
A Y
A Y
B Z
B Z
A Z
A Z
A X
A Y
A X
B Y
B Z
B Y
A X
A X
B Z
B Y
B Y
A Y
B X
C X
B Z
C X
B Y
C Y
B Z
A Y
C Z
A Y
A Y
B Y
A Y
B Y
B Z
A Y
B Y
A Z
C Z
A Y
C Y
A Y
A X
B Z
B Y
C Z
B Z
B Z
B Y
B Z
C Z
A Y
B Y
B Y
B Y
B Z
B Y
B Z
B Y
A Z
B Y
B Z
B Y
A Z
A Y
C X
A Z
A Y
A Z
A Y
A Y
B Y
B Y
A Y
B Y
B Z
B Y
A X
A Z
A Z
A Y
A Z
A X
B Z
B Y
B Z
A Y
C Z
A Y
A Y
A Y
A Y
A Y
C Z
A Y
A Z
B Z
A Z
B Y
A Y
B Y
C Z
B Z
B Z
B Z
B Y
A Z
A Y
C Z
A Y
C Z
A Z
A Y
B Z
A Y
A Y
B Y
B X
B X
A Y
A Y
A Y
A Y
C Z
B Y
A X
A Y
B X
A Y
A Y
A Y
A Z
A Y
C Y
C Y
C Y
A Y
B Z
A Z
C Z
C Y
A Y
A Y
B Y
C X
A Y
A Z
A Y
C Z
A Y
B Y
A Y
A Y
A Z
B Z
B Z
A Y
B Z
A Z
A Y
C Z
B Z
B Z
B Z
A Y
A Y
A Y
B Z
B Y
A Y
B Y
C Z
A Y
A Y
B Z
C Z
A Y
A Z
C X
B Z
A Y
B Y
A Y
A Y
B Z
A Z
C X
B Z
A Y
B Z
A X
A Y
A Y
B Z
B Z
B Y
A Z
A Z
C Y
B Y
A Y
A Y
C X
A Y
A Y
B Y
A Y
A Z
A Y
A Y
A Z
A Y
C Z
A X
A Z
A Y
C Z
A Y
B Y
A Z
A X
B Y
B Y
A Z
A Y
A Y
B Y
B Y
B Y
A Y
B Y
A Y
B Z
A Z
A Z
B Z
A Y
B Y
A Y
B Y
A Y
C Z
A Y
A Z
C Y
B X
B Z
B Z
A Z
B Z
B Y
A Y
A Y
B Z
A Y
B Z
A Y
B Y
A X
A Y
C Z
B Z
B Y
B Y
B Y
B Z
A Y
A Y
B Z
B Y
C Z
B Y
A Y
A Y
B Z
B Y
B Z
B Y
A Y
A Y
A Z
B Z
C Z
A Y
C X
A Z
B Y
B Z
B Y
A Y
B Y
B Y
A X
B Z
A X
A Z
A X
A Y
A Y
A Z
A Z
A Y
C Z
A Y
A X
B Y
B Z
A Y
B Y
A Y
C X
B Y
A Y
B X
B Z
A Z
A Y
B Y
C Z
A Y
C Z
C Y
C Y
B Y
A Y
B Z
A Y
B X
A Z
A Y
B Z
B Z
B Z
A Z
A Z
B Y
B Z
C Y
A Y
A Y
A Y
C X
B Y
B Y
B Y
B Z
A X
A Y
A Z
A Y
C Z
A Y
A Y
C Z
B Y
B Y
A Z
A Z
A X
B X
B Z
B Z
A Z
B Y
C Z
B Y
A X
A Y
B Z
B Y
A Z
C Z
B Y
B Z
C Z
B Z
A Y
C Z
A Y
A Z
B Z
B Z
A Y
B Y
A Z
A Z
A Y
C Z
C Z
A Z
B Y
A Y
A Z
C Z
A Y
C Y
C X
A Y
B Y
A Y
A Z
B X
A Z
A Z
B Y
B Z
B Y
A Y
A Y
B X
B Z
A Y
A Y
B Z
A Y
A Y
B Y
A Y
B Y
C Z
B Y
B Y
A Y
A Y
B X
C X
A Y
A Y
B Z
B Y
B Z
A Y
A Y
C Y
A X
B Z
A Y
A Y
A Y
A Y
C Y
A Y
A Z
A X
B Z
A Y
A Z
B Z
B Z
A Y
B Y
B Y
A Z
B Z
B Z
C Z
A Y
B Z
A Y
B Y
A Y
B Z
C X
B Y
A Y
B Z
A X
C X
B Z
B Z
B Y
A Y
C X
A Y
A Z
B Y
B Z
B Y
A Y
B Y
A Z
B Y
A Z
B Z
A Y
B Y
A Y
A Y
C Z
A Y
B Z
B Y
A Z
C Z
C Z
C Z
A Y
B Y
B Z
B Z
A Y
A Z
B Z
A Y
B Z
B Z
B Z
A X
A Y
A Y
A Y
B Y
B Y
B Z
B Y
B Z
B Z
B Y
B Y
B Z
A Y
A Z
A Z
B Y
A Y
A X
B Z
B Y
B Z
A Y
A Y
A Y
B Y
A Y
A Y
A Z
B Z
A Y
A Y
A Y
A Y
C Z
A X
C Z
B Z
B Z
B Y
A Y
B Y
A Z
A Z
B Z
C Z
B Z
A Z
B X
A Y
A Y
B Y
A Z
B Z
A Y
A Z
A Y
A Y
A Y
A Y
C Y
A Y
B Y
B Z
C X
A Y
B Z
C Z
B Z
B Z
A Z
B Y
B Z
A Z
A Y
B X
B Z
B Z
B Y
B Z
A Y
B Z
B Y
C X
B Y
A Y
B Z
A Z
B X
B Y
B Z
B Z
B Z
B Y
A X
A Y
B Z
A Y
A X
C X
A Y
A X
A Z
B Y
B Z
A Y
B Z
B Z
A Y
B Z
A Y
C X
A X
B X
A Z
C Z
B Z
A Y
B Z
C Z
B Y
A Y
A Y
B Z
A Z
B Y
A X
A Y
A Y
C Z
C Z
C Z
A Y
B Y
A Y
A Y
C Z
A Y
A X
B Z
A Y
C Z
B Y
A X
B Z
A Z
B Y
B Y
A Y
B Y
A Z
B Z
B Z
A Y
A Y
A Z
B Z
C Z
C Z
B Z
A X
B Z
B Z
B Z
A Z
A Z
C Z
A Y
A Y
B Y
B Y
A Z
B Z
A Y
A Y
B Z
A Y
A Z
A Z
B Z
A Z
A Y
A Y
C Y
B Z
A Y
A Z
B Y
A X
A Z
A Y
B Z
A Y
A Y
B Y
B Z
A Z
A Z
A Y
A Z
B Y
C Z
B Y
A Y
C Z
C Y
B Y
A Y
A Z
B Y
B Y
A Y
C Y
C Z
A Y
B Y
A Z
B Z
A Y
C Y
A Y
A Y
B Z
B Y
A Y
C X
A X
B Z
A Z
B Z
B Z
A Y
A Y
B Z
B Z
C Z
C Z
C Z
B Z
A Y
A Y
B Y
B Y
A Y
A Y
A Z
B Y
A Y
A Z
C Y
A Y
C Y
B Z
C Z
B Z
A Y
B Y
A Z
A Z
C Z
B Y
B Z
A Z
A Y
B Y
B Y
C Z
B Y
A Y
A Y
B Y
B Z
A Y
A Y
B X
B Y
B Y
A Y
A Y
A Y
B Y
C X
B Y
A Z
B Z
B Y
B Y
A Z
A Y
A X
B Z
C Z
A Z
C Z
A Y
B Y
B Z
A Y
A Y
A Y
A Z
C X
A Y
A Z
A Y
B Y
A Y
B Z
B Z
B Y
A Y
B Z
C Z
B X
B Y
B Y
B Z
B Y
A X
B Y
A Z
A Y
A Y
A Y
A Z
A Y
B Y
A Y
C Z
B Z
B Y
A Z
A Y
A Z
A Y
A Y
A Y
C Z
B Y
A Z
A Y
B Y
A Y
A Y
A Y
A Y
B Z
A Z
B Y
A Y
B Z
B Z
A Z
A Y
A Y
A Y
A Z
B Y
A Y
B Y
A Z
A Y
B Z
A Y
B Y
B Z
C Y
A Y
A Z
A Z
A Y
A Y
B Z
A Z
A Z
A Y
A Y
A Y
C Y
B Y
B Y
A Z
C Y
C X
B Z
A Y
A Y
C Y
C Z
A Z
C Z
A Z
A Z
B Y
A Y
A Z
A Y
C X
B Y
B Z
C Z
A Y
A X
C Y
B Y
B Y
A Z
A Z
A Z
A Z
C Z
B Y
B Z
A Y
B Z
B Z
A X
C Z
B X
C Z
A X
B Z
A Y
B Z
B Z
A Y
A Y
A X
C X
A Y
A Y
A Y
C Z
A Z
B Y
A Y
A X
A Y
A Y
B Z
A Y
A Y
A Z
B Y
A Y
B X
A X
B Y
A Y
A Y
B Y
B Z
B Z
B Z
A Y
A Z
A Z
A Z
A Y
B Y
C Y
B Y
A Z
A Z
B Y
A Z
A Y
B Z
A Z
C Z
B Y
A Z
B Y
A Y
A Z
A Y
B Y
B Y
A Y
A Z
B Z
B Y
A X
B Y
A Z
B Y
A Y
A Z
B Y
C X
B Z
A Y
C Z
A Y
B X
B Y
A Y
C Z
B X
C X
A Y
A Z
B Z
C Z
B Y
B Y
B Y
A Y
A Y
B Y
B Y
B Y
A Y
B Z
B Z
A Z
A X
A Z
A Z
B Y
A Y
A Y
C Z
A Y
A Y
A Z
B Z
C Z
A Y
B Z
A Z
A Z
B Y
B Y
A Y
A Y
A Y
B Y
B Z
B Z
B Y
B Y
B Z
B Z
C Z
A Y
B Z
B Y
B Y
B Y
A Z
B Z
A Y
B Y
A Z
A Y
A Y";
