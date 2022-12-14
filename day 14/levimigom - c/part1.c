#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define MAX_X 800
#define MAX_Y 400
#define LEN 10

int min(int a, int b) { return(a < b ? a : b); }

int max(int a, int b) { return(a < b ? b : a); }

void parse_input(int **grid){
    int i, j, x, y, prev_x, prev_y;
    char s[LEN];

    prev_x = prev_y = -1;
    while(scanf(" %s", s) != EOF){
        if(s[0] == '-'){
            prev_x = x;
            prev_y = y;
        } else {
            x = y = 0;
            for(i = 0; s[i] != ',' && i < strlen(s); i++) y = (y * 10) + s[i] - '0';
            for(++i; i < strlen(s); i++) x = (x * 10) + s[i] - '0';

            if(prev_x != -1){
                for(i = min(x, prev_x); i <= max(x, prev_x); i++) grid[i][y] = 1;
                for(i = min(y, prev_y); i <= max(y, prev_y); i++) grid[x][i] = 1;

                prev_x = -1;
                prev_y = -1;
            }
        }
    }
}

int add_sand(int **grid){
    int x, y;

    x = 500; y = 0;
    while(y + 1 < MAX_Y && 0 < x && x + 1 < MAX_X){
        if(grid[y + 1][x] == 0) y++;
        else if(grid[y + 1][x - 1] == 0) {
            y++; x--;
        } 
        else if(grid[y + 1][x + 1] == 0) {
            y++; x++;
        } else {
            break;
        }
    }

    if(y + 1 == MAX_Y) return 0;
    
    grid[y][x] = 2;
    return 1;
}

int main(){
    int i, j, **grid;

    grid = malloc(MAX_Y * sizeof(int *));
    for(i = 0; i < MAX_Y; i++){
        grid[i] = malloc(MAX_X * sizeof(int)); 
        for(j = 0; j < MAX_X; j++) grid[i][j] = 0;
    }

    parse_input(grid);

    i = 0;
    while(add_sand(grid)) i++;
    
    printf("%d\n", i);

    for(i = 0; i < MAX_Y; i++) free(grid[i]);
    free(grid);

    return 0;
}