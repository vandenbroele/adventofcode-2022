#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <math.h>
#define DIM 512
#define NUM 10

struct position {
	int x, y;
};	

void init(struct position *pos){
	pos->x = pos->y = DIM / 2;
}

int positions_are_touching(struct position *head, struct position *tail){
	return(abs(head->x - tail->x) <= 1 && abs(head->y - tail->y) <= 1);
}

void move(struct position *head, struct position *tail){
	if(!positions_are_touching(head, tail)){
		if(head->x == tail->x) tail->y += (head->y - tail->y) / 2;
		else if(head->y == tail->y) tail->x += (head->x - tail->x) / 2;	
		else {
			tail->y = (head->y + tail->y + (head->y < tail->y ? 0 : 1)) / 2;
			tail->x = (head->x + tail->x + (head->x < tail->x ? 0 : 1)) / 2;
		}
	}
}

int main(){
	int i, visited_count, steps, visited[DIM][DIM];
	struct position *positions;
	char direction;

	memset(visited, 0, sizeof(visited));	
	
	positions = malloc(NUM * sizeof(struct position));
	for(i = 0; i < NUM; i++) init(&(positions[i]));	

	visited_count = 0;
	while(scanf(" %c %d", &direction, &steps) != EOF){
		while(steps--){
			if(direction == 'L') positions[0].y--;
			else if(direction == 'R') positions[0].y++;
			else if(direction == 'U') positions[0].x--;
			else positions[0].x++;
			
			for(i = 1; i < NUM; i++) move(&(positions[i - 1]), &(positions[i]));	
			
			if(!visited[positions[NUM - 1].x][positions[NUM - 1].y]){
				visited[positions[NUM - 1].x][positions[NUM - 1].y] = 1;
				visited_count++;
			}
		}	
	}

	printf("%d\n", visited_count);
	free(positions);

	return 0;
}
