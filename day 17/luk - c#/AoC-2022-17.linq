<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	GetHeight(sampleInput, 2022).Dump("sample height@2022 (3068)");
	GetHeight(puzzleInput, 2022).Dump("puzzle height@2022 (3232)");

	GetHeight(sampleInput, 1000000000000).Dump("sample height@1000000000000 (1514285714288)");
	GetHeight(puzzleInput, 1000000000000).Dump("puzzle height@1000000000000 (?)");
}

public class ProgressTracker
{
	public long Value { get; set; }
	public bool Done { get; set; }
}

public long GetHeight(string input, long nrOfBlocks)
{
	// line idx 0 ==> floor
	var lines = new List<byte>();

	var height = 0;
	var removedLines = 0L;
	var moveIndex = 0;

	var patternEndIndex = 0;
	var patternStart = new List<byte>();

	for (long i = 0; i < nrOfBlocks; i++)
	{
		var start = height + 3;
		var rock = GetRock(i);

		var lineCount = (start + rock.Length) - lines.Count;
		if (lineCount > 0) lines.AddRange(Enumerable.Repeat<byte>(0, lineCount));

		//Print(lines, rock, start);
		while (start >= 0)
		{
			moveIndex %= input.Length;
			Move(lines, rock, start, input[moveIndex++]);

			if (!Move(lines, rock, start - 1, 'V')) break;
			start--;
			//Print(lines, rock, start);
		}

		for (int j = 0; j < rock.Length; j++)
		{
			lines[start + j] = (byte)(lines[start + j] | rock[j]);
			if (start + j + 1 > height) height = start + j + 1;
		}
		//Print(lines, null, start);

		if (i == 100)
		{
			patternStart.AddRange(lines.Take(height));
			patternEndIndex = moveIndex;
		}
		else if (patternEndIndex == moveIndex && Compare(lines, patternStart, height) > 10)
		{	
			throw new Exception($"{i} - {height}".ToString());
		}

		while (lines.Count > 10000)
		{
			lines.RemoveAt(0);
			height--;
			removedLines++;
		}

		if (i == 100000000) "100.000.000".Dump();
		if (i == 500000000) "500.000.000".Dump();
		if (i == 1000000000) "1.000.000.000".Dump();
	}

	return height + removedLines;
}

int Compare(List<byte> lines, List<byte> patternStart, int height)
{
	for (int i = 0; i < patternStart.Count; i++)
	{
		if (patternStart[i] != lines[height - patternStart.Count + i]) return i;
	}
	return patternStart.Count;
}

public bool Move(List<byte> lines, byte[] rock, int y, char move)
{
	if (y < 0) return false;
	var wallMask =
		move == '<' ? 0xC0 :
		move == '>' ? 0x01 :
		0x00;

	for (int i = 0; i < rock.Length; i++)
	{
		if ((rock[i] & wallMask) > 0) return false;
		var value =
			move == '<' ? rock[i] << 1 :
			move == '>' ? rock[i] >> 1 :
			rock[i];

		if ((lines[y + i] & value) > 0) return false;
	}

	if (move != '<' && move != '>') return true;

	for (int i = 0; i < rock.Length; i++)
		rock[i] = (byte)(move == '<'
			? rock[i] << 1
			: rock[i] >> 1);

	return true;
}


public void Print(List<byte> lines, byte[] rock, int rockStartY)
{
	rock ??= new byte[0];
	var sb = new StringBuilder("<pre>");

	for (int y = lines.Count - 1; y >= 0; y--)
	{
		var lineY = lines[y];
		var rockIndex = (y - rockStartY);
		var rockY = rockIndex >= 0 && rockIndex < rock.Length ? rock[rockIndex] : 0;

		sb.Append('|');
		for (int i = 0; i < 7; i++)
		{
			var c =
				(rockY & 0x40) == 0x40 ? '@' :
				(lineY & 0x40) == 0x40 ? '#' :
				'.';

			sb.Append(c);
			rockY <<= 1;
			lineY <<= 1;
		}
		sb.Append('|');
		sb.AppendLine();
	}
	sb.Append("---------");
	Util.RawHtml(sb.ToString()).Dump();
}

