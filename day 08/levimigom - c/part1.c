#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#define LEN 105

void mark_visible(int **grid, int dimensions, int **is_visible){
	int i, j, largest;

	for(i = 0; i < dimensions; i++){
		largest = -1;
		for(j = 0; j < dimensions; j++){
			if(largest == -1 || largest < grid[i][j]){
				largest = grid[i][j];
				is_visible[i][j] = 1;
			} 
		}		
	}
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
	for(i = 0; i < dimensions; i++){
		grid[i] = malloc(sizeof(int) * dimensions);
		memset(grid[i], 0, sizeof(int) * dimensions);
	}
	return grid;
}

int main(){
	int i, j, visible, dimensions, **grid, **is_visible;
	char row[LEN];
	
	// read input and allocate memory
	fgets(row, LEN, stdin);
	
	dimensions = strlen(row) - 1;
	grid = init_grid(dimensions);
	is_visible = init_grid(dimensions);
	
	for(i = 0; i < dimensions; i++){
		for(j = 0; j < dimensions; j++) grid[i][j] = row[j] - '0';
		fgets(row, LEN, stdin);
	}
	
	// mark visible trees from the left and rotate grid 90 degrees (4x)
	mark_visible(grid, dimensions, is_visible);
	
	for(i = 0; i < 3; i++){
		rotate(grid, dimensions);
		rotate(is_visible, dimensions);
		mark_visible(grid, dimensions, is_visible);
	}
	
	// count visible trees
	visible = 0;

	for(i = 0; i < dimensions; i++){
		for(j = 0; j < dimensions; j++) visible += is_visible[i][j];
	}
	
	printf("%d\n", visible);

	// free memory
	for(i = 0; i < dimensions; i++) {
		free(grid[i]);
		free(is_visible[i]);
	}
	free(grid);
	free(is_visible);

	return 0;
}
