#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#define PIXELS 40

int main(){
	int i, cycle, x, v, wait, signal_strengths;
	char operation[4];

	cycle = x = 1;
	signal_strengths = 0;
	while(scanf(" %s", operation) != EOF){
		if(operation[0] == 'n') {
			wait = 1; 
		 	v = 0;
		}
		else {
			scanf("%d", &v);
			wait = 2;
		}
		
		for(i = 0; i < wait; i++){
			if(abs(((cycle - 1) % PIXELS) - x) <= 1) printf("#");
			else printf("*");
			cycle++;

			if((cycle - 1) % PIXELS == 0) printf("\n");
		}
		x += v;
	}	

	printf("%d\n", signal_strengths);	

	return 0;
}
