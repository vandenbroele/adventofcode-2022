#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define MAX_BLOCKS 2022
#define LEN 12000
char cave[MAX_BLOCKS * 4][7];

int max(int a, int b){ return(a < b ? b : a); }

void print_cave(int highest_block, int **current_blocks, int number_of_current_blocks){
    int i, j, k, is_current;

    for(i = highest_block; 0 <= i; i--) {
        printf("|");
        for(j = 0; j < 7; j++) {
            is_current = 0;
            for(k = 0; k < number_of_current_blocks; k++){
                if(i == current_blocks[k][0] && j == current_blocks[k][1]) is_current = 1;
            }

            if(is_current) printf("@");
            else printf("%c", cave[i][j]);
        }
        printf("|\n");
    } 
    printf("+-------+\n\n");
}

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
    int i, j, can_move_sideways, can_move_down, blocks, move, highest_block, **current_blocks, number_of_current_blocks;

    current_blocks = malloc(sizeof(int*) * 5);
    for(i = 0; i < 5; i++) current_blocks[i] = malloc(sizeof(int) * 2);

    scanf("%s", moves);

    memset(cave, '.', sizeof(cave));

    highest_block = -1;
    move = 0;
    for(blocks = 0; blocks < MAX_BLOCKS; blocks++){
        // add a new block
        add_block(blocks, highest_block, current_blocks, &number_of_current_blocks);

        //printf("Rock %d begins falling\n", blocks);
        //print_cave(highest_block + 7, current_blocks, number_of_current_blocks);

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

        //print_cave(highest_block, current_blocks, number_of_current_blocks);
    }

    printf("Height: %d\n", highest_block + 1);

    for(i = 0; i < 5; i++) free(current_blocks[i]);
    free(current_blocks);

    return 0;
}