#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define LEN 64
#define MINUTES 24
#define MAX_ORE 6
int ore_cost, clay_cost, obsidian_ore_cost, obsidian_clay_cost, geode_ore_cost, geode_obsidian_cost;

int max(int a, int b) { return(a < b ? b : a); }

int solve(int minute, int ore_robots, int clay_robots, int obsidian_robots, int ores, int clay, int obsidian){
    if(minute == MINUTES) return 0;

    int solution = 0;

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

    if(ores < 2 * max(ore_cost, max(clay_cost, max(obsidian_ore_cost, geode_ore_cost)))){
        solution = max(solution, solve(minute + 1, ore_robots, clay_robots, obsidian_robots, ores + ore_robots, clay + clay_robots, obsidian + obsidian_robots));
    }

    return solution;
}

int main(){
    int i, geodes, sum;

    sum = 0;
    for(i = 1; scanf("%*s %*s") != EOF; i++){
        scanf("%*s %*s %*s %*s %d %*s", &ore_cost);
        scanf("%*s %*s %*s %*s %d %*s", &clay_cost);
        scanf("%*s %*s %*s %*s %d %*s %*s %d %*s", &obsidian_ore_cost, &obsidian_clay_cost);
        scanf("%*s %*s %*s %*s %d %*s %*s %d %*s", &geode_ore_cost, &geode_obsidian_cost);

        geodes = solve(0, 1, 0, 0, 0, 0, 0);
        printf("Blueprint %d: %d\n", i, geodes);
        
        sum += geodes * i;
    }

    printf("%d\n", sum);

    return 0;
}