public byte[] GetRock(long counter) => (counter % 5) switch
{
	// ..# ###. 0001 1110 
	0 => new byte[] { 0x1E },
	// ... #... 0000 1000
	// ..# ##.. 0011 1000
	// ... #... 0000 1000
	1 => new byte[] { 0x08, 0x1C, 0x08 },
	// ... .#.. 0000 0100
	// ... .#.. 0000 0100
	// ..# ##.. 0001 1100
	2 => new byte[] { 0x1C, 0x04, 0x04 },
	// ..# .... 0001 0000
	// ..# .... 0001 0000
	// ..# .... 0001 0000
	// ..# .... 0001 0000
	3 => new byte[] { 0x10, 0x10, 0x10, 0x10 },
	// ..# #... 0001 1000
	// ..# #... 0001 1000
	4 => new byte[] { 0x18, 0x18 },

	_ => throw new ArgumentOutOfRangeException(),
};

public static string sampleInput = @">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";
public static string puzzleInput = @">>>><>>>><>>><<<<>>><<<>>>><>>>><<<<>>><<>>>><<<><><<<>>>><<>><>>>><<><<>><<>><><<>><>>><>>>><>>><>>>><<><><>>><<<><<<<>>><<<><<>>>><<>>><>>><<<>><<>>><>><<>><<<<><<<<>>><<>>><<<<><>>>><<>>>><>>><<>>><<>>><><<<<>><<><<<<>>><>>>><<<><<<<><<<>>><>>>><<>>>><>>>><><<<<>><<>>><<<>><<<<>><<<><<<<>><>>><<>>><<<<>>>><<<<>>>><<<<>><<>>><<<><<<>><>>>><<>><<<>>><<<<>>>><<<<>>><<<<>>><<<<><<>><>><<<<>>><>>>><<<<>><<<>><<<><<<<><<>>>><<<<>>>><<<>>>><<<<>><<>>><>><<<<>>><>>>><<<>>>><<<>><<<>><><<<><<>>><<<<>><<<<><<>>>><>>>><<<><><<>>>><<<>><<<>>>><<<>>><<<<><<><<>>><<<<><<<>><<<<>>>><<<<><<<<>><<>>>><<>>>><<<<>><<>><<<<>>>><<<><>><<<>>>><<<<><<<>><<<<>><<<>><<<<>>>><<<>><>>><<<<>>><>>>><<<>>>><>><<<>>><>>>><<<>>><>>><><<>>>><>>><<<<><<<<>><<>>>><<<<>>><<<>>>><>><<<><<<<>>>><<<<>>>><<<>>>><<>>><>>>><<<><<><<<<>>><<<<>>>><>>><<<<>><<><><<<><<>><<<<>>><<><<>><<><<<>>><<<>><<<<>><<<<><<<>>><>><>><<<<>>><<><<<<>>><>>>><>>>><<<<>>><<>><>>><<><>>><<<<>>><<<<><<<>>><<>><>>><<<<><<>>><<>><<<<><<>><<>>>><>>>><<>><<>>>><<<><><<<<><<<<>>><<<>>><<<>>><><<<<>><<<>>>><<<><<><<<<>>><<>>>><<><<>>>><<<>>>><<<><<<<>><<<<>>>><<<>>>><<<<><>><<<>>>><>><<<>>><<<<><<<<>>><<<<>><<>>><<<<><<<>>><<>><<<>>><<>><<>>>><<<>><>>><<<<>>><<<>>>><<<<>>>><<>>>><<<<><>>><<<>>><<<><<>><>>>><<><<><<<<><<>><<>><>>>><<<>><<<<>>>><<<>>>><<<><><<><<<<><<><<<<>>>><<<<><>>>><<>>><<<<>>>><<>><<>><<>>>><<<<>><<<>><><<<<>>><>>>><<<>><<<<><<<<><<<><>><<<>><<<<><<>>>><<<<>>><>>><<<<>>><<>>>><<<<>>>><<<<>><<<<>>><<<>>>><<<<>>><<<>><<<>>><<>>>><<<<><<>><<<><>><<<<>>><<<<><<<>><<<>>>><<<<>>><<<<><>><>>>><<<<>>><<<<>><><<>><><<<><<<<><<>><>><<>>><<<>><<>>>><<>>><<<<>><<<<>>>><<<>><<>><><<<<><<<<><<<>>>><<>><<>>><<<>>><<><<><<<>><<><<<<>><>>><<<><<>>>><<<<>><<>><<<>>>><<<<>>>><<><<>>><<>><<>>>><>><<<>><<>>>><<<><><<><<<<><<<><>>><><<><><<>><>><<>>>><<>>>><>>>><<<<>>><>>>><>>><<><>>>><<<<><<<<>><<><<<>>>><>><><>>><<<>>><>>><>><<>>><<<><<<>>><<<<>><<<>>><<>><<<>><<>>>><<<<>><<<><>>>><<<<>>>><<>>><<<<>>>><<<><<>>>><<>>><<<>>><<<>>><>>><>><>>><<<<>>><<>>><<>><<>>>><<<<>>><><<<>>>><><<<<><><<<<><<><<<>>>><<<>>><>><<<>>>><<<<>>><<<<>><<<<>>><>>>><<><>>>><<<<><>>><>>><<<<>>>><<<><>><<><<<>><<<<>><<<<><<<>>>><<>>><<><<<<>><>><>>><<<>>><<<><>>>><><<>>><<>>><<>>>><>><><<<<>><<>><<<<>><<<<>>><>><>><<<><<<<><<<>><<>>><<<<>>><<>>><<<<>>>><>>>><<<<><<<<>>>><<>>><><<><<>>>><<<<>>><<<<>>><<<>>>><<<>><<<<>>><>>>><>><<<>>>><>><<>><<><<>>><>>><<<>>>><><<<>>>><<<><>>>><<<>><<<<>>><<<><<>>><<<>><<<><>>>><<<>><<<>>>><<<<>>>><>><<<>>><<>>><<<>><<<<><>><<>>>><<>><<>><>><<<>>><>><<<<>>><<<<>>>><>><>>><<<>>>><<<<>><><<<><<<<>>><<>>>><<<><<<><<<>>>><<<><<<>><<<<>>><<<<><<<<>><<<<><<<>><>>>><<<>><>><<<><><<<>><<<>>><<<<>>>><<<>><<><>>>><><<<><><>><<>>><>><<<<>>>><<><>><<<<><<>>>><<<>>>><>>><<<>>><<<<><<<<>>><>><<<<>><><>>>><><><>>>><>><<>><<<<>>>><<<><>>><<>>>><>><<<<>>>><<<<><<>>>><>>>><<<<><<<>>><>>>><<>><<<<>><<<<>><><<><<<<><<><<<>><<>><>>><><<>><<<<>><<<<>>><<<>>><<<>>><<><<>><<<<>>>><<<>>><<<<>>>><<><<<<>>><<<<>>><<>>><<>>><<<>>><<<<>>>><>>><<<>>>><<<><><><<>><<>><<<<>>><>>><<<>><>>><<<>>><<<>>><<<<>><<<<>>>><<<>>><<<><<>>><<<>>>><<>><>><<>>>><<<<>>><<>><<><<<>>>><>><<<>>>><<<><<<<>>>><<<<>>><>>>><<<<>>><<>>><>>>><<>>>><<<><<>>>><<><<<><<>>><<<>><<<><<>><<<<>>>><<<<>>><>>>><<<>>><<<<>>><>>><<<<>><<<>>>><<<<><<>>>><<<>>><>><<<>>>><<<<>>>><<<><>><<<>><<<>><><<<><<>><<<>><<<>><>>><<<><>>>><<<>><>><>>><<<<>><<>>><<>>>><><<<<>>><<>><<<>>>><<>><<<<><>>>><<<><<><<><<<>>>><>>><>><<>>>><<>>><<><><<<<>><<>><<<<>>><>>><<<><<<<><>>>><>>>><<>>><<<><<<>>><<<<>><<<>>>><><><<<<><<>>>><>><<<<><<<<>><<>>>><<<>>><<>>>><<<<>>><>><>><<<<>><<<>>><<<<>>>><<<>>>><><<<>>><><>>>><<<<>>><<<><<><<<<>>>><<><<<><<<>>><<<<>>><<<<>>><>>>><<<<><>><<>>><<<>>>><<<><<><<<>>><><<<>>><<<<><><>>>><>><>><<<>><<<><>>>><>>><<<><>><<>>><<><<<<>>>><<<>><<<>><<<>>><<<>><>>>><<>><>>>><<>><>>>><<>>><<>>>><<>><>><>>><<<<>><<>>>><<<<>>><>>><>>><<<<>><<>>><<<<>>><<<<>>><<>>><<<><<<><><<<<>>><<<>>>><><<<><<<>><>>>><<<<>>>><<<>>><<><<>>>><<<><<>>><<<><<<>>><>><<<<><<<>><>><<>>><<>><<<><>>><<>>>><<>>><<<<><<<<>><<<>><>>><<<<><>><<<<>><<<<><<>><>>>><<<<><<<>>><<>>>><<<>><<<><<>>>><<<<>>><<<<>>><<<>>><>><<<<>>><<>>>><>><<>>><<><<<>>>><<<>>>><<>>><<<>>>><<<>>><<<>><<<><<>>><<<>><><<>>><<<><<><<<><<<>><>>>><<>><<><<<><<<<>>>><>>><<<><<>><<>>><<<><<<<>>>><<>>><>>>><<<<><<<<><<>><<>>><<<<>>><><><<<><<<>><<<>>><<>>>><<>>><<>>><<<>>><<><<<<>>><<<>>><><<<<>><<>>>><<<<>>><<>><<>>>><><<<>><>>><<<>><<<><<<>>><<<><<<>>><<<<>>>><<>><<<><>>>><<<<>>>><<<<>><>>>><<><<<>>>><<<>>><<>>><<<<><<<<>>><>>><<<>>><><<<><<>><<>><<<>>><<<>>>><<<>>>><<>>>><<>>><>><<<<>>>><<>><<<>>><<<>>><>>><<<<><<>>><>>>><<<<>>><>><<<<><<<<>>><<<>>>><>><<<<>><<<>>>><<<>>>><>>><<<>><<<>>>><<>>><<<>>>><<<<><<<>><<<><<>>><<<<>>><<<<><>>>><<>>><<><>>>><<>>>><<>>>><>>>><>>>><>>><<<<><<<>><<<><<<<>>>><<<<>><>><<>><<>><<<>>>><<><>>><<<<>>>><<>>><<>>><>>>><>><<<<>><<<<>><<>>><<><<<>>><<<<>><<<<><<>>><<<<><<><<>>><<>>><>>>><<>>>><<<<><>>>><<<<>>>><<<<>>>><<>>>><<<<>>><<>><<<>>><<<>>>><<<><<<<>>><>>><<>>>><><<<<>>><<<>><<<>>><<>>>><<>>><>><<<<>>>><<<<>>><>>><<<>><<>>>><<<>><<<><<<>><<<<>>><<<>><<<>><<<><><<<<>><<>>>><>>><<<>>>><<<>>><<>>><>><<<>>><>><<>>><<<<>><<<>>>><<<<><<<<>><>><<><<<>><<>>><>>><<<><<<<><<<<>>>><<>>>><<<<>>><><<<>>>><>>><<<>>>><>>><<<>>><>>><<<><<<><<<>>>><<<<>>>><>>>><>>><<<>>><<<>><<<<><>>><<<<>><><<<<><<<>>><<<>>>><>>><<>>>><<<>>><>>>><<>>><<<<>>>><>>>><<>>>><<>>><<<<>><<<<>>>><>>>><<<>>>><>>><<><<<><>>>><<<<><>>>><<<><<<><<<><>>><<<<>>>><<<<>>>><>>><<<>>><<>>><<<<>>><<<>><<><<<><><<<<>>>><<<<><<<>>>><<<<>><<<<>>>><<<><<<<>>><<><><<<<>>><<<>>><>>>><<<><<<>><<>>>><<<>><<<>>><<>><<>><<<>>><<><<>><>>><<<<>><><<<><<><><><<<<>>>><<<<><<<><<>>><><<>>>><>><<<<><<>>><>>>><<<<>>><><<>>>><<>>><<<<>>><<<<>>>><<<><<<<>>><<<>><<<<>>><<<><<<><<>><><<>><<<<>><>>>><<<<>><<<>><<<<>>><<<<>>><>><>><<<>><<<><<<<><>><<>><<<<>><<<<>>>><<><<><>>><<>>><<<<>>><<<>>><<><<>>>><<<>>><<<<>>>><<>><<<<>>>><<>><<<><<<>>><<>>><<<<>>><>>>><>>><>>><>>>><>>>><>>><<>><>>><<>><<><<<<>>><>>><<<>><<<<>>><>>>><<>><>><><<<><<<<>><<<>>><<<><<><<<<>><<<<><>>><<<>>><<>><>>><<><<<<>><<>><<<><<>><<>>>><<><<<>>>><<<<>><<<>>><><<>>><><<<<><<<>><<>><><<<<>>>><<>><<<>><<>>><<<<>>>><<<><<<<>>><<>><<>>>><<<<>><<>><<<<><<<>><>>><<<<>><<<<>>>><>>><>>>><<>>><>><>>>><><<<<>><<<<>><>>><<>>>><<<<>>>><>>>><<<><<<><<<<><<<<><<<<>>>><<>>><<>>>><<<<><>>>><>>><<<>>><<<<><<<<>>><<<><<<<>>>><<<<>>><><<>>><<>>><<>>>><<<><<<<>>>><<<>>><<<<>>><<<>>>><<>>>><<<><<<><<<><>><<<<>>>><<<<>>>><>><<<<><<<<><<<>>>><<<<><<>>><<<<>><>><<<><<<<>><<<><>>>><<<<>>><<<<>>>><<>>><<><<<>>>><<<<>>>><<>><<<<>>><<<<>>><<><<<<><<<<>><<<<>>><<<<>>><<<><<<>><<><<<<>>><><<<<><><<<<><<<>>>><<<<><<<<>><<<<>>>><<<<>>>><<>><><<<<>><<<<>>>><<<>><<<<><><<<<><>>>><<<>><>><<>><<<>>>><>>>><>>><<<>>>><><<<<>>>><<<>>><<><<<<>>><<<><<<<>>>><<<<>>>><>>><<><>>><>>><>><>>>><>>>><<<>>>><>>>><<><<>>><<<<><<<>><>>><<<>><<<>><<<>>>><<><<<>>><<<<>>>><<>>>><<<>>><<<<>><<>><>>>><<<><<<>>><>>><><<<<>>>><<<>>>><<>>>><<>><><<<<>>>><<><<<<>>>><<<>>>><><>>><<<<>>>><>>>><<>>><>><<<><<>>>><<><<<<><>>><<><<<>>>><<<>>><<<>><<<>><<>>>><<<<>>><<<<>>>><<>>>><<<<>><<<<>>><<<<><<<<>><<>><<>>>><<<<>><<>>><>>>><><<><>>><<><<<><>>><<><<>><<>>>><<<>><<>>><<<<>>><<<<><<<<><>>>><<>>>><<<<>>>><>>>><><<>><<<<>><<<>><<>><<<<><<>><<<><>><<<>>>><>>>><<<>>>><<<>>><><<<>>>><<<<>>>><<<>>>><<<>><<>>>><>><<<>><>><<<><<<<>>><<<>>>><<><>>>><>>><<>>><>>>><<<<>>>><<<>><<>><<<>><<<<>><<<><<>><<>>><<>>><<<><<>>>><<<>>><<>><><<<<>><<<<>><>>>><<>>>><><>><>>>><<<<>><<<<>><<<<>>>><<>>>><<>>>><<<><<>>><><<>><<<>><>>><<>>><>>><<><>>><<>><<<<>>><<<>><<<><<<>>><<<<>>><>><<<>>><<<><>>><<><<<<><<<<>><>>><>>>><>>>><>>>><<<>>><<<<>>>><>>><<>>><>>><<<<>><<<>><<<<>><<><<<>><<<>>><<<<>><>>>><>>><<<<>><<<<>><<<<>><>>>><>><<>>>><<>>><<<<>>><<<<><<<>>><<>>>><<>><<>><<<><<>>>><<<>>>><>>><><<>>>><<>>><>>><<>>><>>>><<<<>><<>>><<<>><<>><<>>><>><>><<<<>>>><<>><<<>>>><>><<<<><<<<>>><<<<>>>><<<>>><<>>><<<<>>><>><>>><<>>><>>><<<<><<><<><<><<<><<<>>>><><<<<>>>><<<<>>><>>>><>>><<<<>>><>><<<<><<<><>>>><<<>>><>>><<<<>>>><<><<<<><<><<>>><<>><<<<><<>>>><<<>>>><<<>>><><<>>>><<>>>><>>><<<>>><>>>><>><<<>>><><<<<>>><<>>><<<<>>><><>><<<>>><<><<<<>>><<>>>><>>><<<<>>>><<<<><<<>><<>>><<<>><<<<>><>>><<<>>><>>>><<<<><<>>><<>>><<<<>>>><<>>>><<>>>><<<<>><<<>>><><<<>>>><>>>><<<<><<<>>>><<<<>>><>><<>><<<<>><<<<><<>><>>>><<>><<<>>>><>>><<>>><<<<>><<<>>><<>><>>>><<<<>>><>><>>><<<<>>>><<<<><<<<>>>><<>>><<<<><>><<>>>><<<<><<<>><<<>><<<<><<<<><<><<>><>><<<><<><<>><>>><>>>><<<<>>><>>><>>>><<<>>>><<<<><<<<>>>><>>>><>>><<<<>>>><<<<>><<>><<<>>>><<<<><>><<<>>><<>><><<>>>><<>>>><>>>><><<<<>>>><<>><<<>><<<<>><<>>><<<><<<><>><><<><<<>><<>><<<>><>>>><>>><<>>><<<<>>>><<<<>>><<<<><>>>><<>>>><<><<<>>>><<<<>>><<<><<<>><>>>><<><<<<>>><<<>>><<<<>><>>>><<>>><<><>><<<>>>><<<><<<<><<<>>>><<<<>><<>>>><>>><<>>><<>>>><<<<><<>>>><<<<>>>><<<<><<>>>><<<<>>>><<>>><<<>><<>>><<<<>>><>>><<><<<<><<<<>><<<>>>><>><<<>>><<<>><<<><<<><>>><<<>>>><<<><<<><<>>><>><<<>>><<<>>>><<<>>>><>>>><<<>>><<<<>>>><<><<><<<<>><<<><>>>><<>>><<>><<<<>><<<><<<>><<<>>>><<<><>>><<<><<<<>>><<>>>><<><>><<>>>><<<<><<<><<<<><<<>><<><>><>><<>>>><>>>><<<><>>><<>><<<<>><><<<>>><<>>><<>>>><<<>><<>>>><><<<<>><><<<<>>><<>><>>><<>>>><<<>>><<<<>>><>>>><<<<><<<<>>><<<><<>>><><<<><<>><>>><>><<<<>>>><>>><>>>><<<<>>>><<<>><<<<>><>>><<<>>>><>>>><<>>><<>>>><<<<>>>><<<>>>><>>>><>><<<<>>>><>><<<<>>>><<<<>>><<<>><<<>>>><<<><<<<><<>>><<<>>>><<>><<>>>><<>>><<<>>>><<>>>><<>>>><>>>><><<<>><<><<><>>><><<<>>>><>>><><<>>><<<<>>><<>>>><<<>>><<<<><<<><>><<<<>><<<>><<<>><<>>><<<><<<>><<>><<><<><<<<>>><<>>><<<<>>><>>><<<>>><<>>><<<>><><<<<>>>><>><<<>><<<>>>><<>><<>>><>>><<<<>>>><<>>><><<<>>><>>><>><<<<>>>><>>><>>><<>>>><<>><><<>><<<<><<>>>><<<>>><>>>><<<<>><<<<><<<>><<>>>><>>><<>>>><<>><<>><<<>>><<<><<<<>>><>>>><<<>>>><<<<>>><<>>>><<>>>><<<<>><<<<>><<<><>>><<>><<>>>><>><<<<>><>>>><<<>>><<<>>><<>>>><<<>>>><>><<<><<<<>><>>>><>>><<<<>>><<<>>><<<><<<<>>>><<<<><<>>><>>>><>>><>><>><<<<>>><<>>>><<<><<<<><<<>><<<<>><>><<<>><<<<>>>><><<<><<<><<>>>><<<>><<>>>><><>>><>><<<<>>>><<>>>><<<<>>>><<>>><<<<>>>><<<>>><<<>><><<>><><<><>><<>>>><<<<><<<<>>><<<<>><<>><<<<><<<";