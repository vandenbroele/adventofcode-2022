#include <stdio.h>
#include <stdlib.h>
#define INTERVAL 40
#define BEGIN 20
#define END 220

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
			if(BEGIN <= cycle && cycle <= END && (cycle - BEGIN) % INTERVAL == 0) signal_strengths += cycle * x;
			cycle++;
		}
		x += v;
	}	

	printf("%d\n", signal_strengths);	

	return 0;
}
