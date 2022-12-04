#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#define LEN 20

int my_atoi(char *s, int l){
	int i, n = 0;

	for(i = 0; i < l; i++) n = (n * 10) + (s[i] - '0');

	return n;
}

int *get_sections(char *assignments){
	int* sections = malloc(4 * sizeof(int));
	int i, j, k;
	
	j = k = 0;
	for(i = 0; i < strlen(assignments); i++){
		if(assignments[i] == '-' || assignments[i] == ','){
			sections[k++] = my_atoi(assignments + j, i - j);
			j = i + 1;
		}		
	}
	sections[k] = my_atoi(assignments + j, i - j);
	
	return sections;
}

int overlaps(char *assignments){
	int i, result, *sections = get_sections(assignments);
	
	result = (sections[0] <= sections[2] && sections[2] <= sections[1])
		|| (sections[0] <= sections[3] && sections[3] <= sections[1])
		|| (sections[2] <= sections[0] && sections[0] <= sections[3])
		|| (sections[2] <= sections[1] && sections[1] <= sections[3]);	
	
	free(sections);

	return result;
}	

int main(){
	char assignments[LEN];
	int overlap = 0;
 
	while(scanf(" %s", assignments) != EOF) overlap += overlaps(assignments);		
	
	printf("%d\n", overlap);

	return 0;
}
