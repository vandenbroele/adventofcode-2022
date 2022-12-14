#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define LEN 512
#define DIV1 "[[2]]"
#define DIV2 "[[6]]"

#pragma region list
typedef struct l{
    int size, value, is_list, is_divider;
    struct l **items;
} list;

list* empty_list(){
    list *l = malloc(sizeof(list));
    l->size = 0;
    l->is_list = 1;
    l->is_divider = 0;

    return l;
}

list* value_list(int value){
    list *l = empty_list();
    l->is_list = 0;
    l->value = value;

    return l;
}

void add(list *l, list *c){
    if(l->size == 0) l->items = malloc(sizeof(list*));
    else l->items = realloc(l->items, (l->size + 1) * sizeof(list*));

    l->items[l->size++] = c;
}

void destroy(list *l){
    int i;

    for(i = 0; i < l->size; i++) destroy(l->items[i]);

    if(l->size != 0) free(l->items);
    free(l);
}
#pragma endregion list

int is_digit(char c){
    return('0' <= c && c <= '9');
}

int cmp(int a, int b){
    if(a < b) return 1;
    else if(b < a) return -1;
    else return 0;
}

list *parse_input(char *line, int position){
    list *l = empty_list();
    int brackets = 1;
    int number = 0;

    while(0 < brackets){
        if(brackets == 1){
            if(line[position] == '[') add(l, parse_input(line, position + 1));
            else if(is_digit(line[position])) number = (number * 10) + line[position] - '0';
            else if(is_digit(line[position - 1])){
                add(l, value_list(number));
                number = 0;
            }
        }

        if(line[position] == '[') brackets++;
        else if(line[position] == ']') brackets--;
        position++;
    }

    return l;
}

int compare(list *left, list *right){
    int c;

    // both values are lists
    if(left->is_list && right->is_list){
        int i;

        for(i = 0; i < left->size || i < right->size; i++){
            if(i == left->size) return 1;
            else if(i == right->size) return -1;
            else if(compare(left->items[i], right->items[i]) != 0) return compare(left->items[i], right->items[i]);
        }

        return 0;
    }
    // both values are integers
    else if(!left->is_list && !right->is_list) {
        return cmp(left->value, right->value);
    }
    // exactly one value is an integer
    else {
        list *l = empty_list();

        if(left->is_list) {
            add(l, right);
            c = compare(left, l);
        } else {
            add(l, left);
            c = compare(l, right);
        }

        free(l->items);
        free(l);
        return c;
    } 
}

void insert(list **packets, int size, list *packet){
    int i;
    list *placeholder;

    for(i = 0; i < size; i++){
        if(compare(packet, packets[i]) == 1){
            placeholder = packets[i];
            packets[i] = packet;
            packet = placeholder;
        }
    }
    packets[size] = packet;
}

int main(){
    char line[LEN];
    list *l, **packets;
    int i, solution, size;

    size = 0;
    packets = malloc(2 * sizeof(list *));

    l = parse_input(DIV2, 1);
    l->is_divider = 1;
    insert(packets, size++, l);

    l = parse_input(DIV1, 1);
    l->is_divider = 1;
    insert(packets, size++, l);

    while(fgets(line, LEN, stdin)){
        packets = realloc(packets, (size + 2) * sizeof(list *));
        insert(packets, size++, parse_input(line, 1));
        
        fgets(line, LEN, stdin);
        insert(packets, size++, parse_input(line, 1));

        fgets(line, LEN, stdin);
    }

    solution = 1;
    for(i = 0; i < size; i++){
            if(packets[i]->is_divider) solution *= (i + 1);
            destroy(packets[i]);
    }
    free(packets);

    printf("%d\n", solution);
    
    return 0;
}

