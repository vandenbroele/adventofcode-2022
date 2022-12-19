#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define LEN 64
#define MINUTES 32
#define ORE_LIM 28
#define CLAY_LIM 77
#define OBSIDIAN_LIM 10
int ore_cost, clay_cost, obsidian_ore_cost, obsidian_clay_cost, geode_ore_cost, geode_obsidian_cost;
int memo[MINUTES][5][21][15][ORE_LIM][CLAY_LIM][OBSIDIAN_LIM];

int max(int a, int b) { return(a < b ? b : a); }

int solve(int minute, int ore_robots, int clay_robots, int obsidian_robots, int ores, int clay, int obsidian){
    // brute-force solution with partial memoisation and some optimisations 
    // still takes +- 10 mins to complete

    if(minute == MINUTES) return 0;

    // if you have more ore robots than you can use ores in one min => suboptimal solution
    if(max(clay_cost, max(obsidian_ore_cost, geode_ore_cost)) < ore_robots) return 0;

    // if you have more clay robots than you can use clay in one min => suboptimal solution
    if(obsidian_clay_cost < clay_robots) return 0;

    // if you have more obsidian robots than you can use obsidian in one min => suboptimal solution
    if(geode_obsidian_cost < obsidian_robots) return 0;

    if(ores < ORE_LIM && clay < CLAY_LIM && obsidian < OBSIDIAN_LIM && memo[minute][ore_robots][clay_robots][obsidian_robots][ores][clay][obsidian] != -1){
        return memo[minute][ore_robots][clay_robots][obsidian_robots][ores][clay][obsidian];
    }

    int solution = solve(minute + 1, ore_robots, clay_robots, obsidian_robots, ores + ore_robots, clay + clay_robots, obsidian + obsidian_robots);

    if(ore_cost <= ores){
        solution = max(solution, solve(minute + 1, ore_robots + 1, clay_robots, obsidian_robots, ores + ore_robots - ore_cost, clay + clay_robots, obsidian + obsidian_robots));
    }

    if(clay_cost <= ores){
        solution = max(solution, solve(minute + 1, ore_robots, clay_robots + 1, obsidian_robots, ores + ore_robots - clay_cost, clay + clay_robots, obsidian + obsidian_robots));
    }

    if(obsidian_ore_cost <= ores && obsidian_clay_cost <= clay){
        solution = max(solution, solve(minute + 1, ore_robots, clay_robots, obsidian_robots + 1, ores + ore_robots - obsidian_ore_cost, clay + clay_robots - obsidian_clay_cost, obsidian + obsidian_robots));
    }

    if(geode_ore_cost <= ores && geode_obsidian_cost <= obsidian){
        solution = max(solution, MINUTES - minute - 1 + solve(minute + 1, ore_robots, clay_robots, obsidian_robots, ores + ore_robots - geode_ore_cost, clay + clay_robots, obsidian + obsidian_robots - geode_obsidian_cost));
    }

    if(ores < ORE_LIM && clay < CLAY_LIM && obsidian < OBSIDIAN_LIM && memo[minute][ore_robots][clay_robots][obsidian_robots][ores][clay][obsidian] == -1){
        memo[minute][ore_robots][clay_robots][obsidian_robots][ores][clay][obsidian] = solution;
    }

    return solution;
}

int main(){
    int i, geodes;

    for(i = 1; scanf("%*s %*s") != EOF; i++){
        scanf("%*s %*s %*s %*s %d %*s", &ore_cost);
        scanf("%*s %*s %*s %*s %d %*s", &clay_cost);
        scanf("%*s %*s %*s %*s %d %*s %*s %d %*s", &obsidian_ore_cost, &obsidian_clay_cost);
        scanf("%*s %*s %*s %*s %d %*s %*s %d %*s", &geode_ore_cost, &geode_obsidian_cost);

        memset(memo, -1, sizeof(memo));
        geodes = solve(0, 1, 0, 0, 0, 0, 0);
        printf("Blueprint %d: %d\n", i, geodes);
    }

    return 0;
}