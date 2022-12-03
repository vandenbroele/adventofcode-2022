#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define LEN 64

int main(){
	char line[LEN], contains['z' + 1];
	int i, j, sum, items;

	sum = 0;
	while(scanf("%s", line) != EOF){
		memset(contains, 0, sizeof(contains));
		
		for(i = 0; i < 2; i++){
			for(j = 0; j < strlen(line); j++){
				if(contains[line[j]] == i) contains[line[j]]++;
			}
			scanf("%s", line);
		}
		
		for(i = 0; i < strlen(line); i++){
			if(contains[line[i]] == 2){
				sum += ('a' <= line[i]) ? line[i] - 'a' + 1 : line[i] - 'A' + 27;
				contains[line[i]] = 0;
			}
		}
	}

	printf("%d\n", sum);
	
	return 0;
}
