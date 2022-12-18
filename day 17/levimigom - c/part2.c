#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define MAX_BLOCKS 1000000000000
#define LEN 12000
#define LIM 5000
char cave[LIM * 4][7];
char cave_state[LIM][100][7];
long long move_state[LIM], heights[LIM];

int max(int a, int b){ return(a < b ? b : a); }

void add_current_block(int **current_blocks, int *number_of_current_blocks, int row, int col){
    current_blocks[*number_of_current_blocks][0] = row;
    current_blocks[*number_of_current_blocks][1] = col;
    (*number_of_current_blocks)++;
}

void add_block(int move, int highest_block, int **current_blocks, int *number_of_current_blocks){
    int height = highest_block + 4;

    *number_of_current_blocks = 0;
    // ####
    if(move % 5 == 0){
        add_current_block(current_blocks, number_of_current_blocks, height, 2);
        add_current_block(current_blocks, number_of_current_blocks, height, 3);
        add_current_block(current_blocks, number_of_current_blocks, height, 4);
        add_current_block(current_blocks, number_of_current_blocks, height, 5);
    } 
    // .#.
    // ###
    // .#.
    else if(move % 5 == 1){
        add_current_block(current_blocks, number_of_current_blocks, height + 2, 3);
        add_current_block(current_blocks, number_of_current_blocks, height + 1, 2);
        add_current_block(current_blocks, number_of_current_blocks, height + 1, 3);
        add_current_block(current_blocks, number_of_current_blocks, height + 1, 4);
        add_current_block(current_blocks, number_of_current_blocks, height, 3);
    }
    // ..#
    // ..#
    // ###
    else if(move % 5 == 2){
        add_current_block(current_blocks, number_of_current_blocks, height + 2, 4);
        add_current_block(current_blocks, number_of_current_blocks, height + 1, 4);
        add_current_block(current_blocks, number_of_current_blocks, height, 2);
        add_current_block(current_blocks, number_of_current_blocks, height, 3);
        add_current_block(current_blocks, number_of_current_blocks, height, 4);
    }
    // #
    // #
    // #
    // #
    else if(move % 5 == 3){
        add_current_block(current_blocks, number_of_current_blocks, height + 3, 2);
        add_current_block(current_blocks, number_of_current_blocks, height + 2, 2);
        add_current_block(current_blocks, number_of_current_blocks, height + 1, 2);
        add_current_block(current_blocks, number_of_current_blocks, height, 2);
    }
    // ##
    // ##
    else {
        add_current_block(current_blocks, number_of_current_blocks, height + 1, 2);
        add_current_block(current_blocks, number_of_current_blocks, height + 1, 3);
        add_current_block(current_blocks, number_of_current_blocks, height, 2);
        add_current_block(current_blocks, number_of_current_blocks, height, 3);
    }
}

int main(){
    char moves[LEN];
    int i, j, k, is_same, can_move_sideways, can_move_down, blocks, move, highest_block, **current_blocks, number_of_current_blocks;
    long long solution;

    current_blocks = malloc(sizeof(int*) * 5);
    for(i = 0; i < 5; i++) current_blocks[i] = malloc(sizeof(int) * 2);

    scanf("%s", moves);

    memset(cave, '.', sizeof(cave));

    highest_block = -1;
    move = 0;
    for(blocks = 0; blocks < MAX_BLOCKS; blocks++){
        // add a new block
        add_block(blocks, highest_block, current_blocks, &number_of_current_blocks);

        while(1){
            if(moves[move % strlen(moves)] == '<') j = -1;
            else j = 1;

            // check if all current blocks can move sideways
            can_move_sideways = 1;
            for(i = 0; i < number_of_current_blocks; i++){
                if(current_blocks[i][1] + j < 0 || 7 <= current_blocks[i][1] + j || cave[current_blocks[i][0]][current_blocks[i][1] + j] != '.') {
                    can_move_sideways = 0;
                    break;
                }
            }

            // move sideways
            if(can_move_sideways){
                for(i = 0; i < number_of_current_blocks; i++) current_blocks[i][1] += j;
            } 

            move++;

            // check if all current blocks can move down
            can_move_down = 1;
            for(i = 0; i < number_of_current_blocks; i++){
                if(current_blocks[i][0] - 1 < 0 || cave[current_blocks[i][0] - 1][current_blocks[i][1]] != '.') {
                    can_move_down = 0;
                    break;
                }
            }

            // move down
            if(can_move_down){
                for(i = 0; i < number_of_current_blocks; i++) current_blocks[i][0]--;
            } else {
                break;
            }
        }

        // set # where block landed
        for(i = 0; i < number_of_current_blocks; i++){
            cave[current_blocks[i][0]][current_blocks[i][1]] = '#';
            highest_block = max(highest_block, current_blocks[i][0]);
        } 
        number_of_current_blocks = 0;

        // store the state of the cave, the moves and the current height
        if(100 <= blocks){
            for(i = 0; i < 100; i++){
                for(j = 0; j < 7; j++){
                    cave_state[blocks][i][j] = cave[highest_block - i][j];
                }
            }
            move_state[blocks] = move % strlen(moves);
            heights[blocks] = highest_block + 1;
        }

        // try to find a pattern in the states
        for(i = 100; i < blocks; i++){
            is_same = 1;
            for(j = 0; j < 100 && is_same; j++){
                for(k = 0; k < 7 && is_same; k++){
                    if(cave_state[i][j][k] != cave_state[blocks][j][k]) is_same = 0;
                }
            }

            if(is_same && move_state[i] == move_state[blocks]) {
                printf("Pattern found! Cave state is same after %d blocks and after %d blocks!\n", i, blocks);
                break;
            }
        }

        // pattern found => solution can be calculated using % 
        if(is_same){
            solution = heights[i + ((MAX_BLOCKS - i - 1) % (blocks - i))] + (heights[blocks] - heights[i]) * ((MAX_BLOCKS - i - 1) / (blocks - i));
            printf("Solution = %lld\n", solution);
            break;
        }
    }

    for(i = 0; i < 5; i++) free(current_blocks[i]);
    free(current_blocks);

    return 0;
}