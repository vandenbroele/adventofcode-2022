#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#define LEN 100
#define ROUNDS 20

struct monkey {
    int item_count, *items, test, on_true, on_false, inspections;
    char operator, *operator_num;
};

void add_item(struct monkey *m, int item){
    if(m->item_count == 0) m->items = malloc(sizeof(int));
    else m->items = realloc(m->items, (m->item_count + 1) * sizeof(int));

    m->items[m->item_count] = item;
    m->item_count++;
}

void read(struct monkey *m){
    int i, item;
    char line[LEN];

    m->inspections = 0;

    fgets(line, LEN, stdin);

    item = 0;
    m->item_count = 0;
    for(i = 18; i < strlen(line); i++){
        if('0' <= line[i] && line[i] <= '9') item = (10 * item) + line[i] - '0';
        else if(line[i] == ' ') {
            add_item(m, item);
            item = 0;
        }
    }
    add_item(m, item);

    fgets(line, LEN, stdin);
    m->operator = line[23];
    m->operator_num = malloc(sizeof(char) * (strlen(line + 25) + 1));
    strcpy(m->operator_num, line + 25);

    fgets(line, LEN, stdin);
    m->test = atoi(line + 21);

    fgets(line, LEN, stdin);
    m->on_true = atoi(line + 28);

    fgets(line, LEN, stdin);
    m->on_false = atoi(line + 29);

    fgets(line, LEN, stdin);
}

int main(){
    char line[LEN];
    struct monkey *monkeys;
    int i, j, k, worry_level, worry_operand, monkey_count;

    monkey_count = 0;
    while(fgets(line, LEN, stdin)){
        monkey_count++;

        if(monkey_count == 1) monkeys = malloc(sizeof(struct monkey));
        else monkeys = realloc(monkeys, monkey_count * sizeof(struct monkey));
        
        read(&(monkeys[monkey_count - 1]));
    }

    for(i = 0; i < ROUNDS; i++){
        for(j = 0; j < monkey_count; j++){
            for(k = 0; k < monkeys[j].item_count; k++){
                monkeys[j].inspections++;

                worry_level = monkeys[j].items[k];

                if('0' <= monkeys[j].operator_num[0] && monkeys[j].operator_num[0] <= '9') worry_operand = atoi(monkeys[j].operator_num);
                else worry_operand = worry_level;

                if(monkeys[j].operator == '+') worry_level += worry_operand;
                else worry_level *= worry_operand;

                worry_level = worry_level / 3;

                if(worry_level % monkeys[j].test == 0) add_item(&(monkeys[monkeys[j].on_true]), worry_level);
                else add_item(&(monkeys[monkeys[j].on_false]), worry_level);
            }

            if(monkeys[j].item_count != 0){
                monkeys[j].item_count = 0;
                free(monkeys[j].items);
            }
        }
    }

    for(i = 0; i < monkey_count; i++){
        printf("Monkey %d inspected items %d times.\n", i, monkeys[i].inspections);
        if(monkeys[i].item_count != 0) free(monkeys[i].items);
        free(monkeys[i].operator_num);
    }
    free(monkeys);

    return 0;
}