var data = (await File.ReadAllLinesAsync("input.txt")).ToList();

/*

--- Day 9: Smoke Basin ---
These caves seem to be lava tubes. Parts are even still volcanically active; small hydrothermal vents release smoke into the caves that slowly settles like rain.

If you can model how the smoke flows through the caves, you might be able to avoid it and be that much safer. The submarine generates a heightmap of the floor of the nearby caves for you (your puzzle input).

Smoke flows to the lowest point of the area it's in. For example, consider the following heightmap:

2199943210
3987894921
9856789892
8767896789
9899965678
Each number corresponds to the height of a particular location, where 9 is the highest and 0 is the lowest a location can be.

Your first goal is to find the low points - the locations that are lower than any of its adjacent locations. Most locations have four adjacent locations (up, down, left, and right); locations on the edge or corner of the map have three or two adjacent locations, respectively. (Diagonal locations do not count as adjacent.)

In the above example, there are four low points, all highlighted: two are in the first row (a 1 and a 0), one is in the third row (a 5), and one is in the bottom row (also a 5). All other locations on the heightmap have some lower adjacent location, and so are not low points.

The risk level of a low point is 1 plus its height. In the above example, the risk levels of the low points are 2, 1, 6, and 6. The sum of the risk levels of all low points in the heightmap is therefore 15.

Find all of the low points on your heightmap. What is the sum of the risk levels of all low points on your heightmap?

*/

var rows = data.Count;
var columns = data[0].Length;

var map = new int[rows, columns];

for (var i = 0; i < rows; i++)
{
    var row = data[i];
    for (var j = 0; j < columns; j++)
    {
         map[i, j] = row[j] - '0';
    }
}

var lowPoints = new List<(int x, int y)>();

for (var i = 0; i < rows; i++)
{
    var row = data[i];
    for (var j = 0; j < columns; j++)
    {
        var current = map[i, j];
        var left = j == 0 || map[i, j - 1] > current;
        var right = j == columns - 1 || map[i, j + 1] > current;
        var up = i == 0 || map[i - 1, j] > current;
        var down = i == rows - 1 || map[i + 1, j] > current;
        if(left && right && up && down) lowPoints.Add((i, j));
    }
}

var total = 0;

foreach (var (x, y) in lowPoints)
{
    var riskLevel = map[x, y] + 1;
    total += riskLevel;
}

Console.WriteLine(total);

/*

--- Part Two ---
Next, you need to find the largest basins so you know what areas are most important to avoid.

A basin is all locations that eventually flow downward to a single low point. Therefore, every low point has a basin, although some basins are very small. Locations of height 9 do not count as being in any basin, and all other locations will always be part of exactly one basin.

The size of a basin is the number of locations within the basin, including the low point. The example above has four basins.

The top-left basin, size 3:

2199943210
3987894921
9856789892
8767896789
9899965678
The top-right basin, size 9:

2199943210
3987894921
9856789892
8767896789
9899965678
The middle basin, size 14:

2199943210
3987894921
9856789892
8767896789
9899965678
The bottom-right basin, size 9:

2199943210
3987894921
9856789892
8767896789
9899965678
Find the three largest basins and multiply their sizes together. In the above example, this is 9 * 14 * 9 = 1134.

What do you get if you multiply together the sizes of the three largest basins?

*/

var basins = new List<List<(int, int)>>();

foreach (var (x, y) in lowPoints)
{
    var basinLocations = new HashSet<(int, int)> { (x, y) };

    while (true)
    {
        var candidates = new HashSet<(int, int)>();

        foreach (var (bx, by) in basinLocations)
        {
            if (bx - 1 > -1 && !basinLocations.Contains((bx - 1, by)) && map[bx - 1, by] != 9) candidates.Add((bx - 1, by));
            if (bx + 1 < rows && !basinLocations.Contains((bx + 1, by)) && map[bx + 1, by] != 9) candidates.Add((bx + 1, by));
            if (by - 1 > -1 && !basinLocations.Contains((bx, by - 1)) && map[bx, by - 1] != 9) candidates.Add((bx, by - 1));
            if (by + 1 < columns && !basinLocations.Contains((bx, by + 1)) && map[bx, by + 1] != 9) candidates.Add((bx, by + 1));
        }

        if (candidates.Any())
        {
            foreach (var candidate in candidates) basinLocations.Add(candidate);
        }
        else
        {
            break;
        }

    }

    basins.Add(basinLocations.ToList());
}

var top3basins = basins.OrderByDescending(x => x.Count).Take(3).ToArray();

var mult = top3basins[0].Count * top3basins[1].Count * top3basins[2].Count;

Console.WriteLine(mult);
