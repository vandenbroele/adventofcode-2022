#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define COLS 200
#define ROWS 200
#define LEN 16384
char arrows[] = { '>', 'v', '<', '^' };

int get_next_x(int x, int direction){
    // down
    if(direction == 1) return((x + 1) % ROWS);
    // up
    else if(direction == 3 && 0 < x) return(x - 1);
    else if(direction == 3 && x == 0) return(ROWS - 1);
    // left or right
    else return x;
}

int get_next_y(int y, int direction){
    // right
    if(direction == 0) return((y + 1) % COLS);
    // left
    else if(direction == 2 && 0 < y) return(y - 1);
    else if(direction == 2 && y == 0) return(COLS - 1);
    // up or down
    else return y;
}

void do_moves(char grid[ROWS][COLS], int *x, int *y, int direction, int moves){
    int i, next_x, next_y;

    //grid[*x][*y] = arrows[direction];
    for(i = 0; i < moves; i++){
        next_x = get_next_x(*x, direction);
        next_y = get_next_y(*y, direction);

        while(grid[next_x][next_y] != '.' && grid[next_x][next_y] != '#'){
            next_x = get_next_x(next_x, direction);
            next_y = get_next_y(next_y, direction);
        }

        if(grid[next_x][next_y] == '#') break;

        *x = next_x;
        *y = next_y;
        //grid[*x][*y] = arrows[direction];
    }
}

int main(){
    char grid[ROWS][COLS];
    int i, j, x, y, moves, direction;
    char password[LEN];

    memset(grid, -1, sizeof(grid));
    for(i = 0; i < ROWS; i++) fgets(grid[i], COLS, stdin);

    fgets(password, LEN, stdin);
    fgets(password, LEN, stdin);

    // 1. find starting position
    direction = x = 0;

    for(i = 0; i < COLS; i++){
        if(grid[x][i] == '.'){
            y = i;
            break;
        } 
    }

    // 2. do moves
    moves = 0;
    for(i = 0; i < strlen(password); i++){
        if('0' <= password[i] && password[i] <= '9'){
            moves = (10 * moves) + password[i] - '0';
        } else {
            printf("Move %d times in direction %c\n", moves, arrows[direction]);
            do_moves(grid, &x, &y, direction, moves);
            moves = 0;

            if(password[i] == 'R') direction = (direction + 1) % 4;
            else if(0 < direction) direction--;
            else direction = 3;
        }
    }
    printf("Move %d times in direction %c\n", moves, arrows[direction]);
    do_moves(grid, &x, &y, direction, moves);

    // 3. print grid
    for(i = 0; i < ROWS; i++) printf("%s", grid[i]);

    printf("password: %s\n", password);

    // 4. calculate solution

    // 1579 => too low
    // 113022 => too high
    // 113020 => ook niet
    printf("%d\n", (1000 * (x + 1)) + (4 * (y + 1)) + direction);

    return 0;
}