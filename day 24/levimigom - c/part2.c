#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define MAX_ROWS 22
#define MAX_COLS 155
#define MAX_STATE wall + empty + up + down + left + right + 1
// directions (left, right, up, down)
int xp[] = { 0, 0, -1, 1 };
int yp[] = { -1, 1, 0, 0 };

enum state { empty = 0, wall = 1, up = 2, down = 4, left = 8, right = 16 };

typedef struct p {
    short x, y, minute;
} position;

int my_mod(int n, int m){
    if(n < 0) return m + n;
    else if(m <= n) return n % m;
    else return n;
}

void parse_input(int *rows, int *cols, short states[MAX_ROWS][MAX_COLS]){
    char line[MAX_COLS];
    int i;

    for(*rows = 0; scanf("%s", line) != EOF; (*rows)++){
        if(*rows == 0) *cols = strlen(line);

        for(i = 0; i < *cols; i++){
            if(line[i] == '#') states[*rows][i] = wall;
            else if(line[i] == '.') states[*rows][i] = empty;
            else if(line[i] == '>') states[*rows][i] = right;
            else if(line[i] == '<') states[*rows][i] = left;
            else if(line[i] == 'v') states[*rows][i] = down;
            else if(line[i] == '^') states[*rows][i] = up;
        }
    }
}

void do_move(int rows, int cols, short states[MAX_ROWS][MAX_COLS]){
    int i, j, k, new_x, new_y;
    short new_states[MAX_ROWS][MAX_COLS];
    enum state direction[] = { right, left, down, up };

    // calculate new state for each tile
    for(i = 0; i < rows; i++){
        for(j = 0; j < cols; j++){
            new_states[i][j] = 0;

            if(states[i][j] & wall){
                new_states[i][j] = wall;
                continue;
            }

            for(k = 0; k < 4; k++){
                new_x = my_mod(i + xp[k], rows);
                new_y = my_mod(j + yp[k], cols);

                while(states[new_x][new_y] & wall){
                    new_x = my_mod(new_x + xp[k], rows);
                    new_y = my_mod(new_y + yp[k], cols);
                }

                if(states[new_x][new_y] & direction[k]) new_states[i][j] += direction[k];
            }
        }
    }
    
    // copy new states to states
    for(i = 0; i < rows; i++){
        for(j = 0; j < cols; j++) states[i][j] = new_states[i][j];
    }
}

void add_to_queue(position **queue, int size, int *items, short x, short y, short minute){
    queue[*items] = malloc(sizeof(position));
    queue[*items]->x = x;
    queue[*items]->y = y;
    queue[*items]->minute = minute;
    (*items)++;
}

int bfs(int rows, int cols, short states[MAX_ROWS][MAX_COLS], int start_x, int start_y, int goal_x, int goal_y){
    int i, j, new_x, new_y, minute, size, items;
    short visited[MAX_ROWS][MAX_COLS];
    position **queue;

    size = 64;
    items = 0;

    queue = malloc(sizeof(position *) * size);

    // add starting position to queue
    add_to_queue(queue, size, &items, start_x, start_y, 1);

    // bfs
    minute = 0;

    for(i = 0; i < items && !(queue[i]->x == goal_x && queue[i]->y == goal_y); i++){
        if(minute < queue[i]->minute){
            do_move(rows, cols, states);
            minute++;

            memset(visited, 0, sizeof(visited));
        } 

        // check for move in every direction
        for(j = 0; j < 4; j++){
            new_x = queue[i]->x + xp[j];
            new_y = queue[i]->y + yp[j];

            if(0 <= new_x && new_x < rows && 0 <= new_y && new_y < cols && states[new_x][new_y] == empty && !visited[new_x][new_y]){
                if(items == size){
                    size *= 2;
                    queue = realloc(queue, sizeof(position *) * size);
                }

                add_to_queue(queue, size, &items, new_x, new_y, minute + 1);
                visited[new_x][new_y] = 1;
            }
        }

        // wait
        if(states[queue[i]->x][queue[i]->y] == empty && !visited[queue[i]->x][queue[i]->y]){
            if(items == size){
                size *= 2;
                queue = realloc(queue, sizeof(position *) * size);
            }

            add_to_queue(queue, size, &items, queue[i]->x, queue[i]->y, minute + 1);
            visited[queue[i]->x][queue[i]->y] = 1;
        }

        free(queue[i]);
    }

    printf("End (%d, %d) has been reached after %d minutes!\n", queue[i]->x, queue[i]->y, queue[i]->minute - 1);

    // free memory
    for(i; i < items; i++) free(queue[i]);
    free(queue);

    return minute - 1;
}

int main(){
    int i, j, sum, rows, cols, start_x, start_y, goal_x, goal_y;
    char line[MAX_COLS];
    short states[MAX_ROWS][MAX_COLS];

    parse_input(&rows, &cols, states);

    // find start & goal
    for(i = 0; i < cols; i++){
        if(states[0][i] == empty){
            start_x = 0;
            start_y = i;
        }
        if(states[rows - 1][i] == empty){
            goal_x = rows - 1;
            goal_y = i;
        }
    }

    sum = bfs(rows, cols, states, start_x, start_y, goal_x, goal_y) +
        bfs(rows, cols, states, goal_x, goal_y, start_x, start_y) +
        bfs(rows, cols, states, start_x, start_y, goal_x, goal_y);

    printf("%d\n", sum + 2);

    return 0;
}