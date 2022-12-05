#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define LEN 50
#define MOVE "move"
#define BRACKET "["

typedef struct stack {
	int number_of_crates, size;
	char *crates;
};

void init(struct stack *s){
	s->size = 1;
	s->number_of_crates = 0;
	s->crates = malloc(s->size * sizeof(char));
}

void push(struct stack *s, char c){
	if(s->number_of_crates == s->size){
		s->size *= 2;
		s->crates = realloc(s->crates, s->size * sizeof(char));
	}
	s->crates[s->number_of_crates++] = c;
}

char pop(struct stack *s){
	return s->crates[s->number_of_crates-- - 1];
}

void reverse(struct stack *s){
	int i;
	char d;

	for(i = 0; i < s->number_of_crates / 2; i++){
		d = s->crates[i];
		s->crates[i] = s->crates[s->number_of_crates - i - 1];
		s->crates[s->number_of_crates - i - 1] = d;	
	}
}

int my_atoi(char *s){
	int i, n = 0;

	for(i = 0; '0' <= s[i] && s[i] <= '9'; i++) n = (10 * n) + s[i] - '0';

	return n;
}

int main(){
	char line[LEN];
	struct stack **stacks = malloc(sizeof(struct stack*));	
	int i, j, word, count, from, to, number_of_stacks = 1; 
	
	stacks[0] = malloc(sizeof(struct stack));
	init(stacks[0]);

	while(fgets(line, LEN, stdin)){
		if(strstr(line, BRACKET)){
			if(number_of_stacks < strlen(line) / 4){
				stacks = realloc(stacks, (strlen(line) / 4) * sizeof(struct stack*));
				for(i = number_of_stacks; i < strlen(line) / 4; i++) {
					stacks[i] = malloc(sizeof(struct stack));
					init(stacks[i]);				
				}
				number_of_stacks = strlen(line) / 4;	
			}

			for(i = 0; i < strlen(line); i += 4){
				if(line[i] == '[') push(stacks[i / 4], line[i + 1]);
			}
		} else if(strstr(line, MOVE)){
			j = count = from = to = 0;
			
			for(i = 0; i < strlen(line); i++){
				if(line[i] == ' ' && '0' <= line[i + 1] && line[i + 1] <= '9'){
					if(j == 0) count = my_atoi(line + i + 1);
					else if(j == 1) from =  my_atoi(line + i + 1);
					else to = my_atoi(line + i + 1);
					j++;
				}
			}	
				
			for(i = stacks[from - 1]->number_of_crates - count; i < stacks[from - 1]->number_of_crates; i++){
				push(stacks[to - 1], stacks[from - 1]->crates[i]);
			}
			stacks[from - 1]->number_of_crates -= count;
		} else if(line[1] == '1'){
			for(i = 0; i < number_of_stacks; i++) reverse(stacks[i]);
		}
	}

	for(i = 0; i < number_of_stacks; i++){
		printf("%c", pop(stacks[i]));
		free(stacks[i]->crates);
		free(stacks[i]);
	}
	free(stacks);

	return 0;
}
