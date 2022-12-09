#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <math.h>
#define DIM 512

struct position {
	int x, y;
};

int positions_are_touching(struct position *a, struct position *b){
	return(abs(a->x - b->x) <= 1 && abs(a->y - b->y) <= 1);
}

int main(){
	int visited_count, steps, visited[DIM][DIM];
	struct position head, tail;
	char direction;

	memset(visited, 0, sizeof(visited));	

	head.x = tail.x = head.y = tail.y = DIM / 2;
	visited_count = 0;
	while(scanf(" %c %d", &direction, &steps) != EOF){
		while(steps--){
			if(direction == 'L') head.y--;
			else if(direction == 'R') head.y++;
			else if(direction == 'U') head.x--;
			else head.x++;

			if(!positions_are_touching(&head, &tail)){
				if(head.x == tail.x) tail.y += (head.y - tail.y) / 2;
				else if(head.y == tail.y) tail.x += (head.x - tail.x) / 2;	
				else {
					tail.y = (head.y + tail.y + (head.y < tail.y ? 0 : 1)) / 2;
					tail.x = (head.x + tail.x + (head.x < tail.x ? 0 : 1)) / 2;
				}
			}
				
			if(!visited[tail.x][tail.y]){
				visited[tail.x][tail.y] = 1;
				visited_count++;
			}
		}	
	}

	printf("%d\n", visited_count);

	return 0;
}
