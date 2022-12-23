#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define ROUNDS 10
#define LEN 100
#define MOVES 12
// 0-2  => NW, N, NE
// 3-5  => SW, S, SE
// 6-8  => NW, W, SW
// 9-11 => NE, E, SE
int xp[] = { -1, -1, -1, 1, 1, 1, -1, 0, 1, -1, 0, 1 };
int yp[] = { -1, 0, 1, -1, 0, 1, -1, -1, -1, 1, 1, 1 };

typedef struct m {
    int x, y, proposed_x, proposed_y;
} move;

int has_neighbour(int x, int y, char **grid, int rows, int cols, int pos, int len){
    int i, neighbour_x, neighbour_y;

    for(i = pos; i < len; i++){
        neighbour_x = x + xp[i];
        neighbour_y = y + yp[i];
        if(0 <= neighbour_x && neighbour_x < rows && 0 <= neighbour_y && neighbour_y < cols && grid[neighbour_x][neighbour_y] == '#') return 1;
    }

    return 0;
}

move *get_proposed_move(int x, int y, char **grid, int rows, int cols, int direction){
    int i;
    move *m = malloc(sizeof(move));

    m->x = x; m->y = y;
    m->proposed_x = m->proposed_y = -1;
    for(i = 0; i < 4; i++){
        // north
        if((i + direction) % 4 == 0 && !has_neighbour(x, y, grid, rows, cols, 0, 3) && 0 < x){
            m->proposed_x = x - 1;
            m->proposed_y = y;
            break;
        }

        // south
        if((i + direction) % 4 == 1 && !has_neighbour(x, y, grid, rows, cols, 3, 6) && x + 1 < rows){
            m->proposed_x = x + 1;
            m->proposed_y = y;
            break;
        }

        // west
        if((i + direction) % 4 == 2 && !has_neighbour(x, y, grid, rows, cols, 6, 9) && 0 < y){
            m->proposed_x = x;
            m->proposed_y = y  - 1;
            break;
        }

        // east
        if((i + direction) % 4 == 3 && !has_neighbour(x, y, grid, rows, cols, 9, 12) && y + 1 < cols){
            m->proposed_x = x;
            m->proposed_y = y  + 1;
            break;
        }
    }

    return m;
}

void do_round(char **grid, int rows, int cols, int direction){
    int i, j, size, unique;
    move *m, **proposed_moves;

    // 1. find all proposed moves
    size = 0;

    for(i = 0; i < rows; i++){
        for(j = 0; j < cols; j++){
            if(grid[i][j] == '#' && has_neighbour(i, j, grid, rows, cols, 0, MOVES)){
                //printf("Elf on position %d, %d is able to move\n", i, j);

                m = get_proposed_move(i, j, grid, rows, cols, direction);

                if(m->proposed_x != -1 && m->proposed_y != -1){
                    //printf("Proposed move for %d, %d is %d, %d\n", m->x, m->y, m->proposed_x, m->proposed_y);

                    if(size == 0) proposed_moves = malloc(sizeof(move *));
                    else proposed_moves = realloc(proposed_moves, (size + 1) * sizeof(move *));

                    proposed_moves[size++] = m;
                } else free(m);
            }
        }
    }

    // 2. do all moves to unique tiles
    for(i = 0; i < size; i++){
        unique = 1;
        for(j = 0; j < size; j++){
            if(i != j && proposed_moves[i]->proposed_x == proposed_moves[j]->proposed_x && proposed_moves[i]->proposed_y == proposed_moves[j]->proposed_y){
                unique = 0;
                break;
            }
        }

        if(unique){
            //printf("Moving elf from position %d, %d to %d, %d (%d rows and %d cols)\n", proposed_moves[i]->x, proposed_moves[i]->y, proposed_moves[i]->proposed_x, proposed_moves[i]->proposed_y, rows, cols);
            grid[proposed_moves[i]->x][proposed_moves[i]->y] = '.';
            grid[proposed_moves[i]->proposed_x][proposed_moves[i]->proposed_y] = '#';
        }
    }
    
    for(i = 0; i < size; i++) free(proposed_moves[i]);
    if(size != 0)free(proposed_moves);
}

void print_grid(char **grid, int rows, int cols){
    int i, j;

    for(i = 0; i < rows; i++){
        for(j = 0; j < cols; j++) printf("%c", grid[i][j]);
        printf("\n");
    }
}

void solve(char **grid, int rows, int cols){
    int i, j, min_x, max_x, min_y, max_y, count;

    // find bounds of smallest rectangle containing all elves
    min_x = max_x = min_y = max_y = -1;
    for(i = 0; i < rows; i++){
        for(j = 0; j < cols; j++){
            if(grid[i][j] == '#'){
                if(min_x == -1 || i < min_x) min_x = i;
                if(max_x == -1 || max_x < i) max_x = i;
                if(min_y == -1 || j < min_y) min_y = j;
                if(max_y == -1 || max_y < j) max_y = j; 
            }
        }
    }

    // count ground tiles in the rectangle
    count = 0;

    printf("== Smallest rectangle ==\n");
    for(i = min_x; i <= max_x; i++){
        for(j = min_y; j <= max_y; j++){
            if(grid[i][j] == '.') count++;
            printf("%c", grid[i][j]);
        }
        printf("\n");
    }

    printf("%d\n", count);
}

int main(){
    char line[LEN], **grid;
    int i, j, k, rows, cols;
    
    // read input
    rows = 0;
    while(scanf("%s", line) != EOF){
        if(rows == 0){
            grid = malloc(sizeof(char *));
            cols = strlen(line);
        }
        else grid = realloc(grid, sizeof(char *) * (rows + 1));

        grid[rows] = malloc(sizeof(char) * (cols + 1));
        strcpy(grid[rows], line);

        rows++;
    }

    // resize grid
    for(k = 0; k < ROUNDS; k++){
        grid = realloc(grid, sizeof(char *) * (rows + 1));

        for(i = rows; 0 <= i; i--){
            if(i == rows) grid[i] = malloc(sizeof(char) * (cols + 1));
            else grid[i] = realloc(grid[i], sizeof(char) * (cols + 1));

            for(j = cols; 0 <= j; j--){
                if(i == 0 || j == 0) grid[i][j] = '.';
                else grid[i][j] = grid[i - 1][j - 1];
            }
        }

        rows++;
        cols++;
    }

    grid = realloc(grid, sizeof(char *) * (rows + ROUNDS));
    for(i = 0; i < rows + ROUNDS; i++){
        if(i < rows) grid[i] = realloc(grid[i], sizeof(char) * (cols + ROUNDS));
        else grid[i] = malloc(sizeof(char) * (cols + ROUNDS));

        for(j = 0; j < cols + ROUNDS; j++){
            if(rows <= i || cols <= j) grid[i][j] = '.';
        }
    }

    rows += ROUNDS;
    cols += ROUNDS;

    // do rounds
    printf("== Initial State ==\n");
    print_grid(grid, rows, cols);

    for(i = 0; i < ROUNDS; i++){
        do_round(grid, rows, cols, i % 4);
        printf("\n== End of Round %d ==\n", i + 1);
        print_grid(grid, rows, cols);
    }

    // calculate solution
    solve(grid, rows, cols);

    // free memory
    for(i = 0; i < rows; i++) free(grid[i]);
    free(grid);

    return 0;
}