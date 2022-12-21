#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define MAX 456977
#define LEN 8

typedef struct e {
    long long value;
    int lhs, rhs, contains_human;
    char operator;
} exp;

int get_hash(char *s){
    int i, hash;

    hash = 0;
    for(i = 0; i < strlen(s); i++) hash = (hash * 26) + s[i] - 'a';

    return hash;
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
    }

    return expressions[hash]->value;
}

void check_contains_human(int hash, exp *expressions[MAX]){
    if(expressions[hash]->value == -1) {
        int lhs, rhs;

        lhs = expressions[hash]->lhs;
        rhs = expressions[hash]->rhs;

        check_contains_human(lhs, expressions);
        check_contains_human(rhs, expressions);

        expressions[hash]->contains_human = expressions[lhs]->contains_human || expressions[rhs]->contains_human;
    }
}

long long find_human_value(int hash, exp *expressions[MAX], long long expected_value){
    exp *current = expressions[hash];

    if(current->value != -1) return expected_value;
    
    if(expressions[current->lhs]->contains_human) {
        if(current->operator == '+') return find_human_value(current->lhs, expressions, expected_value - eval(current->rhs, expressions));
        else if(current->operator == '-') return find_human_value(current->lhs, expressions, expected_value + eval(current->rhs, expressions));
        else if(current->operator == '*') return find_human_value(current->lhs, expressions, expected_value / eval(current->rhs, expressions));
        else return find_human_value(current->lhs, expressions, expected_value * eval(current->rhs, expressions));
    } else {
        if(current->operator == '+') return find_human_value(current->rhs, expressions, expected_value - eval(current->lhs, expressions));
        else if(current->operator == '-') return find_human_value(current->rhs, expressions, eval(current->lhs, expressions) - expected_value);
        else if(current->operator == '*') return find_human_value(current->rhs, expressions, expected_value / eval(current->lhs, expressions));
        else return find_human_value(current->rhs, expressions, expected_value * eval(current->lhs, expressions));
    }
}

int main(){
    exp *expressions[MAX], *root_expr;
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

        if('0' <= lhs[0] && lhs[0] <= '9'){
            expressions[hashes[expression_count]]->value = atoi(lhs);

            if(strcmp(name, "humn") == 0) expressions[hashes[expression_count]]->contains_human = 1;
            else expressions[hashes[expression_count]]->contains_human = 0;
        }
        else {
            scanf(" %c %s", &(expressions[hashes[expression_count]]->operator), rhs);
            expressions[hashes[expression_count]]->lhs = get_hash(lhs);
            expressions[hashes[expression_count]]->rhs = get_hash(rhs);
            expressions[hashes[expression_count]]->value = -1;
        }
    }

    // find all subtrees that contain the human
    check_contains_human(hashes[root], expressions);

    // evaluate expression tree starting from the root
    root_expr = expressions[hashes[root]];

    if(expressions[root_expr->lhs]->contains_human) printf("%lld\n", find_human_value(root_expr->lhs, expressions, eval(root_expr->rhs, expressions)));
    else printf("%lld\n", find_human_value(root_expr->rhs, expressions, eval(root_expr->lhs, expressions)));

    // free memory
    for(i = 0; i < expression_count; i++) free(expressions[hashes[i]]);
    free(hashes);

    return 0;
}
