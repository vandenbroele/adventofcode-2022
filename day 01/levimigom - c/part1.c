#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define DIGITS 8

int main(){
	char line[DIGITS];
	int calories, max;	

	calories = max = 0;
	while(fgets(line, DIGITS, stdin)){
		if(line[0] < '0' || '9' < line[0]){
			if(max < calories) max = calories;
			calories = 0;
		} else {
			calories += atoi(line);
		}
	}

	printf("%d\n", max);	

	return 0;
}
