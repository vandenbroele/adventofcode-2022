<Query Kind="Statements" />

var input = File.ReadAllText(Path.Combine(Util.CurrentQueryPath, "..", "day6-input.txt"));

bool AllUnique(Queue<char> q)
{
	return q.Distinct().Count() == q.Count();
}

//--- Part 1

Queue<char> buffer1 = new Queue<char>(input.Substring(0, 4));
var pos1 = 4;
while(!AllUnique(buffer1) && pos1 < input.Length)
{
	buffer1.Dequeue();
	buffer1.Enqueue(input[pos1]);
	pos1++;
}
pos1.Dump("Answer1");

//--- Part 2

Queue<char> buffer2 = new Queue<char>(input.Substring(0, 14));
var pos2 = 14;
while (!AllUnique(buffer2) && pos2 < input.Length)
{
	buffer2.Dequeue();
	buffer2.Enqueue(input[pos2]);
	pos2++;
}
pos2.Dump("Answer2");
