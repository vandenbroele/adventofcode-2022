#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#define NUMBERS 5000

typedef struct l {
    int number;
    struct l *previous, *next;
} listitem;

void mix(listitem *item){
    int i;
    listitem *prev, *next;

    for(i = 0; i < abs(item->number) % (NUMBERS * NUMBERS); i++){
        if(item->number < 0){
            prev = item->previous;
            next = item->next;

            item->previous = prev->previous;
            item->next = prev;

            item->previous->next = item;
            item->next->previous = item;

            next->previous = prev;
            prev->next = next;
        } else {
            prev = item->previous;
            next = item->next;

            item->previous = next;
            item->next = next->next;

            item->previous->next = item;
            item->next->previous = item;

            next->previous = prev;
            prev->next = next;
        }
    }
}


int sum(listitem *item){
    int i, sum;

    sum = 0;
    for(i = 1; i <= 3000; i++){
        item = item->next;
        if(i % 1000 == 0) sum += item->number;
    }

    return sum;
}

int main(){
    int i;
    listitem items[NUMBERS];

    // read input and create linked list
    for(i = 0; i < NUMBERS; i++){
        scanf("%d", &(items[i].number));

        if(i != 0) {
            items[i].previous = &(items[i - 1]);
            items[i - 1].next = &(items[i]);
        }
    }

    items[0].previous = &(items[NUMBERS - 1]);
    items[NUMBERS - 1].next = &(items[0]);

    // mix the numbers
    for(i = 0; i < NUMBERS; i++) mix(&(items[i]));

    for(i = 0; i < NUMBERS; i++){
        if(items[i].number == 0) {
            printf("%d\n", sum(&(items[i])));
        }
    }

    return 0;
}