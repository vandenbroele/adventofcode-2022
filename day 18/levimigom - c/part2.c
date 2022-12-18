#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#define DIM 20
#define MOVES (int[]){ 1, -1, 0, 0, 0, 0 }
#define MOVES_COUNT 6

typedef struct c {
    int x, y, z;
} cube;

int are_touching(cube *a, cube *b){
    return(abs(a->x - b->x) + abs(a->y - b->y) + abs(a->z - b->z) == 1);
}

cube **parse_input(int *cube_count){
    int x, y, z;
    cube **cubes;

    *cube_count = 0;
    while(scanf("%d %d %d", &x, &y, &z) != EOF){
        if(*cube_count == 0) cubes = malloc(sizeof(cube *));
        else cubes = realloc(cubes, sizeof(cube *) * (*cube_count + 1));

        cubes[*cube_count] = malloc(sizeof(cube));
        cubes[*cube_count]->x = x;
        cubes[*cube_count]->y = y;
        cubes[*cube_count]->z = z;
        (*cube_count)++;
    }

    return cubes;
}

int is_trapped(cube **cubes, int cube_count, cube *c, int *is_lava){ 
    // try to find a path from this cube to the edge of the grid
    int *queue, visited[DIM * DIM * DIM], size, position, index, i, j, x, y, z;

    memset(visited, 0, sizeof(visited));

    size = 64;
    queue = malloc(sizeof(int) * size);
    queue[0] = c->x + (c->y * DIM) + (c->z * DIM * DIM);
    visited[c->x + (c->y * DIM) + (c->z * DIM * DIM)] = 1;
    position = 1;

    for(i = 0; i < position; i++){
        x = queue[i] % DIM;
        z = (queue[i] / (DIM * DIM));
        y = (queue[i] % (DIM * DIM)) / DIM;

        if(x == 0 || x == DIM - 1 || y == 0 || y == DIM - 1 || z == 0 || z == DIM - 1) {
            // edge reached => cube is not trapped
            free(queue);
            return 0;
        }

        for(j = 0; j <  MOVES_COUNT; j++){
            index = x +  MOVES[j] + ((y +  MOVES[(j + 2) % 6]) * DIM) + ((z +  MOVES[(j + 4) % 6]) * DIM * DIM);

            if(!is_lava[index] && !visited[index]){
                if(position == size){
                    size *= 2;
                    queue = realloc(queue, sizeof(int) * (size));
                }
                queue[position++] = index;
                visited[index] = 1;
            }
        }
    }

    free(queue);

    // edge not reached => cube is trapped
    return 1;
}


void solve(cube **cubes, int cube_count){
    int i, j, k, l, sides, *is_lava;
    cube *c;

    is_lava = malloc(sizeof(int) * (DIM * DIM * DIM));
    for(i = 0; i < DIM * DIM * DIM; i++) is_lava[i] = 0;

    sides = 0;
    for(i = 0; i < cube_count; i++){
        sides += 6;
        for(j = 0; j < cube_count; j++){
            if(are_touching(cubes[i], cubes[j])) sides--;
        }

        is_lava[cubes[i]->x + (cubes[i]->y * DIM) + (cubes[i]->z * DIM * DIM)] = 1;
    }

    c = malloc(sizeof(cube));
    for(i = 1; i < DIM; i++){
        for(j = 1; j < DIM; j++){
            for(k = 1; k < DIM; k++){
                if(!is_lava[i + (j * DIM) + (k * DIM * DIM)]){
                    c->x = i;
                    c->y = j;
                    c->z = k;

                    if(is_trapped(cubes, cube_count, c, is_lava)){
                        for(l = 0; l < cube_count; l++){
                            if(are_touching(c, cubes[l])) sides--;
                        }
                    }
                }
            }
        }
    }

    free(is_lava);
    free(c);

    printf("%d\n", sides);
}

int main(){
    cube **cubes;
    int i, cube_count;

    // read input
    cubes = parse_input(&cube_count);

    // find solution
    solve(cubes, cube_count);

    // free memory
    for(i = 0; i < cube_count; i++) free(cubes[i]);
    free(cubes);

    return 0;
}