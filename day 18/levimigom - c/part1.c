#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>

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

void solve(cube **cubes, int cube_count){
    int i, j, sides;

    sides = 0;
    for(i = 0; i < cube_count; i++){
        sides += 6;
        for(j = 0; j < cube_count; j++){
            if(are_touching(cubes[i], cubes[j])) sides--;
        }
    }

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