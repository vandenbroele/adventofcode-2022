#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define DIGITS 8
#define TOP 3

void rotate(int *scores, int pos, int value){
	if(pos < 0) return;

	if(scores[pos] < value){
		int x = scores[pos];
		scores[pos] = value;
		rotate(scores, pos - 1, x);
	} else {
		rotate(scores, pos - 1, value);
	}
}

int main(){
	char line[DIGITS];
	int calories = 0;
	int scores[TOP] = { 0 };

	while(fgets(line, DIGITS, stdin)){
		if(line[0] < '0' || '9' < line[0]){
			rotate(scores, TOP - 1, calories);
			calories = 0;
		} else {
			calories += atoi(line);
		}
	}

	int i, sum = 0;
	for(i = 0; i < TOP; i++) {
		sum += scores[i];
	}
	
	printf("%d\n", sum);	

	return 0;
}
