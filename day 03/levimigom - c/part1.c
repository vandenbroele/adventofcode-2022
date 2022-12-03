#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define LEN 64

int main(){
	char line[LEN], contains['z' + 1];
	int i, sum, items;

	sum = 0;
	while(scanf("%s", line) != EOF){
		items = strlen(line);

		memset(contains, 0, sizeof(contains));
		for(i = 0; i < items / 2; i++) contains[line[i]] = 1;

		for(i = items / 2; i < items; i++){
			if(contains[line[i]] == 1){
				sum += ('a' <= line[i]) ? line[i] - 'a' + 1 : line[i] - 'A' + 27;
				contains[line[i]]++;
			}
		}			
	}

	printf("%d\n", sum);
	
	return 0;
}
