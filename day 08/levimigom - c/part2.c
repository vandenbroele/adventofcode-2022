#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#define LEN 105

int viewing_distance(int **grid, int dimensions, int x, int y){
	int i, distance;
	
	distance = 0;
	for(i = x - 1; 0 <= i; i--){
		distance++;
		if(grid[x][y] <= grid[i][y]) break;
	}
	
	return distance;
}

void rotate(int **grid, int dimensions){
	int i, j, k;

	for(i = 0; i < dimensions; i++) {
		for(j = i; j < dimensions; j++){
			k = grid[i][j];
			grid[i][j] = grid[j][i];
			grid[j][i] = k;	
		}
	}

	for(i = 0; i < dimensions; i++){
		for(j = 0; j < dimensions / 2; j++){
			k = grid[i][j];
			grid[i][j] = grid[i][dimensions - j - 1];
			grid[i][dimensions - j - 1] = k;
		}
	}
}

int **init_grid(int dimensions){
	int i, **grid;

	grid = malloc(sizeof(int *) * dimensions);
	for(i = 0; i < dimensions; i++) grid[i] = malloc(sizeof(int) * dimensions);

	return grid;
}

int max(int a, int b) { return(a < b ? b : a); }

int main(){
	int i, j, k, highest_score, dimensions, **grid, **scenic_scores;
	char row[LEN];
	
	// read input and allocate memory
	fgets(row, LEN, stdin);
	
	dimensions = strlen(row) - 1;
	grid = init_grid(dimensions);
	scenic_scores = init_grid(dimensions);
	
	for(i = 0; i < dimensions; i++){
		for(j = 0; j < dimensions; j++) grid[i][j] = row[j] - '0';
		fgets(row, LEN, stdin);
	}
	
	// calculate scenic scores
	for(i = 0; i < 4; i++){
		for(j = 0; j < dimensions; j++){
			for(k = 0; k < dimensions; k++){
				if(i == 0) scenic_scores[j][k] = viewing_distance(grid, dimensions, j, k);
				else scenic_scores[j][k] *= viewing_distance(grid, dimensions, j, k);
			}
		}
		rotate(grid, dimensions);
		rotate(scenic_scores, dimensions);
	}
	
	// calculate highest score
	highest_score = 0;
	
	for(i = 0; i < dimensions; i++){
		for(j = 0; j < dimensions; j++) highest_score = max(highest_score, scenic_scores[i][j]);
	}

	printf("%d\n", highest_score);

	// free memory
	for(i = 0; i < dimensions; i++) {
		free(grid[i]);
		free(scenic_scores[i]);
	}
	free(grid);
	free(scenic_scores);

	return 0;
}
