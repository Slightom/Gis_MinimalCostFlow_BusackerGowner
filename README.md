# Gis_MinimalCostFlow_BusackerGowner

// This program has been made for university classes
// It solve problem of minimum cost flow, perfect bipartite matching using Successive Shortest Path Algorithm, Busackera-Gowena
//
// Input: In the first line of the file there are two integers n and m (2 = <n, m <= 200) respectively of programs and computers respectively. 
// Computers and programs are numbered with natural numbers.
// In the next n lines, there are written down the cost of execution time of the programs on the individual computers.
// Cij are integers (1 = <cij <= 100000). In line i + 1 and in column j the cost of executing Pi on the Kj computer is written.
//
// Output: In the output file we save the first n lines of numbers indicating the resultant optimal allocation of programs to computers. 
// The second line contains one number indicating the total cost of executing all programs on the allocated computers.

// Example:
// in.txt
// 3 3
// 1 3 2
// 2 6 4
// 3 7 6
// out. txt
// 2 3 1
// 10


// Time complexity: (C*n^2*log(n) + C*n^2*m) -> (n^2 + n^3). You can improve it by implement dijkstry using fibonacci mound
