#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define MAX 456977
#define LEN 8

typedef struct e {
    long long value;
    int lhs, rhs;
    char operator;
} exp;

int get_hash(char *s){
    int i, hash;

    hash = 0;
    for(i = 0; i < strlen(s); i++) hash = (hash * 26) + s[i] - 'a';

    return hash;
}

char *get_name(int hash){
    int i;
    char *name = malloc(sizeof(char) * 4);

    for(i = 3; 0 <= i; i--){
        name[i] = 'a' + (hash % 26);
        hash /= 26;
    }

    return name;
}

long long eval(int hash, exp *expressions[MAX]){
    if(expressions[hash]->value == -1) {
        long long lhs, rhs;

        lhs = eval(expressions[hash]->lhs, expressions);
        rhs = eval(expressions[hash]->rhs, expressions);

        if(expressions[hash]->operator == '+') expressions[hash]->value = lhs + rhs;
        else if(expressions[hash]->operator == '-') expressions[hash]->value = lhs - rhs;
        else if(expressions[hash]->operator == '*') expressions[hash]->value = lhs * rhs;
        else expressions[hash]->value = lhs / rhs;

        char *name = get_name(hash);
        printf("Monkey '%s' yells %lld\n", name, expressions[hash]->value);
        free(name);
    }

    return expressions[hash]->value;
}

int main(){
    exp *expressions[MAX];
    int i, j, expression_count, root, *hashes;
    char name[LEN], lhs[LEN], rhs[LEN];

    // parse input
    for(expression_count = 0; scanf(" %s %s", name, lhs) != EOF; expression_count++){
        name[4] = '\0';
        if(strcmp(name, "root") == 0) root = expression_count;

        if(expression_count == 0) hashes = malloc(sizeof(int));
        else hashes = realloc(hashes, sizeof(int) * (expression_count + 1));

        hashes[expression_count] = get_hash(name);
        expressions[hashes[expression_count]] = malloc(sizeof(exp));

        if('0' <= lhs[0] && lhs[0] <= '9') expressions[hashes[expression_count]]->value = atoi(lhs);
        else {
            scanf(" %c %s", &(expressions[hashes[expression_count]]->operator), rhs);
            expressions[hashes[expression_count]]->lhs = get_hash(lhs);
            expressions[hashes[expression_count]]->rhs = get_hash(rhs);
            expressions[hashes[expression_count]]->value = -1;
        }
    }

    // evaluate expression tree starting from the root
    eval(hashes[root], expressions);

    // free memory
    for(i = 0; i < expression_count; i++) free(expressions[hashes[i]]);
    free(hashes);

    return 0;
}
