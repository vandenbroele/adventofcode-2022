#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define LEN 32
#define MAX_MINUTES 26
#define MAX_MASK 32768
#define MAX_VALVES 51
short memo[MAX_MINUTES][MAX_VALVES][MAX_VALVES][MAX_MASK];

int max(int a, int b){ return(a < b ? b : a); }

int my_atoi(char *s){
    int i, value, inverse;

    for(i = 0, value = 0, inverse = 0; i < strlen(s); i++){
        if('0' <= s[i] && s[i] <= '9') value = (value * 10) + s[i] - '0';
        else if(s[i] == '-') inverse = 1;
    }

    if(inverse) value = -value;

    return value;
}

typedef struct v {
    char *name, **tunnels;
    int flow_rate, neighbour_count, released, index, bitmask;
    struct v **neighbours;
} valve;

valve **parse_input(int *valve_count){
    char name[LEN], flow[LEN], tunnel[LEN];
    valve *v, **valves;
    int bitmask;

    *valve_count = 0;
    bitmask = 1;

    scanf("%*s");
    while(scanf("%s %*s %*s %s %*s %*s %*s %*s", name, flow) != EOF){
        v = malloc(sizeof(valve));
        v->name = malloc(sizeof(char) * (strlen(name) + 1));
        strcpy(v->name, name);
        v->flow_rate = my_atoi(flow);
        v->neighbour_count = 0;

        if(v->flow_rate != 0) {
            v->bitmask = bitmask;
            bitmask *= 2;
        } else v->bitmask = 0;

        while(scanf("%s", tunnel) != EOF && strcmp(tunnel, "Valve") != 0){
            if(v->neighbour_count == 0) v->tunnels = malloc(sizeof(char *));
            else v->tunnels = realloc(v->tunnels, (v->neighbour_count + 1) * sizeof(char *));

            v->tunnels[v->neighbour_count] = malloc(sizeof(char) * (strlen(tunnel) + 1)); 
            strcpy(v->tunnels[v->neighbour_count++], tunnel);
        }

        if(*valve_count == 0) valves = malloc(sizeof(valve*));
        else valves = realloc(valves, sizeof(valve *) * (*valve_count + 1));

        valves[(*valve_count)++] = v;
    }

    return valves;
}

int solve(short minute, valve *my_valve, valve *elephant_valve, int bitmask){
    if(minute == 0) return 0;

    if(memo[minute - 1][my_valve->index][elephant_valve->index][bitmask] == -1){
        int i, j, max_flow;
    
        max_flow = 0;
        if(!my_valve->released) max_flow += (minute - 1) * my_valve->flow_rate;
        if(my_valve->index != elephant_valve->index && !elephant_valve->released) max_flow += (minute - 1) * elephant_valve->flow_rate;

        for(i = 0; i < my_valve->neighbour_count; i++){
            for(j = 0; j < elephant_valve->neighbour_count; j++){
                max_flow = max(max_flow, solve(minute - 1, my_valve->neighbours[i], elephant_valve->neighbours[j], bitmask));

                if(!my_valve->released && my_valve->flow_rate != 0){
                    my_valve->released = 1;
                    max_flow = max(max_flow, ((minute - 1) * my_valve->flow_rate) + solve(minute - 1, my_valve, elephant_valve->neighbours[j], bitmask + my_valve->bitmask));
                    my_valve->released = 0;
                }
            
                if(!elephant_valve->released && elephant_valve->flow_rate != 0){
                    elephant_valve->released = 1;
                    max_flow = max(max_flow, ((minute - 1) * elephant_valve->flow_rate) + solve(minute - 1, my_valve->neighbours[i], elephant_valve, bitmask + elephant_valve->bitmask));\
                    elephant_valve->released = 0;
                }

                if(2 <= minute && !my_valve->released && !elephant_valve->released && my_valve->index != elephant_valve->index && my_valve->flow_rate != 0 && elephant_valve->flow_rate != 0){
                    elephant_valve->released = 1;
                    my_valve->released = 1;
                    max_flow = max(max_flow, ((minute - 1) * (my_valve->flow_rate + elephant_valve->flow_rate)) 
                            + solve(minute - 2, my_valve->neighbours[i], elephant_valve->neighbours[j], bitmask + my_valve->bitmask + elephant_valve->bitmask));
                    elephant_valve->released = 0;
                    my_valve->released = 0;
                }
            }
        }
        
        memo[minute - 1][my_valve->index][elephant_valve->index][bitmask] = max_flow;
    }
    
    return memo[minute - 1][my_valve->index][elephant_valve->index][bitmask];
}

int main(){
    int i, j, k, valve_count, *released;
    valve **valves;
    
    // read input
    valves = parse_input(&valve_count);

    // add neighbours by parsing tunnels
    for(i = 0; i < valve_count; i++){
        valves[i]->index = i;
        valves[i]->neighbours = malloc(sizeof(valve *) * valves[i]->neighbour_count);
        for(j = 0; j < valves[i]->neighbour_count; j++) {
            for(k = 0; k < valve_count; k++){
                if(valves[i]->tunnels[j][0] == valves[k]->name[0] && valves[i]->tunnels[j][1] == valves[k]->name[1]){
                    valves[i]->neighbours[j] = valves[k];
                    free(valves[i]->tunnels[j]);
                    break;
                }
            }
        }
    }

    // find solution through dp
    memset(memo, -1, sizeof(memo)); 

    for(i = 0; i < valve_count; i++) valves[i]->released = 0;

    for(i = 0; i < valve_count; i++){
        printf("Starting from %s => ", valves[i]->name);

        printf("%d\n", solve(MAX_MINUTES, valves[i], valves[i], 0));
    }

    // free memory
    for(i = 0; i < valve_count; i++){
        free(valves[i]->name);
        free(valves[i]->neighbours);
        free(valves[i]->tunnels);
        free(valves[i]);
    }

    free(valves);

    return 0;
}