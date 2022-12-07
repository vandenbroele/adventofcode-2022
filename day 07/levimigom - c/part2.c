#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#define LEN 50
#define TOTAL 70000000
#define UNUSED 30000000

struct directory {
	char *name;
	int size, number_of_children;
	struct directory *parrent;
	struct directory **children;
};

void init(struct directory *d, char *name, struct directory *p){
	d->name = malloc(sizeof(char) * (strlen(name) + 1));
	strcpy(d->name, name);

	d->number_of_children = d->size = 0;
	d->parrent = p; 
}

void destroy(struct directory *d){
	int i;

	free(d->name);
	for(i = 0; i < d->number_of_children; i++) {
		destroy(d->children[i]);
		free(d->children[i]);
	}
	free(d->children);
}

struct directory *get_child(struct directory *d, char *name){
	int i;

	for(i = 0; i < d->number_of_children; i++){
		if(strcmp(d->children[i]->name, name) == 0) return d->children[i];
	}
	return 0;
}

void add_child(struct directory *d, char *name){
	if(d->number_of_children == 0) {
		d->children = calloc(sizeof(struct directory *), 1);
	}
	else {
		d->children = realloc(d->children, (d->number_of_children + 1) * sizeof(struct directory *));
	}

	d->children[d->number_of_children] = calloc(sizeof(struct directory), 1);
	init(d->children[d->number_of_children++], name, d);
}

void calculate_size(struct directory *d){
	int i;

	for(i = 0; i < d->number_of_children; i++){
		calculate_size(d->children[i]);
		d->size += d->children[i]->size;
	}
}

int smallest_directory_above_limit(struct directory *d, int limit){
	int i, size, smallest;

	smallest = limit <= d->size ? d->size : -1;	
	for(i = 0; i < d->number_of_children; i++){
		size = smallest_directory_above_limit(d->children[i], limit);
		
		if(limit <= size && (smallest == -1 || size < smallest)) smallest = size;
	}
	
	return smallest;
}

int main(){
	char a[LEN];
	struct directory *root, *current;
	int i;

	current = root = malloc(sizeof(struct directory));
	init(root, "/", 0);

	fgets(a, LEN, stdin);	
	while(scanf(" %s", a) != EOF){
		if(strcmp(a, "$") == 0){
			scanf("%s", a);
			if(strcmp(a, "cd") == 0) {
				scanf("%s", a);
				if(strcmp(a, "/") == 0) current = root;
				else if(strcmp(a, "..") == 0) current = current->parrent;
				else current = get_child(current, a);
			}
		} else if (strcmp(a, "dir") == 0) {
			scanf("%s", a);
			if(get_child(current, a) == 0) add_child(current, a);
		} else {
			current->size += atoi(a);
			scanf("%s", a);
		}	
	}
	
	calculate_size(root);
	printf("%d\n", smallest_directory_above_limit(root, UNUSED - (TOTAL - root->size)));
	
	destroy(root);
	free(root);

	return 0;
}
