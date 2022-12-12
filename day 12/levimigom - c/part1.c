#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define LEN 100
#define DIRECTIONS (int[]){ 0, 0, 1, -1 }
#define DIRECTIONS_COUNT 4

#pragma region position
typedef struct p {
    int x, y;
} position;

position* create_pos(int x, int y){
    position *p = malloc(sizeof(position));
    p->x = x;
    p->y = y;
    return p;
}
#pragma endregion position

#pragma region queue
typedef struct queue {
    position** items;
    int size, position;
} queue;

queue *init(int max_size){
    queue *q = malloc(sizeof(queue));
    q->items = malloc(sizeof(position*) * max_size);
    q->size = q->position = 0;
}

void push(queue *q, int x, int y){
    q->items[q->size++] = create_pos(x, y);
}

position *top(queue *q){
    return q->items[q->position++];
}

void destroy(queue *q){
    free(q->items);
    free(q);
}
#pragma endregion queue

char **read_grid(int *rows, int *cols){
    char line[LEN], **grid;
    int size = 4;

    grid = malloc(size * sizeof(char *));

    *rows = 0;
    while(scanf(" %s", line) != EOF){
        *cols = strlen(line); 

        if(*rows == size) {
            size *= 2;
            grid = realloc(grid, size * sizeof(char *));
        }

        grid[*rows] = malloc((*cols + 1) * sizeof(char));
        strcpy(grid[*rows], line);
        (*rows)++;
    }

    return grid;
}

int main(){
    int i, j, rows, cols, **distances;
    char **grid = read_grid(&rows, &cols);
    position *start, *end, *p;

    // find start & end
    for(i = 0; i < rows; i++){
        for(j = 0; j < cols; j++){
            if(grid[i][j] == 'S') start = create_pos(i , j);
            else if(grid[i][j] == 'E') end = create_pos(i, j);
        }
    }
    
    // init all distances to -1
    distances = malloc(rows * sizeof(int*));
    for(i = 0; i < rows; i++){
        distances[i] = malloc(cols * sizeof(int));
        for(j = 0; j < cols; j++) distances[i][j] = -1;
    }

    grid[start->x][start->y] = 'a';
    grid[end->x][end->y] = 'z';
    
    // bfs
    queue* q = init(rows * cols);
    int x, y;

    distances[start->x][start->y] = 0;
    push(q, start->x, start->y);

    while(q->position < q->size){
        p = top(q);

        for(i = 0; i < DIRECTIONS_COUNT; i++){
            x = p->x + DIRECTIONS[i];
            y = p->y + DIRECTIONS[DIRECTIONS_COUNT - 1 - i];

            if(0 <= x && x < rows && 0 <= y && y < cols && (grid[x][y] == grid[p->x][p->y] || grid[x][y] - grid[p->x][p->y] <= 1) && distances[x][y] == -1){
                distances[x][y] = distances[p->x][p->y] + 1;
                push(q, x, y);
            }
        }

        free(p);
    }
    destroy(q);

    printf("%d\n", distances[end->x][end->y]);

    // free memory
    free(start);
    free(end);
    for(i = 0; i < rows; i++) {
        free(grid[i]);
        free(distances[i]);
    }
    free(grid);
    free(distances);

    return 0;
}